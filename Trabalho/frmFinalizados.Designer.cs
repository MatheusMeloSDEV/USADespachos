namespace Trabalho
{
    partial class frmFinalizados
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmFinalizados));
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            TSMenuItajai = new ToolStrip();
            BtnRemover = new ToolStripButton();
            BtnAjuda = new ToolStripButton();
            BtnCancelar = new ToolStripButton();
            BtnPesquisar = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            CmbPesquisar = new ToolStripComboBox();
            TxtPesquisar = new ToolStripTextBox();
            panel1 = new Panel();
            DGVFinalizados = new DataGridView();
            BsProcesso = new BindingSource(components);
            TSMenuItajai.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DGVFinalizados).BeginInit();
            ((System.ComponentModel.ISupportInitialize)BsProcesso).BeginInit();
            SuspendLayout();
            // 
            // TSMenuItajai
            // 
            TSMenuItajai.AutoSize = false;
            TSMenuItajai.Items.AddRange(new ToolStripItem[] { BtnRemover, BtnAjuda, BtnCancelar, BtnPesquisar, toolStripSeparator1, CmbPesquisar, TxtPesquisar });
            TSMenuItajai.Location = new Point(0, 0);
            TSMenuItajai.Name = "TSMenuItajai";
            TSMenuItajai.Size = new Size(800, 40);
            TSMenuItajai.TabIndex = 4;
            TSMenuItajai.Text = "toolStrip1";
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
            panel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panel1.Controls.Add(DGVFinalizados);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 40);
            panel1.Name = "panel1";
            panel1.Size = new Size(800, 410);
            panel1.TabIndex = 5;
            // 
            // DGVFinalizados
            // 
            DGVFinalizados.AllowUserToAddRows = false;
            DGVFinalizados.AllowUserToDeleteRows = false;
            DGVFinalizados.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = Color.WhiteSmoke;
            DGVFinalizados.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            DGVFinalizados.BackgroundColor = Color.White;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.DarkGray;
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            dataGridViewCellStyle2.ForeColor = Color.White;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            DGVFinalizados.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            DGVFinalizados.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.White;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle3.ForeColor = Color.Black;
            dataGridViewCellStyle3.SelectionBackColor = Color.LightBlue;
            dataGridViewCellStyle3.SelectionForeColor = Color.Black;
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            DGVFinalizados.DefaultCellStyle = dataGridViewCellStyle3;
            DGVFinalizados.Dock = DockStyle.Fill;
            DGVFinalizados.EnableHeadersVisualStyles = false;
            DGVFinalizados.Location = new Point(0, 0);
            DGVFinalizados.MultiSelect = false;
            DGVFinalizados.Name = "DGVFinalizados";
            DGVFinalizados.ReadOnly = true;
            DGVFinalizados.RowHeadersVisible = false;
            DGVFinalizados.RowTemplate.Height = 25;
            DGVFinalizados.Size = new Size(800, 410);
            DGVFinalizados.TabIndex = 2;
            DGVFinalizados.CellDoubleClick += DGVFinalizados_CellDoubleClick;
            // 
            // frmFinalizados
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            ControlBox = false;
            Controls.Add(panel1);
            Controls.Add(TSMenuItajai);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmFinalizados";
            Text = "Finalizados";
            Shown += FrmFinalizados_Shown;
            KeyDown += FrmFinalizados_KeyDown;
            TSMenuItajai.ResumeLayout(false);
            TSMenuItajai.PerformLayout();
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)DGVFinalizados).EndInit();
            ((System.ComponentModel.ISupportInitialize)BsProcesso).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private ToolStrip TSMenuItajai;
        private ToolStripButton BtnRemover;
        private ToolStripButton BtnAjuda;
        private ToolStripButton BtnCancelar;
        private ToolStripButton BtnPesquisar;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripComboBox CmbPesquisar;
        private ToolStripTextBox TxtPesquisar;
        private Panel panel1;
        private DataGridView DGVFinalizados;
        private BindingSource BsProcesso;
    }
}