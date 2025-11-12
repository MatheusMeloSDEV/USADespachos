namespace Trabalho
{
    partial class FrmVistorias
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmVistorias));
            TableVistorias = new TableLayoutPanel();
            DGVProcessosDadoEntrada = new DataGridView();
            label6 = new Label();
            BtnDefere = new PictureBox();
            DGVLaudo = new DataGridView();
            LblAguardandoLaudo = new Label();
            label4 = new Label();
            DGVAguardandoChegAgendVistoria = new DataGridView();
            DGVSolicitadoDataVistoria = new DataGridView();
            LblSolicitadoDataVistoria = new Label();
            LblVistoriaAgendada = new Label();
            DGVVistoriaAgendada = new DataGridView();
            DGVAguardandoDef = new DataGridView();
            LblAguardandoDeferimento = new Label();
            BtnDesceDeferimento = new PictureBox();
            BtnDeferido = new PictureBox();
            btnDesceParaAgendada = new PictureBox();
            BtnSobeLaudo = new PictureBox();
            btnDesceParaSolicitado = new PictureBox();
            BtnSobeAguardDef = new PictureBox();
            btnDesceParaAguardando = new PictureBox();
            BtnSobeAgendada = new PictureBox();
            BtnSobeSolicitado = new PictureBox();
            _timer = new System.Windows.Forms.Timer(components);
            toolStrip1 = new ToolStrip();
            BtnRecarrega = new ToolStripButton();
            panel1 = new Panel();
            TableVistorias.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DGVProcessosDadoEntrada).BeginInit();
            ((System.ComponentModel.ISupportInitialize)BtnDefere).BeginInit();
            ((System.ComponentModel.ISupportInitialize)DGVLaudo).BeginInit();
            ((System.ComponentModel.ISupportInitialize)DGVAguardandoChegAgendVistoria).BeginInit();
            ((System.ComponentModel.ISupportInitialize)DGVSolicitadoDataVistoria).BeginInit();
            ((System.ComponentModel.ISupportInitialize)DGVVistoriaAgendada).BeginInit();
            ((System.ComponentModel.ISupportInitialize)DGVAguardandoDef).BeginInit();
            ((System.ComponentModel.ISupportInitialize)BtnDesceDeferimento).BeginInit();
            ((System.ComponentModel.ISupportInitialize)BtnDeferido).BeginInit();
            ((System.ComponentModel.ISupportInitialize)btnDesceParaAgendada).BeginInit();
            ((System.ComponentModel.ISupportInitialize)BtnSobeLaudo).BeginInit();
            ((System.ComponentModel.ISupportInitialize)btnDesceParaSolicitado).BeginInit();
            ((System.ComponentModel.ISupportInitialize)BtnSobeAguardDef).BeginInit();
            ((System.ComponentModel.ISupportInitialize)btnDesceParaAguardando).BeginInit();
            ((System.ComponentModel.ISupportInitialize)BtnSobeAgendada).BeginInit();
            ((System.ComponentModel.ISupportInitialize)BtnSobeSolicitado).BeginInit();
            toolStrip1.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // TableVistorias
            // 
            TableVistorias.AutoSize = true;
            TableVistorias.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            TableVistorias.BackColor = SystemColors.ActiveBorder;
            TableVistorias.ColumnCount = 4;
            TableVistorias.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            TableVistorias.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 30F));
            TableVistorias.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 30F));
            TableVistorias.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 30F));
            TableVistorias.Controls.Add(DGVProcessosDadoEntrada, 0, 12);
            TableVistorias.Controls.Add(label6, 0, 11);
            TableVistorias.Controls.Add(BtnDefere, 1, 3);
            TableVistorias.Controls.Add(DGVLaudo, 0, 2);
            TableVistorias.Controls.Add(LblAguardandoLaudo, 0, 1);
            TableVistorias.Controls.Add(label4, 0, 9);
            TableVistorias.Controls.Add(DGVAguardandoChegAgendVistoria, 0, 10);
            TableVistorias.Controls.Add(DGVSolicitadoDataVistoria, 0, 8);
            TableVistorias.Controls.Add(LblSolicitadoDataVistoria, 0, 7);
            TableVistorias.Controls.Add(LblVistoriaAgendada, 0, 5);
            TableVistorias.Controls.Add(DGVVistoriaAgendada, 0, 6);
            TableVistorias.Controls.Add(DGVAguardandoDef, 0, 4);
            TableVistorias.Controls.Add(LblAguardandoDeferimento, 0, 3);
            TableVistorias.Controls.Add(BtnDesceDeferimento, 3, 1);
            TableVistorias.Controls.Add(BtnDeferido, 2, 1);
            TableVistorias.Controls.Add(btnDesceParaAgendada, 3, 3);
            TableVistorias.Controls.Add(BtnSobeLaudo, 2, 3);
            TableVistorias.Controls.Add(btnDesceParaSolicitado, 3, 5);
            TableVistorias.Controls.Add(BtnSobeAguardDef, 2, 5);
            TableVistorias.Controls.Add(btnDesceParaAguardando, 3, 7);
            TableVistorias.Controls.Add(BtnSobeAgendada, 2, 7);
            TableVistorias.Controls.Add(BtnSobeSolicitado, 3, 9);
            TableVistorias.Dock = DockStyle.Top;
            TableVistorias.Location = new Point(0, 0);
            TableVistorias.Name = "TableVistorias";
            TableVistorias.RowCount = 13;
            TableVistorias.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            TableVistorias.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            TableVistorias.RowStyles.Add(new RowStyle());
            TableVistorias.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            TableVistorias.RowStyles.Add(new RowStyle());
            TableVistorias.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            TableVistorias.RowStyles.Add(new RowStyle());
            TableVistorias.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            TableVistorias.RowStyles.Add(new RowStyle());
            TableVistorias.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            TableVistorias.RowStyles.Add(new RowStyle());
            TableVistorias.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            TableVistorias.RowStyles.Add(new RowStyle());
            TableVistorias.Size = new Size(1118, 952);
            TableVistorias.TabIndex = 0;
            // 
            // DGVProcessosDadoEntrada
            // 
            DGVProcessosDadoEntrada.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            DGVProcessosDadoEntrada.BackgroundColor = Color.LightGray;
            DGVProcessosDadoEntrada.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            TableVistorias.SetColumnSpan(DGVProcessosDadoEntrada, 4);
            DGVProcessosDadoEntrada.Location = new Point(3, 810);
            DGVProcessosDadoEntrada.Name = "DGVProcessosDadoEntrada";
            DGVProcessosDadoEntrada.RowTemplate.Height = 25;
            DGVProcessosDadoEntrada.Size = new Size(1112, 139);
            DGVProcessosDadoEntrada.TabIndex = 29;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.BackColor = SystemColors.Control;
            TableVistorias.SetColumnSpan(label6, 4);
            label6.Dock = DockStyle.Fill;
            label6.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            label6.Location = new Point(3, 777);
            label6.Name = "label6";
            label6.Size = new Size(1112, 30);
            label6.TabIndex = 26;
            label6.Text = "Processos Dado Entrada";
            label6.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // BtnDefere
            // 
            BtnDefere.BackColor = SystemColors.Control;
            BtnDefere.BackgroundImage = (Image)resources.GetObject("BtnDefere.BackgroundImage");
            BtnDefere.BackgroundImageLayout = ImageLayout.Stretch;
            BtnDefere.Dock = DockStyle.Fill;
            BtnDefere.Location = new Point(1031, 117);
            BtnDefere.Name = "BtnDefere";
            BtnDefere.Size = new Size(24, 24);
            BtnDefere.TabIndex = 25;
            BtnDefere.TabStop = false;
            BtnDefere.Click += BtnDeferido_Click;
            // 
            // DGVLaudo
            // 
            DGVLaudo.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            DGVLaudo.BackgroundColor = Color.LightGray;
            DGVLaudo.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            TableVistorias.SetColumnSpan(DGVLaudo, 4);
            DGVLaudo.Location = new Point(3, 60);
            DGVLaudo.Name = "DGVLaudo";
            DGVLaudo.RowTemplate.Height = 25;
            DGVLaudo.ScrollBars = ScrollBars.None;
            DGVLaudo.Size = new Size(1112, 51);
            DGVLaudo.TabIndex = 24;
            // 
            // LblAguardandoLaudo
            // 
            LblAguardandoLaudo.AutoSize = true;
            LblAguardandoLaudo.BackColor = SystemColors.ActiveBorder;
            TableVistorias.SetColumnSpan(LblAguardandoLaudo, 2);
            LblAguardandoLaudo.Dock = DockStyle.Fill;
            LblAguardandoLaudo.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            LblAguardandoLaudo.Location = new Point(3, 25);
            LblAguardandoLaudo.Name = "LblAguardandoLaudo";
            LblAguardandoLaudo.Size = new Size(1052, 32);
            LblAguardandoLaudo.TabIndex = 22;
            LblAguardandoLaudo.Text = "Aguardando Laudo";
            LblAguardandoLaudo.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.BackColor = SystemColors.ActiveBorder;
            TableVistorias.SetColumnSpan(label4, 3);
            label4.Dock = DockStyle.Fill;
            label4.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            label4.Location = new Point(3, 518);
            label4.Name = "label4";
            label4.Size = new Size(1082, 30);
            label4.TabIndex = 9;
            label4.Text = "Aguardando Chegada para Agendar Vistoria";
            label4.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // DGVAguardandoChegAgendVistoria
            // 
            DGVAguardandoChegAgendVistoria.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            DGVAguardandoChegAgendVistoria.BackgroundColor = Color.LightGray;
            DGVAguardandoChegAgendVistoria.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            TableVistorias.SetColumnSpan(DGVAguardandoChegAgendVistoria, 4);
            DGVAguardandoChegAgendVistoria.Location = new Point(3, 551);
            DGVAguardandoChegAgendVistoria.Name = "DGVAguardandoChegAgendVistoria";
            DGVAguardandoChegAgendVistoria.RowTemplate.Height = 25;
            DGVAguardandoChegAgendVistoria.ScrollBars = ScrollBars.None;
            DGVAguardandoChegAgendVistoria.Size = new Size(1112, 223);
            DGVAguardandoChegAgendVistoria.TabIndex = 1;
            // 
            // DGVSolicitadoDataVistoria
            // 
            DGVSolicitadoDataVistoria.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            DGVSolicitadoDataVistoria.BackgroundColor = Color.LightGray;
            DGVSolicitadoDataVistoria.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            TableVistorias.SetColumnSpan(DGVSolicitadoDataVistoria, 4);
            DGVSolicitadoDataVistoria.Location = new Point(3, 321);
            DGVSolicitadoDataVistoria.Name = "DGVSolicitadoDataVistoria";
            DGVSolicitadoDataVistoria.RowTemplate.Height = 25;
            DGVSolicitadoDataVistoria.ScrollBars = ScrollBars.None;
            DGVSolicitadoDataVistoria.Size = new Size(1112, 194);
            DGVSolicitadoDataVistoria.TabIndex = 0;
            // 
            // LblSolicitadoDataVistoria
            // 
            LblSolicitadoDataVistoria.AutoSize = true;
            LblSolicitadoDataVistoria.BackColor = SystemColors.Control;
            TableVistorias.SetColumnSpan(LblSolicitadoDataVistoria, 2);
            LblSolicitadoDataVistoria.Dock = DockStyle.Fill;
            LblSolicitadoDataVistoria.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            LblSolicitadoDataVistoria.Location = new Point(3, 288);
            LblSolicitadoDataVistoria.Name = "LblSolicitadoDataVistoria";
            LblSolicitadoDataVistoria.Size = new Size(1052, 30);
            LblSolicitadoDataVistoria.TabIndex = 8;
            LblSolicitadoDataVistoria.Text = "Solicitado Data de Vistoria";
            LblSolicitadoDataVistoria.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // LblVistoriaAgendada
            // 
            LblVistoriaAgendada.AutoSize = true;
            LblVistoriaAgendada.BackColor = SystemColors.ActiveBorder;
            TableVistorias.SetColumnSpan(LblVistoriaAgendada, 2);
            LblVistoriaAgendada.Dock = DockStyle.Fill;
            LblVistoriaAgendada.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            LblVistoriaAgendada.Location = new Point(3, 201);
            LblVistoriaAgendada.Name = "LblVistoriaAgendada";
            LblVistoriaAgendada.Size = new Size(1052, 30);
            LblVistoriaAgendada.TabIndex = 7;
            LblVistoriaAgendada.Text = "Vistoria Agendada";
            LblVistoriaAgendada.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // DGVVistoriaAgendada
            // 
            DGVVistoriaAgendada.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            DGVVistoriaAgendada.BackgroundColor = Color.LightGray;
            DGVVistoriaAgendada.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            TableVistorias.SetColumnSpan(DGVVistoriaAgendada, 4);
            DGVVistoriaAgendada.Location = new Point(3, 234);
            DGVVistoriaAgendada.Name = "DGVVistoriaAgendada";
            DGVVistoriaAgendada.RowTemplate.Height = 25;
            DGVVistoriaAgendada.ScrollBars = ScrollBars.None;
            DGVVistoriaAgendada.Size = new Size(1112, 51);
            DGVVistoriaAgendada.TabIndex = 2;
            // 
            // DGVAguardandoDef
            // 
            DGVAguardandoDef.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            DGVAguardandoDef.BackgroundColor = Color.LightGray;
            DGVAguardandoDef.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            TableVistorias.SetColumnSpan(DGVAguardandoDef, 4);
            DGVAguardandoDef.Location = new Point(3, 147);
            DGVAguardandoDef.Name = "DGVAguardandoDef";
            DGVAguardandoDef.RowTemplate.Height = 25;
            DGVAguardandoDef.ScrollBars = ScrollBars.None;
            DGVAguardandoDef.Size = new Size(1112, 51);
            DGVAguardandoDef.TabIndex = 3;
            // 
            // LblAguardandoDeferimento
            // 
            LblAguardandoDeferimento.AutoSize = true;
            LblAguardandoDeferimento.BackColor = SystemColors.Control;
            LblAguardandoDeferimento.Dock = DockStyle.Fill;
            LblAguardandoDeferimento.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            LblAguardandoDeferimento.Location = new Point(3, 114);
            LblAguardandoDeferimento.Name = "LblAguardandoDeferimento";
            LblAguardandoDeferimento.Size = new Size(1022, 30);
            LblAguardandoDeferimento.TabIndex = 6;
            LblAguardandoDeferimento.Text = "Aguardando Deferimento";
            LblAguardandoDeferimento.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // BtnDesceDeferimento
            // 
            BtnDesceDeferimento.BackColor = SystemColors.ActiveBorder;
            BtnDesceDeferimento.BackgroundImage = (Image)resources.GetObject("BtnDesceDeferimento.BackgroundImage");
            BtnDesceDeferimento.BackgroundImageLayout = ImageLayout.Stretch;
            BtnDesceDeferimento.Dock = DockStyle.Fill;
            BtnDesceDeferimento.Location = new Point(1091, 28);
            BtnDesceDeferimento.Name = "BtnDesceDeferimento";
            BtnDesceDeferimento.Size = new Size(24, 26);
            BtnDesceDeferimento.TabIndex = 23;
            BtnDesceDeferimento.TabStop = false;
            BtnDesceDeferimento.Click += BtnDesceDeferimento_Click;
            // 
            // BtnDeferido
            // 
            BtnDeferido.BackColor = SystemColors.ActiveBorder;
            BtnDeferido.BackgroundImage = (Image)resources.GetObject("BtnDeferido.BackgroundImage");
            BtnDeferido.BackgroundImageLayout = ImageLayout.Stretch;
            BtnDeferido.Dock = DockStyle.Fill;
            BtnDeferido.Location = new Point(1061, 28);
            BtnDeferido.Name = "BtnDeferido";
            BtnDeferido.Size = new Size(24, 26);
            BtnDeferido.TabIndex = 17;
            BtnDeferido.TabStop = false;
            BtnDeferido.Click += BtnDeferido_Click_1;
            // 
            // btnDesceParaAgendada
            // 
            btnDesceParaAgendada.BackColor = SystemColors.Control;
            btnDesceParaAgendada.BackgroundImage = (Image)resources.GetObject("btnDesceParaAgendada.BackgroundImage");
            btnDesceParaAgendada.BackgroundImageLayout = ImageLayout.Stretch;
            btnDesceParaAgendada.Dock = DockStyle.Fill;
            btnDesceParaAgendada.Location = new Point(1091, 117);
            btnDesceParaAgendada.Name = "btnDesceParaAgendada";
            btnDesceParaAgendada.Size = new Size(24, 24);
            btnDesceParaAgendada.TabIndex = 18;
            btnDesceParaAgendada.TabStop = false;
            btnDesceParaAgendada.Click += btnDesceParaAgendada_Click;
            // 
            // BtnSobeLaudo
            // 
            BtnSobeLaudo.BackColor = SystemColors.Control;
            BtnSobeLaudo.BackgroundImage = (Image)resources.GetObject("BtnSobeLaudo.BackgroundImage");
            BtnSobeLaudo.BackgroundImageLayout = ImageLayout.Stretch;
            BtnSobeLaudo.Dock = DockStyle.Fill;
            BtnSobeLaudo.Location = new Point(1061, 117);
            BtnSobeLaudo.Name = "BtnSobeLaudo";
            BtnSobeLaudo.Size = new Size(24, 24);
            BtnSobeLaudo.TabIndex = 21;
            BtnSobeLaudo.TabStop = false;
            BtnSobeLaudo.Click += BtnSobeLaudo_Click;
            // 
            // btnDesceParaSolicitado
            // 
            btnDesceParaSolicitado.BackColor = SystemColors.ActiveBorder;
            btnDesceParaSolicitado.BackgroundImage = (Image)resources.GetObject("btnDesceParaSolicitado.BackgroundImage");
            btnDesceParaSolicitado.BackgroundImageLayout = ImageLayout.Stretch;
            btnDesceParaSolicitado.Dock = DockStyle.Fill;
            btnDesceParaSolicitado.Location = new Point(1091, 204);
            btnDesceParaSolicitado.Name = "btnDesceParaSolicitado";
            btnDesceParaSolicitado.Size = new Size(24, 24);
            btnDesceParaSolicitado.TabIndex = 19;
            btnDesceParaSolicitado.TabStop = false;
            btnDesceParaSolicitado.Click += btnDesceParaSolicitado_Click;
            // 
            // BtnSobeAguardDef
            // 
            BtnSobeAguardDef.BackColor = SystemColors.ActiveBorder;
            BtnSobeAguardDef.BackgroundImage = (Image)resources.GetObject("BtnSobeAguardDef.BackgroundImage");
            BtnSobeAguardDef.BackgroundImageLayout = ImageLayout.Stretch;
            BtnSobeAguardDef.Dock = DockStyle.Fill;
            BtnSobeAguardDef.Location = new Point(1061, 204);
            BtnSobeAguardDef.Name = "BtnSobeAguardDef";
            BtnSobeAguardDef.Size = new Size(24, 24);
            BtnSobeAguardDef.TabIndex = 14;
            BtnSobeAguardDef.TabStop = false;
            BtnSobeAguardDef.Click += BtnSobeAguardDef_Click;
            // 
            // btnDesceParaAguardando
            // 
            btnDesceParaAguardando.BackColor = SystemColors.Control;
            btnDesceParaAguardando.BackgroundImage = (Image)resources.GetObject("btnDesceParaAguardando.BackgroundImage");
            btnDesceParaAguardando.BackgroundImageLayout = ImageLayout.Stretch;
            btnDesceParaAguardando.Dock = DockStyle.Fill;
            btnDesceParaAguardando.Location = new Point(1091, 291);
            btnDesceParaAguardando.Name = "btnDesceParaAguardando";
            btnDesceParaAguardando.Size = new Size(24, 24);
            btnDesceParaAguardando.TabIndex = 20;
            btnDesceParaAguardando.TabStop = false;
            btnDesceParaAguardando.Click += btnDesceParaAguardando_Click;
            // 
            // BtnSobeAgendada
            // 
            BtnSobeAgendada.BackColor = SystemColors.Control;
            BtnSobeAgendada.BackgroundImage = (Image)resources.GetObject("BtnSobeAgendada.BackgroundImage");
            BtnSobeAgendada.BackgroundImageLayout = ImageLayout.Stretch;
            BtnSobeAgendada.Dock = DockStyle.Fill;
            BtnSobeAgendada.Location = new Point(1061, 291);
            BtnSobeAgendada.Name = "BtnSobeAgendada";
            BtnSobeAgendada.Size = new Size(24, 24);
            BtnSobeAgendada.TabIndex = 15;
            BtnSobeAgendada.TabStop = false;
            BtnSobeAgendada.Click += BtnSobeAgendada_Click;
            // 
            // BtnSobeSolicitado
            // 
            BtnSobeSolicitado.BackColor = SystemColors.ActiveBorder;
            BtnSobeSolicitado.BackgroundImage = (Image)resources.GetObject("BtnSobeSolicitado.BackgroundImage");
            BtnSobeSolicitado.BackgroundImageLayout = ImageLayout.Stretch;
            BtnSobeSolicitado.Dock = DockStyle.Fill;
            BtnSobeSolicitado.Location = new Point(1091, 521);
            BtnSobeSolicitado.Name = "BtnSobeSolicitado";
            BtnSobeSolicitado.Size = new Size(24, 24);
            BtnSobeSolicitado.TabIndex = 16;
            BtnSobeSolicitado.TabStop = false;
            BtnSobeSolicitado.Click += BtnSobeSolicitado_Click;
            // 
            // toolStrip1
            // 
            toolStrip1.Items.AddRange(new ToolStripItem[] { BtnRecarrega });
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(1118, 25);
            toolStrip1.TabIndex = 1;
            toolStrip1.Text = "toolStrip1";
            // 
            // BtnRecarrega
            // 
            BtnRecarrega.ForeColor = Color.Chartreuse;
            BtnRecarrega.Image = (Image)resources.GetObject("BtnRecarrega.Image");
            BtnRecarrega.ImageTransparentColor = Color.Magenta;
            BtnRecarrega.Name = "BtnRecarrega";
            BtnRecarrega.Size = new Size(23, 22);
            BtnRecarrega.Click += BtnRecarrega_Click;
            // 
            // panel1
            // 
            panel1.AutoScroll = true;
            panel1.Controls.Add(TableVistorias);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(1118, 952);
            panel1.TabIndex = 2;
            // 
            // FrmVistorias
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1118, 952);
            ControlBox = false;
            Controls.Add(toolStrip1);
            Controls.Add(panel1);
            Name = "FrmVistorias";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Vistorias";
            WindowState = FormWindowState.Maximized;
            Shown += FrmVistorias_Shown;
            TableVistorias.ResumeLayout(false);
            TableVistorias.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)DGVProcessosDadoEntrada).EndInit();
            ((System.ComponentModel.ISupportInitialize)BtnDefere).EndInit();
            ((System.ComponentModel.ISupportInitialize)DGVLaudo).EndInit();
            ((System.ComponentModel.ISupportInitialize)DGVAguardandoChegAgendVistoria).EndInit();
            ((System.ComponentModel.ISupportInitialize)DGVSolicitadoDataVistoria).EndInit();
            ((System.ComponentModel.ISupportInitialize)DGVVistoriaAgendada).EndInit();
            ((System.ComponentModel.ISupportInitialize)DGVAguardandoDef).EndInit();
            ((System.ComponentModel.ISupportInitialize)BtnDesceDeferimento).EndInit();
            ((System.ComponentModel.ISupportInitialize)BtnDeferido).EndInit();
            ((System.ComponentModel.ISupportInitialize)btnDesceParaAgendada).EndInit();
            ((System.ComponentModel.ISupportInitialize)BtnSobeLaudo).EndInit();
            ((System.ComponentModel.ISupportInitialize)btnDesceParaSolicitado).EndInit();
            ((System.ComponentModel.ISupportInitialize)BtnSobeAguardDef).EndInit();
            ((System.ComponentModel.ISupportInitialize)btnDesceParaAguardando).EndInit();
            ((System.ComponentModel.ISupportInitialize)BtnSobeAgendada).EndInit();
            ((System.ComponentModel.ISupportInitialize)BtnSobeSolicitado).EndInit();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TableLayoutPanel TableVistorias;
        private DataGridView DGVVistoriaAgendada;
        private DataGridView DGVAguardandoChegAgendVistoria;
        private DataGridView DGVAguardandoDef;
        private DataGridView DGVSolicitadoDataVistoria;
        private Label label4;
        private Label LblSolicitadoDataVistoria;
        private Label LblVistoriaAgendada;
        private Label LblAguardandoDeferimento;
        private PictureBox BtnSobeAguardDef;
        private PictureBox BtnDeferido;
        private PictureBox BtnSobeSolicitado;
        private PictureBox BtnSobeAgendada;
        private PictureBox btnDesceParaAguardando;
        private PictureBox btnDesceParaSolicitado;
        private PictureBox btnDesceParaAgendada;
        private PictureBox BtnDesceDeferimento;
        private Label LblAguardandoLaudo;
        private PictureBox BtnSobeLaudo;
        private DataGridView DGVLaudo;
        private PictureBox BtnDefere;
        private DataGridView DGVProcessosDadoEntrada;
        private PictureBox pictureBox2;
        private Label label6;
        private System.Windows.Forms.Timer _timer;
        private ToolStrip toolStrip1;
        private ToolStripButton BtnRecarrega;
        private Panel panel1;
    }
}