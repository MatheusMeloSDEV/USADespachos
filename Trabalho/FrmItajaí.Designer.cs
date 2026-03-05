namespace Trabalho
{
    partial class FrmItajaí
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmItajaí));
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            TSMenuItajai = new ToolStrip();
            BtnAdicionar = new ToolStripButton();
            toolStripSeparator2 = new ToolStripSeparator();
            BtnEditar = new ToolStripButton();
            toolStripSeparator3 = new ToolStripSeparator();
            BtnExcluir = new ToolStripButton();
            toolStripSeparator4 = new ToolStripSeparator();
            BtnExportar = new ToolStripButton();
            BtnDownloadTabela = new ToolStripButton();
            BtnAjuda = new ToolStripButton();
            BtnCancelar = new ToolStripButton();
            BtnPesquisar = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            CmbPesquisar = new ToolStripComboBox();
            TxtPesquisar = new ToolStripTextBox();
            BsProcesso = new BindingSource(components);
            panel1 = new Panel();
            panel2 = new Panel();
            tableLayoutPanel1 = new TableLayoutPanel();
            DGVItajai = new DataGridView();
            lblQtd = new Label();
            btnForward = new Button();
            btnPrevious = new Button();
            TSMenuItajai.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)BsProcesso).BeginInit();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DGVItajai).BeginInit();
            SuspendLayout();
            // 
            // TSMenuItajai
            // 
            TSMenuItajai.AutoSize = false;
            TSMenuItajai.Items.AddRange(new ToolStripItem[] { BtnAdicionar, toolStripSeparator2, BtnEditar, toolStripSeparator3, BtnExcluir, toolStripSeparator4, BtnExportar, BtnDownloadTabela, BtnAjuda, BtnCancelar, BtnPesquisar, toolStripSeparator1, CmbPesquisar, TxtPesquisar });
            TSMenuItajai.Location = new Point(0, 0);
            TSMenuItajai.Name = "TSMenuItajai";
            TSMenuItajai.Size = new Size(800, 40);
            TSMenuItajai.TabIndex = 1;
            TSMenuItajai.Text = "toolStrip1";
            // 
            // BtnAdicionar
            // 
            BtnAdicionar.Image = (Image)resources.GetObject("BtnAdicionar.Image");
            BtnAdicionar.ImageTransparentColor = Color.Magenta;
            BtnAdicionar.Margin = new Padding(0);
            BtnAdicionar.Name = "BtnAdicionar";
            BtnAdicionar.Size = new Size(78, 40);
            BtnAdicionar.Text = "Adicionar";
            BtnAdicionar.Click += BtnAdicionar_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Margin = new Padding(5, 0, 5, 0);
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(6, 40);
            // 
            // BtnEditar
            // 
            BtnEditar.Image = (Image)resources.GetObject("BtnEditar.Image");
            BtnEditar.ImageTransparentColor = Color.Magenta;
            BtnEditar.Margin = new Padding(0);
            BtnEditar.Name = "BtnEditar";
            BtnEditar.Size = new Size(57, 40);
            BtnEditar.Text = "Editar";
            BtnEditar.Click += BtnEditar_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Margin = new Padding(5, 0, 5, 0);
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(6, 40);
            // 
            // BtnExcluir
            // 
            BtnExcluir.Image = (Image)resources.GetObject("BtnExcluir.Image");
            BtnExcluir.ImageTransparentColor = Color.Magenta;
            BtnExcluir.Margin = new Padding(0);
            BtnExcluir.Name = "BtnExcluir";
            BtnExcluir.Size = new Size(61, 40);
            BtnExcluir.Text = "Excluir";
            BtnExcluir.Click += BtnExcluir_Click;
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Margin = new Padding(5, 0, 5, 0);
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new Size(6, 40);
            // 
            // BtnExportar
            // 
            BtnExportar.Image = (Image)resources.GetObject("BtnExportar.Image");
            BtnExportar.ImageTransparentColor = Color.Magenta;
            BtnExportar.Margin = new Padding(0);
            BtnExportar.Name = "BtnExportar";
            BtnExportar.Size = new Size(70, 40);
            BtnExportar.Text = "Exportar";
            BtnExportar.Click += BtnExportar_Click;
            // 
            // BtnDownloadTabela
            // 
            BtnDownloadTabela.Alignment = ToolStripItemAlignment.Right;
            BtnDownloadTabela.AutoSize = false;
            BtnDownloadTabela.DisplayStyle = ToolStripItemDisplayStyle.Image;
            BtnDownloadTabela.Image = Properties.Resources.Download;
            BtnDownloadTabela.ImageTransparentColor = Color.Magenta;
            BtnDownloadTabela.Name = "BtnDownloadTabela";
            BtnDownloadTabela.Size = new Size(25, 37);
            BtnDownloadTabela.Text = "toolStripButton1";
            BtnDownloadTabela.Click += BtnDownloadTabela_Click;
            // 
            // BtnAjuda
            // 
            BtnAjuda.Alignment = ToolStripItemAlignment.Right;
            BtnAjuda.DisplayStyle = ToolStripItemDisplayStyle.Text;
            BtnAjuda.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            BtnAjuda.Image = (Image)resources.GetObject("BtnAjuda.Image");
            BtnAjuda.ImageTransparentColor = Color.Magenta;
            BtnAjuda.Name = "BtnAjuda";
            BtnAjuda.Size = new Size(24, 37);
            BtnAjuda.Text = "?";
            BtnAjuda.Click += BtnAjuda_Click;
            // 
            // BtnCancelar
            // 
            BtnCancelar.Alignment = ToolStripItemAlignment.Right;
            BtnCancelar.DisplayStyle = ToolStripItemDisplayStyle.Image;
            BtnCancelar.Image = (Image)resources.GetObject("BtnCancelar.Image");
            BtnCancelar.ImageTransparentColor = Color.Magenta;
            BtnCancelar.Name = "BtnCancelar";
            BtnCancelar.Size = new Size(23, 37);
            BtnCancelar.Text = "toolStripButton4";
            BtnCancelar.Click += BtnCancelar_Click;
            // 
            // BtnPesquisar
            // 
            BtnPesquisar.Alignment = ToolStripItemAlignment.Right;
            BtnPesquisar.DisplayStyle = ToolStripItemDisplayStyle.Image;
            BtnPesquisar.Image = (Image)resources.GetObject("BtnPesquisar.Image");
            BtnPesquisar.ImageTransparentColor = Color.Magenta;
            BtnPesquisar.Name = "BtnPesquisar";
            BtnPesquisar.Size = new Size(23, 37);
            BtnPesquisar.Text = "toolStripButton5";
            BtnPesquisar.Click += BtnPesquisar_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Alignment = ToolStripItemAlignment.Right;
            toolStripSeparator1.Margin = new Padding(5, 0, 5, 0);
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 40);
            // 
            // CmbPesquisar
            // 
            CmbPesquisar.Alignment = ToolStripItemAlignment.Right;
            CmbPesquisar.AutoSize = false;
            CmbPesquisar.Margin = new Padding(5, 0, 0, 0);
            CmbPesquisar.Name = "CmbPesquisar";
            CmbPesquisar.Size = new Size(150, 23);
            CmbPesquisar.SelectedIndexChanged += CmbPesquisar_SelectedIndexChanged;
            // 
            // TxtPesquisar
            // 
            TxtPesquisar.Alignment = ToolStripItemAlignment.Right;
            TxtPesquisar.AutoSize = false;
            TxtPesquisar.Name = "TxtPesquisar";
            TxtPesquisar.Size = new Size(150, 40);
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel1.Controls.Add(panel2);
            panel1.Location = new Point(0, 43);
            panel1.Name = "panel1";
            panel1.Size = new Size(800, 408);
            panel1.TabIndex = 2;
            // 
            // panel2
            // 
            panel2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel2.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panel2.Controls.Add(tableLayoutPanel1);
            panel2.Location = new Point(0, 0);
            panel2.Name = "panel2";
            panel2.Size = new Size(800, 409);
            panel2.TabIndex = 5;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 4;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 25F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 25F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel1.Controls.Add(DGVItajai, 0, 0);
            tableLayoutPanel1.Controls.Add(lblQtd, 3, 1);
            tableLayoutPanel1.Controls.Add(btnForward, 2, 1);
            tableLayoutPanel1.Controls.Add(btnPrevious, 1, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            tableLayoutPanel1.Size = new Size(800, 409);
            tableLayoutPanel1.TabIndex = 3;
            // 
            // DGVItajai
            // 
            DGVItajai.AllowUserToAddRows = false;
            DGVItajai.AllowUserToDeleteRows = false;
            DGVItajai.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = Color.WhiteSmoke;
            DGVItajai.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            DGVItajai.BackgroundColor = Color.White;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.DarkGray;
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            dataGridViewCellStyle2.ForeColor = Color.White;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            DGVItajai.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            DGVItajai.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            tableLayoutPanel1.SetColumnSpan(DGVItajai, 4);
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.White;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle3.ForeColor = Color.Black;
            dataGridViewCellStyle3.SelectionBackColor = Color.LightBlue;
            dataGridViewCellStyle3.SelectionForeColor = Color.Black;
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            DGVItajai.DefaultCellStyle = dataGridViewCellStyle3;
            DGVItajai.Dock = DockStyle.Fill;
            DGVItajai.EnableHeadersVisualStyles = false;
            DGVItajai.Location = new Point(3, 3);
            DGVItajai.Name = "DGVItajai";
            DGVItajai.ReadOnly = true;
            DGVItajai.RowHeadersVisible = false;
            DGVItajai.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            DGVItajai.Size = new Size(794, 378);
            DGVItajai.TabIndex = 2;
            DGVItajai.ColumnHeaderMouseClick += DGV_ColumnHeaderMouseClick;
            // 
            // lblQtd
            // 
            lblQtd.AutoSize = true;
            lblQtd.Dock = DockStyle.Fill;
            lblQtd.Location = new Point(732, 384);
            lblQtd.Name = "lblQtd";
            lblQtd.Size = new Size(65, 25);
            lblQtd.TabIndex = 3;
            lblQtd.Text = "1 - 50 / 500";
            lblQtd.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btnForward
            // 
            btnForward.Dock = DockStyle.Fill;
            btnForward.FlatAppearance.BorderSize = 0;
            btnForward.FlatStyle = FlatStyle.Flat;
            btnForward.Location = new Point(704, 384);
            btnForward.Margin = new Padding(0);
            btnForward.Name = "btnForward";
            btnForward.Size = new Size(25, 25);
            btnForward.TabIndex = 5;
            btnForward.Text = ">";
            btnForward.UseVisualStyleBackColor = true;
            // 
            // btnPrevious
            // 
            btnPrevious.FlatAppearance.BorderSize = 0;
            btnPrevious.FlatStyle = FlatStyle.Flat;
            btnPrevious.Location = new Point(679, 384);
            btnPrevious.Margin = new Padding(0);
            btnPrevious.Name = "btnPrevious";
            btnPrevious.Size = new Size(21, 25);
            btnPrevious.TabIndex = 6;
            btnPrevious.Text = "<";
            btnPrevious.UseVisualStyleBackColor = true;
            // 
            // FrmItajaí
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            ControlBox = false;
            Controls.Add(panel1);
            Controls.Add(TSMenuItajai);
            Name = "FrmItajaí";
            Text = "Gerenciamento de Processos";
            WindowState = FormWindowState.Maximized;
            Shown += FrmItajaí_Shown;
            KeyDown += FrmProcesso_KeyDown;
            TSMenuItajai.ResumeLayout(false);
            TSMenuItajai.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)BsProcesso).EndInit();
            panel1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)DGVItajai).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private ToolStrip TSMenuItajai;
        private ToolStripButton BtnAdicionar;
        private ToolStripButton BtnEditar;
        private ToolStripButton BtnExportar;
        private ToolStripButton BtnCancelar;
        private ToolStripButton BtnPesquisar;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripComboBox CmbPesquisar;
        private ToolStripTextBox TxtPesquisar;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripSeparator toolStripSeparator3;
        private BindingSource BsProcesso;
        private Panel panel1;
        private ToolStripButton BtnAjuda;
        private ToolStripButton BtnExcluir;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripButton BtnDownloadTabela;
        private Panel panel2;
        private TableLayoutPanel tableLayoutPanel1;
        private DataGridView DGVItajai;
        private Label lblQtd;
        private Button btnForward;
        private Button btnPrevious;
    }
}