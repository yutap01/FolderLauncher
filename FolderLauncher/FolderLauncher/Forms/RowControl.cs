
namespace FolderLauncher.Forms
{
    /// <summary>
    /// 行単位のコントロール
    /// </summary>
    public partial class RowControl : UserControl
    {
        /// <summary>
        /// Padding比率
        /// </summary>
        /// <remarks>
        /// 当コントロールの高さに対するPaddingの比率
        /// </remarks>
        private const double paddingRatio = 0.1;

        /// <summary>
        /// フォントサイズ比率
        /// </summary>
        /// <remarks>
        /// コントロールの高さに対する比率
        /// </remarks>
        private const double fontSizeRatio = 0.5;

        /// <summary>
        /// 正方形の子コントロール
        /// </summary>
        private IList<Control> squareChildControls = new List<Control>();

        /// <summary>
        /// 子コントロール
        /// </summary>
        /// <remarks>
        /// より左に配置されるものほど、リストの先頭に記述する
        /// </remarks>
        private IList<Control> childControls = new List<Control>();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="charCode">対象のキーの文字コード</param>
        public RowControl(char charCode)
        {
            InitializeComponent();

            // 子コントロールのうち、形状が正方形のもの
            squareChildControls.Add(lblKey);

            // 子コントロールの配置順を定義するリスト
            childControls.Add(lblKey);
            childControls.Add(lblNote);

            lblKey.Text = charCode.ToString();
        }

        /// <summary>
        /// 自コントロールのPaddingを決定する
        /// </summary>
        private void DetectPadding()
        {
            var padding = (int)(this.Height * paddingRatio);
            Padding = new Padding(padding);
        }

        /// <summary>
        /// 子コントロールのサイズを決定する
        /// </summary>
        private void DetectChildControlSize()
        {
            // 全ての子コントロールの高さは、自コントロールの高さから縦方向のPaddingを引いたもの
            var childHeight = this.Height - Padding.Vertical;

            foreach (Control control in Controls)
            {
                // 正方形のコントロール
                if (squareChildControls!.Contains(control))
                {
                    control.Size = new Size(childHeight, childHeight);
                }
            }

            // 残りの幅
            var remainWidth = this.Width - (Padding.Left * (childControls!.Count + 1)) - squareChildControls!.Sum(c => c.Width);

            // ノートラベルの幅は、残りの幅
            lblNote.Size = new Size(remainWidth, childHeight);
        }

        /// <summary>
        /// 子コントロールのフォントサイズを決定する
        /// </summary>
        private void DetectChildControlFontSize()
        {
            // フォントサイズを決定
            var fontSize = (int)(lblKey.Height * fontSizeRatio);

            lblKey.Font = new Font(lblKey.Font.FontFamily, fontSize);
            lblNote.Font = new Font(lblNote.Font.FontFamily, fontSize);
        }

        /// <summary>
        /// 子コントロールの位置を決定する
        /// </summary>
        private void DetectChildControlLocation()
        {
            // 上位置は全ての子コントロールに共通
            var top = Padding.Top;

            // 次のコントロールの左位置
            var nextLeft = 0;

            // 各コントロールの位置を決定
            foreach (Control control in childControls!)
            {
                // 左位置
                var left = nextLeft + Padding.Left;
                nextLeft = left + control.Width;

                // 位置を設定
                control.Location = new Point(left, top);
            }
        }

        /// <summary>
        /// [ハンドラ]サイズ変更
        /// </summary>
        /// <param name="sender">イベント送信者</param>
        /// <param name="e">イベントパラメータ</param>
        private void RowControll_SizeChanged(object sender, EventArgs e)
        {
            //自コントロールのPaddingを決定する
            DetectPadding();

            //子コントロールのサイズと位置を決定
            DetectChildControlSize();

            //子コントロールのフォントのサイズを決定
            DetectChildControlFontSize();
            
            //子コントロールの位置を決定
            DetectChildControlLocation();
        }
    }
}
