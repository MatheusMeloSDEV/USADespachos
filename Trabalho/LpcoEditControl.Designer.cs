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
            components = new System.ComponentModel.Container();
            CbParametrizacao = new ComboBox();
            CbMotivoExigencia = new ComboBox();
            lblDataDeferimento = new Label();
            DtpDataDeferimentoLPCO = new DateTimePicker();
            lblDataRegistro = new Label();
            DtpDataRegistroLPCO = new DateTimePicker();
            lblParametrizacao = new Label();
            TxtLPCO = new TextBox();
            lblLPCO = new Label();
            bindingSource1 = new BindingSource(components);
            CbStatusLPCO = new ComboBox();
            tableLayoutPanel1 = new TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)bindingSource1).BeginInit();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // CbParametrizacao
            // 
            CbParametrizacao.Dock = DockStyle.Fill;
            CbParametrizacao.FlatStyle = FlatStyle.Flat;
            CbParametrizacao.FormattingEnabled = true;
            CbParametrizacao.Items.AddRange(new object[] { "", "Documental", "Exame Físico", "Conferência Física", "Coleta de Amostra", "Inspeção Física" });
            CbParametrizacao.Location = new Point(151, 31);
            CbParametrizacao.Name = "CbParametrizacao";
            CbParametrizacao.Size = new Size(139, 23);
            CbParametrizacao.TabIndex = 2;
            // 
            // CbMotivoExigencia
            // 
            CbMotivoExigencia.Dock = DockStyle.Fill;
            CbMotivoExigencia.FlatStyle = FlatStyle.Flat;
            CbMotivoExigencia.FormattingEnabled = true;
            CbMotivoExigencia.Items.AddRange(new object[] { "", "EXIGÊNCIA PENDENTE", "EXIGÊNCIA CUMPRIDA", "DEFERIDO", "CANCELADA" });
            CbMotivoExigencia.Location = new Point(296, 62);
            CbMotivoExigencia.Name = "CbMotivoExigencia";
            CbMotivoExigencia.Size = new Size(139, 23);
            CbMotivoExigencia.TabIndex = 5;
            CbMotivoExigencia.Text = "Motivo LPCO...";
            // 
            // lblDataDeferimento
            // 
            lblDataDeferimento.AutoSize = true;
            lblDataDeferimento.Dock = DockStyle.Fill;
            lblDataDeferimento.Font = new Font("Microsoft Sans Serif", 11.25F);
            lblDataDeferimento.Location = new Point(441, 0);
            lblDataDeferimento.Name = "lblDataDeferimento";
            lblDataDeferimento.Size = new Size(141, 28);
            lblDataDeferimento.TabIndex = 435;
            lblDataDeferimento.Text = "Data Deferimento";
            lblDataDeferimento.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // DtpDataDeferimentoLPCO
            // 
            DtpDataDeferimentoLPCO.Dock = DockStyle.Fill;
            DtpDataDeferimentoLPCO.Format = DateTimePickerFormat.Short;
            DtpDataDeferimentoLPCO.Location = new Point(441, 31);
            DtpDataDeferimentoLPCO.Name = "DtpDataDeferimentoLPCO";
            DtpDataDeferimentoLPCO.Size = new Size(141, 23);
            DtpDataDeferimentoLPCO.TabIndex = 4;
            // 
            // lblDataRegistro
            // 
            lblDataRegistro.AutoSize = true;
            lblDataRegistro.Dock = DockStyle.Fill;
            lblDataRegistro.Font = new Font("Microsoft Sans Serif", 11.25F);
            lblDataRegistro.Location = new Point(296, 0);
            lblDataRegistro.Name = "lblDataRegistro";
            lblDataRegistro.Size = new Size(139, 28);
            lblDataRegistro.TabIndex = 433;
            lblDataRegistro.Text = "Data Registro";
            lblDataRegistro.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // DtpDataRegistroLPCO
            // 
            DtpDataRegistroLPCO.Dock = DockStyle.Fill;
            DtpDataRegistroLPCO.Format = DateTimePickerFormat.Short;
            DtpDataRegistroLPCO.Location = new Point(296, 31);
            DtpDataRegistroLPCO.Name = "DtpDataRegistroLPCO";
            DtpDataRegistroLPCO.Size = new Size(139, 23);
            DtpDataRegistroLPCO.TabIndex = 3;
            // 
            // lblParametrizacao
            // 
            lblParametrizacao.AutoSize = true;
            lblParametrizacao.Dock = DockStyle.Fill;
            lblParametrizacao.Font = new Font("Microsoft Sans Serif", 11.25F);
            lblParametrizacao.Location = new Point(151, 0);
            lblParametrizacao.Name = "lblParametrizacao";
            lblParametrizacao.Size = new Size(139, 28);
            lblParametrizacao.TabIndex = 431;
            lblParametrizacao.Text = "Parametrização";
            lblParametrizacao.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // TxtLPCO
            // 
            TxtLPCO.Dock = DockStyle.Fill;
            TxtLPCO.Location = new Point(3, 31);
            TxtLPCO.Name = "TxtLPCO";
            TxtLPCO.Size = new Size(142, 23);
            TxtLPCO.TabIndex = 1;
            // 
            // lblLPCO
            // 
            lblLPCO.AutoSize = true;
            lblLPCO.Dock = DockStyle.Fill;
            lblLPCO.Font = new Font("Microsoft Sans Serif", 11.25F);
            lblLPCO.Location = new Point(3, 0);
            lblLPCO.Name = "lblLPCO";
            lblLPCO.Size = new Size(142, 28);
            lblLPCO.TabIndex = 429;
            lblLPCO.Text = "LPCO";
            lblLPCO.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // CbStatusLPCO
            // 
            CbStatusLPCO.Dock = DockStyle.Fill;
            CbStatusLPCO.FlatStyle = FlatStyle.Flat;
            CbStatusLPCO.FormattingEnabled = true;
            CbStatusLPCO.Items.AddRange(new object[] { "Pronto para Entrada", "Pendência Documental", "Entrada Concluída" });
            CbStatusLPCO.Location = new Point(441, 62);
            CbStatusLPCO.Name = "CbStatusLPCO";
            CbStatusLPCO.Size = new Size(141, 23);
            CbStatusLPCO.TabIndex = 436;
            CbStatusLPCO.Text = "Status LPCO...";
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 4;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 148F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333359F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333359F));
            tableLayoutPanel1.Controls.Add(lblLPCO, 0, 0);
            tableLayoutPanel1.Controls.Add(CbStatusLPCO, 3, 2);
            tableLayoutPanel1.Controls.Add(TxtLPCO, 0, 1);
            tableLayoutPanel1.Controls.Add(DtpDataDeferimentoLPCO, 3, 1);
            tableLayoutPanel1.Controls.Add(lblDataDeferimento, 3, 0);
            tableLayoutPanel1.Controls.Add(CbMotivoExigencia, 2, 2);
            tableLayoutPanel1.Controls.Add(CbParametrizacao, 1, 1);
            tableLayoutPanel1.Controls.Add(lblParametrizacao, 1, 0);
            tableLayoutPanel1.Controls.Add(lblDataRegistro, 2, 0);
            tableLayoutPanel1.Controls.Add(DtpDataRegistroLPCO, 2, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 47.4576263F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 52.5423737F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 31F));
            tableLayoutPanel1.Size = new Size(585, 90);
            tableLayoutPanel1.TabIndex = 437;
            // 
            // LpcoEditControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanel1);
            Name = "LpcoEditControl";
            Size = new Size(585, 90);
            ((System.ComponentModel.ISupportInitialize)bindingSource1).EndInit();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
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
        private BindingSource bindingSource1;
        private ComboBox CbStatusLPCO;
        private TableLayoutPanel tableLayoutPanel1;
    }
}
