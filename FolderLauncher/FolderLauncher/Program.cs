using FolderLauncher.Guardian;
using FolderLauncher.Utilities;
using System.Diagnostics;
using static FolderLauncher.Define.Define;

namespace FolderLauncher
{
    internal static class Program
    {
        #region メンバ

        /// <summary>
        /// タスクトレイのアイコン
        /// </summary>
        private static NotifyIcon notifyIcon;

        #endregion

        /// <summary>
        /// メインエントリポイント
        /// </summary>
        [STAThread]
        static void Main()
         {
            Log.Trace("多重起動チェック");
            if(MultiInvocationGuardian.IsAlreadyRunning())
            {
                MessageBox.Show(errorMessageOfMultipleInvocation);
                return;
            }

            try
            {
                Log.Trace("アプリケーション設定の初期化");
                ApplicationConfiguration.Initialize();

                Log.Trace("メインウィンドウの生成");
                var mainWindow = new MainForm();

                Log.Trace("アプリケーションの初期化");
                Initialize(mainWindow);

                Log.Trace("アプリケーションをタスクトレイへ常駐させる");
                ResidesTaskTray(mainWindow);

                Log.Trace("アプリケーションの実行");
                Application.Run(mainWindow);
            }
            finally
            {
                Log.Trace("後処理");
                PostProcessing();
            }
        }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="form">メインウィンドウ</param>
        private static void Initialize(MainForm form)
        {
            Log.IndentUp();

            // TODO:設定の読み込み

            Log.Trace("メインウィンドウの初期化");
            form.Initialize();

            Log.Trace("デスクトップダブルクリックのフック");
            MouseEventGuardian.DesktopDoubleClick += (sender, e) =>
            {
                Debug.WriteLine("デスクトップダブルクリック");
                ToggleMainForm(form);
            };

            Log.Trace("キーペア押下のフック");
            KeyboardEventGuardian.KeyPairDown += (sender, e) =>
            {
                Debug.WriteLine("キーペア押下");
                ToggleMainForm(form);
            };

            Log.IndentDown();
        }

        /// <summary>
        /// アプリケーションをタスクトレイに常駐させる
        /// </summary>
        /// <param name="mainWindow">メインウィンドウ</param>
        /// <returns>タスクトレイ上のアイコン</returns>
        private static NotifyIcon ResidesTaskTray(MainForm mainWindow)
        {
            Log.IndentUp();

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

            Log.IndentDown();
            return notifyIcon;
        }

        /// <summary>
        /// 指定のフォームを表示状態にする
        /// </summary>
        /// <param name="form">フォーム</param>
        private static void ShowMainForm(Form form)
        {
            // フォームの位置とサイズを決定する
            // TODO:マルチモニタ対応
            form.Enabled = false;   //一時的に無効化
            DetectSize(form);
            form.Enabled = true;    //有効化
            
            form.WindowState = FormWindowState.Normal;
        }

        /// <summary>
        /// フォームのサイズを決定する
        /// </summary>
        /// <param name="form">フォーム</param>
        private static void DetectSize(Form form)
        {
            // フォームのサイズを決定する
            var width = Screen.PrimaryScreen.WorkingArea.Width / 2;
            var height = Screen.PrimaryScreen.WorkingArea.Height;
            form.Size = new Size(width, height);
        }

        /// <summary>
        /// 指定のフォームを非表示状態にする
        /// </summary>
        /// <param name="form">フォーム</param>
        private static void HideMainForm(Form form)
        {
            form.WindowState = FormWindowState.Minimized;
        }

        /// <summary>
        /// フォームの表示状態を反転する
        /// </summary>
        /// <param name="form">フォーム</param>
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
        /// 後処理
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