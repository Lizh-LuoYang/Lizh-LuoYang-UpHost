using System;
using System.Collections.Generic;
using System.Linq;

namespace ChipAgingCheck.Domain.Modbus
{
    public enum ConnectState
    {
        Connect,
        Connecting,
        DisConnect
    }

    // 电源状态：0 待机 / 1 输出中 / 2 故障
    public enum PowersupplyState
    {
        Standby = 0,
        Outputting = 1,
        Malfunction = 2
    }

    public class ModbusStatus
    {
        private object AlarmLock = new object();

        public ConnectState ConnState { get; set; } = ConnectState.DisConnect;

        // 1-64 路，单位 mA
        public double[] Current { get; set; }

        // 1-64 路，单位 V
        public double[] Voltage { get; set; }

        public double Furnacetemperature { get; set; }

        public List<Alarm> Alarms = new List<Alarm>();

        public PowersupplyState powersupplyState = PowersupplyState.Malfunction;

        // 故障码，0 表示无故障
        public ushort ErrorCode { get; set; }

        public DateTime NextRetryTime = DateTime.MinValue;

        // 界面遍历报警时走这个，不然采集线程正在 Add，界面这边会抛
        public Alarm[] GetAlarmSnapshot()
        {
            lock (AlarmLock)
            {
                return Alarms.ToArray();
            }
        }

        public void AddAlarm(Alarm alarm)
        {
            lock (AlarmLock)
            {
                if (Alarms.Count > 500)
                {
                    Alarms.RemoveAt(0);
                }

                if (Alarms.Count == 0)
                {
                    Alarms.Add(alarm);
                    return;
                }

                Alarm last = GetLastAlarm();
                bool sameAsLast = alarm.Type == last.Type && alarm.Index == last.Index;
                if (sameAsLast)
                {
                    return;
                }

                Alarms.Add(alarm);
            }
        }

        public Alarm GetLastAlarm()
        {
            lock (AlarmLock)
            {
                return Alarms.Last();
            }
        }

        // 这一路这类报警已经有了吗
        public bool HasAlarm(AlarmType type, int channelIndex)
        {
            for (int i = 0; i < Alarms.Count; i++)
            {
                if (Alarms[i].Type == type && Alarms[i].Index == channelIndex)
                {
                    return true;
                }
            }
            return false;
        }

        // 这一路恢复正常了，把它的报警撤掉
        public void RemoveChannelAlarm(AlarmType type, int channelIndex)
        {
            // 从后往前删，下标才不会乱
            for (int i = Alarms.Count - 1; i >= 0; i--)
            {
                if (Alarms[i].Type == type && Alarms[i].Index == channelIndex)
                {
                    Alarms.RemoveAt(i);
                }
            }
        }
    }
}