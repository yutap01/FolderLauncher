using FolderLauncher.Forms;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Runtime;
using System.Windows.Forms;

namespace FolderLauncher
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// アクションを発行するキーのリスト
        /// </summary>
        /// <remarks>
        /// 配置順に定義
        /// </remarks>
        private static readonly IList<char> actionKeyList = new List<char>(){
            'a','s','d','f','g','h',
            'q','w','e','r','t','y',
            'z','x','c','v','b','n',
        };

        /// <summary>
        /// フォームを隠すキーのリスト
        /// </summary>
        private static readonly IList<Keys> hideKeyList = new List<Keys>()
        {
            Keys.Escape,Keys.Space
        };

        /// <summary>
        /// RowControlの数
        /// </summary>
        private int RowCount
        {
            get
            {
                return actionKeyList.Count;
            }
        }

        /// <summary>
        /// RowControlのリスト
        /// </summary>
        private IList<RowControl> rowControls = new List<RowControl>();

        /// <summary>
        /// 余白を決定するための比率
        /// </summary>
        /// <remarks>
        /// 余白のサイズは、当コントロールの高さに対するこの比率で決定する
        /// </remarks>
        private double paddingRatio = 0.005;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// [ハンドラ]メインウィンドウの初期化時
        /// </summary>
        public void Initialize()
        {
            Hide();
            ShowInTaskbar = false;
            WindowState = FormWindowState.Minimized;

            // 行コントロールの生成
            foreach (var ch in actionKeyList)
            {
                var rowControl = new RowControl(ch);
                rowControl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                rowControls.Add(rowControl);
                Controls.Add(rowControl);
            }
        }

        /// <summary>
        /// [ハンドラ] サイズが変更された
        /// </summary>
        /// <param name="sender">イベント送信者</param>
        /// <param name="e">イベントパラメータ</param>
        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            // 非表示時は何もしない
            if (WindowState == FormWindowState.Minimized)
            {
                return;
            }

            Debug.WriteLine("MainForm_SizeChanged");

            // 余白の大きさを決定する
            Padding = new Padding((int)(ClientSize.Height * paddingRatio));

            // 余白分を除いた高さ
            var remainHeight = ClientSize.Height - (Padding.Top * (RowCount + 1));

            // 行コントロールの高さを決定する
            var rowHeight = (int)(remainHeight / RowCount);

            // 行コントロールの幅を決定する
            var rowWidth = ClientSize.Width - Padding.Left - Padding.Right;

            // 行コントロールのサイズと位置を設定する
            var top = 0;
            foreach (var rowControl in rowControls)
            {
                top += Padding.Top;

                // サイズ
                rowControl.Size = new Size(rowWidth, rowHeight);

                // 位置
                rowControl.Location = new Point(Padding.Left, top);
                top += rowHeight;
            }
        }

        /// <summary>
        /// [ハンドラ] (フォームにフォカスがある状態で)キーが離された
        /// </summary>
        /// <param name="sender">イベント送信者</param>
        /// <param name="e">イベントパラメータ</param>
        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            Debug.WriteLine("MainForm_KeyUp");

            var key = (Keys)e.KeyCode;
            if (hideKeyList.Contains(key) ){
                WindowState = FormWindowState.Minimized;
            }
        }
    }
}
