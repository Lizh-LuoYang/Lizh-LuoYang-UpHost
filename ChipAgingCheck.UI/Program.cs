using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChipAgingCheck.UI
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ChipAgingCheck.Domain.Common.CurveSelfCheck.Run();   // 启动前先自检温度曲线，结果进 VS 输出窗口
            Application.Run(new MainForm());
        }
    }
}
