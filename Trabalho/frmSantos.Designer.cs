using System.Windows.Forms;

namespace Trabalho
{
    partial class frmSantos
    {
        private System.ComponentModel.IContainer components = null;
        private BindingSource BsProcesso;
        /// <summary>
        /// Liberar recursos utilizados.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
                BsProcesso.Dispose();
            }
            base.Dispose(disposing);
        }


        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSantos));
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            BsProcesso = new BindingSource(components);
            TSMenuItajai = new ToolStrip();
            BtnAdicionar = new ToolStripButton();
            toolStripSeparator2 = new ToolStripSeparator();
            toolStripButton1 = new ToolStripButton();
            toolStripSeparator4 = new ToolStripSeparator();
            BtnRemover = new ToolStripButton();
            toolStripSeparator3 = new ToolStripSeparator();
            BtnExportar = new ToolStripButton();
            BtnDownloadTabela = new ToolStripButton();
            BtnAjuda = new ToolStripButton();
            BtnCancelar = new ToolStripButton();
            BtnPesquisar = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            CmbPesquisar = new ToolStripComboBox();
            TxtPesquisar = new ToolStripTextBox();
            DGVSantos = new DataGridView();
            panel1 = new Panel();
            tableLayoutPanel1 = new TableLayoutPanel();
            lblQtd = new Label();
            btnForward = new Button();
            btnPrevious = new Button();
            cbMaxRows = new ComboBox();
            ((System.ComponentModel.ISupportInitialize)BsProcesso).BeginInit();
            TSMenuItajai.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DGVSantos).BeginInit();
            panel1.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // TSMenuItajai
            // 
            TSMenuItajai.AutoSize = false;
            TSMenuItajai.Items.AddRange(new ToolStripItem[] { BtnAdicionar, toolStripSeparator2, toolStripButton1, toolStripSeparator4, BtnRemover, toolStripSeparator3, BtnExportar, BtnDownloadTabela, BtnAjuda, BtnCancelar, BtnPesquisar, toolStripSeparator1, CmbPesquisar, TxtPesquisar });
            TSMenuItajai.Location = new Point(0, 0);
            TSMenuItajai.Name = "TSMenuItajai";
            TSMenuItajai.Size = new Size(800, 40);
            TSMenuItajai.TabIndex = 3;
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
            // toolStripButton1
            // 
            toolStripButton1.Image = (Image)resources.GetObject("toolStripButton1.Image");
            toolStripButton1.ImageTransparentColor = SystemColors.Menu;
            toolStripButton1.Margin = new Padding(0);
            toolStripButton1.Name = "toolStripButton1";
            toolStripButton1.Size = new Size(57, 40);
            toolStripButton1.Text = "Editar";
            toolStripButton1.Click += BtnEditar_Click;
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Margin = new Padding(5, 0, 5, 0);
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new Size(6, 40);
            // 
            // BtnRemover
            // 
            BtnRemover.Image = (Image)resources.GetObject("BtnRemover.Image");
            BtnRemover.ImageTransparentColor = SystemColors.Menu;
            BtnRemover.Margin = new Padding(0);
            BtnRemover.Name = "BtnRemover";
            BtnRemover.Size = new Size(74, 40);
            BtnRemover.Text = "Remover";
            BtnRemover.Click += BtnExcluir_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Margin = new Padding(5, 0, 5, 0);
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(6, 40);
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
            BtnDownloadTabela.DisplayStyle = ToolStripItemDisplayStyle.Image;
            BtnDownloadTabela.Image = Properties.Resources.Download;
            BtnDownloadTabela.ImageTransparentColor = Color.Magenta;
            BtnDownloadTabela.Name = "BtnDownloadTabela";
            BtnDownloadTabela.Size = new Size(23, 37);
            BtnDownloadTabela.Text = "toolStripButton2";
            BtnDownloadTabela.Click += BtnDownloadTabela_Click;
            // 
            // BtnAjuda
            // 
            BtnAjuda.Alignment = ToolStripItemAlignment.Right;
            BtnAjuda.DisplayStyle = ToolStripItemDisplayStyle.Text;
            BtnAjuda.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            BtnAjuda.Image = (Image)resources.GetObject("BtnAjuda.Image");
            BtnAjuda.ImageTransparentColor = Color.Magenta;
            BtnAjuda.Margin = new Padding(0, 1, 1, 2);
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
            // DGVSantos
            // 
            DGVSantos.AllowUserToAddRows = false;
            DGVSantos.AllowUserToDeleteRows = false;
            DGVSantos.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = Color.WhiteSmoke;
            DGVSantos.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            DGVSantos.BackgroundColor = Color.White;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.DarkGray;
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            dataGridViewCellStyle2.ForeColor = Color.White;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            DGVSantos.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            DGVSantos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            tableLayoutPanel1.SetColumnSpan(DGVSantos, 5);
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.White;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle3.ForeColor = Color.Black;
            dataGridViewCellStyle3.SelectionBackColor = Color.LightBlue;
            dataGridViewCellStyle3.SelectionForeColor = Color.Black;
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            DGVSantos.DefaultCellStyle = dataGridViewCellStyle3;
            DGVSantos.Dock = DockStyle.Fill;
            DGVSantos.EnableHeadersVisualStyles = false;
            DGVSantos.Location = new Point(3, 3);
            DGVSantos.Name = "DGVSantos";
            DGVSantos.ReadOnly = true;
            DGVSantos.RowHeadersVisible = false;
            DGVSantos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            DGVSantos.Size = new Size(794, 373);
            DGVSantos.TabIndex = 2;
            DGVSantos.CellDoubleClick += DGVSantos_CellDoubleClick;
            DGVSantos.ColumnHeaderMouseClick += DGV_ColumnHeaderMouseClick;
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panel1.Controls.Add(tableLayoutPanel1);
            panel1.Location = new Point(0, 43);
            panel1.Name = "panel1";
            panel1.Size = new Size(800, 409);
            panel1.TabIndex = 4;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 5;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 30F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 30F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
            tableLayoutPanel1.Controls.Add(DGVSantos, 0, 0);
            tableLayoutPanel1.Controls.Add(lblQtd, 3, 1);
            tableLayoutPanel1.Controls.Add(btnForward, 2, 1);
            tableLayoutPanel1.Controls.Add(btnPrevious, 1, 1);
            tableLayoutPanel1.Controls.Add(cbMaxRows, 4, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tableLayoutPanel1.Size = new Size(800, 409);
            tableLayoutPanel1.TabIndex = 3;
            // 
            // lblQtd
            // 
            lblQtd.AutoSize = true;
            lblQtd.Dock = DockStyle.Fill;
            lblQtd.Location = new Point(652, 379);
            lblQtd.Name = "lblQtd";
            lblQtd.Size = new Size(65, 30);
            lblQtd.TabIndex = 3;
            lblQtd.Text = "1 - 50 / 500";
            lblQtd.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btnForward
            // 
            btnForward.Dock = DockStyle.Fill;
            btnForward.FlatAppearance.BorderSize = 0;
            btnForward.FlatStyle = FlatStyle.Flat;
            btnForward.Location = new Point(619, 379);
            btnForward.Margin = new Padding(0);
            btnForward.Name = "btnForward";
            btnForward.Size = new Size(30, 30);
            btnForward.TabIndex = 5;
            btnForward.Text = ">";
            btnForward.UseVisualStyleBackColor = true;
            btnForward.Click += BtnForward_Click;
            // 
            // btnPrevious
            // 
            btnPrevious.FlatAppearance.BorderSize = 0;
            btnPrevious.FlatStyle = FlatStyle.Flat;
            btnPrevious.Location = new Point(589, 379);
            btnPrevious.Margin = new Padding(0);
            btnPrevious.Name = "btnPrevious";
            btnPrevious.Size = new Size(21, 25);
            btnPrevious.TabIndex = 6;
            btnPrevious.Text = "<";
            btnPrevious.UseVisualStyleBackColor = true;
            btnPrevious.Click += BtnPrevious_Click;
            // 
            // cbMaxRows
            // 
            cbMaxRows.Dock = DockStyle.Fill;
            cbMaxRows.FormattingEnabled = true;
            cbMaxRows.Items.AddRange(new object[] { "10", "25", "50", "100", "Sem Limite" });
            cbMaxRows.Location = new Point(723, 382);
            cbMaxRows.Name = "cbMaxRows";
            cbMaxRows.Size = new Size(74, 23);
            cbMaxRows.TabIndex = 7;
            cbMaxRows.Text = "50";
            cbMaxRows.SelectedIndexChanged += cbMaxRows_SelectedIndexChanged;
            // 
            // frmSantos
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            ControlBox = false;
            Controls.Add(TSMenuItajai);
            Controls.Add(panel1);
            Name = "frmSantos";
            Text = "Gerenciamento de Processos";
            Shown += FrmSantos_Shown;
            KeyDown += FrmProcesso_KeyDown;
            ((System.ComponentModel.ISupportInitialize)BsProcesso).EndInit();
            TSMenuItajai.ResumeLayout(false);
            TSMenuItajai.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)DGVSantos).EndInit();
            panel1.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);

        }
        private ToolStrip TSMenuItajai;
        private ToolStripButton BtnAdicionar;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton BtnRemover;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripButton BtnExportar;
        private ToolStripButton BtnCancelar;
        private ToolStripButton BtnPesquisar;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripComboBox CmbPesquisar;
        private ToolStripTextBox TxtPesquisar;
        private DataGridView DGVSantos;
        private ToolStripButton toolStripButton1;
        private ToolStripSeparator toolStripSeparator4;
        private Panel panel1;
        private ToolStripButton BtnAjuda;
        private ToolStripButton BtnDownloadTabela;
        private TableLayoutPanel tableLayoutPanel1;
        private Label lblQtd;
        private Button btnForward;
        private Button btnPrevious;
        private ComboBox cbMaxRows;
    }
}
