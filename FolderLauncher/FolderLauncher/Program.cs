using FolderLauncher.Guardian;
using FolderLauncher.Utilities;
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
            Log.Error("�����[�X�e�X�g");

            Log.Trace("���d�N���`�F�b�N");
            if(MultiInvocationGuardian.IsAlreadyRunning())
            {
                MessageBox.Show(errorMessageOfMultipleInvocation);
                return;
            }

            try
            {
                Log.Trace("�A�v���P�[�V�����ݒ�̏�����");
                ApplicationConfiguration.Initialize();

                Log.Trace("���C���E�B���h�E�̐���");
                var mainWindow = new Form1();

                Log.Trace("�A�v���P�[�V�����̏�����");
                Initialize(mainWindow);

                Log.Trace("�A�v���P�[�V�������^�X�N�g���C�֏풓������");
                ResidesTaskTray(mainWindow);

                Log.Trace("�A�v���P�[�V�����̎��s");
                Application.Run(mainWindow);
            }
            finally
            {
                Log.Trace("�㏈��");
                PostProcessing();
            }
        }

        /// <summary>
        /// ������
        /// </summary>
        /// <param name="mainWindow">���C���E�B���h�E</param>
        private static void Initialize(Form1 mainWindow)
        {
            Log.IndentUp();

            // TODO:�ݒ�̓ǂݍ���

            Log.Trace("���C���E�B���h�E�̏�����");
            mainWindow.Initialize();

            Log.Trace("�f�X�N�g�b�v�_�u���N���b�N�̃t�b�N");
            MouseEventGuardian.DesktopDoubleClick += (sender, e) =>
            {
                MessageBox.Show("�f�X�N�g�b�v���_�u���N���b�N����܂���");
            };

            Log.Trace("�L�[�y�A�����̃t�b�N");
            KeyboardEventGuardian.KeyPairDown += (sender, e) =>
            {
                MessageBox.Show("�L�[�y�A��������܂���");
            };

            Log.IndentDown();
        }

        /// <summary>
        /// �A�v���P�[�V�������^�X�N�g���C�ɏ풓������
        /// </summary>
        /// <param name="mainWindow">���C���E�B���h�E</param>
        /// <returns>�^�X�N�g���C��̃A�C�R��</returns>
        private static NotifyIcon ResidesTaskTray(Form1 mainWindow)
        {
            Log.IndentUp();

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

            Log.IndentDown();
            return notifyIcon;
        }

        /// <summary>
        /// �㏈��
        /// </summary>
        private static void PostProcessing()
        {
            Log.IndentUp();

            notifyIcon.Dispose();
            MultiInvocationGuardian.Dispose();
            Log.IndentDown();
            
            Log.Shutdown();
        }
    }
}