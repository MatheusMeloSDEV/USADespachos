namespace Trabalho
{
    partial class FrmModificaCatalogo
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
            BtnRemoverCatalogo = new Button();
            BtnRemoverOrgao = new Button();
            BtnAdicionarOrgao = new Button();
            TbOrgao = new TabControl();
            MAPA = new TabPage();
            lblComunicado = new Label();
            lblColeta = new Label();
            lblInspecao = new Label();
            lblParametrizacao = new Label();
            cbOrgaoComunicado = new ComboBox();
            cbOrgaoParametrizacao = new ComboBox();
            dtpOrgaoColeta = new DateTimePicker();
            dtpOrgaoInspecao = new DateTimePicker();
            cbOrgao = new ComboBox();
            txtcClassTrib = new TextBox();
            txtNCM = new TextBox();
            BtnSalvar = new Button();
            MAPA.SuspendLayout();
            SuspendLayout();
            // 
            // BtnRemoverCatalogo
            // 
            BtnRemoverCatalogo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            BtnRemoverCatalogo.FlatAppearance.BorderSize = 0;
            BtnRemoverCatalogo.Location = new Point(411, 10);
            BtnRemoverCatalogo.Margin = new Padding(0);
            BtnRemoverCatalogo.Name = "BtnRemoverCatalogo";
            BtnRemoverCatalogo.Size = new Size(25, 25);
            BtnRemoverCatalogo.TabIndex = 20;
            BtnRemoverCatalogo.Text = "X";
            BtnRemoverCatalogo.UseVisualStyleBackColor = true;
            // 
            // BtnRemoverOrgao
            // 
            BtnRemoverOrgao.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            BtnRemoverOrgao.FlatAppearance.BorderSize = 0;
            BtnRemoverOrgao.Location = new Point(386, 10);
            BtnRemoverOrgao.Margin = new Padding(0);
            BtnRemoverOrgao.Name = "BtnRemoverOrgao";
            BtnRemoverOrgao.Size = new Size(25, 25);
            BtnRemoverOrgao.TabIndex = 19;
            BtnRemoverOrgao.Text = "-";
            BtnRemoverOrgao.UseVisualStyleBackColor = true;
            // 
            // BtnAdicionarOrgao
            // 
            BtnAdicionarOrgao.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            BtnAdicionarOrgao.FlatAppearance.BorderSize = 0;
            BtnAdicionarOrgao.Location = new Point(361, 10);
            BtnAdicionarOrgao.Margin = new Padding(0);
            BtnAdicionarOrgao.Name = "BtnAdicionarOrgao";
            BtnAdicionarOrgao.Size = new Size(25, 25);
            BtnAdicionarOrgao.TabIndex = 18;
            BtnAdicionarOrgao.Text = "+";
            BtnAdicionarOrgao.UseVisualStyleBackColor = true;
            // 
            // TbOrgao
            // 
            TbOrgao.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            TbOrgao.Location = new Point(8, 41);
            TbOrgao.Name = "TbOrgao";
            TbOrgao.SelectedIndex = 0;
            TbOrgao.Size = new Size(428, 79);
            TbOrgao.TabIndex = 17;
            // 
            // MAPA
            // 
            MAPA.Controls.Add(lblComunicado);
            MAPA.Controls.Add(lblColeta);
            MAPA.Controls.Add(lblInspecao);
            MAPA.Controls.Add(lblParametrizacao);
            MAPA.Controls.Add(cbOrgaoComunicado);
            MAPA.Controls.Add(cbOrgaoParametrizacao);
            MAPA.Controls.Add(dtpOrgaoColeta);
            MAPA.Controls.Add(dtpOrgaoInspecao);
            MAPA.Location = new Point(4, 24);
            MAPA.Name = "MAPA";
            MAPA.Padding = new Padding(3);
            MAPA.Size = new Size(420, 51);
            MAPA.TabIndex = 0;
            MAPA.Text = "MAPA";
            MAPA.UseVisualStyleBackColor = true;
            // 
            // lblComunicado
            // 
            lblComunicado.AutoSize = true;
            lblComunicado.Location = new Point(324, 6);
            lblComunicado.Name = "lblComunicado";
            lblComunicado.Size = new Size(76, 15);
            lblComunicado.TabIndex = 12;
            lblComunicado.Text = "Comunicado";
            // 
            // lblColeta
            // 
            lblColeta.AutoSize = true;
            lblColeta.Location = new Point(237, 6);
            lblColeta.Name = "lblColeta";
            lblColeta.Size = new Size(41, 15);
            lblColeta.TabIndex = 11;
            lblColeta.Text = "Coleta";
            // 
            // lblInspecao
            // 
            lblInspecao.AutoSize = true;
            lblInspecao.Location = new Point(128, 6);
            lblInspecao.Name = "lblInspecao";
            lblInspecao.Size = new Size(54, 15);
            lblInspecao.TabIndex = 10;
            lblInspecao.Text = "Inspeção";
            // 
            // lblParametrizacao
            // 
            lblParametrizacao.AutoSize = true;
            lblParametrizacao.Location = new Point(9, 6);
            lblParametrizacao.Name = "lblParametrizacao";
            lblParametrizacao.Size = new Size(88, 15);
            lblParametrizacao.TabIndex = 9;
            lblParametrizacao.Text = "Parametrização";
            // 
            // cbOrgaoComunicado
            // 
            cbOrgaoComunicado.FormattingEnabled = true;
            cbOrgaoComunicado.Items.AddRange(new object[] { "", "Pendente", "Recebido" });
            cbOrgaoComunicado.Location = new Point(312, 21);
            cbOrgaoComunicado.Name = "cbOrgaoComunicado";
            cbOrgaoComunicado.Size = new Size(100, 23);
            cbOrgaoComunicado.TabIndex = 8;
            cbOrgaoComunicado.Text = "Recebido";
            // 
            // cbOrgaoParametrizacao
            // 
            cbOrgaoParametrizacao.FormattingEnabled = true;
            cbOrgaoParametrizacao.Items.AddRange(new object[] { "", "Verde", "Amarelo", "Vermelho" });
            cbOrgaoParametrizacao.Location = new Point(5, 21);
            cbOrgaoParametrizacao.Name = "cbOrgaoParametrizacao";
            cbOrgaoParametrizacao.Size = new Size(97, 23);
            cbOrgaoParametrizacao.TabIndex = 5;
            cbOrgaoParametrizacao.Text = "Vermelho";
            // 
            // dtpOrgaoColeta
            // 
            dtpOrgaoColeta.Format = DateTimePickerFormat.Short;
            dtpOrgaoColeta.Location = new Point(209, 21);
            dtpOrgaoColeta.Name = "dtpOrgaoColeta";
            dtpOrgaoColeta.Size = new Size(97, 23);
            dtpOrgaoColeta.TabIndex = 7;
            // 
            // dtpOrgaoInspecao
            // 
            dtpOrgaoInspecao.Format = DateTimePickerFormat.Short;
            dtpOrgaoInspecao.Location = new Point(108, 21);
            dtpOrgaoInspecao.Name = "dtpOrgaoInspecao";
            dtpOrgaoInspecao.Size = new Size(95, 23);
            dtpOrgaoInspecao.TabIndex = 6;
            // 
            // cbOrgao
            // 
            cbOrgao.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cbOrgao.FormattingEnabled = true;
            cbOrgao.Items.AddRange(new object[] { "", "MAPA", "ANVISA", "DECEX" });
            cbOrgao.Location = new Point(264, 12);
            cbOrgao.Name = "cbOrgao";
            cbOrgao.Size = new Size(94, 23);
            cbOrgao.TabIndex = 16;
            cbOrgao.Text = "Órgão Anue...";
            // 
            // txtcClassTrib
            // 
            txtcClassTrib.Location = new Point(158, 12);
            txtcClassTrib.Name = "txtcClassTrib";
            txtcClassTrib.PlaceholderText = "cClassTrib";
            txtcClassTrib.Size = new Size(100, 23);
            txtcClassTrib.TabIndex = 15;
            txtcClassTrib.TextChanged += txtcClassTrib_TextChanged;
            // 
            // txtNCM
            // 
            txtNCM.Location = new Point(12, 12);
            txtNCM.Name = "txtNCM";
            txtNCM.PlaceholderText = "NCM";
            txtNCM.Size = new Size(140, 23);
            txtNCM.TabIndex = 14;
            txtNCM.TextChanged += txtNCM_TextChanged;
            // 
            // BtnSalvar
            // 
            BtnSalvar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            BtnSalvar.FlatAppearance.BorderSize = 0;
            BtnSalvar.Location = new Point(359, 41);
            BtnSalvar.Margin = new Padding(0);
            BtnSalvar.Name = "BtnSalvar";
            BtnSalvar.Size = new Size(75, 22);
            BtnSalvar.TabIndex = 21;
            BtnSalvar.Text = "Salvar";
            BtnSalvar.UseVisualStyleBackColor = true;
            BtnSalvar.Click += BtnSalvar_Click;
            // 
            // FrmModificaCatalogo
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(442, 125);
            Controls.Add(BtnSalvar);
            Controls.Add(BtnRemoverCatalogo);
            Controls.Add(BtnRemoverOrgao);
            Controls.Add(BtnAdicionarOrgao);
            Controls.Add(TbOrgao);
            Controls.Add(cbOrgao);
            Controls.Add(txtcClassTrib);
            Controls.Add(txtNCM);
            Name = "FrmModificaCatalogo";
            Text = "Catálogo de Produtos";
            MAPA.ResumeLayout(false);
            MAPA.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button BtnRemoverCatalogo;
        private Button BtnRemoverOrgao;
        private Button BtnAdicionarOrgao;
        private TabControl TbOrgao;
        private TabPage MAPA;
        private Label lblComunicado;
        private Label lblColeta;
        private Label lblInspecao;
        private Label lblParametrizacao;
        private ComboBox cbOrgaoComunicado;
        private ComboBox cbOrgaoParametrizacao;
        private DateTimePicker dtpOrgaoColeta;
        private DateTimePicker dtpOrgaoInspecao;
        private ComboBox cbOrgao;
        private TextBox txtcClassTrib;
        private TextBox txtNCM;
        private Button BtnSalvar;
    }
}