using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChipAgingCheck.Domain
{
    public class TempCurveStep
    {
        public string Name { get; set; } = "升温";

        /// <summary>这一段要到达的温度（℃）。</summary>
        public double TargetTemp { get; set; } = 25;

        /// <summary>从上一段温度走到这个温度用多少分钟。0 = 不升温（恒温段）。</summary>
        public int RampMinutes { get; set; } = 0;

        /// <summary>到温后保持多少分钟。0 = 不保持（升温/降温段）。</summary>
        public int HoldMinutes { get; set; } = 0;

    }
}
