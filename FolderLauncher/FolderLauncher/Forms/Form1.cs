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
        /// ���C���E�B���h�E�̏�����
        /// </summary>
        public void Initialize()
        {
            Hide();
            ShowInTaskbar = false;
            WindowState = FormWindowState.Minimized;
        }
    }
}
