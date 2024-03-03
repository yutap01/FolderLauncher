using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderLauncher.Define
{
    internal static class Define
    {
        /// <summary>
        /// アプリケーション名
        /// </summary>
        public const string applicationName = "Folder Launcher";

        /// <summary>
        /// ミューテックス名
        /// </summary>
        public const string mutexName = "MutexOfFolderLauncher";

        #region メッセージ

        /// <summary>
        /// エラーメッセージ：多重起動
        /// </summary>
        public static readonly string errorMessageOfMultipleInvocation = "既に起動しています。";

        #endregion

        #region イベント識別用

        public const int WH_KEYBOARD_LL = 13;
        public const int WM_KEYDOWN = 0x100;
        public const int WM_KEYUP = 0x101;

        /// <summary>
        /// フック処理コード
        /// </summary>
        /// <remarks>
        /// 0:メッセージがアプリケーションに送信される前に、フックプロシージャによって処理される
        public const int HC_ACTION = 0;
    
        #endregion
    }
}
