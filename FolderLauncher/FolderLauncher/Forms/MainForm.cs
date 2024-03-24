using FolderLauncher.Forms;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Runtime;
using System.Windows.Forms;

namespace FolderLauncher
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// �A�N�V�����𔭍s����L�[�̃��X�g
        /// </summary>
        /// <remarks>
        /// �z�u���ɒ�`
        /// </remarks>
        private static readonly IList<char> actionKeyList = new List<char>(){
            'a','s','d','f','g','h',
            'q','w','e','r','t','y',
            'z','x','c','v','b','n',
        };

        /// <summary>
        /// �t�H�[�����B���L�[�̃��X�g
        /// </summary>
        private static readonly IList<Keys> hideKeyList = new List<Keys>()
        {
            Keys.Escape,Keys.Space
        };

        /// <summary>
        /// RowControl�̐�
        /// </summary>
        private int RowCount
        {
            get
            {
                return actionKeyList.Count;
            }
        }

        /// <summary>
        /// RowControl�̃��X�g
        /// </summary>
        private IList<RowControl> rowControls = new List<RowControl>();

        /// <summary>
        /// �]�������肷�邽�߂̔䗦
        /// </summary>
        /// <remarks>
        /// �]���̃T�C�Y�́A���R���g���[���̍����ɑ΂��邱�̔䗦�Ō��肷��
        /// </remarks>
        private double paddingRatio = 0.005;

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// [�n���h��]���C���E�B���h�E�̏�������
        /// </summary>
        public void Initialize()
        {
            Hide();
            ShowInTaskbar = false;
            WindowState = FormWindowState.Minimized;

            // �s�R���g���[���̐���
            foreach (var ch in actionKeyList)
            {
                var rowControl = new RowControl(ch);
                rowControl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                rowControls.Add(rowControl);
                Controls.Add(rowControl);
            }
        }

        /// <summary>
        /// [�n���h��] �T�C�Y���ύX���ꂽ
        /// </summary>
        /// <param name="sender">�C�x���g���M��</param>
        /// <param name="e">�C�x���g�p�����[�^</param>
        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            // ��\�����͉������Ȃ�
            if (WindowState == FormWindowState.Minimized)
            {
                return;
            }

            Debug.WriteLine("MainForm_SizeChanged");

            // �]���̑傫�������肷��
            Padding = new Padding((int)(ClientSize.Height * paddingRatio));

            // �]����������������
            var remainHeight = ClientSize.Height - (Padding.Top * (RowCount + 1));

            // �s�R���g���[���̍��������肷��
            var rowHeight = (int)(remainHeight / RowCount);

            // �s�R���g���[���̕������肷��
            var rowWidth = ClientSize.Width - Padding.Left - Padding.Right;

            // �s�R���g���[���̃T�C�Y�ƈʒu��ݒ肷��
            var top = 0;
            foreach (var rowControl in rowControls)
            {
                top += Padding.Top;

                // �T�C�Y
                rowControl.Size = new Size(rowWidth, rowHeight);

                // �ʒu
                rowControl.Location = new Point(Padding.Left, top);
                top += rowHeight;
            }
        }

        /// <summary>
        /// [�n���h��] (�t�H�[���Ƀt�H�J�X�������Ԃ�)�L�[�������ꂽ
        /// </summary>
        /// <param name="sender">�C�x���g���M��</param>
        /// <param name="e">�C�x���g�p�����[�^</param>
        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            Debug.WriteLine("MainForm_KeyUp");

            var key = (Keys)e.KeyCode;
            if (hideKeyList.Contains(key) ){
                WindowState = FormWindowState.Minimized;
            }
        }
    }
}
