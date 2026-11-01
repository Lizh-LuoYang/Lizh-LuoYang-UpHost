using System;

namespace ChipAgingCheck.Domain
{
    public enum AlarmType
    {
        TempAlarm,
        CurrentAlarm,        // 电流超限
        CurrentOpen,         // 电流开路
        VoltageAlarm,
        TCPAlarm,
        Powersupply
    }
    public enum AlarmLevel
    {
        info,
        Warning,
        error,
        Fatal
    }
    public class Alarm
    {
        public string Message { get; set; } = "";
        public DateTime Time { get; set; }
        public AlarmType Type { get; set; }
        public AlarmLevel Level { get; set; }
        public string Source { get; set; }
        //设备索引为0-7
        public int Index {  get; set; }

    }
}
