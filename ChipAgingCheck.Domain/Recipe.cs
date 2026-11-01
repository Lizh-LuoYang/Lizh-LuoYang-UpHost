using System.Collections.Generic;
using System.ComponentModel;

namespace ChipAgingCheck.Domain
{
    /// <summary>老化配方</summary>
    public class Recipe
    {
        [Category("0.配方信息")]
        [DisplayName("配方名称")]
        public string Name { get; set; } = "默认配方";

        [Category("0.配方信息")]
        [DisplayName("产品型号")]
        public string ProductModel { get; set; } = "TEST-001";

        [Category("1.温度条件")]
        [DisplayName("温度曲线")]
        public List<TempCurveStep> Curve { get; set; } = new List<TempCurveStep>();

        [Category("1.温度条件")]
        [DisplayName("目标恒温温度(℃)")]
        public double TempTarget { get; set; } = 25.0;

        [Category("1.温度条件")]
        [DisplayName("温度上限(℃)")]
        public double TempHigh { get; set; } = 80.0;

        [Category("1.温度条件")]
        [DisplayName("温度下限(℃)")]
        public double TempLow { get; set; } = -20.0;

        [Category("1.温度条件")]
        [DisplayName("恒温时长(h)")]
        public double HoldHour { get; set; } = 24;

        [Category("2.判定条件")]
        [DisplayName("温度允许偏差(±℃)")]
        public double TempTolerance { get; set; } = 5.0;

        [Category("2.判定条件")]
        [DisplayName("连续N次无数据报警")]
        public int NoDataAlarmCount { get; set; } = 3;

        [Category("2.判定条件")]
        [DisplayName("电流近似0阈值比例(%)")]
        [Description("工艺：电流低于设定电流的该比例，判定≈0，默认10")]
        public double CurrentNearZeroPercent { get; set; } = 10.0;

        [Category("2.判定条件")]
        [DisplayName("电流超限比例(倍)")]
        [Description("工艺：电流超过设定电流该倍数判定电源异常，默认1.2")]
        public double CurrentOverMultiplier { get; set; } = 1.2;

        [Category("2.判定条件")]
        [DisplayName("电压超限比例(倍)")]
        [Description("工艺：电压超过限压该倍数判定电压偏高，默认1.05")]
        public double VoltageOverMultiplier { get; set; } = 1.05;

        [Category("3.数据记录")]
        [DisplayName("是否记录数据")]
        public bool RecordData { get; set; } = true;

        [Category("4.电流条件")]
        [DisplayName("设定电流(mA)")]
        public double SetCurrentMa { get; set; } = 10.0;

        // 上下限不再写死，全按比例算：开路阈值 = 设定电流×比例，超限阈值 = 设定电流×倍数

        [Category("5.电压条件")]
        [DisplayName("限压(V)")]
        public double ComplianceVoltage { get; set; } = 3.0;

        // 电压超限 = 限压×倍数；短路判定工艺没给比例，先留个 0.2V 的固定阈值

        /// <summary>下拉框直接显示配方名</summary>
        public override string ToString() => Name;

        public static List<Recipe> DefaultRecipes => new List<Recipe>()
        {
            new Recipe
            {
                Name = "常高温",
                ProductModel = "TEST-HT-01",
                TempTarget = 85,
                TempHigh = 90,
                TempLow = 80,
                HoldHour = 48,
                TempTolerance = 5,
                NoDataAlarmCount = 3,
                CurrentNearZeroPercent = 10,
                CurrentOverMultiplier = 1.2,
                VoltageOverMultiplier = 1.05,
                RecordData = true,
                SetCurrentMa = 10.0,
                ComplianceVoltage = 3.0,
                Curve = new List<TempCurveStep>
                {
                    new TempCurveStep { Name = "升温", TargetTemp = 85, RampMinutes = 30, HoldMinutes = 0 },
                    new TempCurveStep { Name = "恒温", TargetTemp = 85, RampMinutes = 0, HoldMinutes = 48 * 60 },
                    new TempCurveStep { Name = "降温", TargetTemp = 25, RampMinutes = 30, HoldMinutes = 0 }
                }
            },
            new Recipe
            {
                Name = "高温加速",
                ProductModel = "TEST-HAST-01",
                TempTarget = 125,
                TempHigh = 130,
                TempLow = 120,
                HoldHour = 24,
                TempTolerance = 5,
                NoDataAlarmCount = 3,
                CurrentNearZeroPercent = 10,
                CurrentOverMultiplier = 1.2,
                VoltageOverMultiplier = 1.05,
                RecordData = true,
                SetCurrentMa = 15.0,
                ComplianceVoltage = 3.5,
                Curve = new List<TempCurveStep>
                {
                    new TempCurveStep { Name = "升温", TargetTemp = 125, RampMinutes = 30, HoldMinutes = 0 },
                    new TempCurveStep { Name = "恒温", TargetTemp = 125, RampMinutes = 0, HoldMinutes = 24 * 60 },
                    new TempCurveStep { Name = "降温", TargetTemp = 25, RampMinutes = 30, HoldMinutes = 0 }
                }
            },
            new Recipe
            {
                Name = "低温",
                ProductModel = "TEST-LT-01",
                TempTarget = -40,
                TempHigh = -35,
                TempLow = -45,
                HoldHour = 12,
                TempTolerance = 5,
                NoDataAlarmCount = 3,
                CurrentNearZeroPercent = 10,
                CurrentOverMultiplier = 1.2,
                VoltageOverMultiplier = 1.05,
                RecordData = true,
                SetCurrentMa = 8.0,
                ComplianceVoltage = 3.0,
                Curve = new List<TempCurveStep>
                {
                    new TempCurveStep { Name = "降温", TargetTemp = -40, RampMinutes = 30, HoldMinutes = 0 },
                    new TempCurveStep { Name = "恒温", TargetTemp = -40, RampMinutes = 0, HoldMinutes = 12 * 60 },
                    new TempCurveStep { Name = "回温", TargetTemp = 25, RampMinutes = 30, HoldMinutes = 0 }
                }
            }
        };

        // 只读算出来的阈值，不参与序列化，PropertyGrid 里也不显示
        [Browsable(false)]
        public double CurrentNearZeroThresh => SetCurrentMa * CurrentNearZeroPercent / 100.0;
        [Browsable(false)]
        public double CurrentOverThresh => SetCurrentMa * CurrentOverMultiplier;
        [Browsable(false)]
        public double VoltageOverThresh => ComplianceVoltage * VoltageOverMultiplier;
    }
}
