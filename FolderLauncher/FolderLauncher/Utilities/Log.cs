using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderLauncher.Utilities
{
    internal static class Log
    {
        #region メンバ
#if DEBUG
        /// <summary>
        /// ロガー(デバッグモード)
        /// </summary>
        private static readonly Logger logger = LogManager.GetLogger("forDebug");
#else
        /// <summary>
        /// ロガー(デバッグモード以外)
        /// </summary>
        private static readonly Logger logger = LogManager.GetLogger("forRelease");
#endif

        /// <summary>
        /// インデントレベル
        /// </summary>
        private static int indentLevel = 0;

        /// <summary>
        /// インデントレベル当たりのスペース数
        /// </summary>
        private const int indentSpace = 4;

        #endregion


        /// <summary>
        /// Traceログ
        /// </summary>
        /// <param name="message">メッセージ</param>
        public static void Trace(string message) => Trace(message,indentLevel);

        /// <summary>
        /// Traceログ
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="specifiedIndentLevel">インデントレベル</param>
        public static void Trace(string message,int specifiedIndentLevel)
        {
            logger.Trace(GetIndent(specifiedIndentLevel) + message);
        }

        /// <summary>
        /// Debugログ
        /// </summary>
        /// <param name="message">メッセージ</param>
        public static void Debug(string message) => Debug(message,indentLevel);

        /// <summary>
        /// Debugログ
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="specifiedUndentLevel">インデントレベル</param>
        public static void Debug(string message,int specifiedUndentLevel)
        {
            logger.Debug(GetIndent(specifiedUndentLevel) + message);
        }

        /// <summary>
        /// Infoログ
        /// </summary>
        /// <param name="message">メッセージ</param>
        public static void Info(string message) => Info(message,indentLevel);

        /// <summary>
        /// Infoログ
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="specifiedIndentLevel">インデントレベル</param>
        public static void Info(string message,int specifiedIndentLevel)
        {
            logger.Info(GetIndent(specifiedIndentLevel) + message);
        }

        /// <summary>
        /// Warnログ
        /// </summary>
        /// <param name="message">メッセージ</param>
        public static void Warn(string message) => logger.Warn(GetIndent() + message);

        /// <summary>
        /// Warnログ
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="specifiedIndentLevel">インデントレベル</param>
        public static void Warn(string message,int specifiedIndentLevel)
        {
            logger.Warn(GetIndent(specifiedIndentLevel) + message);
        }

        /// <summary>
        /// Errorログ
        /// </summary>
        /// <param name="message">メッセージ</param>
        public static void Error(string message) => Error(message, indentLevel);

        /// <summary>
        /// Errorログ
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="specifiedIndentLevel">インデントレベル</param>
        public static void Error(string message, int specifiedIndentLevel)
        {
            logger.Error(GetIndent(specifiedIndentLevel) + message);
        }

        /// <summary>
        /// Fatalログ
        /// </summary>
        /// <param name="message">メッセージ</param>
        public static void Fatal(string message) => Fatal(message, indentLevel);
        
        /// <summary>
        /// Fatalログ
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="specifiedIndentLevel">インデントレベル</param>
        public static void Fatal(string message, int specifiedIndentLevel)
        {
            logger.Fatal(GetIndent(specifiedIndentLevel) + message);
        }

        /// <summary>
        /// ログマネージャのシャットダウンを明示的に行う
        /// </summary>
        public static void Shutdown() => LogManager.Shutdown();

        /// <summary>
        /// インデントレベルを増やす
        /// </summary>
        public static void IndentUp() => indentLevel++;

        /// <summary>
        /// インデントレベルを減らす
        /// </summary>
        public static void IndentDown()
        {
            indentLevel = Math.Max(0, indentLevel - 1);
        }

        /// <summary>
        /// インデントを取得する
        /// </summary>
        /// <returns>インデント文字列</returns>
        private static string GetIndent() => GetIndent(indentLevel);

        /// <summary>
        /// インデントを取得する
        /// </summary>
        /// <param name="level">インデントレベル</param>
        /// <remarks>
        /// インデントレベルが変更される
        /// </remarks>
        /// <returns>インデント文字列</returns>
        private static string GetIndent(int level)
        {
            indentLevel = level;
            return new string(' ', level * indentSpace);
        }
    }
}
