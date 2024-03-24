
namespace FolderLauncher.Utilities
{

    /// <summary>
    /// キーと文字コード、キーと行インデックスのマッピング
    /// </summary>
    internal class KeyMapper
    {
        /// <summary>
        /// 大文字のキーコードと小文字のキーコードの差
        /// </summary>
        private const int differencBetweenUpperAndLower = 'a' - 'A';

        /// <summary>
        /// 行が対象とするキーの最小値
        /// </summary>
        private const Keys minKeyOfForRow = Keys.A;

        /// <summary>
        /// 行が対象とするキーの最大値
        /// </summary>
        private const Keys maxKeyOfForRow = Keys.Z;

        /// <summary>
        /// キーに対応する文字コードを取得する
        /// </summary>
        /// <param name="key">対象のキー</param>  
        /// <param name="withShiftKey">Shiftキーが押下されているか？</param>
        /// <returns>対応するキーコード</returns>
        public static char GetChar(Keys key,bool withShiftKey = false)
        {
            return withShiftKey ? (char)((int)key + differencBetweenUpperAndLower) :(char)key;
        }

        /// <summary>
        /// キーに対応する行インデックスを取得する
        /// </summary>
        /// <param name="key">対象のキー</param>
        /// <returns>行インデックス</returns>
        public static int GetRowIndex(Keys key)
        {
            if(key < minKeyOfForRow || maxKeyOfForRow < key)
            {
                throw new ArgumentOutOfRangeException(nameof(key));
            }

            return (int)key - (int)minKeyOfForRow;
        }
    }
}
