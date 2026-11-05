using ChipAgingCheck.Domain;
using ChipAgingCheck.Domain.Common;
using ChipAgingCheck.Domain.Ctrs;
using ChipAgingCheck.Domain.Modbus;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Media;
using System.Text;
using System.Windows.Forms;

namespace ChipAgingCheck.UI
{
    public partial class MainForm : Form
    {
        private readonly Timer refreshTimer = new Timer();
        private DateTime runStartTime = DateTime.MinValue;
        private string lastAlarmText = "";  // 内容没变就不重写，TextBox 赋值会把滚动条弹回顶部
        private int lastAlarmCount = 0;  // 报警条数涨了才响一声
        private bool loadingRecipeList = false;  // 填下拉框时会触发选中事件，用它挡掉
        private const int MaxLogLines = 500;
        private const int DeviceMax = 8;
        private const int ChannelMax = 64;
        private double curveMinTemp = 0;        // 纵轴最低温度，每次重画前重新算
        private double curveMaxTemp = 150;      // 纵轴最高温度，每次重画前重新算
        private const int SampleSeconds = 10;//10s画一次
        private readonly List<PointF> actualSample= new List<PointF>();//实际温度
        private DateTime lastSampleTime = DateTime.MinValue;//上次记录的时间
        private int curveTotalMinutes = 0;//横轴总长
        public MainForm()
        {
            InitializeComponent();
            refreshTimer.Interval = 1000;
            refreshTimer.Tick += RefreshTimer_Tick;

            TaskProcess.Instance.Log += OnTaskProcessLog;
            btnStop.Click += btnStop_Click;
            btnRecipeParameters.Click += btnRecipeParameters_Click;
            btnDeviceParam.Click += btnDeviceParam_Click;
            btnAlarmConfirm.Click += btnAlarmConfirm_Click;
            cmbRecipe.SelectedIndexChanged += cmbRecipe_SelectedIndexChanged;

            FillRecipeList();
            FillDeviceList();
            ShowResumeBatch();
            AppendLog("程序已启动，配置文件：" + ConfigManager.ConfigFilePath);
        }

        // 内置那几套，加上配置文件里正在用的
        private void FillRecipeList()
        {
            loadingRecipeList = true;
            cmbRecipe.Items.Clear();
            List<Recipe> builtInRecipes = Recipe.DefaultRecipes;
            for (int i = 0; i < builtInRecipes.Count; i++)
            {
                cmbRecipe.Items.Add(builtInRecipes[i].Name);
            }
            Recipe currentRecipe = TaskProcess.Instance.GetRecipe();
            bool currentIsBuiltIn = false;
            for (int i = 0; i < builtInRecipes.Count; i++)
            {
                bool sameName = builtInRecipes[i].Name == currentRecipe.Name;
                if (sameName)
                {
                    currentIsBuiltIn = true;
                    cmbRecipe.SelectedIndex = i;
                }
            }
            if (currentIsBuiltIn == false)  // 自己改过的配方，内置列表里没有，补一项
            {
                cmbRecipe.Items.Add(currentRecipe.Name);
                cmbRecipe.SelectedIndex = cmbRecipe.Items.Count - 1;
            }
            loadingRecipeList = false;
        }

        private void FillDeviceList()
        {
            cmbDevice.Items.Clear();
            for (int i = 0; i < DeviceMax; i++)
            {
                cmbDevice.Items.Add("第 " + (i + 1) + " 台电源");
            }
            cmbDevice.SelectedIndex = 0;
        }

        // 上次的批次没封就接着显示，按启动会用它
        private void ShowResumeBatch()
        {
            BatchInfo unfinished = BatchRecorder.ResumeUnfinished();
            bool hasUnfinished = unfinished != null;
            if (hasUnfinished)
            {
                lblBatch.Text = "批次：" + unfinished.BatchNo + "（未结束）";
                AppendLog("发现未结束批次 " + unfinished.BatchNo + "，按启动接着记录");
                return;
            }
            lblBatch.Text = "批次：未开始";
        }

