namespace Trabalho
{
    partial class LpcoEditControl
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
            CbParametrizacao = new ComboBox();
            CbMotivoExigencia = new ComboBox();
            lblDataDeferimento = new Label();
            DtpDataDeferimentoLPCO = new DateTimePicker();
            lblDataRegistro = new Label();
            DtpDataRegistroLPCO = new DateTimePicker();
            lblParametrizacao = new Label();
            TxtLPCO = new TextBox();
            lblLPCO = new Label();
            SuspendLayout();
            // 
            // CbParametrizacao
            // 
            CbParametrizacao.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            CbParametrizacao.FlatStyle = FlatStyle.Flat;
            CbParametrizacao.FormattingEnabled = true;
            CbParametrizacao.Items.AddRange(new object[] { "", "Documental", "Exame Físico", "Conferência Física", "Coleta de Amostra", "Inspeção Física" });
            CbParametrizacao.Location = new Point(113, 61);
            CbParametrizacao.Name = "CbParametrizacao";
            CbParametrizacao.Size = new Size(140, 23);
            CbParametrizacao.TabIndex = 2;
            // 
            // CbMotivoExigencia
            // 
            CbMotivoExigencia.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            CbMotivoExigencia.FlatStyle = FlatStyle.Flat;
            CbMotivoExigencia.FormattingEnabled = true;
            CbMotivoExigencia.Items.AddRange(new object[] { "EXIGÊNCIA PENDENTE", "EXIGÊNCIA CUMPRIDA", "DEFERIDO" });
            CbMotivoExigencia.Location = new Point(345, 69);
            CbMotivoExigencia.Name = "CbMotivoExigencia";
            CbMotivoExigencia.Size = new Size(135, 23);
            CbMotivoExigencia.TabIndex = 5;
            CbMotivoExigencia.Text = "Status do LPCO...";
            // 
            // lblDataDeferimento
            // 
            lblDataDeferimento.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblDataDeferimento.AutoSize = true;
            lblDataDeferimento.Font = new Font("Segoe UI", 11F);
            lblDataDeferimento.Location = new Point(422, 16);
            lblDataDeferimento.Name = "lblDataDeferimento";
            lblDataDeferimento.Size = new Size(129, 20);
            lblDataDeferimento.TabIndex = 435;
            lblDataDeferimento.Text = "Data Deferimento";
            // 
            // DtpDataDeferimentoLPCO
            // 
            DtpDataDeferimentoLPCO.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            DtpDataDeferimentoLPCO.Format = DateTimePickerFormat.Short;
            DtpDataDeferimentoLPCO.Location = new Point(419, 40);
            DtpDataDeferimentoLPCO.Name = "DtpDataDeferimentoLPCO";
            DtpDataDeferimentoLPCO.Size = new Size(135, 23);
            DtpDataDeferimentoLPCO.TabIndex = 4;
            // 
            // lblDataRegistro
            // 
            lblDataRegistro.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblDataRegistro.AutoSize = true;
            lblDataRegistro.Font = new Font("Segoe UI", 11F);
            lblDataRegistro.Location = new Point(287, 16);
            lblDataRegistro.Name = "lblDataRegistro";
            lblDataRegistro.Size = new Size(100, 20);
            lblDataRegistro.TabIndex = 433;
            lblDataRegistro.Text = "Data Registro";
            // 
            // DtpDataRegistroLPCO
            // 
            DtpDataRegistroLPCO.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            DtpDataRegistroLPCO.Format = DateTimePickerFormat.Short;
            DtpDataRegistroLPCO.Location = new Point(270, 40);
            DtpDataRegistroLPCO.Name = "DtpDataRegistroLPCO";
            DtpDataRegistroLPCO.Size = new Size(135, 23);
            DtpDataRegistroLPCO.TabIndex = 3;
            // 
            // lblParametrizacao
            // 
            lblParametrizacao.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblParametrizacao.AutoSize = true;
            lblParametrizacao.Location = new Point(19, 64);
            lblParametrizacao.Name = "lblParametrizacao";
            lblParametrizacao.Size = new Size(88, 15);
            lblParametrizacao.TabIndex = 431;
            lblParametrizacao.Text = "Parametrização";
            // 
            // TxtLPCO
            // 
            TxtLPCO.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            TxtLPCO.Location = new Point(63, 27);
            TxtLPCO.Name = "TxtLPCO";
            TxtLPCO.Size = new Size(190, 23);
            TxtLPCO.TabIndex = 1;
            // 
            // lblLPCO
            // 
            lblLPCO.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblLPCO.AutoSize = true;
            lblLPCO.Location = new Point(19, 30);
            lblLPCO.Name = "lblLPCO";
            lblLPCO.Size = new Size(37, 15);
            lblLPCO.TabIndex = 429;
            lblLPCO.Text = "LPCO";
            // 
            // LpcoEditControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(CbParametrizacao);
            Controls.Add(CbMotivoExigencia);
            Controls.Add(lblDataDeferimento);
            Controls.Add(DtpDataDeferimentoLPCO);
            Controls.Add(lblDataRegistro);
            Controls.Add(DtpDataRegistroLPCO);
            Controls.Add(lblParametrizacao);
            Controls.Add(TxtLPCO);
            Controls.Add(lblLPCO);
            Name = "LpcoEditControl";
            Size = new Size(585, 109);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox CbParametrizacao;
        private ComboBox CbMotivoExigencia;
        private Label lblDataDeferimento;
        private DateTimePicker DtpDataDeferimentoLPCO;
        private Label lblDataRegistro;
        private DateTimePicker DtpDataRegistroLPCO;
        private Label lblParametrizacao;
        private TextBox TxtLPCO;
        private Label lblLPCO;
    }
}
