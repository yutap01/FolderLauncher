namespace FolderLauncher.Model
{
    /// <summary>
    /// キーのペア
    /// </summary>
    /// <remarks>
    /// 値は仮想キーコード
    /// </remarks>
    internal class KeyPair
    {
        /// <summary>
        /// 1方のキー
        /// </summary>
        public Keys Key1 { get; set; }

        /// <summary>
        /// ペアとなる他方のキー
        /// </summary>
        public Keys Key2 { get; set; }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="key1">1方のキー</param>
        /// <param name="key2">ペアとなる他方のキー</param>
        public KeyPair(Keys key1, Keys key2)
        {
            Key1 = key1;
            Key2 = key2;
        }
    }
}
