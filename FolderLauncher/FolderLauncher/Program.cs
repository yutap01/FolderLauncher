using FolderLauncher.Define;
using FolderLauncher.Model;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static FolderLauncher.Define.Define;

namespace FolderLauncher
{
    internal static class Program
    {
        #region デリゲート

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

        #region メンバ

        /// <summary>
        /// アプリケーションのミューテックス
        /// </summary>
        private static Mutex mutex = new Mutex(true, mutexName);

        /// <summary>
        /// タスクトレイのアイコン
        /// </summary>
        private static NotifyIcon notifyIcon;

        /// <summary>
        /// フックハンドル
        /// </summary>
        private static IntPtr hookHandle;

        /// <summary>
        /// 現在押下されているキーのリスト
        /// </summary>
        private static List<Keys> pressedKeys = new();

        /// <summary>
        /// フック対象として有効なキーペアのリスト
        /// </summary>
        private static List<KeyPair> keyPairs = new();

        #endregion


        /// <summary>
        /// メインエントリポイント
        /// </summary>
        [STAThread]
        static void Main()
        {
            // 多重起動チェック
            if (!mutex.WaitOne(TimeSpan.Zero, true))
            {
                // 既に別のインスタンスが起動している場合は終了する
                MessageBox.Show(errorMessageOfMultipleInvocation, applicationName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            try
            {
                ApplicationConfiguration.Initialize();

                // メインウィンドウ
                var mainWindow = new Form1();

                // アプリケーションの初期化
                Initialize(mainWindow);

                // タスクトレイに常駐する
                ResidesTaskTray(mainWindow);

                // アプリケーションの実行
                Application.Run(mainWindow);

            } finally
            {
                // 後処理
                PostProcessing();
            }
        }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="mainWindow">メインウィンドウ</param>
        private static void Initialize(Form1 mainWindow)
        {
            // TODO:設定の読み込み

            // メインウィンドウの初期化
            mainWindow.Initialize();

            // フックの設定
            hookHandle = SetWindowsHookEx(WH_KEYBOARD_LL, KeyboardHookProc, IntPtr.Zero, 0);

            // TODO:フック対象のキーペアの設定
            keyPairs.Add(new KeyPair(Keys.LShiftKey,Keys.RShiftKey));
            keyPairs.Add(new KeyPair(Keys.LControlKey,Keys.RControlKey));
            keyPairs.Add(new KeyPair(Keys.Left,Keys.Right));
            keyPairs.Add(new KeyPair(Keys.Up,Keys.Down));
            // 変換キー,無変換キー
            keyPairs.Add(new KeyPair(Keys.IMEConvert, Keys.IMENonconvert));
            keyPairs.Add(new KeyPair(Keys.PageUp, Keys.PageDown));
            keyPairs.Add(new KeyPair(Keys.Home, Keys.End));

        }

        /// <summary>
        /// アプリケーションをタスクトレイに常駐させる
        /// </summary>
        /// <param name="mainWindow">メインウィンドウ</param>
        /// <returns>タスクトレイ上のアイコン</returns>
        private static NotifyIcon ResidesTaskTray(Form1 mainWindow)
        {
            var notifyIcon = new NotifyIcon();
            notifyIcon.Icon = SystemIcons.Application;
            notifyIcon.Text = applicationName;
            notifyIcon.Visible = true;

            // タスクトレイのアイコンのダブルクリックイベントハンドリング
            notifyIcon.DoubleClick += (sender, e) =>
            {
                mainWindow.Show();
                mainWindow.ShowInTaskbar = true;
                mainWindow.WindowState = FormWindowState.Normal;
            };

            return notifyIcon;
        }

        /// <summary>
        /// キーボードフックプロシージャ
        /// </summary>
        /// <param name="nCode">フックコード</param>
        /// <param name="wParam">メッセージwParam</param>
        /// <param name="lParam">メッセージlParam</param>
        /// <returns>0：メッセージ処理を続行する, 1：メッセージ処理を中止する</returns>
        private static int KeyboardHookProc(int nCode, int wParam, IntPtr lParam)
        {
            if (isBothOfKeyPairDown(nCode,wParam,lParam))
            {
                MessageBox.Show("イベントが発行されました");
            }
            return CallNextHookEx(hookHandle, nCode, wParam, lParam);
        }

        /// <summary>
        /// 左右Shiftキーが押されたかどうか
        /// </summary>
        /// <param name="nCode">フックコード</param>
        /// <param name="wParam">メッセージwParam</param>
        /// <param name="lParam">メッセージlParam</param>
        /// <returns>キーペアに所属する2つのキーが同時に押されているか否か</returns>
        private static bool isBothOfKeyPairDown(int nCode, int wParam, IntPtr lParam)
        {
            //キーストロークアクションでなければ何もしない
            if(nCode != HC_ACTION)
            {
                return false;
            }

            // キーダウン、キーアップ以外のメッセージは無視
            if(wParam != WM_KEYDOWN && wParam != WM_KEYUP)
            {
                return false;
            }

            // キー状態の取得
            var keyInfo = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT))!;
            
            // キーコードの取得
            var key = (Keys)keyInfo.vkCode;

            /// キーダウン対応
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

                //有効なキーペアのうち、いずれかのペアが満たされている場合trueを返す
                return keyPairs.Any(
                    pair => pair.Key1 == pressedKeys[0] && pair.Key2 == pressedKeys[1] || 
                    pair.Key1 == pressedKeys[1] && pair.Key2 == pressedKeys[0]);
            }

            /// キーアップ対応
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
        /// 後処理
        /// </summary>
        private static void PostProcessing()
        {
            notifyIcon.Dispose();
            mutex.ReleaseMutex();
        }
    }
}