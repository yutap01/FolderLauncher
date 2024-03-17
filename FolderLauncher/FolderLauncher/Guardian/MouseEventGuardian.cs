using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace FolderLauncher.Guardian
{
    /// <summary>
    /// マウスイベントを監視するクラス
    /// </summary>
    internal class MouseEventGuardian : IDisposable
    {
        #region private const

        #region マウスイベントメッセージ

        /// <summary>
        /// 低レベルのマウスイベントをフックするためのフックID
        /// </summary>
        private const int WH_MOUSE_LL = 14;

        /// <summary>
        /// マウス左ボタンアップを表すメッセージ
        /// </summary>
        private const int WM_LBUTTONUP = 0x0202;

        /// <summary>
        /// マウス右ボタンアップを表すメッセージ
        /// </summary>
        private const int WM_RBUTTONUP = 0x0205;

        #endregion

        #region ListViewイベントメッセージ

        /// <summary>
        /// ListViewの最初のメッセージ
        /// </summary>
        private const int LVM_FIRST = 0x1000;

        /// <summary>
        /// ListViewのサブアイテムヒットテストメッセージ
        /// </summary>
        private const int LVM_SUBITEMHITTEST = (LVM_FIRST + 57);

        #endregion

        #region プロセス仮想メモリ管理操作のアクセス権

        /// <summary>
        /// 別のプロセスの仮想メモリを操作
        /// </summary>
        private const int PROCESS_VM_OPERATION = 8;

        /// <summary>
        /// 別のプロセスの仮想メモリを読み取るアクセス権
        /// </summary>
        private const int PROCESS_VM_READ = 16;

        /// <summary>
        /// 別のプロセスの仮想メモリに書き込むアクセス権
        /// </summary>
        private const int PROCESS_VM_WRITE = 32;

        #endregion

        #region 仮想メモリに対する操作
        /// <summary>
        /// 仮想メモリのコミット
        /// </summary>
        private const int MEM_COMMIT = 4096;

        /// <summary>
        /// 仮想メモリの解放
        /// </summary>
        private const int MEM_RELEASE = 32768;

        /// <summary>
        /// 読み書き可能なページ
        /// </summary>
        private const int PAGE_READWRITE = 4;
        #endregion
        
        /// <summary>
        /// フックの種類：メッセージがウィンドウに送信される前にフックプロシージャに渡される
        /// </summary>
        private const int HC_ACTION = 0;

        #endregion

        #region private field

        /// <summary>
        /// フックプロシージャのハンドル
        /// </summary>
        private IntPtr hookHandle;

        /// <summary>
        /// フックプロシージャを格納するデリゲート変数
        /// </summary>
        private MouseHookProcDelegate hookProc;

        /// <summary>
        /// ダブルクリック判定者
        /// </summary>
        private MultiEventChecker doubleClickChecker;

        /// <summary>
        /// 右ダブルクリック判定者
        /// </summary>
        private MultiEventChecker rightDoubleClickChecker;

        #endregion

        #region public event

        /// <summary>
        /// デスクトップがダブルクリックされたイベント
        /// </summary>
        public event EventHandler<DesktopMouseEventArgs>? DesktopDoubleClick = null;

        /// <summary>
        /// デスクトップが右ダブルクリックされたイベント
        /// </summary>
        public event EventHandler<DesktopMouseEventArgs>? DesktopRightDoubleClick = null;

        #endregion

        #region public method

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MouseEventGuardian()
        {
            hookProc = MouseHookProc;
            hookHandle = SetWindowsHookEx(WH_MOUSE_LL, hookProc, IntPtr.Zero, 0);

            if (hookHandle == IntPtr.Zero)
            {
                throw new Exception("Failed to set mouse hook.");
            }

            doubleClickChecker = new MultiEventChecker((IntPtr)WM_LBUTTONUP, SystemInformation.DoubleClickTime, 2);
            rightDoubleClickChecker = new MultiEventChecker((IntPtr)WM_RBUTTONUP, SystemInformation.DoubleClickTime, 2);
        }

        /// <summary>
        /// リソースの破棄
        /// </summary>
        public void Dispose()
        {
            if (hookHandle != IntPtr.Zero)
            {
                UnhookWindowsHookEx(hookHandle);
                hookHandle = IntPtr.Zero;
            }
        }

        #endregion

        #region private method

        /// <summary>
        /// マウスイベントをフックするメソッド
        /// </summary>
        /// <param name="nCode">イベントの内容</param>
        /// <param name="wParam">イベントのwパラメータ</param>
        /// <param name="lParam">イベントのlパラメータ</param>
        /// <returns>フックプロシージャのハンドル</returns>
        private IntPtr MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode == HC_ACTION)
            {
                // アクティブなウィンドウがデスクトップであることを確認する
                var point = GetMousePosition();                
                if(IsDesktopSpace(point))
                {
                    if (doubleClickChecker.IsMultiEventInvoked(wParam))
                    {
                        // ダブクリックイベントの発火
                        OnDesktopDoubleClick(point.X, point.Y);
                    } else if (rightDoubleClickChecker.IsMultiEventInvoked(wParam))
                    {
                        // 右ダブクリックイベントの発火
                        OnDesktopRightDoubleClick(point.X, point.Y);
                    }
                }
            }

            return CallNextHookEx(hookHandle, nCode, wParam, lParam);
        }

        /// <summary>マウス座標がデスクトップ内の何もない領域であることを判定する</summary>
        /// <param name="x">x座標</param>
        /// <param name="y">y座標</param>
        /// <returns>デスクトップであり、アイコンがない場合true</returns>
        private bool IsDesktopSpace(Point pt)
        {
            IntPtr hWnd = WindowFromPoint(pt);

            if (this.IsDesktopWindow(hWnd))
            {
                int pID = 0;
                IntPtr hProcess = IntPtr.Zero;
                IntPtr p = IntPtr.Zero;

                POINT point = new POINT()
                    { 
                        x = pt.X,
                        y = pt.Y 
                    };
                try
                {
                    ScreenToClient(hWnd, ref point);
                    GetWindowThreadProcessId(hWnd, ref pID);
                    hProcess = OpenProcess(PROCESS_VM_OPERATION | PROCESS_VM_READ | PROCESS_VM_WRITE, false, pID);
                    if (hProcess != IntPtr.Zero)
                    {
                        LVHITTESTINFO lhi = new LVHITTESTINFO();
                        int dw = 0;
                        lhi.pt = point;
                        p = VirtualAllocEx(hProcess, IntPtr.Zero,
                            Marshal.SizeOf(lhi), MEM_COMMIT, PAGE_READWRITE);
                        WriteProcessMemory(hProcess, p, ref lhi, Marshal.SizeOf(lhi), ref dw);
                        SendMessage(hWnd, LVM_SUBITEMHITTEST, IntPtr.Zero, p);
                        ReadProcessMemory(hProcess, p, ref lhi, Marshal.SizeOf(lhi), ref dw);
                        return lhi.iItem < 0;
                    }
                } finally
                {
                    if (p != IntPtr.Zero)
                        VirtualFreeEx(hProcess, p, 0, MEM_RELEASE);
                    if (hProcess != IntPtr.Zero)
                        CloseHandle(hProcess);
                }
            }
            return false;
        }

        /// <summary>
        /// 指定のウィンドウハンドルが、デスクトップを指すかを判定する
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル</param>
        /// <returns>指定のウィンドウハンドルがデスクトップを指す場合true</returns>
        private bool IsDesktopWindow(IntPtr hWnd)
        {
            const string targetWindowClassName = "SysListView32";   // デスクトップのウィンドウクラス名

            // ウィンドウクラス名判定
            if (GetWindowClassName(hWnd) != targetWindowClassName)
            {
                return false;
            }

            // 親ウィンドウを取得
            var hWndParent = GetParent(hWnd);
            const string targetParentWindowClassName = "SHELLDLL_DefView";
            
            // ウィンドウクラス名判定
            return GetWindowClassName(hWndParent) == targetParentWindowClassName;
        }

        /// <summary>
        /// 指定のウィンドウハンドルが指すウィンドウの、ウィンドウクラス名を取得する
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル</param>
        /// <returns>ウィンドウハンドルが指すウィンドウのウィンドウクラス名</returns>
        private string GetWindowClassName(IntPtr hWnd)
        {
            const int bufferSize = 255;
            var stringBuilder = new StringBuilder(bufferSize);
            GetClassName(hWnd, stringBuilder, bufferSize);
            return stringBuilder.ToString();
        }
    
        /// <summary>
        /// マウス位置を取得する
        /// </summary>
        /// <returns>現在のマウス位置</returns>
        private Point GetMousePosition()
        {
            var mousePoint = new Point();
            GetCursorPos(ref mousePoint);
            return mousePoint;
        }

        #endregion

        #region イベント発火プロシージャ

        /// <summary>
        /// デスクトップがダブルクリックされたイベントを発行する
        /// </summary>
        /// <param name="x">マウス位置:x</param>
        /// <param name="y">マウス位置:y</param>
        protected virtual void OnDesktopDoubleClick(int x, int y)
        {
            DesktopDoubleClick?.Invoke(this, new DesktopMouseEventArgs(x, y));
        }

        /// <summary>
        /// デスクトップが右ダブルクリックされたイベントを発行する
        /// </summary>
        /// <param name="x">マウス位置:x</param>
        /// <param name="y">マウス位置:y</param>
        protected virtual void OnDesktopRightDoubleClick(int x, int y)
        {
            DesktopRightDoubleClick?.Invoke(this, new DesktopMouseEventArgs(x, y));
        }

        #endregion

        #region DllImport

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, MouseHookProcDelegate lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(ref Point pt);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(Point point);

        [DllImport("user32.dll")]
        static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);

        [DllImport("user32.dll")]
        static extern int GetWindowThreadProcessId(IntPtr hwnd, ref int pID);
        
        [DllImport("kernel32.dll")]
        static extern IntPtr OpenProcess(int fdwAccess, bool fInherit, int IDProcess);

        [DllImport("kernel32.dll")]
        static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, int dwFreeType);
        
        [DllImport("kernel32.dll")]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,
            ref LVHITTESTINFO lpBuffer, int nSize, ref int lpNumberOfBytesWritten);
        
        [DllImport("kernel32.dll")]
        static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress,int dwSize, int flAllocationType, int flProtect);

        [DllImport("user32.dll")]
        static extern int SendMessage(IntPtr hwnd, int msg, IntPtr wp, IntPtr lp);

        [DllImport("kernel32.dll")]
        static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,
            ref LVHITTESTINFO lpBuffer, int nSize, ref int lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        static extern bool CloseHandle(IntPtr hObject);
        
        [DllImport("User32.Dll", CharSet = CharSet.Unicode)]
        public static extern int GetClassName(IntPtr hWnd,StringBuilder s,int nMaxCount);

        [DllImport("user32.dll")]
        static extern IntPtr GetParent(IntPtr hwnd);

        #endregion

        #region private struct

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct LVHITTESTINFO
        {
            public POINT pt;
            public uint flags;
            public int iItem;
            public int iSubItem;
        }

        #endregion

        #region private delegate

        /// <summary>
        /// マウスフックプロシージャのシグネチャを表すデリゲート型
        /// </summary>
        /// <param name="nCode">対象のメッセージ</param>
        /// <param name="wParam">メッセージのwパラメータ</param>
        /// <param name="lParam">メッセージのlパラメータ</param>
        /// <returns>フックプロシージャのハンドル</returns>
        private delegate IntPtr MouseHookProcDelegate(int nCode, IntPtr wParam, IntPtr lParam);

        #endregion
    }

    /// <summary>
    /// デスクトップのマウスイベントのパラメータ
    /// </summary>
    public class DesktopMouseEventArgs : EventArgs
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="x">x座標</param>
        /// <param name="y">y座標</param>
        public DesktopMouseEventArgs(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// マウス位置:x
        /// </summary>
        public int X { get; }

        /// <summary>
        /// マウス位置:y
        /// </summary>
        public int Y { get; }
    }

}