        // 连设备、开批次、跑时钟，哪一步失败都只记日志
        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                TaskProcess.Instance.Start();
                runStartTime = DateTime.Now;
                actualSample.Clear();
                lastSampleTime = DateTime.Now;
                refreshTimer.Start();
                lblRunState.Text = "状态：运行中";
                StartOrResumeBatch();
                int connectedCount = TaskProcess.Instance.DeviceCount;
                AppendLog("启动完成，已连接 " + connectedCount + " 台");
            }
            catch (Exception ex)
            {
                AppendLog("启动失败：" + ex.Message);
            }
        }

        // 有没跑完的就接着记，没有就开新的
        private void StartOrResumeBatch()
        {
            BatchInfo running = BatchRecorder.Current;
            bool batchExists = running != null && running.Ended == false;  // 已封的批次不算
            if (batchExists)
            {
                lblBatch.Text = "批次：" + running.BatchNo;
                lblStatus.Text = "继续记录批次 " + running.BatchNo;
                return;
            }
            Recipe recipe = TaskProcess.Instance.GetRecipe();
            BatchInfo brandNew = BatchRecorder.Start(recipe.ProductModel, recipe.Name);
            lblBatch.Text = "批次：" + brandNew.BatchNo;
            lblStatus.Text = "新批次 " + brandNew.BatchNo;
            AppendLog("新批次 " + brandNew.BatchNo + "，数据文件 " + BatchRecorder.CsvPath);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            TaskProcess.Instance.Stop();
            BatchRecorder.Stop();  // 封批，结束时间靠这一步写进去
            refreshTimer.Stop();
            lblRunState.Text = "状态：已停止";
            lblStatus.Text = "已停止，本批数据已封存";
            lblBatch.Text = "批次：未开始";
            GridPanel.Invalidate();
        }

        private void btnRecipeParameters_Click(object sender, EventArgs e)
        {
            Recipe currentRecipe = TaskProcess.Instance.GetRecipe();
            RecipeForm form = new RecipeForm();
            form.LoadRecipe(currentRecipe);  // 里面自己拷一份，改到取消也不影响在用的
            DialogResult result = form.ShowDialog(this);
            bool clickedOk = result == DialogResult.OK;
            if (clickedOk == false)
            {
                return;  // 取消，什么都不写回去
            }
            Recipe edited = form.GetRecipe();
            TaskProcess.Instance.SetRecipe(edited);
            FillRecipeList();
            AppendLog("配方已保存：" + edited.Name);
            lblStatus.Text = "配方已保存";
        }

        private void btnDeviceParam_Click(object sender, EventArgs e)
        {
            ModbusParam currentParam = TaskProcess.Instance.GetModbusParam();
            DeviceForm form = new DeviceForm();
            form.LoadParam(currentParam);
            DialogResult result = form.ShowDialog(this);
            bool clickedOk = result == DialogResult.OK;
            if (clickedOk == false)
            {
                return;
            }
            ModbusParam edited = form.GetParam();
            TaskProcess.Instance.SetParam(edited);  // 只存起来，重新启动采集才用新的
            AppendLog("设备参数已保存，重新按启动后生效");
            lblStatus.Text = "设备参数已保存，需重新启动采集";
        }

        // 只处理内置配方，自定义那套不在这里切
        private void cmbRecipe_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (loadingRecipeList)
            {
                return;
            }
            string selectedName = cmbRecipe.SelectedItem as string;
            bool validSelection = selectedName != null;
            if (validSelection == false)
            {
                return;
            }
            List<Recipe> builtInRecipes = Recipe.DefaultRecipes;
            for (int i = 0; i < builtInRecipes.Count; i++)
            {
                bool sameName = builtInRecipes[i].Name == selectedName;
                if (sameName)
                {
                    TaskProcess.Instance.SetRecipe(builtInRecipes[i]);
                    curvePanel.Invalidate();
                    AppendLog("已切换配方：" + selectedName);
                    lblStatus.Text = "配方已切换为 " + selectedName;
                    return;
                }
            }
        }

        private void btnAlarmConfirm_Click(object sender, EventArgs e)
        {
            int currentCount = CountAllAlarms();
            lastAlarmCount = currentCount;  // 认成已看过，声音不再响
            lblStatus.Text = "报警已确认，当前 " + currentCount + " 条";
            AppendLog("操作员确认报警 " + currentCount + " 条");
        }

        // 界面靠这个时钟驱动，采集线程只写数据不碰控件
        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            UpdateRunTime();
            UpdateConnectionCount();
            UpdateTemperature();
            RecordActualTemperature();
            UpdateAlarmList();
            SaveDataSnapshot();
            GridPanel.Invalidate();
        }
        //每10秒记一个温度点
        private void RecordActualTemperature()
        {
            bool noStart = runStartTime == DateTime.MinValue;//未启动
            if (noStart)
            {
                return;
            }
            TimeSpan sinceLast = DateTime.Now - lastSampleTime;
            bool tooSoon = sinceLast.TotalSeconds < SampleSeconds;
            if (tooSoon)
            {
                return;
            }
            List<ModbusStatus> modbusStatuses = TaskProcess.Instance.Status;
            ModbusStatus FristStatue = modbusStatuses[0];
            bool offline = FristStatue.ConnState != ConnectState.Connect;
            if (offline)
            {
                return;
            }
            TimeSpan runSpan = DateTime.Now - runStartTime;
            float elapsedMinutes = (float)runSpan.TotalMinutes;
            float temp = (float)FristStatue.Furnacetemperature;
            PointF sample =new PointF(elapsedMinutes, temp);
            actualSample.Add(sample);
            lastSampleTime = DateTime.Now;
            curvePanel.Invalidate();                              // 记完点叫面板重画，否则红线画上去也看不见
        }
        //画实际温度红线
        private void DrawActualTemp(PaintEventArgs e)
        {
            bool tooFewSample = actualSample.Count < 2;
            if (tooFewSample)
            {
                return;

            }
            List<Point> points = new List<Point>();
            for (int i = 0; i < actualSample.Count; i++) 
            {
                PointF sample = actualSample[i];
                double elapsedMinutes = sample.X;
                double temp = sample.Y;
                bool beyondCurve = elapsedMinutes > curveTotalMinutes;
                if (beyondCurve)
                {
                    break;
                }
                points.Add(GetPoint(elapsedMinutes, temp, curveTotalMinutes));
            }
            bool tooFewPoints = points.Count < 2;
            if (tooFewPoints) { return; }
            e.Graphics.DrawLines(Pens.Red, points.ToArray());
            
        }

        private void UpdateRunTime()
        {
            if (runStartTime == DateTime.MinValue)  // 还没按过启动
            {
                return;
            }
            TimeSpan span = DateTime.Now - runStartTime;
            lblRunTime.Text = "运行：" + span.ToString(@"hh\:mm\:ss");
        }

        private void UpdateConnectionCount()
        {
            int onlineCount = TaskProcess.Instance.DeviceCount;
            lblConnCount.Text = "在线：" + onlineCount + " / " + DeviceMax + " 台";
        }

        // 炉温只接在 1 号电源上
        private void UpdateTemperature()
        {
            List<ModbusStatus> statusList = TaskProcess.Instance.Status;
            ModbusStatus firstStatus = statusList[0];
            if (firstStatus.ConnState != ConnectState.Connect)
            {
                lblTemp.Text = "炉温：--.- ℃";  // 没连上就别把上次的值留着
                return;
            }
            double temperature = firstStatus.Furnacetemperature;
            lblTemp.Text = "炉温：" + temperature.ToString("F1") + " ℃";
        }

        private void UpdateAlarmList()
        {
            List<Alarm> allAlarms = CollectAllAlarms();
            int alarmCount = allAlarms.Count;
            bool alarmGrew = alarmCount > lastAlarmCount;
            lastAlarmCount = alarmCount;
            if (alarmGrew)
            {
                SystemSounds.Exclamation.Play();  // 只在变多时响一声，不然每秒都叫
            }
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < allAlarms.Count; i++)
            {
                Alarm alarm = allAlarms[i];
                builder.Append(alarm.Time.ToString("HH:mm:ss"));
                builder.Append("  ");
                builder.Append(alarm.Message);
                builder.Append("  (");
                builder.Append(alarm.Source);
                builder.Append(")");
                builder.Append(Environment.NewLine);
            }
            string newText = builder.ToString();
            if (newText == lastAlarmText)
            {
                return;  // 内容没变就别赋值，TextBox 重写会把滚动条弹回顶部
            }
            lastAlarmText = newText;
            txtAlarm.Text = newText;
            lblStatus.Text = "当前报警 " + alarmCount + " 条";
        }

        // 8 台的报警合一份，按时间排
        private List<Alarm> CollectAllAlarms()
        {
            List<ModbusStatus> statusList = TaskProcess.Instance.Status;
            List<Alarm> allAlarms = new List<Alarm>();
            for (int i = 0; i < statusList.Count; i++)
            {
                Alarm[] deviceAlarms = statusList[i].GetAlarmSnapshot();  // 必须拿快照，边遍历边加会抛
                for (int j = 0; j < deviceAlarms.Length; j++)
                {
                    allAlarms.Add(deviceAlarms[j]);
                }
            }
            allAlarms.Sort(CompareByTime);
            if (allAlarms.Count > 100)
            {
                int removeCount = allAlarms.Count - 100;
                allAlarms.RemoveRange(0, removeCount);  // 只留最近 100 条
            }
            return allAlarms;
        }

        private int CountAllAlarms()
        {
            List<Alarm> allAlarms = CollectAllAlarms();
            return allAlarms.Count;
        }

        private static int CompareByTime(Alarm left, Alarm right)
        {
            return left.Time.CompareTo(right.Time);
        }

        // 到没到 60 秒由 BatchRecorder 自己判断
        private void SaveDataSnapshot()
        {
            Recipe recipe = TaskProcess.Instance.GetRecipe();
            List<ModbusStatus> statusList = TaskProcess.Instance.Status;
            BatchRecorder.WriteSnapshot(statusList, recipe);
        }

        // 格子按 ClientSize 平分，窗口一缩放就跟着变
        private void GridPanel_Paint(object sender, PaintEventArgs e)
        {
            List<ModbusStatus> statusList = TaskProcess.Instance.Status;
            int cellWidth = GridPanel.ClientSize.Width / ChannelMax;
            int cellHeight = GridPanel.ClientSize.Height / DeviceMax;
            for (int device = 0; device < statusList.Count; device++)
            {
                ModbusStatus status = statusList[device];
                bool[] alarmChannels = BuildAlarmChannels(status);
                for (int channel = 0; channel < ChannelMax; channel++)
                {
                    Brush brush = PickBrush(status, alarmChannels, channel);
                    int left = channel * cellWidth;
                    int top = device * cellHeight;
                    int width = cellWidth - 1;  // 留 1 像素当格线
                    int height = cellHeight - 1;
                    e.Graphics.FillRectangle(brush, left, top, width, height);
                }
            }
        }

        // 这台设备哪几路在报警，整台性的报警算满 64 路
        private bool[] BuildAlarmChannels(ModbusStatus status)
        {
            bool[] result = new bool[ChannelMax];
            Alarm[] deviceAlarms = status.GetAlarmSnapshot();
            for (int i = 0; i < deviceAlarms.Length; i++)
            {
                Alarm alarm = deviceAlarms[i];
                bool deviceWide = IsDeviceWide(alarm.Type);
                if (deviceWide)
                {
                    for (int channel = 0; channel < ChannelMax; channel++)
                    {
                        result[channel] = true;
                    }
                    return result;
                }
                bool channelAlarm = IsChannelAlarm(alarm.Type);
                if (channelAlarm == false)
                {
                    continue;
                }
                if (alarm.Index < 0)  // 下标越界的报警直接丢
                {
                    continue;
                }
                if (alarm.Index >= ChannelMax)
                {
                    continue;
                }
                result[alarm.Index] = true;
            }
            return result;
        }

        private static bool IsDeviceWide(AlarmType type)
        {
            if (type == AlarmType.Powersupply)
            {
                return true;
            }
            if (type == AlarmType.TCPAlarm)
            {
                return true;
            }
            return false;
        }

        private static bool IsChannelAlarm(AlarmType type)
        {
            if (type == AlarmType.CurrentAlarm)
            {
                return true;
            }
            if (type == AlarmType.CurrentOpen)
            {
                return true;
            }
            if (type == AlarmType.VoltageAlarm)
            {
                return true;
            }
            return false;
        }

        private static Brush PickBrush(ModbusStatus status, bool[] alarmChannels, int channel)
        {
            if (status.ConnState != ConnectState.Connect)
            {
                return Brushes.Silver;
            }
            if (alarmChannels[channel])
            {
                return Brushes.Red;
            }
            return Brushes.LimeGreen;
        }

        // 采集层在后台线程调用，不能直接碰控件
        private void OnTaskProcessLog(string message)
        {
            if (txtLog.IsDisposed)
            {
                return;  // 窗口关掉后控件已经没了
            }
            if (txtLog.IsHandleCreated == false)
            {
                return;  // 句柄还没建出来，先丢掉
            }
            if (txtLog.InvokeRequired == false)
            {
                AppendLog(message);
                return;
            }
            txtLog.BeginInvoke(new Action<string>(AppendLog), message);
        }

        private void AppendLog(string message)
        {
            if (txtLog.Lines.Length > MaxLogLines)
            {
                txtLog.Clear();  // 到上限整个清掉，逐行删太慢
            }
            string line = DateTime.Now.ToString("HH:mm:ss") + "  " + message;
            txtLog.AppendText(line + Environment.NewLine);
        }

        // 关窗口得自己收尾，采集线程不放掉会卡住退出
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            refreshTimer.Stop();
            TaskProcess.Instance.Stop();
            BatchRecorder.Stop();
            base.OnFormClosing(e);
        }

        private void curvePanel_Paint(object sender, PaintEventArgs e)
        {
            Point[] curvePoints = BuildCurvePoints();               // 先把要画的点全算出来
            bool tooFewPoints = curvePoints.Length < 2;             // 少于两个点连不成线
            if (tooFewPoints)
            {
                return;                                             // 直接退出，什么都不画
            }
            e.Graphics.DrawLines(Pens.SteelBlue, curvePoints);
            DrawActualTemp(e);
            DrawNowMarker(e);                                     // 最后画“现在时刻”竖线// 目标线画完，再画实际线// 一次画完整条折线
        }

        private Point[] BuildCurvePoints()
        {
            List<TempCurveStep> steps = TaskProcess.Instance.GetRecipe().Curve;   // 读当前选中的配方
            int totalTime = 0;                                                    // 横轴总长度
            for (int stepIndex = 0; stepIndex < steps.Count; stepIndex++)
            {
                totalTime = totalTime + steps[stepIndex].RampMinutes + steps[stepIndex].HoldMinutes;
            }
            curveTotalMinutes = totalTime;
            bool noTime = totalTime == 0;                                         // 全 0 会除出 Infinity
            if (noTime)
            {
                return new Point[0];
            }

            int startTemp = 25;                                                   // 曲线起点

            // 第一遍：只扫纵轴范围，一个点都不算
            curveMinTemp = startTemp;
            curveMaxTemp = startTemp;
            for (int stepIndex = 0; stepIndex < steps.Count; stepIndex++)
            {
                TempCurveStep rangeStep = steps[stepIndex];
                bool lowerThanMin = rangeStep.TargetTemp < curveMinTemp;
                if (lowerThanMin)
                {
                    curveMinTemp = rangeStep.TargetTemp;
                }
                bool higherThanMax = rangeStep.TargetTemp > curveMaxTemp;
                if (higherThanMax)
                {
                    curveMaxTemp = rangeStep.TargetTemp;
                }
            }
            double tempSpan = curveMaxTemp - curveMinTemp;                        // 温度跨度
            bool flatCurve = tempSpan < 1;                                        // 全程同温，跨度为 0
            if (flatCurve)
            {
                curveMaxTemp = curveMinTemp + 10;                                 // 撑开一点，否则下面会除以 0
            }

            // 第二遍：范围已经定死，只算点
            List<Point> points = new List<Point>();
            int elapsedTime = 0;
            points.Add(GetPoint(elapsedTime, startTemp, totalTime));               // 起点
            for (int stepIndex = 0; stepIndex < steps.Count; stepIndex++)
            {
                TempCurveStep step = steps[stepIndex];
                elapsedTime = elapsedTime + step.RampMinutes;                     // 时间走到升温结束
                points.Add(GetPoint(elapsedTime, step.TargetTemp, totalTime));     // 到达目标温度
                elapsedTime = elapsedTime + step.HoldMinutes;                     // 时间走到保温结束
                points.Add(GetPoint(elapsedTime, step.TargetTemp, totalTime));     // 温度不变，画横线
            }
            return points.ToArray();
        }
        // 画“现在时刻”的灰色竖线，左上角写已经跑了多久
        private void DrawNowMarker(PaintEventArgs e)
        {
            bool noStart = runStartTime == DateTime.MinValue;                 // 没按过启动，不画
            if (noStart)
            {
                return;
            }
            bool noCurve = curveTotalMinutes <= 0;                            // 配方时长是 0，没法换算
            if (noCurve)
            {
                return;
            }
            TimeSpan runSpan = DateTime.Now - runStartTime;
            double elapsedMinutes = runSpan.TotalMinutes;                     // 已经跑了多少分钟
            bool beyondCurve = elapsedMinutes > curveTotalMinutes;             // 跑超了，竖线停在右边界
            if (beyondCurve)
            {
                elapsedMinutes = curveTotalMinutes;
            }
            Point markerTop = GetPoint(elapsedMinutes, curveMaxTemp, curveTotalMinutes);      // 上端：用曲线最高温
            Point markerBottom = GetPoint(elapsedMinutes, curveMinTemp, curveTotalMinutes);   // 下端：用曲线最低温
            e.Graphics.DrawLine(Pens.Gray, markerTop, markerBottom);                          // 灰笔，别跟两条线撞色
            double doneHours = elapsedMinutes / 60.0;                          // 已跑几小时
            double totalHours = curveTotalMinutes / 60.0;                      // 全程几小时
            string progressText = "已跑 " + doneHours.ToString("F1") + " / " + totalHours.ToString("F1") + " 小时";
            e.Graphics.DrawString(progressText, this.Font, Brushes.Black, 8, 4);   // 写在左上角
        }
        private Point GetPoint(double elaspedTime,double temp,int totalTime)
        {
            const int Margin = 20;//留白20
            
            int width = curvePanel.Width;
            int height = curvePanel.Height;
            int canUseWidth = width - Margin * 2;
            int canUseHight = height - Margin * 2;
            //先算出来一共多少个像素y轴，然后再根据时间对应 可用高除以最大值得到每个值对应的像素位置
            double y = height - Margin - canUseHight * (temp - curveMinTemp) / (curveMaxTemp - curveMinTemp);
            double topLimit = Margin;                             // 上边界：超了贴住上边框，别画到画布外
            double bottomLimit = height - Margin;                  // 下边界：超了贴住下边框
            bool aboveTop = y < topLimit;
            if (aboveTop)
            {
                y = topLimit;
            }
            bool belowBottom = y > bottomLimit;
            if (belowBottom)
            {
                y = bottomLimit;
            }
            double x = canUseWidth * (elaspedTime/totalTime)+Margin;
            int realY = (int)y;
            int realX = (int)x;
            return new Point(realX, realY);
        }
    }
}