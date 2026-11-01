using ChipAgingCheck.Domain;
using ChipAgingCheck.Domain.Common;
using ChipAgingCheck.Domain.Ctrs;
using System.Windows.Forms;

namespace ChipAgingCheck.UI
{
  
    public partial class RecipeForm : Form
    {
        private Recipe editingRecipe = null;  // 编辑的是拷贝，点取消不影响原配方

     
        public RecipeForm()
        {
            InitializeComponent();
        }

        // 打开窗口前调用，把要编辑的配方拷一份塞给表格
        public void LoadRecipe(Recipe source)
        {
            if (source == null)
            {
                return;
            }
            editingRecipe = source.DeepCopy();
            propertyGrid.SelectedObject = editingRecipe;
            lblHint.Text = "保存位置：" + ConfigManager.ConfigFilePath;
        }

        // 点了确定之后，外面用这个方法取结果
        public Recipe GetRecipe()
        {
            return editingRecipe;
        }
    }
}
