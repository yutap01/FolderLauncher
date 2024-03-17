using FolderLauncher.Define;
using FolderLauncher.Model;
using System.Runtime.InteropServices;

namespace FolderLauncher.Guardian
{
    /// <summary>
    /// キーボードイベントを監視するクラス
    /// </summary>
    internal static class KeyboardEventGuardian
    {
        #region イベントメッセージ

        /// <summary>
        /// 低レベルのキーボード入力イベントを監視する
        /// </summary>
        private const int WH_KEYBOARD_LL = 13;

        /// <summary>
        /// キーダウンイベントのメッセージ
        /// </summary>
        private const int WM_KEYDOWN = 0x100;

        /// <summary>
        /// キーアップイベントのメッセージ
        /// </summary>
        private const int WM_KEYUP = 0x101;

        #endregion

        /// <summary>
        /// フック処理コード
        /// </summary>
        /// <remarks>
        /// 0:メッセージがアプリケーションに送信される前に、フックプロシージャによって処理される

        public const int HC_ACTION = 0;

        #region デリゲート

        private delegate int KeyboardEventHookProc(int nCode, int wParam, IntPtr lParam);

        #endregion

        #region private field

        /// <summary>
        /// フックプロシージャのハンドル
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

        #region public event

        /// <summary>
        /// 有効なキーペアが同時に押されたときに発生するイベント
        /// </summary>
        public static event EventHandler<KeyPairEventArgs>? KeyPairDown = null;

        #endregion

        #region public method

        /// <summary>
        /// コンストラクタ
        /// </summary>
        static KeyboardEventGuardian()
        {
            // フックの設定
            hookHandle = SetWindowsHookEx(WH_KEYBOARD_LL, KeyboardHookProc, IntPtr.Zero, 0);

            // フック対象のキーペアの設定
            keyPairs.Add(new KeyPair(Keys.LShiftKey, Keys.RShiftKey));
            keyPairs.Add(new KeyPair(Keys.LControlKey, Keys.RControlKey));
            keyPairs.Add(new KeyPair(Keys.Left, Keys.Right));
            keyPairs.Add(new KeyPair(Keys.Up, Keys.Down));
            keyPairs.Add(new KeyPair(Keys.IMEConvert, Keys.IMENonconvert)); // 変換キー,無変換キー
            keyPairs.Add(new KeyPair(Keys.PageUp, Keys.PageDown));
            keyPairs.Add(new KeyPair(Keys.Home, Keys.End));
        }

        #endregion

        #region private method

        /// <summary>
        /// キーボードフックプロシージャ
        /// </summary>
        /// <param name="nCode">フックコード</param>
        /// <param name="wParam">メッセージwParam</param>
        /// <param name="lParam">メッセージlParam</param>
        /// <returns>0：メッセージ処理を続行する, 1：メッセージ処理を中止する</returns>
        private static int KeyboardHookProc(int nCode, int wParam, IntPtr lParam)
        {
            if (isBothOfKeyPairDown(nCode, wParam, lParam))
            {
                OnKeyPairDown(pressedKeys);                
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
            if (nCode != HC_ACTION)
            {
                return false;
            }

            // キーダウン、キーアップ以外のメッセージは無視
            if (wParam != WM_KEYDOWN && wParam != WM_KEYUP)
            {
                return false;
            }

            // キー状態の取得
            var keyInfo = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT))!;

            // キーコードの取得
            var key = (Keys)keyInfo.vkCode;

            /// キーダウン対応
            if (wParam == WM_KEYDOWN)
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
            if (wParam == WM_KEYUP)
            {
                if (pressedKeys.Contains(key))
                {
                    pressedKeys.Remove(key);
                }
            }

            return false;
        }

        #endregion

        #region イベント発火プロシージャ

        /// <summary>
        /// キーペアが同時に押下されたイベントを発行する
        /// </summary>
        /// <param name="keys">押下されたキーの一覧</param>
        private static void OnKeyPairDown(IList<Keys> keys)
        {
            KeyPairDown?.Invoke(null, new KeyPairEventArgs(keys));
        }

        #endregion

        #region DllImport

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, KeyboardEventHookProc lpfn, IntPtr hMod, int dwThreadId);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        private static extern int CallNextHookEx(IntPtr hhk, int nCode, int wParam, IntPtr lParam);

        #endregion
    }

    /// <summary>
    /// キーペア押下イベントのパラメータ
    /// </summary>
    public class KeyPairEventArgs : EventArgs
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="keys">押下したキーのリスト</param>
        public KeyPairEventArgs(IList<Keys> keys)
        {
            Keys = keys;
        }

        /// <summary>
        /// 押下されたキーのリスト
        /// </summary>
        public IList<Keys> Keys { get; private set; }
    }

}
