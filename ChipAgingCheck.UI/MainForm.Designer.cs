namespace ChipAgingCheck.UI
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblRecipeCaption = new System.Windows.Forms.Label();
            this.cmbRecipe = new System.Windows.Forms.ComboBox();
            this.btnRecipeParameters = new System.Windows.Forms.Button();
            this.lblDeviceCaption = new System.Windows.Forms.Label();
            this.cmbDevice = new System.Windows.Forms.ComboBox();
            this.btnDeviceParam = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnAlarmConfirm = new System.Windows.Forms.Button();
            this.lblRunState = new System.Windows.Forms.Label();
            this.lblTemp = new System.Windows.Forms.Label();
            this.lblConnCount = new System.Windows.Forms.Label();
            this.lblBatch = new System.Windows.Forms.Label();
            this.lblRunTime = new System.Windows.Forms.Label();
            this.GridPanel = new System.Windows.Forms.Panel();
            this.txtAlarm = new System.Windows.Forms.TextBox();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.curvePanel = new System.Windows.Forms.Panel();
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabPageGrid = new System.Windows.Forms.TabPage();
            this.tabPageCurve = new System.Windows.Forms.TabPage();
            this.tabPageAlarm = new System.Windows.Forms.TabPage();
            this.tabMain.SuspendLayout();
            this.tabPageGrid.SuspendLayout();
            this.tabPageCurve.SuspendLayout();
            this.tabPageAlarm.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("阿里巴巴普惠体 3.0 65 Medium", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblTitle.Location = new System.Drawing.Point(16, 10);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(204, 42);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "芯片老化检测";
            // 
            // lblRecipeCaption
            // 
            this.lblRecipeCaption.AutoSize = true;
            this.lblRecipeCaption.Location = new System.Drawing.Point(16, 66);
            this.lblRecipeCaption.Name = "lblRecipeCaption";
            this.lblRecipeCaption.Size = new System.Drawing.Size(54, 28);
            this.lblRecipeCaption.TabIndex = 1;
            this.lblRecipeCaption.Text = "配方";
            // 
            // cmbRecipe
            // 
            this.cmbRecipe.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRecipe.Location = new System.Drawing.Point(86, 60);
            this.cmbRecipe.Name = "cmbRecipe";
            this.cmbRecipe.Size = new System.Drawing.Size(220, 36);
            this.cmbRecipe.TabIndex = 2;
            // 
            // btnRecipeParameters
            // 
            this.btnRecipeParameters.Location = new System.Drawing.Point(318, 58);
            this.btnRecipeParameters.Name = "btnRecipeParameters";
            this.btnRecipeParameters.Size = new System.Drawing.Size(130, 40);
            this.btnRecipeParameters.TabIndex = 3;
            this.btnRecipeParameters.Text = "配方配置";
            this.btnRecipeParameters.UseVisualStyleBackColor = true;
            // 
            // lblDeviceCaption
            // 
            this.lblDeviceCaption.AutoSize = true;
            this.lblDeviceCaption.Location = new System.Drawing.Point(470, 66);
            this.lblDeviceCaption.Name = "lblDeviceCaption";
            this.lblDeviceCaption.Size = new System.Drawing.Size(54, 28);
            this.lblDeviceCaption.TabIndex = 4;
            this.lblDeviceCaption.Text = "设备";
            // 
            // cmbDevice
            // 
            this.cmbDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDevice.Location = new System.Drawing.Point(540, 60);
            this.cmbDevice.Name = "cmbDevice";
            this.cmbDevice.Size = new System.Drawing.Size(200, 36);
            this.cmbDevice.TabIndex = 5;
            // 
            // btnDeviceParam
            // 
            this.btnDeviceParam.Location = new System.Drawing.Point(752, 58);
            this.btnDeviceParam.Name = "btnDeviceParam";
            this.btnDeviceParam.Size = new System.Drawing.Size(130, 40);
            this.btnDeviceParam.TabIndex = 6;
            this.btnDeviceParam.Text = "设备配置";
            this.btnDeviceParam.UseVisualStyleBackColor = true;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(16, 112);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(140, 52);
            this.btnStart.TabIndex = 7;
            this.btnStart.Text = "启动";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(172, 112);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(140, 52);
            this.btnStop.TabIndex = 8;
            this.btnStop.Text = "停止";
            this.btnStop.UseVisualStyleBackColor = true;
            // 
            // btnAlarmConfirm
            // 
            this.btnAlarmConfirm.Location = new System.Drawing.Point(0, 0);
            this.btnAlarmConfirm.Name = "btnAlarmConfirm";
            this.btnAlarmConfirm.Size = new System.Drawing.Size(75, 23);
            this.btnAlarmConfirm.TabIndex = 21;
            // 
            // lblRunState
            // 
            this.lblRunState.AutoSize = true;
            this.lblRunState.Location = new System.Drawing.Point(492, 124);
            this.lblRunState.Name = "lblRunState";
            this.lblRunState.Size = new System.Drawing.Size(138, 28);
            this.lblRunState.TabIndex = 9;
            this.lblRunState.Text = "状态：未运行";
            // 
            // lblTemp
            // 
            this.lblTemp.AutoSize = true;
            this.lblTemp.Location = new System.Drawing.Point(900, 66);
            this.lblTemp.Name = "lblTemp";
            this.lblTemp.Size = new System.Drawing.Size(138, 28);
            this.lblTemp.TabIndex = 10;
            this.lblTemp.Text = "炉温：--.- ℃";
            // 
            // lblConnCount
            // 
            this.lblConnCount.AutoSize = true;
            this.lblConnCount.Location = new System.Drawing.Point(1090, 66);
            this.lblConnCount.Name = "lblConnCount";
            this.lblConnCount.Size = new System.Drawing.Size(151, 28);
            this.lblConnCount.TabIndex = 11;
            this.lblConnCount.Text = "在线：0 / 8 台";
            // 
            // lblBatch
            // 
            this.lblBatch.AutoSize = true;
            this.lblBatch.Location = new System.Drawing.Point(1090, 124);
            this.lblBatch.Name = "lblBatch";
            this.lblBatch.Size = new System.Drawing.Size(138, 28);
            this.lblBatch.TabIndex = 12;
            this.lblBatch.Text = "批次：未开始";
            // 
            // lblRunTime
            // 
            this.lblRunTime.AutoSize = true;
            this.lblRunTime.Location = new System.Drawing.Point(900, 124);
            this.lblRunTime.Name = "lblRunTime";
            this.lblRunTime.Size = new System.Drawing.Size(171, 28);
            this.lblRunTime.TabIndex = 13;
            this.lblRunTime.Text = "运行：00:00:00";
            // 
            // GridPanel
            // 
            this.GridPanel.BackColor = System.Drawing.Color.White;
            this.GridPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.GridPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GridPanel.Location = new System.Drawing.Point(0, 0);
            this.GridPanel.Name = "GridPanel";
            this.GridPanel.Size = new System.Drawing.Size(1252, 476);
            this.GridPanel.TabIndex = 15;
            this.GridPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.GridPanel_Paint);
            // 
            // txtAlarm
            // 
            this.txtAlarm.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.txtAlarm.BackColor = System.Drawing.Color.White;
            this.txtAlarm.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtAlarm.Location = new System.Drawing.Point(8, 8);
            this.txtAlarm.Multiline = true;
            this.txtAlarm.Name = "txtAlarm";
            this.txtAlarm.ReadOnly = true;
            this.txtAlarm.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtAlarm.Size = new System.Drawing.Size(600, 460);
            this.txtAlarm.TabIndex = 17;
            // 
            // txtLog
            // 
            this.txtLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLog.BackColor = System.Drawing.Color.White;
            this.txtLog.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtLog.Location = new System.Drawing.Point(616, 8);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(628, 460);
            this.txtLog.TabIndex = 19;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblStatus.ForeColor = System.Drawing.Color.DimGray;
            this.lblStatus.Location = new System.Drawing.Point(16, 734);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(39, 20);
            this.lblStatus.TabIndex = 20;
            this.lblStatus.Text = "就绪";
            // 
            // curvePanel
            // 
            this.curvePanel.BackColor = System.Drawing.Color.White;
            this.curvePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.curvePanel.Location = new System.Drawing.Point(0, 0);
            this.curvePanel.Name = "curvePanel";
            this.curvePanel.Size = new System.Drawing.Size(1252, 476);
            this.curvePanel.TabIndex = 22;
            this.curvePanel.Paint += new System.Windows.Forms.PaintEventHandler(this.curvePanel_Paint);
            // 
            // tabMain
            // 
            this.tabMain.Controls.Add(this.tabPageGrid);
            this.tabMain.Controls.Add(this.tabPageCurve);
            this.tabMain.Controls.Add(this.tabPageAlarm);
            this.tabMain.Location = new System.Drawing.Point(16, 178);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(1260, 508);
            this.tabMain.TabIndex = 14;
            // 
            // tabPageGrid
            // 
            this.tabPageGrid.Controls.Add(this.GridPanel);
            this.tabPageGrid.Location = new System.Drawing.Point(4, 30);
            this.tabPageGrid.Name = "tabPageGrid";
            this.tabPageGrid.Size = new System.Drawing.Size(1252, 474);
            this.tabPageGrid.TabIndex = 0;
            this.tabPageGrid.Text = "实时状态";
            this.tabPageGrid.UseVisualStyleBackColor = true;
            // 
            // tabPageCurve
            // 
            this.tabPageCurve.Controls.Add(this.curvePanel);
            this.tabPageCurve.Location = new System.Drawing.Point(4, 30);
            this.tabPageCurve.Name = "tabPageCurve";
            this.tabPageCurve.Size = new System.Drawing.Size(1252, 474);
            this.tabPageCurve.TabIndex = 1;
            this.tabPageCurve.Text = "温度曲线";
            this.tabPageCurve.UseVisualStyleBackColor = true;
            // 
            // tabPageAlarm
            // 
            this.tabPageAlarm.Controls.Add(this.txtAlarm);
            this.tabPageAlarm.Controls.Add(this.txtLog);
            this.tabPageAlarm.Location = new System.Drawing.Point(4, 30);
            this.tabPageAlarm.Name = "tabPageAlarm";
            this.tabPageAlarm.Size = new System.Drawing.Size(1252, 474);
            this.tabPageAlarm.TabIndex = 2;
            this.tabPageAlarm.Text = "报警 · 日志";
            this.tabPageAlarm.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1300, 768);
            this.Controls.Add(this.tabMain);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblRunTime);
            this.Controls.Add(this.lblBatch);
            this.Controls.Add(this.lblConnCount);
            this.Controls.Add(this.lblTemp);
            this.Controls.Add(this.lblRunState);
            this.Controls.Add(this.btnAlarmConfirm);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnDeviceParam);
            this.Controls.Add(this.cmbDevice);
            this.Controls.Add(this.lblDeviceCaption);
            this.Controls.Add(this.btnRecipeParameters);
            this.Controls.Add(this.cmbRecipe);
            this.Controls.Add(this.lblRecipeCaption);
            this.Controls.Add(this.lblTitle);
            this.Font = new System.Drawing.Font("阿里巴巴普惠体 3.0 65 Medium", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "芯片老化检测";
            this.tabPageAlarm.ResumeLayout(false);
            this.tabPageAlarm.PerformLayout();
            this.tabPageCurve.ResumeLayout(false);
            this.tabPageGrid.ResumeLayout(false);
            this.tabMain.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblRecipeCaption;
        private System.Windows.Forms.ComboBox cmbRecipe;
        private System.Windows.Forms.Button btnRecipeParameters;
        private System.Windows.Forms.Label lblDeviceCaption;
        private System.Windows.Forms.ComboBox cmbDevice;
        private System.Windows.Forms.Button btnDeviceParam;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnAlarmConfirm;
        private System.Windows.Forms.Label lblRunState;
        private System.Windows.Forms.Label lblTemp;
        private System.Windows.Forms.Label lblConnCount;
        private System.Windows.Forms.Label lblBatch;
        private System.Windows.Forms.Label lblRunTime;
        private System.Windows.Forms.Panel GridPanel;
        private System.Windows.Forms.TextBox txtAlarm;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Panel curvePanel;
        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tabPageGrid;
        private System.Windows.Forms.TabPage tabPageCurve;
        private System.Windows.Forms.TabPage tabPageAlarm;
    }
}