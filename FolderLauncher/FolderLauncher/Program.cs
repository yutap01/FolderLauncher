using FolderLauncher.Guardian;
using static FolderLauncher.Define.Define;

namespace FolderLauncher
{
    internal static class Program
    {
        #region �����o

        /// <summary>
        /// �^�X�N�g���C�̃A�C�R��
        /// </summary>
        private static NotifyIcon notifyIcon;

        #endregion


        /// <summary>
        /// ���C���G���g���|�C���g
        /// </summary>
        [STAThread]
        static void Main()
         {
            // ���d�N���`�F�b�N
            if(MultiInvocationGuardian.IsAlreadyRunning())
            {
                MessageBox.Show(errorMessageOfMultipleInvocation);
                return;
            }

            try
            {
                ApplicationConfiguration.Initialize();

                // ���C���E�B���h�E
                var mainWindow = new Form1();

                // �A�v���P�[�V�����̏�����
                Initialize(mainWindow);

                // �^�X�N�g���C�ɏ풓����
                ResidesTaskTray(mainWindow);

                // �A�v���P�[�V�����̎��s
                Application.Run(mainWindow);

            }
            finally
            {
                // �㏈��
                PostProcessing();
            }
        }

        /// <summary>
        /// ������
        /// </summary>
        /// <param name="mainWindow">���C���E�B���h�E</param>
        private static void Initialize(Form1 mainWindow)
        {
            // TODO:�ݒ�̓ǂݍ���

            // ���C���E�B���h�E�̏�����
            mainWindow.Initialize();

            MouseEventGuardian.DesktopDoubleClick += (sender, e) =>
            {
                MessageBox.Show("�f�X�N�g�b�v���_�u���N���b�N����܂���");
            };

            KeyboardEventGuardian.KeyPairDown += (sender, e) =>
            {
                MessageBox.Show("�L�[�y�A��������܂���");
            };

        }

        /// <summary>
        /// �A�v���P�[�V�������^�X�N�g���C�ɏ풓������
        /// </summary>
        /// <param name="mainWindow">���C���E�B���h�E</param>
        /// <returns>�^�X�N�g���C��̃A�C�R��</returns>
        private static NotifyIcon ResidesTaskTray(Form1 mainWindow)
        {
            var notifyIcon = new NotifyIcon();
            notifyIcon.Icon = SystemIcons.Application;
            notifyIcon.Text = applicationName;
            notifyIcon.Visible = true;

            // �^�X�N�g���C�̃A�C�R���̃_�u���N���b�N�C�x���g�n���h�����O
            notifyIcon.DoubleClick += (sender, e) =>
            {
                mainWindow.Show();
                mainWindow.ShowInTaskbar = true;
                mainWindow.WindowState = FormWindowState.Normal;
            };

            return notifyIcon;
        }

        /// <summary>
        /// �㏈��
        /// </summary>
        private static void PostProcessing()
        {
            notifyIcon.Dispose();
            MultiInvocationGuardian.Dispose();
        }
    }
}