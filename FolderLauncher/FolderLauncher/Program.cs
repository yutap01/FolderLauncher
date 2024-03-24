using FolderLauncher.Guardian;
using FolderLauncher.Utilities;
using System.Diagnostics;
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
                var mainWindow = new MainForm();

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
        /// <param name="form">���C���E�B���h�E</param>
        private static void Initialize(MainForm form)
        {
            Log.IndentUp();

            // TODO:�ݒ�̓ǂݍ���

            Log.Trace("���C���E�B���h�E�̏�����");
            form.Initialize();

            Log.Trace("�f�X�N�g�b�v�_�u���N���b�N�̃t�b�N");
            MouseEventGuardian.DesktopDoubleClick += (sender, e) =>
            {
                Debug.WriteLine("�f�X�N�g�b�v�_�u���N���b�N");
                ToggleMainForm(form);
            };

            Log.Trace("�L�[�y�A�����̃t�b�N");
            KeyboardEventGuardian.KeyPairDown += (sender, e) =>
            {
                Debug.WriteLine("�L�[�y�A����");
                ToggleMainForm(form);
            };

            Log.IndentDown();
        }

        /// <summary>
        /// �A�v���P�[�V�������^�X�N�g���C�ɏ풓������
        /// </summary>
        /// <param name="mainWindow">���C���E�B���h�E</param>
        /// <returns>�^�X�N�g���C��̃A�C�R��</returns>
        private static NotifyIcon ResidesTaskTray(MainForm mainWindow)
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
        /// �w��̃t�H�[����\����Ԃɂ���
        /// </summary>
        /// <param name="form">�t�H�[��</param>
        private static void ShowMainForm(Form form)
        {
            // �t�H�[���̈ʒu�ƃT�C�Y�����肷��
            // TODO:�}���`���j�^�Ή�
            form.Enabled = false;   //�ꎞ�I�ɖ�����
            DetectSize(form);
            form.Enabled = true;    //�L����
            
            form.WindowState = FormWindowState.Normal;
        }

        /// <summary>
        /// �t�H�[���̃T�C�Y�����肷��
        /// </summary>
        /// <param name="form">�t�H�[��</param>
        private static void DetectSize(Form form)
        {
            // �t�H�[���̃T�C�Y�����肷��
            var width = Screen.PrimaryScreen.WorkingArea.Width / 2;
            var height = Screen.PrimaryScreen.WorkingArea.Height;
            form.Size = new Size(width, height);
        }

        /// <summary>
        /// �w��̃t�H�[�����\����Ԃɂ���
        /// </summary>
        /// <param name="form">�t�H�[��</param>
        private static void HideMainForm(Form form)
        {
            form.WindowState = FormWindowState.Minimized;
        }

        /// <summary>
        /// �t�H�[���̕\����Ԃ𔽓]����
        /// </summary>
        /// <param name="form">�t�H�[��</param>
        private static void ToggleMainForm(Form form)
        {
            if (form.WindowState == FormWindowState.Minimized)
            {
                ShowMainForm(form);
            }
            else
            {
                HideMainForm(form);
            }
        }

        /// <summary>
        /// �㏈��
        /// </summary>
        private static void PostProcessing()
        {
            Log.IndentUp();

            notifyIcon?.Dispose();
            MultiInvocationGuardian.Dispose();
            Log.IndentDown();
            
            Log.Shutdown();
        }
    }
}