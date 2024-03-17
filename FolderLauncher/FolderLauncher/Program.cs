using FolderLauncher.Guardian;
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
            // 多重起動チェック
            if(MultiInvocationGuardian.IsAlreadyRunning())
            {
                MessageBox.Show(errorMessageOfMultipleInvocation);
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

            }
            finally
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

            MouseEventGuardian.DesktopDoubleClick += (sender, e) =>
            {
                MessageBox.Show("デスクトップがダブルクリックされました");
            };

            KeyboardEventGuardian.KeyPairDown += (sender, e) =>
            {
                MessageBox.Show("キーペアが押されました");
            };

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
        /// 後処理
        /// </summary>
        private static void PostProcessing()
        {
            notifyIcon.Dispose();
            MultiInvocationGuardian.Dispose();
        }
    }
}