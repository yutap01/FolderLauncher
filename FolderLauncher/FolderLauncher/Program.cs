using FolderLauncher.Define;
using FolderLauncher.Model;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static FolderLauncher.Define.Define;

namespace FolderLauncher
{
    internal static class Program
    {
        #region �f���Q�[�g

        private delegate int HookProc(int nCode, int wParam, IntPtr lParam);

        #endregion


        #region DllImport

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, int dwThreadId);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        private static extern int CallNextHookEx(IntPtr hhk, int nCode, int wParam, IntPtr lParam);

        #endregion

        #region �����o

        /// <summary>
        /// �A�v���P�[�V�����̃~���[�e�b�N�X
        /// </summary>
        private static Mutex mutex = new Mutex(true, mutexName);

        /// <summary>
        /// �^�X�N�g���C�̃A�C�R��
        /// </summary>
        private static NotifyIcon notifyIcon;

        /// <summary>
        /// �t�b�N�n���h��
        /// </summary>
        private static IntPtr hookHandle;

        /// <summary>
        /// ���݉�������Ă���L�[�̃��X�g
        /// </summary>
        private static List<Keys> pressedKeys = new();

        /// <summary>
        /// �t�b�N�ΏۂƂ��ėL���ȃL�[�y�A�̃��X�g
        /// </summary>
        private static List<KeyPair> keyPairs = new();

        #endregion


        /// <summary>
        /// ���C���G���g���|�C���g
        /// </summary>
        [STAThread]
        static void Main()
        {
            // ���d�N���`�F�b�N
            if (!mutex.WaitOne(TimeSpan.Zero, true))
            {
                // ���ɕʂ̃C���X�^���X���N�����Ă���ꍇ�͏I������
                MessageBox.Show(errorMessageOfMultipleInvocation, applicationName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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

            } finally
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

            // �t�b�N�̐ݒ�
            hookHandle = SetWindowsHookEx(WH_KEYBOARD_LL, KeyboardHookProc, IntPtr.Zero, 0);

            // TODO:�t�b�N�Ώۂ̃L�[�y�A�̐ݒ�
            keyPairs.Add(new KeyPair(Keys.LShiftKey,Keys.RShiftKey));
            keyPairs.Add(new KeyPair(Keys.LControlKey,Keys.RControlKey));
            keyPairs.Add(new KeyPair(Keys.Left,Keys.Right));
            keyPairs.Add(new KeyPair(Keys.Up,Keys.Down));
            // �ϊ��L�[,���ϊ��L�[
            keyPairs.Add(new KeyPair(Keys.IMEConvert, Keys.IMENonconvert));
            keyPairs.Add(new KeyPair(Keys.PageUp, Keys.PageDown));
            keyPairs.Add(new KeyPair(Keys.Home, Keys.End));

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
        /// �L�[�{�[�h�t�b�N�v���V�[�W��
        /// </summary>
        /// <param name="nCode">�t�b�N�R�[�h</param>
        /// <param name="wParam">���b�Z�[�WwParam</param>
        /// <param name="lParam">���b�Z�[�WlParam</param>
        /// <returns>0�F���b�Z�[�W�����𑱍s����, 1�F���b�Z�[�W�����𒆎~����</returns>
        private static int KeyboardHookProc(int nCode, int wParam, IntPtr lParam)
        {
            if (isBothOfKeyPairDown(nCode,wParam,lParam))
            {
                MessageBox.Show("�C�x���g�����s����܂���");
            }
            return CallNextHookEx(hookHandle, nCode, wParam, lParam);
        }

        /// <summary>
        /// ���EShift�L�[�������ꂽ���ǂ���
        /// </summary>
        /// <param name="nCode">�t�b�N�R�[�h</param>
        /// <param name="wParam">���b�Z�[�WwParam</param>
        /// <param name="lParam">���b�Z�[�WlParam</param>
        /// <returns>�L�[�y�A�ɏ�������2�̃L�[�������ɉ�����Ă��邩�ۂ�</returns>
        private static bool isBothOfKeyPairDown(int nCode, int wParam, IntPtr lParam)
        {
            //�L�[�X�g���[�N�A�N�V�����łȂ���Ή������Ȃ�
            if(nCode != HC_ACTION)
            {
                return false;
            }

            // �L�[�_�E���A�L�[�A�b�v�ȊO�̃��b�Z�[�W�͖���
            if(wParam != WM_KEYDOWN && wParam != WM_KEYUP)
            {
                return false;
            }

            // �L�[��Ԃ̎擾
            var keyInfo = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT))!;
            
            // �L�[�R�[�h�̎擾
            var key = (Keys)keyInfo.vkCode;

            /// �L�[�_�E���Ή�
            if(wParam == WM_KEYDOWN)
            {
                if (!pressedKeys.Contains(key))
                {
                    pressedKeys.Add(key);
                }

                if (pressedKeys.Count != 2)
                {
                    return false;
                }

                //�L���ȃL�[�y�A�̂����A�����ꂩ�̃y�A����������Ă���ꍇtrue��Ԃ�
                return keyPairs.Any(
                    pair => pair.Key1 == pressedKeys[0] && pair.Key2 == pressedKeys[1] || 
                    pair.Key1 == pressedKeys[1] && pair.Key2 == pressedKeys[0]);
            }

            /// �L�[�A�b�v�Ή�
            if(wParam == WM_KEYUP)
            {
                if (pressedKeys.Contains(key))
                {
                    pressedKeys.Remove(key);
                }
            }
            
            return false;
        }


        /// <summary>
        /// �㏈��
        /// </summary>
        private static void PostProcessing()
        {
            notifyIcon.Dispose();
            mutex.ReleaseMutex();
        }
    }
}