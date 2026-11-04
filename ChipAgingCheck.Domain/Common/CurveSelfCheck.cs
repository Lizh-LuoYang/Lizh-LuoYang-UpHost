using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace ChipAgingCheck.Domain.Common
{
    /// <summary>温度曲线自检：8 条断言，全过才算曲线算对了。</summary>
    public static class CurveSelfCheck
    {
        public static void Run()
        {
            List<TempCurveStep> curve = new List<TempCurveStep>();
            curve.Add(new TempCurveStep { RampMinutes = 30, HoldMinutes = 0, TargetTemp = 85 });
            curve.Add(new TempCurveStep { RampMinutes = 0, HoldMinutes = 2880, TargetTemp = 85 });
            curve.Add(new TempCurveStep { RampMinutes = 30, HoldMinutes = 0, TargetTemp = 25 });

            int failCount = 0;
            failCount = failCount + CheckOne(curve, 0, 25);
            failCount = failCount + CheckOne(curve, 15, 55);
            failCount = failCount + CheckOne(curve, 30, 85);
            failCount = failCount + CheckOne(curve, 60, 85);
            failCount = failCount + CheckOne(curve, 2910, 85);
            failCount = failCount + CheckOne(curve, 2915, 75);
            failCount = failCount + CheckOne(curve, 2925, 55);
            failCount = failCount + CheckOne(curve, 2941, 25);

            string title = "曲线自检结果：失败 " + failCount + " 条";
            Debug.WriteLine(title);
            CurveSelfCheckResult = title;
        }

        public static string CurveSelfCheckResult { get; private set; } = "自检未执行";

        private static int CheckOne(List<TempCurveStep> curve, double minutes, double expect)
        {
            double actual = CurveCalculator.GetTargetTemperature(curve, 25, minutes);
            double diff = Math.Abs(actual - expect);
            bool passed = diff < 0.001;

            if (passed)
            {
                Write("PASS  " + minutes + " 分钟 → " + actual);
                return 0;
            }

            Write("FAIL  " + minutes + " 分钟 → 应为 " + expect + "，实际 " + actual);
            return 1;
        }

        private static void Write(string line)
        {
            Debug.WriteLine(line);
            string path = Path.Combine(Path.GetTempPath(), "curve_selfcheck.txt");
            File.AppendAllText(path, line + Environment.NewLine);
        }
    }
}