namespace Trabalho
{
    partial class FrmStatusProcessos
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmStatusProcessos));
            Blocos = new TableLayoutPanel();
            BtnDIDUIMPParaDigitacao = new Label();
            BtnSolicitarNumerario = new Label();
            BtnDeferidos = new Label();
            BtnAguardandoCE = new Label();
            BtnAtracadosSPresencaDeCarga = new Label();
            BtnSituacaoSIGVIG = new Label();
            BtnAtracadosCPresencaDeCarga = new Label();
            BtnParaRedestinar = new Label();
            BtnRedestinados = new Label();
            MostrarItens = new TableLayoutPanel();
            LblTitulo = new Label();
            BtnVoltar = new PictureBox();
            DGVSelecionado = new DataGridView();
            Blocos.SuspendLayout();
            MostrarItens.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)BtnVoltar).BeginInit();
            ((System.ComponentModel.ISupportInitialize)DGVSelecionado).BeginInit();
            SuspendLayout();
            // 
            // Blocos
            // 
            Blocos.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            Blocos.ColumnCount = 3;
            Blocos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            Blocos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333359F));
            Blocos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333359F));
            Blocos.Controls.Add(BtnDIDUIMPParaDigitacao, 2, 2);
            Blocos.Controls.Add(BtnSolicitarNumerario, 1, 2);
            Blocos.Controls.Add(BtnDeferidos, 0, 2);
            Blocos.Controls.Add(BtnAguardandoCE, 0, 0);
            Blocos.Controls.Add(BtnAtracadosSPresencaDeCarga, 0, 1);
            Blocos.Controls.Add(BtnSituacaoSIGVIG, 1, 1);
            Blocos.Controls.Add(BtnAtracadosCPresencaDeCarga, 2, 1);
            Blocos.Controls.Add(BtnParaRedestinar, 1, 0);
            Blocos.Controls.Add(BtnRedestinados, 2, 0);
            Blocos.Location = new Point(160, 73);
            Blocos.Name = "Blocos";
            Blocos.RowCount = 3;
            Blocos.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            Blocos.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            Blocos.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            Blocos.Size = new Size(944, 397);
            Blocos.TabIndex = 11;
            // 
            // BtnDIDUIMPParaDigitacao
            // 
            BtnDIDUIMPParaDigitacao.AutoSize = true;
            BtnDIDUIMPParaDigitacao.BackColor = Color.FromArgb(192, 0, 0);
            BtnDIDUIMPParaDigitacao.Dock = DockStyle.Fill;
            BtnDIDUIMPParaDigitacao.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            BtnDIDUIMPParaDigitacao.Location = new Point(648, 284);
            BtnDIDUIMPParaDigitacao.Margin = new Padding(20);
            BtnDIDUIMPParaDigitacao.Name = "BtnDIDUIMPParaDigitacao";
            BtnDIDUIMPParaDigitacao.Padding = new Padding(30, 15, 30, 15);
            BtnDIDUIMPParaDigitacao.Size = new Size(276, 93);
            BtnDIDUIMPParaDigitacao.TabIndex = 11;
            BtnDIDUIMPParaDigitacao.Text = "DI/DUIMP para Digitação";
            BtnDIDUIMPParaDigitacao.TextAlign = ContentAlignment.MiddleCenter;
            BtnDIDUIMPParaDigitacao.UseCompatibleTextRendering = true;
            BtnDIDUIMPParaDigitacao.Click += BtnDIDUIMPParaDigitacao_Click;
            // 
            // BtnSolicitarNumerario
            // 
            BtnSolicitarNumerario.AutoSize = true;
            BtnSolicitarNumerario.BackColor = Color.FromArgb(255, 192, 192);
            BtnSolicitarNumerario.Dock = DockStyle.Fill;
            BtnSolicitarNumerario.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            BtnSolicitarNumerario.Location = new Point(334, 284);
            BtnSolicitarNumerario.Margin = new Padding(20);
            BtnSolicitarNumerario.Name = "BtnSolicitarNumerario";
            BtnSolicitarNumerario.Padding = new Padding(30, 15, 30, 15);
            BtnSolicitarNumerario.Size = new Size(274, 93);
            BtnSolicitarNumerario.TabIndex = 10;
            BtnSolicitarNumerario.Text = "Solicitar Numerário";
            BtnSolicitarNumerario.TextAlign = ContentAlignment.MiddleCenter;
            BtnSolicitarNumerario.UseCompatibleTextRendering = true;
            BtnSolicitarNumerario.Click += BtnSolicitarNumerario_Click;
            // 
            // BtnDeferidos
            // 
            BtnDeferidos.AutoSize = true;
            BtnDeferidos.BackColor = Color.Lime;
            BtnDeferidos.Dock = DockStyle.Fill;
            BtnDeferidos.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            BtnDeferidos.Location = new Point(20, 284);
            BtnDeferidos.Margin = new Padding(20);
            BtnDeferidos.Name = "BtnDeferidos";
            BtnDeferidos.Padding = new Padding(30, 15, 30, 15);
            BtnDeferidos.Size = new Size(274, 93);
            BtnDeferidos.TabIndex = 9;
            BtnDeferidos.Text = "Deferidos";
            BtnDeferidos.TextAlign = ContentAlignment.MiddleCenter;
            BtnDeferidos.UseCompatibleTextRendering = true;
            BtnDeferidos.Click += BtnDeferidos_Click;
            // 
            // BtnAguardandoCE
            // 
            BtnAguardandoCE.AutoSize = true;
            BtnAguardandoCE.BackColor = Color.BlueViolet;
            BtnAguardandoCE.Dock = DockStyle.Fill;
            BtnAguardandoCE.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            BtnAguardandoCE.Location = new Point(20, 20);
            BtnAguardandoCE.Margin = new Padding(20);
            BtnAguardandoCE.Name = "BtnAguardandoCE";
            BtnAguardandoCE.Padding = new Padding(30, 15, 30, 15);
            BtnAguardandoCE.Size = new Size(274, 92);
            BtnAguardandoCE.TabIndex = 3;
            BtnAguardandoCE.Text = "Aguardando CE";
            BtnAguardandoCE.TextAlign = ContentAlignment.MiddleCenter;
            BtnAguardandoCE.UseCompatibleTextRendering = true;
            BtnAguardandoCE.Click += BtnAguardandoCE_Click;
            // 
            // BtnAtracadosSPresencaDeCarga
            // 
            BtnAtracadosSPresencaDeCarga.AutoSize = true;
            BtnAtracadosSPresencaDeCarga.BackColor = Color.Yellow;
            BtnAtracadosSPresencaDeCarga.Dock = DockStyle.Fill;
            BtnAtracadosSPresencaDeCarga.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            BtnAtracadosSPresencaDeCarga.Location = new Point(20, 152);
            BtnAtracadosSPresencaDeCarga.Margin = new Padding(20);
            BtnAtracadosSPresencaDeCarga.Name = "BtnAtracadosSPresencaDeCarga";
            BtnAtracadosSPresencaDeCarga.Padding = new Padding(30, 15, 30, 15);
            BtnAtracadosSPresencaDeCarga.Size = new Size(274, 92);
            BtnAtracadosSPresencaDeCarga.TabIndex = 4;
            BtnAtracadosSPresencaDeCarga.Text = "Atracados S/Presença de Carga";
            BtnAtracadosSPresencaDeCarga.TextAlign = ContentAlignment.MiddleCenter;
            BtnAtracadosSPresencaDeCarga.UseCompatibleTextRendering = true;
            BtnAtracadosSPresencaDeCarga.Click += BtnAtracadosSPresencaDeCarga_Click;
            // 
            // BtnSituacaoSIGVIG
            // 
            BtnSituacaoSIGVIG.AutoSize = true;
            BtnSituacaoSIGVIG.BackColor = Color.FromArgb(255, 128, 0);
            BtnSituacaoSIGVIG.Dock = DockStyle.Fill;
            BtnSituacaoSIGVIG.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            BtnSituacaoSIGVIG.Location = new Point(334, 152);
            BtnSituacaoSIGVIG.Margin = new Padding(20);
            BtnSituacaoSIGVIG.Name = "BtnSituacaoSIGVIG";
            BtnSituacaoSIGVIG.Padding = new Padding(30, 15, 30, 15);
            BtnSituacaoSIGVIG.Size = new Size(274, 92);
            BtnSituacaoSIGVIG.TabIndex = 7;
            BtnSituacaoSIGVIG.Text = "Situação SIGVIG";
            BtnSituacaoSIGVIG.TextAlign = ContentAlignment.MiddleCenter;
            BtnSituacaoSIGVIG.UseCompatibleTextRendering = true;
            BtnSituacaoSIGVIG.Click += BtnSituacaoSIGVIG_Click;
            // 
            // BtnAtracadosCPresencaDeCarga
            // 
            BtnAtracadosCPresencaDeCarga.AutoSize = true;
            BtnAtracadosCPresencaDeCarga.BackColor = Color.Black;
            BtnAtracadosCPresencaDeCarga.Dock = DockStyle.Fill;
            BtnAtracadosCPresencaDeCarga.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            BtnAtracadosCPresencaDeCarga.ForeColor = Color.White;
            BtnAtracadosCPresencaDeCarga.Location = new Point(648, 152);
            BtnAtracadosCPresencaDeCarga.Margin = new Padding(20);
            BtnAtracadosCPresencaDeCarga.Name = "BtnAtracadosCPresencaDeCarga";
            BtnAtracadosCPresencaDeCarga.Padding = new Padding(30, 15, 30, 15);
            BtnAtracadosCPresencaDeCarga.Size = new Size(276, 92);
            BtnAtracadosCPresencaDeCarga.TabIndex = 8;
            BtnAtracadosCPresencaDeCarga.Text = "Atracados com Presença de Carga";
            BtnAtracadosCPresencaDeCarga.TextAlign = ContentAlignment.MiddleCenter;
            BtnAtracadosCPresencaDeCarga.UseCompatibleTextRendering = true;
            BtnAtracadosCPresencaDeCarga.Click += BtnAtracadosCPresencaDeCarga_Click;
            // 
            // BtnParaRedestinar
            // 
            BtnParaRedestinar.AutoSize = true;
            BtnParaRedestinar.BackColor = Color.Red;
            BtnParaRedestinar.Dock = DockStyle.Fill;
            BtnParaRedestinar.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            BtnParaRedestinar.Location = new Point(334, 20);
            BtnParaRedestinar.Margin = new Padding(20);
            BtnParaRedestinar.Name = "BtnParaRedestinar";
            BtnParaRedestinar.Padding = new Padding(30, 15, 30, 15);
            BtnParaRedestinar.Size = new Size(274, 92);
            BtnParaRedestinar.TabIndex = 6;
            BtnParaRedestinar.Text = "Para Redestinar";
            BtnParaRedestinar.TextAlign = ContentAlignment.MiddleCenter;
            BtnParaRedestinar.UseCompatibleTextRendering = true;
            BtnParaRedestinar.Click += BtnParaRedestinar_Click;
            // 
            // BtnRedestinados
            // 
            BtnRedestinados.AutoSize = true;
            BtnRedestinados.BackColor = Color.FromArgb(0, 192, 192);
            BtnRedestinados.Dock = DockStyle.Fill;
            BtnRedestinados.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            BtnRedestinados.Location = new Point(648, 20);
            BtnRedestinados.Margin = new Padding(20);
            BtnRedestinados.Name = "BtnRedestinados";
            BtnRedestinados.Padding = new Padding(30, 15, 30, 15);
            BtnRedestinados.Size = new Size(276, 92);
            BtnRedestinados.TabIndex = 5;
            BtnRedestinados.Text = "Redestinados";
            BtnRedestinados.TextAlign = ContentAlignment.MiddleCenter;
            BtnRedestinados.UseCompatibleTextRendering = true;
            BtnRedestinados.Click += BtnRedestinados_Click;
            // 
            // MostrarItens
            // 
            MostrarItens.ColumnCount = 2;
            MostrarItens.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40F));
            MostrarItens.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            MostrarItens.Controls.Add(LblTitulo, 1, 0);
            MostrarItens.Controls.Add(BtnVoltar, 0, 0);
            MostrarItens.Controls.Add(DGVSelecionado, 0, 1);
            MostrarItens.Dock = DockStyle.Fill;
            MostrarItens.Location = new Point(0, 0);
            MostrarItens.Name = "MostrarItens";
            MostrarItens.RowCount = 3;
            MostrarItens.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            MostrarItens.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            MostrarItens.RowStyles.Add(new RowStyle(SizeType.Absolute, 343F));
            MostrarItens.Size = new Size(1264, 681);
            MostrarItens.TabIndex = 12;
            // 
            // LblTitulo
            // 
            LblTitulo.AutoSize = true;
            LblTitulo.Dock = DockStyle.Fill;
            LblTitulo.Font = new Font("Segoe UI", 20F);
            LblTitulo.Location = new Point(43, 0);
            LblTitulo.Name = "LblTitulo";
            LblTitulo.Size = new Size(1218, 40);
            LblTitulo.TabIndex = 1;
            LblTitulo.Text = "Em Andamento";
            LblTitulo.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // BtnVoltar
            // 
            BtnVoltar.Dock = DockStyle.Fill;
            BtnVoltar.Image = (Image)resources.GetObject("BtnVoltar.Image");
            BtnVoltar.Location = new Point(3, 3);
            BtnVoltar.Name = "BtnVoltar";
            BtnVoltar.Size = new Size(34, 34);
            BtnVoltar.SizeMode = PictureBoxSizeMode.StretchImage;
            BtnVoltar.TabIndex = 2;
            BtnVoltar.TabStop = false;
            BtnVoltar.Click += BtnVoltar_Click;
            // 
            // DGVSelecionado
            // 
            DGVSelecionado.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            MostrarItens.SetColumnSpan(DGVSelecionado, 3);
            DGVSelecionado.Dock = DockStyle.Fill;
            DGVSelecionado.Location = new Point(3, 43);
            DGVSelecionado.Name = "DGVSelecionado";
            MostrarItens.SetRowSpan(DGVSelecionado, 2);
            DGVSelecionado.RowTemplate.Height = 25;
            DGVSelecionado.Size = new Size(1258, 635);
            DGVSelecionado.TabIndex = 3;
            DGVSelecionado.CellDoubleClick += DGVSelecionado_CellDoubleClick;
            // 
            // FrmStatusProcessos
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1264, 681);
            Controls.Add(Blocos);
            Controls.Add(MostrarItens);
            Name = "FrmStatusProcessos";
            Text = "FrmStatusProcessos";
            Load += FrmStatusProcessos_Load;
            Blocos.ResumeLayout(false);
            Blocos.PerformLayout();
            MostrarItens.ResumeLayout(false);
            MostrarItens.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)BtnVoltar).EndInit();
            ((System.ComponentModel.ISupportInitialize)DGVSelecionado).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel Blocos;
        private Label BtnAguardandoCE;
        private Label BtnAtracadosSPresencaDeCarga;
        private Label BtnSituacaoSIGVIG;
        private Label BtnAtracadosCPresencaDeCarga;
        private Label BtnParaRedestinar;
        private Label BtnRedestinados;
        private TableLayoutPanel MostrarItens;
        private Label BtnDIDUIMPParaDigitacao;
        private Label BtnSolicitarNumerario;
        private Label BtnDeferidos;
        private Label LblTitulo;
        private PictureBox BtnVoltar;
        private DataGridView DGVSelecionado;
    }
}