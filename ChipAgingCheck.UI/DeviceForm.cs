using ChipAgingCheck.Domain.Common;
using ChipAgingCheck.Domain.Ctrs;
using ChipAgingCheck.Domain.Modbus;
using System.Windows.Forms;

namespace ChipAgingCheck.UI
{
   
    public partial class DeviceForm : Form
    {
        private ModbusParam editingParam = null;  // 改的是拷贝，点取消不动正在跑的参数

        public DeviceForm()
        {
            InitializeComponent();
        }

        // 打开窗口前调用，把当前参数拷一份塞给表格
        public void LoadParam(ModbusParam source)
        {
            if (source == null)
            {
                return;
            }
            editingParam = source.DeepCopy();
            propertyGrid.SelectedObject = editingParam;
            lblPath.Text = "保存位置：" + ConfigManager.ConfigFilePath;
        }

        public ModbusParam GetParam()
        {
            return editingParam;
        }
    }
}
