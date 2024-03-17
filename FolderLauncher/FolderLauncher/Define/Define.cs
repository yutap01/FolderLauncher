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


        #region メッセージ

        /// <summary>
        /// エラーメッセージ：多重起動
        /// </summary>
        public static readonly string errorMessageOfMultipleInvocation = "既に起動しています。";

        #endregion
    }
}
