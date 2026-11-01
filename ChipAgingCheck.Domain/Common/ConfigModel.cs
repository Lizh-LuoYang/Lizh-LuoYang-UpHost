using ChipAgingCheck.Domain.Modbus;

namespace ChipAgingCheck.Domain.Common
{
    // 配置文件的外壳：版本号 + 设备参数 + 当前配方
    public class ConfigModel
    {
        // 加字段就 +1。老版本文件不猜怎么迁，直接重建，省得养兼容分支
        public int Version { get; set; } = 1;

        public ModbusParam Param { get; set; } = new ModbusParam();

        public Recipe Recipe { get; set; } = new Recipe();
    }
}
