namespace Trabalho
{
    partial class NotificacaoUrgente
    {
        /// <summary> 
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Designer de Componentes

        /// <summary> 
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NotificacaoUrgente));
            pictureBox1 = new PictureBox();
            lblUser = new Label();
            tableLayoutPanel1 = new TableLayoutPanel();
            BtnEditar = new PictureBox();
            BtnDone = new PictureBox();
            txtMensagem = new TextBox();
            BtnExcluir = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)BtnEditar).BeginInit();
            ((System.ComponentModel.ISupportInitialize)BtnDone).BeginInit();
            ((System.ComponentModel.ISupportInitialize)BtnExcluir).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Dock = DockStyle.Fill;
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(4, 4);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(19, 19);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // lblUser
            // 
            lblUser.Dock = DockStyle.Fill;
            lblUser.Font = new Font("Segoe UI", 9F);
            lblUser.Location = new Point(30, 4);
            lblUser.Margin = new Padding(3);
            lblUser.Name = "lblUser";
            lblUser.Size = new Size(115, 19);
            lblUser.TabIndex = 1;
            lblUser.Text = "User";
            lblUser.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 25F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(pictureBox1, 0, 0);
            tableLayoutPanel1.Controls.Add(lblUser, 1, 0);
            tableLayoutPanel1.Location = new Point(3, 3);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            tableLayoutPanel1.Size = new Size(149, 25);
            tableLayoutPanel1.TabIndex = 2;
            // 
            // BtnEditar
            // 
            BtnEditar.Image = (Image)resources.GetObject("BtnEditar.Image");
            BtnEditar.Location = new Point(379, 3);
            BtnEditar.Name = "BtnEditar";
            BtnEditar.Size = new Size(25, 25);
            BtnEditar.SizeMode = PictureBoxSizeMode.Zoom;
            BtnEditar.TabIndex = 3;
            BtnEditar.TabStop = false;
            // 
            // BtnDone
            // 
            BtnDone.Image = (Image)resources.GetObject("BtnDone.Image");
            BtnDone.Location = new Point(410, 3);
            BtnDone.Name = "BtnDone";
            BtnDone.Size = new Size(25, 25);
            BtnDone.SizeMode = PictureBoxSizeMode.Zoom;
            BtnDone.TabIndex = 4;
            BtnDone.TabStop = false;
            // 
            // txtMensagem
            // 
            txtMensagem.Location = new Point(3, 32);
            txtMensagem.Multiline = true;
            txtMensagem.Name = "txtMensagem";
            txtMensagem.ReadOnly = true;
            txtMensagem.Size = new Size(432, 41);
            txtMensagem.TabIndex = 5;
            // 
            // BtnExcluir
            // 
            BtnExcluir.Image = (Image)resources.GetObject("BtnExcluir.Image");
            BtnExcluir.Location = new Point(348, 3);
            BtnExcluir.Name = "BtnExcluir";
            BtnExcluir.Size = new Size(25, 25);
            BtnExcluir.SizeMode = PictureBoxSizeMode.Zoom;
            BtnExcluir.TabIndex = 6;
            BtnExcluir.TabStop = false;
            // 
            // NotificacaoUrgente
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(BtnExcluir);
            Controls.Add(txtMensagem);
            Controls.Add(BtnDone);
            Controls.Add(BtnEditar);
            Controls.Add(tableLayoutPanel1);
            Name = "NotificacaoUrgente";
            Size = new Size(441, 86);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)BtnEditar).EndInit();
            ((System.ComponentModel.ISupportInitialize)BtnDone).EndInit();
            ((System.ComponentModel.ISupportInitialize)BtnExcluir).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private Label lblUser;
        private TableLayoutPanel tableLayoutPanel1;
        private PictureBox BtnEditar;
        private PictureBox BtnDone;
        public TextBox txtMensagem;
        public PictureBox BtnExcluir;
    }
}
