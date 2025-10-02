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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmVistorias));
            tableLayoutPanel1 = new TableLayoutPanel();
            BtnDeferido = new PictureBox();
            BtnSobeSolicitado = new PictureBox();
            BtnSobeAgendada = new PictureBox();
            DGVVistoriaAgendada = new DataGridView();
            DGVAguardandoDef = new DataGridView();
            label1 = new Label();
            DGVSolicitadoDataVistoria = new DataGridView();
            DGVAguardandoChegAgendVistoria = new DataGridView();
            label2 = new Label();
            label4 = new Label();
            label3 = new Label();
            BtnSobeAguardDef = new PictureBox();
            btnDesceParaAgendada = new PictureBox();
            btnDesceParaSolicitado = new PictureBox();
            btnDesceParaAguardando = new PictureBox();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)BtnDeferido).BeginInit();
            ((System.ComponentModel.ISupportInitialize)BtnSobeSolicitado).BeginInit();
            ((System.ComponentModel.ISupportInitialize)BtnSobeAgendada).BeginInit();
            ((System.ComponentModel.ISupportInitialize)DGVVistoriaAgendada).BeginInit();
            ((System.ComponentModel.ISupportInitialize)DGVAguardandoDef).BeginInit();
            ((System.ComponentModel.ISupportInitialize)DGVSolicitadoDataVistoria).BeginInit();
            ((System.ComponentModel.ISupportInitialize)DGVAguardandoChegAgendVistoria).BeginInit();
            ((System.ComponentModel.ISupportInitialize)BtnSobeAguardDef).BeginInit();
            ((System.ComponentModel.ISupportInitialize)btnDesceParaAgendada).BeginInit();
            ((System.ComponentModel.ISupportInitialize)btnDesceParaSolicitado).BeginInit();
            ((System.ComponentModel.ISupportInitialize)btnDesceParaAguardando).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 30F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 30F));
            tableLayoutPanel1.Controls.Add(btnDesceParaAguardando, 2, 4);
            tableLayoutPanel1.Controls.Add(btnDesceParaSolicitado, 2, 2);
            tableLayoutPanel1.Controls.Add(btnDesceParaAgendada, 2, 0);
            tableLayoutPanel1.Controls.Add(BtnDeferido, 1, 0);
            tableLayoutPanel1.Controls.Add(BtnSobeAgendada, 1, 4);
            tableLayoutPanel1.Controls.Add(DGVVistoriaAgendada, 0, 3);
            tableLayoutPanel1.Controls.Add(DGVAguardandoDef, 0, 1);
            tableLayoutPanel1.Controls.Add(label1, 0, 0);
            tableLayoutPanel1.Controls.Add(DGVSolicitadoDataVistoria, 0, 5);
            tableLayoutPanel1.Controls.Add(DGVAguardandoChegAgendVistoria, 0, 7);
            tableLayoutPanel1.Controls.Add(label2, 0, 2);
            tableLayoutPanel1.Controls.Add(label4, 0, 6);
            tableLayoutPanel1.Controls.Add(label3, 0, 4);
            tableLayoutPanel1.Controls.Add(BtnSobeAguardDef, 1, 2);
            tableLayoutPanel1.Controls.Add(BtnSobeSolicitado, 2, 6);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 8;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 21.9360924F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 21.93609F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 21.9360924F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 34.1917229F));
            tableLayoutPanel1.Size = new Size(1118, 826);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // BtnDeferido
            // 
            BtnDeferido.BackgroundImage = (Image)resources.GetObject("BtnDeferido.BackgroundImage");
            BtnDeferido.BackgroundImageLayout = ImageLayout.Stretch;
            BtnDeferido.Dock = DockStyle.Fill;
            BtnDeferido.Location = new Point(1061, 3);
            BtnDeferido.Name = "BtnDeferido";
            BtnDeferido.Size = new Size(24, 24);
            BtnDeferido.TabIndex = 17;
            BtnDeferido.TabStop = false;
            BtnDeferido.Click += BtnDeferido_Click;
            // 
            // BtnSobeSolicitado
            // 
            BtnSobeSolicitado.BackgroundImage = (Image)resources.GetObject("BtnSobeSolicitado.BackgroundImage");
            BtnSobeSolicitado.BackgroundImageLayout = ImageLayout.Stretch;
            BtnSobeSolicitado.Dock = DockStyle.Fill;
            BtnSobeSolicitado.Location = new Point(1091, 555);
            BtnSobeSolicitado.Name = "BtnSobeSolicitado";
            BtnSobeSolicitado.Size = new Size(24, 24);
            BtnSobeSolicitado.TabIndex = 16;
            BtnSobeSolicitado.TabStop = false;
            BtnSobeSolicitado.Click += BtnSobeSolicitado_Click;
            // 
            // BtnSobeAgendada
            // 
            BtnSobeAgendada.BackgroundImage = (Image)resources.GetObject("BtnSobeAgendada.BackgroundImage");
            BtnSobeAgendada.BackgroundImageLayout = ImageLayout.Stretch;
            BtnSobeAgendada.Dock = DockStyle.Fill;
            BtnSobeAgendada.Location = new Point(1061, 371);
            BtnSobeAgendada.Name = "BtnSobeAgendada";
            BtnSobeAgendada.Size = new Size(24, 24);
            BtnSobeAgendada.TabIndex = 15;
            BtnSobeAgendada.TabStop = false;
            BtnSobeAgendada.Click += BtnSobeAgendada_Click;
            // 
            // DGVVistoriaAgendada
            // 
            DGVVistoriaAgendada.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            tableLayoutPanel1.SetColumnSpan(DGVVistoriaAgendada, 3);
            DGVVistoriaAgendada.Dock = DockStyle.Fill;
            DGVVistoriaAgendada.Location = new Point(3, 217);
            DGVVistoriaAgendada.Name = "DGVVistoriaAgendada";
            DGVVistoriaAgendada.RowTemplate.Height = 25;
            DGVVistoriaAgendada.Size = new Size(1112, 148);
            DGVVistoriaAgendada.TabIndex = 2;
            // 
            // DGVAguardandoDef
            // 
            DGVAguardandoDef.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            tableLayoutPanel1.SetColumnSpan(DGVAguardandoDef, 3);
            DGVAguardandoDef.Dock = DockStyle.Fill;
            DGVAguardandoDef.Location = new Point(3, 33);
            DGVAguardandoDef.Name = "DGVAguardandoDef";
            DGVAguardandoDef.RowTemplate.Height = 25;
            DGVAguardandoDef.Size = new Size(1112, 148);
            DGVAguardandoDef.TabIndex = 3;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Fill;
            label1.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            label1.Location = new Point(3, 0);
            label1.Name = "label1";
            label1.Size = new Size(1052, 30);
            label1.TabIndex = 6;
            label1.Text = "Aguardando Deferimento";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // DGVSolicitadoDataVistoria
            // 
            DGVSolicitadoDataVistoria.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            tableLayoutPanel1.SetColumnSpan(DGVSolicitadoDataVistoria, 3);
            DGVSolicitadoDataVistoria.Dock = DockStyle.Fill;
            DGVSolicitadoDataVistoria.Location = new Point(3, 401);
            DGVSolicitadoDataVistoria.Name = "DGVSolicitadoDataVistoria";
            DGVSolicitadoDataVistoria.RowTemplate.Height = 25;
            DGVSolicitadoDataVistoria.Size = new Size(1112, 148);
            DGVSolicitadoDataVistoria.TabIndex = 0;
            // 
            // DGVAguardandoChegAgendVistoria
            // 
            DGVAguardandoChegAgendVistoria.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            tableLayoutPanel1.SetColumnSpan(DGVAguardandoChegAgendVistoria, 3);
            DGVAguardandoChegAgendVistoria.Dock = DockStyle.Fill;
            DGVAguardandoChegAgendVistoria.Location = new Point(3, 585);
            DGVAguardandoChegAgendVistoria.Name = "DGVAguardandoChegAgendVistoria";
            DGVAguardandoChegAgendVistoria.RowTemplate.Height = 25;
            DGVAguardandoChegAgendVistoria.Size = new Size(1112, 238);
            DGVAguardandoChegAgendVistoria.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Dock = DockStyle.Fill;
            label2.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            label2.Location = new Point(3, 184);
            label2.Name = "label2";
            label2.Size = new Size(1052, 30);
            label2.TabIndex = 7;
            label2.Text = "Vistoria Agendada";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            label4.AutoSize = true;
            tableLayoutPanel1.SetColumnSpan(label4, 2);
            label4.Dock = DockStyle.Fill;
            label4.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            label4.Location = new Point(3, 552);
            label4.Name = "label4";
            label4.Size = new Size(1082, 30);
            label4.TabIndex = 9;
            label4.Text = "Aguardando Chegada para Agendar Vistoria";
            label4.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Dock = DockStyle.Fill;
            label3.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            label3.Location = new Point(3, 368);
            label3.Name = "label3";
            label3.Size = new Size(1052, 30);
            label3.TabIndex = 8;
            label3.Text = "Solicitado Data de Vistoria";
            label3.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // BtnSobeAguardDef
            // 
            BtnSobeAguardDef.BackgroundImage = (Image)resources.GetObject("BtnSobeAguardDef.BackgroundImage");
            BtnSobeAguardDef.BackgroundImageLayout = ImageLayout.Stretch;
            BtnSobeAguardDef.Dock = DockStyle.Fill;
            BtnSobeAguardDef.Location = new Point(1061, 187);
            BtnSobeAguardDef.Name = "BtnSobeAguardDef";
            BtnSobeAguardDef.Size = new Size(24, 24);
            BtnSobeAguardDef.TabIndex = 14;
            BtnSobeAguardDef.TabStop = false;
            BtnSobeAguardDef.Click += BtnSobeAguardDef_Click;
            // 
            // btnDesceParaAgendada
            // 
            btnDesceParaAgendada.BackgroundImage = (Image)resources.GetObject("btnDesceParaAgendada.BackgroundImage");
            btnDesceParaAgendada.BackgroundImageLayout = ImageLayout.Stretch;
            btnDesceParaAgendada.Dock = DockStyle.Fill;
            btnDesceParaAgendada.Location = new Point(1091, 3);
            btnDesceParaAgendada.Name = "btnDesceParaAgendada";
            btnDesceParaAgendada.Size = new Size(24, 24);
            btnDesceParaAgendada.TabIndex = 18;
            btnDesceParaAgendada.TabStop = false;
            btnDesceParaAgendada.Click += btnDesceParaAgendada_Click;
            // 
            // btnDesceParaSolicitado
            // 
            btnDesceParaSolicitado.BackgroundImage = (Image)resources.GetObject("btnDesceParaSolicitado.BackgroundImage");
            btnDesceParaSolicitado.BackgroundImageLayout = ImageLayout.Stretch;
            btnDesceParaSolicitado.Dock = DockStyle.Fill;
            btnDesceParaSolicitado.Location = new Point(1091, 187);
            btnDesceParaSolicitado.Name = "btnDesceParaSolicitado";
            btnDesceParaSolicitado.Size = new Size(24, 24);
            btnDesceParaSolicitado.TabIndex = 19;
            btnDesceParaSolicitado.TabStop = false;
            btnDesceParaSolicitado.Click += btnDesceParaSolicitado_Click;
            // 
            // btnDesceParaAguardando
            // 
            btnDesceParaAguardando.BackgroundImage = (Image)resources.GetObject("btnDesceParaAguardando.BackgroundImage");
            btnDesceParaAguardando.BackgroundImageLayout = ImageLayout.Stretch;
            btnDesceParaAguardando.Dock = DockStyle.Fill;
            btnDesceParaAguardando.Location = new Point(1091, 371);
            btnDesceParaAguardando.Name = "btnDesceParaAguardando";
            btnDesceParaAguardando.Size = new Size(24, 24);
            btnDesceParaAguardando.TabIndex = 20;
            btnDesceParaAguardando.TabStop = false;
            btnDesceParaAguardando.Click += btnDesceParaAguardando_Click;
            // 
            // FrmVistorias
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoScroll = true;
            ClientSize = new Size(1118, 826);
            ControlBox = false;
            Controls.Add(tableLayoutPanel1);
            Name = "FrmVistorias";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Vistorias";
            WindowState = FormWindowState.Maximized;
            Shown += FrmVistorias_Shown;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)BtnDeferido).EndInit();
            ((System.ComponentModel.ISupportInitialize)BtnSobeSolicitado).EndInit();
            ((System.ComponentModel.ISupportInitialize)BtnSobeAgendada).EndInit();
            ((System.ComponentModel.ISupportInitialize)DGVVistoriaAgendada).EndInit();
            ((System.ComponentModel.ISupportInitialize)DGVAguardandoDef).EndInit();
            ((System.ComponentModel.ISupportInitialize)DGVSolicitadoDataVistoria).EndInit();
            ((System.ComponentModel.ISupportInitialize)DGVAguardandoChegAgendVistoria).EndInit();
            ((System.ComponentModel.ISupportInitialize)BtnSobeAguardDef).EndInit();
            ((System.ComponentModel.ISupportInitialize)btnDesceParaAgendada).EndInit();
            ((System.ComponentModel.ISupportInitialize)btnDesceParaSolicitado).EndInit();
            ((System.ComponentModel.ISupportInitialize)btnDesceParaAguardando).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private DataGridView DGVVistoriaAgendada;
        private DataGridView DGVAguardandoChegAgendVistoria;
        private DataGridView DGVAguardandoDef;
        private DataGridView DGVSolicitadoDataVistoria;
        private Label label4;
        private Label label3;
        private Label label2;
        private Label label1;
        private PictureBox BtnSobeAguardDef;
        private PictureBox BtnDeferido;
        private PictureBox BtnSobeSolicitado;
        private PictureBox BtnSobeAgendada;
        private PictureBox btnDesceParaAguardando;
        private PictureBox btnDesceParaSolicitado;
        private PictureBox btnDesceParaAgendada;
    }
}