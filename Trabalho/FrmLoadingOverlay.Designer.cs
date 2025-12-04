namespace Trabalho
{
    partial class FrmLoadingOverlay
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tblLoading = new TableLayoutPanel();
            picLoading = new PictureBox();
            lblLoading = new Label();
            tblLoading.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picLoading).BeginInit();
            SuspendLayout();
            // 
            // tblLoading
            // 
            tblLoading.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tblLoading.BackColor = Color.Transparent;
            tblLoading.ColumnCount = 1;
            tblLoading.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tblLoading.Controls.Add(picLoading, 0, 0);
            tblLoading.Controls.Add(lblLoading, 0, 1);
            tblLoading.Location = new Point(291, 176);
            tblLoading.Name = "tblLoading";
            tblLoading.RowCount = 2;
            tblLoading.RowStyles.Add(new RowStyle());
            tblLoading.RowStyles.Add(new RowStyle());
            tblLoading.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tblLoading.Size = new Size(218, 98);
            tblLoading.TabIndex = 3;
            // 
            // picLoading
            // 
            picLoading.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            picLoading.BackColor = Color.Transparent;
            picLoading.Image = Properties.Resources.gif;
            picLoading.Location = new Point(3, 3);
            picLoading.Name = "picLoading";
            picLoading.Size = new Size(212, 50);
            picLoading.SizeMode = PictureBoxSizeMode.Zoom;
            picLoading.TabIndex = 1;
            picLoading.TabStop = false;
            // 
            // lblLoading
            // 
            lblLoading.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblLoading.AutoSize = true;
            lblLoading.BackColor = Color.Transparent;
            lblLoading.Font = new Font("Segoe UI", 20F);
            lblLoading.ForeColor = SystemColors.ControlLightLight;
            lblLoading.Location = new Point(3, 56);
            lblLoading.Name = "lblLoading";
            lblLoading.Size = new Size(212, 42);
            lblLoading.TabIndex = 0;
            lblLoading.Text = "Carregando...";
            lblLoading.TextAlign = ContentAlignment.TopCenter;
            // 
            // FrmLoadingOverlay
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(800, 450);
            ControlBox = false;
            Controls.Add(tblLoading);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FrmLoadingOverlay";
            Opacity = 0.4D;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            Text = "FrmLoadingOverlay";
            TopMost = true;
            Load += FrmLoadingOverlay_Load;
            Resize += FrmLoadingOverlay_Resize;
            tblLoading.ResumeLayout(false);
            tblLoading.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picLoading).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tblLoading;
        private PictureBox picLoading;
        public Label lblLoading;
    }
}