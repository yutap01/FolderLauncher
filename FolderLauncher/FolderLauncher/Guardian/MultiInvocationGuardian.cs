
namespace FolderLauncher.Guardian
{
    /// <summary>
    /// 多重起動を監視するクラス
    /// </summary>
    internal static class MultiInvocationGuardian
    {
        /// <summary>
        /// ミューテックス名
        /// </summary>
        private const string mutexName = "MutexOfFolderLauncher";
        
        /// <summary>
        /// アプリケーションのミューテックス
        /// </summary>
        private static Mutex mutex = new Mutex(true, mutexName);

        /// <summary>
        /// 多重起動しているか否かを判定する
        /// </summary>
        /// <returns>多重起動が成立する場合はtrue</returns>
        public static bool IsAlreadyRunning()
        {
            // 多重起動チェック
            return !mutex.WaitOne(TimeSpan.Zero, true);
        }

        /// <summary>
        /// ミューテックスを解放する
        /// </summary>
        public static void Dispose()
        {
            mutex.ReleaseMutex();
        }
    }

}
