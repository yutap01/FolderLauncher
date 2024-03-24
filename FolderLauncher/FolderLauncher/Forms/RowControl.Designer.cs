namespace FolderLauncher.Forms
{
    partial class RowControl
    {
        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            lblKey = new Label();
            lblNote = new Label();
            imgListLocks = new ImageList(components);
            SuspendLayout();
            // 
            // lblKey
            // 
            lblKey.BackColor = Color.White;
            lblKey.BorderStyle = BorderStyle.FixedSingle;
            lblKey.Font = new Font("メイリオ", 24F, FontStyle.Regular, GraphicsUnit.Point, 128);
            lblKey.Location = new Point(10, 10);
            lblKey.Name = "lblKey";
            lblKey.Size = new Size(80, 80);
            lblKey.TabIndex = 0;
            lblKey.Text = "a";
            lblKey.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblNote
            // 
            lblNote.AutoEllipsis = true;
            lblNote.BackColor = Color.White;
            lblNote.BorderStyle = BorderStyle.FixedSingle;
            lblNote.Font = new Font("メイリオ", 24F, FontStyle.Regular, GraphicsUnit.Point, 128);
            lblNote.Location = new Point(100, 10);
            lblNote.Name = "lblNote";
            lblNote.Size = new Size(450, 80);
            lblNote.TabIndex = 1;
            lblNote.Text = "Games";
            lblNote.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // imgListLocks
            // 
            imgListLocks.ColorDepth = ColorDepth.Depth32Bit;
            imgListLocks.ImageSize = new Size(16, 16);
            imgListLocks.TransparentColor = Color.Transparent;
            // 
            // RowControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ButtonShadow;
            BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(lblNote);
            Controls.Add(lblKey);
            Margin = new Padding(0, 0, 10, 0);
            Name = "RowControl";
            Padding = new Padding(10);
            Size = new Size(560, 98);
            SizeChanged += RowControll_SizeChanged;
            ResumeLayout(false);
        }

        #endregion

        private Label lblKey;
        private Label lblNote;
        private ImageList imgListLocks;
    }
}
