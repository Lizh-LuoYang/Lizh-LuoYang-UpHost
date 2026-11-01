using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChipAgingCheck.Domain.Modbus
{
    public class ModbusParam
    {
        [Category("1.连接参数")]
        [DisplayName("IP地址")]
        public string IP { get; set; } 
  
 
        [Category("1.连接参数")]
        [DisplayName("端口号")]//1-8通用
        public int Port { get; set; } = 502;
        [Category("1.连接参数")]
        [DisplayName("超时")]//1-8通用
        public int TimeoutMs { get; set; } = 1000;


        [Category("1.连接参数")]
        [DisplayName("从站地址")]//1-8通用
        public byte SlaveId { get; set; } = 1;
        [Category("2.寄存器地址")]
        [DisplayName("炉温")]//1号电源才有
        public ushort FurnacetemperatureAddress { get; set; } = 0;//一个
        [Category("2.寄存器地址")]
        [DisplayName("炉温换算系数")]//1号电源才有
        public double FurnacetemperatureDev { get; set; } = 10.0;
        [Category("2.寄存器地址")]
        [DisplayName("电流起始地址")]//电流寄存器起始地址
        public ushort CurrentAddress { get; set; } = 1;//往后到64
        [Category("2.寄存器地址")]
        [DisplayName("电压起始地址")]//电压寄存器起始地址
        public ushort VoltageAddress { get; set; } = 65;//往后到128
        
        [Category("2.寄存器地址")]
        [DisplayName("电源地址")]//电源状态寄存器起始地址
        public ushort PowersupplyAddress { get; set; } = 129;//129	电源状态	0 待机 / 1 输出中 / 2 故障
        
        [Category("2.寄存器地址")]
        [DisplayName("故障码地址")]//故障码寄存器起始地址
        public ushort ErrorCodeAddress { get; set; } = 130;//130	故障码	0 = 无

    }
}
