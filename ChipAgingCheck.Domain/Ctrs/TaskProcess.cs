using ChipAgingCheck.Domain.Interface;
using ChipAgingCheck.Domain.Modbus;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading;
using ChipAgingCheck.Domain.Common;

namespace ChipAgingCheck.Domain.Ctrs
{
    public class TaskProcess
    {
        private static TaskProcess _instance;
        private static readonly object _lockObj = new object();

        // 双重检查：只有第一次取实例时才真进 lock
        public static TaskProcess Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new TaskProcess();
                        }
                    }
                }
                return _instance;
            }
        }

        // 8 是写死的，改台数得连着 IP 列表、TryConntectTimes 一起改
        private TaskProcess()
        {
            for (int i = 0; i < 8; i++)
            {
                Status.Add(new ModbusStatus());
            }
            for (int i = 0; i < 8; i++) { master.Add(null); }
            ConfigModel config = ConfigManager.Load(); // 参数和配方都存在配置文件里
            _param = config.Param;
            _recipe = config.Recipe;
        }

        public Thread MasterThread = null;

        private ModbusParam _param = new ModbusParam();
        public List<ModbusStatus> Status = new List<ModbusStatus>();
        private const int MaxRetryDelayMs = 1500; // 重连退避封顶，不然断得越久等得越长
        private bool isRunning = false;
        private int[] TryConntectTimes = new int[8];
        public string LastError = ""; // 最后一次出错的原因，界面和日志
        private Recipe _recipe = new Recipe();
        public ModbusParam GetModbusParamRef()
        {
            lock (_lockObj)
            {
                return _param;
            }
        }
        // 带 Ref 的给的是引用，改之前自己加锁；不带的给副本
        public ModbusParam GetModbusParam()
        {
            lock (_lockObj)
            {
                return _param.DeepCopy();
            }
        }
        public Recipe GetRecipeRef()
        {
            lock (_lockObj)
            {
                return _recipe;
            }
        }
        public Recipe GetRecipe()
        {
            lock (_lockObj)
            {
                return _recipe.DeepCopy();
            }
        }
        // 现场要按实际 IP 改，现在这几个是回环地址占位
        private List<string> IP = new List<string>()
        {
            "127.0.0.1",
            "127.0.0.2",
            "127.0.0.3",
            "127.0.0.4",
            "127.0.0.5",
            "127.0.0.6",
            "127.0.0.7",
            "127.0.0.8",
        };
        private List<IModbus> master = new List<IModbus>();
        public event Action<string> Log;
        public event Action<List<ModbusStatus>> DataChange;
        public void ChangeState(ConnectState state, int index)
        {
            if (Status[index].ConnState == state)
            {
                return;
            }
            Status[index].ConnState = state;
            Logger.LogInfo("第 " + (index + 1) + " 台状态变为 " + state);
        }
        public int DeviceCount
        {
            get
            {
                int count = 0;
                for (int i = 0; i < master.Count; i++)
                {
                    if (master[i] != null) { count++; } // 空槽位不算
                }
                return count;
            }
        }
        public void Start()
        {
            try
            {
                if (isRunning) // 线程已经在跑，别再开一条
                {
                    return;
                }

                // 先连一遍，连不上的放着，等 Run 里按退避重连
                for (int i = 0; i < IP.Count; i++)
                {
                    var param = GetModbusParam();
                    param.IP = IP[i];
                    ModbusModel model = new ModbusModel(param);
                    bool isConnect = model.Connect();

                    if (isConnect)
                    {
                        master[i] = model;
                        ChangeState(ConnectState.Connect, i);
                        Log?.Invoke($"设备{i}连接成功");
                    }
                }
                MasterThread = new Thread(Run)
                {
                    IsBackground = true,
                    Priority = ThreadPriority.AboveNormal
                };
                isRunning = true;
                MasterThread.Start();
                Log?.Invoke("开始采集数据" + DateTime.Now);
            }
            catch (System.Exception ex)
            {
                Log?.Invoke($"启动出错{ex.ToString()}");
            }
        }

        // 界面改完配方后调：先在内存里换掉，再落盘
        public void SetRecipe(Recipe recipe)
        {
            if (recipe == null)
            {
                return;
            }
            lock (_lockObj)
            {
                _recipe = recipe.DeepCopy();
            }
            ConfigModel config = ConfigManager.Load();
            config.Recipe = recipe.DeepCopy();
            ConfigManager.Save(config);
        }

        public void SetParam(ModbusParam param)
        {
            if (param == null)
            {
                return;
            }
            lock (_lockObj)
            {
                _param = param.DeepCopy();
            }
            ConfigModel config = ConfigManager.Load();
            config.Param = param.DeepCopy();
            ConfigManager.Save(config);
        }
        public void Stop()
        {
            isRunning = false; // Run 的循环跑到下一圈就自己退出
            if (MasterThread != null)
            {
                MasterThread.Join(2000); // 最多等 2 秒，别把界面卡死
            }
            for (int i = 0; i < master.Count; i++)
            {
                if (master[i] == null)
                {
                    continue;
                }
                master[i].DisConnect();
                master[i] = null; // 槽位清空，下次 Start 才会重新连
                ChangeState(ConnectState.DisConnect, i);
            }
            Log?.Invoke("已停止采集");
        }

        private void HandleData()
        {
            var param = GetModbusParam(); // 8 台共用一份
            for (int i = 0; i < Status.Count; i++)
            {
                ReadDevice(i, param);
            }
            CheckAlarm();
        }

        // 读一台的电流、电压、电源状态、故障码，0 号电源上还挂着炉温。
        // 哪一步抛异常就只把这台标离线，下一圈 Run 会去重连
        private void ReadDevice(int index, ModbusParam param)
        {
            if (master[index] == null)
            {
                ChangeState(ConnectState.DisConnect, index);
                return; // 从来没连上过，没读的
            }
            try
            {
                ushort[] current = master[index].ReadHoldingRegiseter(param.CurrentAddress, (ushort)(param.VoltageAddress - param.CurrentAddress));
                ushort[] voltage = master[index].ReadHoldingRegiseter(param.VoltageAddress, (ushort)(param.PowersupplyAddress - param.VoltageAddress));

                double[] valueCurrent = new double[current.Length];
                double[] valueVoltage = new double[voltage.Length];
                for (int j = 0; j < current.Length; j++)
                {
                    valueCurrent[j] = current[j] / 10.0; // 寄存器放大了 10 倍，除回来才是 mA
                    valueVoltage[j] = voltage[j] / 10.0;
                }
                Status[index].Current = valueCurrent;
                Status[index].Voltage = valueVoltage;

                ushort[] stateCode = master[index].ReadHoldingRegiseter(param.PowersupplyAddress, 1);
                Status[index].powersupplyState = GetPowerSuppolyState(stateCode[0]); // 0 待机 / 1 输出中 / 2 故障

                ushort[] errorCode = master[index].ReadHoldingRegiseter(param.ErrorCodeAddress, 1);
                Status[index].ErrorCode = errorCode[0]; // 0 就是没故障
                if (index == 0) // 炉温只接在 1 号电源上
                {
                    ushort[] tempRaw = master[index].ReadHoldingRegiseter(param.FurnacetemperatureAddress, 1);
                    Status[0].Furnacetemperature = tempRaw[0] / param.FurnacetemperatureDev; // Dev 一般是 10，不除就只有整数
                }
            }
            catch (Exception ex)
            {
                ChangeState(ConnectState.DisConnect, index); // 只标这一台，别的台照读
                Log?.Invoke(ex.Message); // 原因留给日志和界面，不往上抛
            }
        }

        private PowersupplyState GetPowerSuppolyState(ushort code)
        {
            PowersupplyState state = PowersupplyState.Standby;
            if (code == 1)
            {
                state = PowersupplyState.Outputting;
            }
            if (code == 2)
            {
                state = PowersupplyState.Malfunction;
            }
            return state;
        }
        private void Run()
        {
            try
            {
                while (isRunning)
                {
                    for (int i = 0; i < Status.Count; i++)
                    {
                        if (Status[i].ConnState == ConnectState.DisConnect)
                        {
                            if (DateTime.Now >= Status[i].NextRetryTime) // 退避时间没到就别去打扰它
                            {
                                TryConnect(i);
                            }
                        }
                    }
                    HandleData();
                    DataChange?.Invoke(Status);
                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("采集循环异常", ex);
                Log?.Invoke(ex.Message);
            }
        }

        private void CheckAlarm()
        {
            Recipe recipe = GetRecipe();
            CheckTemperatureAlarm(recipe);
            for (int i = 0; i < Status.Count; i++)
            {
                if (Status[i].Current == null || Status[i].Voltage == null) { continue; }
                CheckChannelAlarms(i, recipe);
                CheckDeviceAlarms(i);
            }
        }

        // 炉温全炉只有一个值，超限也只报一条，Index 固定 0
        private void CheckTemperatureAlarm(Recipe recipe)
        {
            ModbusStatus first = Status[0];
            bool connected = first.ConnState == ConnectState.Connect;
            if (connected == false) // 没连上时读数是 0，判了就是误报
            {
                return;
            }
            double temperature = first.Furnacetemperature;
            double highLimit = recipe.TempHigh + recipe.TempTolerance;
            double lowLimit = recipe.TempLow - recipe.TempTolerance;
            bool reported = first.HasAlarm(AlarmType.TempAlarm, 0);
            if (temperature > highLimit)
            {
                if (reported == false)
                {
                    string message = "炉温过高：" + temperature.ToString("F1") + " ℃";
                    RaiseAlarm(0, AlarmType.TempAlarm, 0, "温度模块", AlarmLevel.Fatal, message);
                }
                return;
            }
            if (temperature < lowLimit)
            {
                if (reported == false)
                {
                    string message = "炉温过低：" + temperature.ToString("F1") + " ℃";
                    RaiseAlarm(0, AlarmType.TempAlarm, 0, "温度模块", AlarmLevel.Fatal, message);
                }
                return;
            }
            first.RemoveChannelAlarm(AlarmType.TempAlarm, 0); // 回到区间里就把这条撤掉
        }

        // 通道级三项：电流超限、电流开路、电压超限，各判各的，不嵌套
        private void CheckChannelAlarms(int deviceIndex, Recipe recipe)
        {
            ModbusStatus status = Status[deviceIndex];
            int channelCount = status.Current.Length;
            for (int channel = 0; channel < channelCount; channel++)
            {
                int position = deviceIndex * 64 + channel + 1; // 位置号，界面上显示第几颗

                double current = status.Current[channel];
                bool currentOver = current > recipe.CurrentOverThresh;
                // HasAlarm 为 true 说明这条还没撤，超限期间只报一次
                if (currentOver)
                {
                    bool reported = status.HasAlarm(AlarmType.CurrentAlarm, channel);
                    if (reported == false)
                    {
                        string message = "第 " + position + " 号位超限：" + current.ToString("F1") + " mA";
                        RaiseAlarm(deviceIndex, AlarmType.CurrentAlarm, channel, "电流模块", AlarmLevel.error, message);
                    }
                }
                else
                {
                    status.RemoveChannelAlarm(AlarmType.CurrentAlarm, channel);
                }

                bool currentOpen = current < recipe.CurrentNearZeroThresh;
                if (currentOpen)
                {
                    bool reported = status.HasAlarm(AlarmType.CurrentOpen, channel);
                    if (reported == false)
                    {
                        string message = "第 " + position + " 号位 开路：" + current.ToString("F1") + " mA";
                        RaiseAlarm(deviceIndex, AlarmType.CurrentOpen, channel, "电流模块", AlarmLevel.error, message);
                    }
                }
                else
                {
                    status.RemoveChannelAlarm(AlarmType.CurrentOpen, channel);
                }

                double voltage = status.Voltage[channel];
                bool voltageOver = voltage > recipe.VoltageOverThresh;
                if (voltageOver)
                {
                    bool reported = status.HasAlarm(AlarmType.VoltageAlarm, channel);
                    if (reported == false)
                    {
                        string message = "第 " + position + " 号位 电压超限：" + voltage.ToString("F1") + " V";
                        RaiseAlarm(deviceIndex, AlarmType.VoltageAlarm, channel, "电压模块", AlarmLevel.error, message);
                    }
                }
                else
                {
                    status.RemoveChannelAlarm(AlarmType.VoltageAlarm, channel);
                }
            }
        }

        // 整台级的：电源故障、错误码非 0
        private void CheckDeviceAlarms(int deviceIndex)
        {
            ModbusStatus status = Status[deviceIndex];
            int deviceNumber = deviceIndex + 1;

            bool malfunction = status.powersupplyState == PowersupplyState.Malfunction;
            if (malfunction)
            {
                bool reported = status.HasAlarm(AlarmType.Powersupply, deviceIndex);
                if (reported == false)
                {
                    string message = "第 " + deviceNumber + " 台电源模块故障";
                    RaiseAlarm(deviceIndex, AlarmType.Powersupply, deviceIndex, "电源模块", AlarmLevel.Fatal, message);
                }
            }
            else
            {
                status.RemoveChannelAlarm(AlarmType.Powersupply, deviceIndex);
            }

            bool hasErrorCode = status.ErrorCode != 0;
            if (hasErrorCode)
            {
                bool reported = status.HasAlarm(AlarmType.TCPAlarm, deviceIndex);
                if (reported == false)
                {
                    string message = "第 " + deviceNumber + " 台错误码：" + status.ErrorCode;
                    RaiseAlarm(deviceIndex, AlarmType.TCPAlarm, deviceIndex, "错误码", AlarmLevel.error, message);
                }
            }
            else
            {
                status.RemoveChannelAlarm(AlarmType.TCPAlarm, deviceIndex);
            }
        }

        // 加报警 + 立刻写 CSV + 写日志，三件事一处做，别处漏了就查不到
        private void RaiseAlarm(int deviceIndex, AlarmType type, int index, string source, AlarmLevel level, string message)
        {
            Alarm alarm = new Alarm
            {
                Index = index,
                Source = source,
                Type = type,
                Level = level,
                Message = message,
                Time = DateTime.Now
            };
            Status[deviceIndex].AddAlarm(alarm);
            BatchRecorder.WriteAlarm(alarm, deviceIndex);
            Log?.Invoke(message);
        }

        private void TryConnect(int index)
        {
            try
            {
                var param = GetModbusParam();
                if (Status[index].ConnState == ConnectState.Connect)
                {
                    return;
                }
                if (master[index] == null)
                {
                    master[index] = new ModbusModel(param); // 先建一台放进去，不然下面就是空引用
                }
                master[index].DisConnect();

                bool isSuccess = master[index].Connect();
                TryConntectTimes[index]++;
                if (isSuccess)
                {
                    ChangeState(ConnectState.Connect, index);
                    TryConntectTimes[index] = 0;
                    Log?.Invoke($"第{index}重连成功" + DateTime.Now.ToString());
                }
                else
                {
                    int delayMs = Math.Min(300 * TryConntectTimes[index], MaxRetryDelayMs);
                    Status[index].NextRetryTime = DateTime.Now.AddMilliseconds(delayMs);
                    Log?.Invoke($"正在重连，需等待时间：{delayMs}Ms");
                }
            }
            catch (Exception ex)
            {
                Log?.Invoke($"重连失败:{index}号" + ex.Message);
            }
        }
    }
}
