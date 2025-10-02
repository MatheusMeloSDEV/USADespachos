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
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle5 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle6 = new DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmItajaí));
            DGVItajai = new DataGridView();
            TSMenuItajai = new ToolStrip();
            BtnAdicionar = new ToolStripButton();
            toolStripSeparator2 = new ToolStripSeparator();
            BtnEditar = new ToolStripButton();
            toolStripSeparator3 = new ToolStripSeparator();
            BtnExportar = new ToolStripButton();
            BtnAjuda = new ToolStripButton();
            BtnCancelar = new ToolStripButton();
            BtnPesquisar = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            CmbPesquisar = new ToolStripComboBox();
            TxtPesquisar = new ToolStripTextBox();
            BsProcesso = new BindingSource(components);
            panel1 = new Panel();
            BtnExcluir = new ToolStripButton();
            toolStripSeparator4 = new ToolStripSeparator();
            ((System.ComponentModel.ISupportInitialize)DGVItajai).BeginInit();
            TSMenuItajai.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)BsProcesso).BeginInit();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // DGVItajai
            // 
            DGVItajai.AllowUserToAddRows = false;
            DGVItajai.AllowUserToDeleteRows = false;
            DGVItajai.AllowUserToResizeColumns = false;
            DGVItajai.AllowUserToResizeRows = false;
            dataGridViewCellStyle4.BackColor = Color.WhiteSmoke;
            DGVItajai.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle4;
            DGVItajai.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            DGVItajai.BackgroundColor = Color.White;
            dataGridViewCellStyle5.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = Color.DarkGray;
            dataGridViewCellStyle5.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            dataGridViewCellStyle5.ForeColor = Color.White;
            dataGridViewCellStyle5.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = DataGridViewTriState.True;
            DGVItajai.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            DGVItajai.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle6.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = Color.White;
            dataGridViewCellStyle6.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle6.ForeColor = Color.Black;
            dataGridViewCellStyle6.SelectionBackColor = Color.LightBlue;
            dataGridViewCellStyle6.SelectionForeColor = Color.Black;
            dataGridViewCellStyle6.WrapMode = DataGridViewTriState.False;
            DGVItajai.DefaultCellStyle = dataGridViewCellStyle6;
            DGVItajai.Dock = DockStyle.Fill;
            DGVItajai.EnableHeadersVisualStyles = false;
            DGVItajai.Location = new Point(0, 0);
            DGVItajai.Name = "DGVItajai";
            DGVItajai.ReadOnly = true;
            DGVItajai.RowHeadersVisible = false;
            DGVItajai.RowTemplate.Height = 25;
            DGVItajai.Size = new Size(800, 408);
            DGVItajai.TabIndex = 0;
            DGVItajai.CellDoubleClick += DGVItajai_CellDoubleClick;
            DGVItajai.ColumnHeaderMouseClick += DGV_ColumnHeaderMouseClick;
            // 
            // TSMenuItajai
            // 
            TSMenuItajai.AutoSize = false;
            TSMenuItajai.Items.AddRange(new ToolStripItem[] { BtnAdicionar, toolStripSeparator2, BtnEditar, toolStripSeparator3, BtnExcluir, toolStripSeparator4, BtnExportar, BtnAjuda, BtnCancelar, BtnPesquisar, toolStripSeparator1, CmbPesquisar, TxtPesquisar });
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
            // BtnAjuda
            // 
            BtnAjuda.Alignment = ToolStripItemAlignment.Right;
            BtnAjuda.DisplayStyle = ToolStripItemDisplayStyle.Text;
            BtnAjuda.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            BtnAjuda.Image = (Image)resources.GetObject("BtnAjuda.Image");
            BtnAjuda.ImageTransparentColor = Color.Magenta;
            BtnAjuda.Margin = new Padding(0, 1, 7, 2);
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
            panel1.Controls.Add(DGVItajai);
            panel1.Location = new Point(0, 43);
            panel1.Name = "panel1";
            panel1.Size = new Size(800, 408);
            panel1.TabIndex = 2;
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
            ((System.ComponentModel.ISupportInitialize)DGVItajai).EndInit();
            TSMenuItajai.ResumeLayout(false);
            TSMenuItajai.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)BsProcesso).EndInit();
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private DataGridView DGVItajai;
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
    }
}