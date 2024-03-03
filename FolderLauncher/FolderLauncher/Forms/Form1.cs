using System.Windows.Forms;

namespace FolderLauncher
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// メインウィンドウの初期化
        /// </summary>
        public void Initialize()
        {
            Hide();
            ShowInTaskbar = false;
            WindowState = FormWindowState.Minimized;
        }
    }
}
