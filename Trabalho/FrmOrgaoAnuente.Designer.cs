namespace Trabalho
{
    partial class FrmOrgaoAnuente
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmOrgaoAnuente));
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            TsMenu = new ToolStrip();
            BtnEditar = new ToolStripButton();
            BtnCancelar = new ToolStripButton();
            BtnPesquisar = new ToolStripButton();
            toolStripSeparator2 = new ToolStripSeparator();
            CbPesquisa = new ToolStripComboBox();
            TxtPesquisa = new ToolStripTextBox();
            DgvOrgaoAnuente = new DataGridView();
            TsMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DgvOrgaoAnuente).BeginInit();
            SuspendLayout();
            // 
            // TsMenu
            // 
            TsMenu.AutoSize = false;
            TsMenu.BackColor = Color.WhiteSmoke;
            TsMenu.GripStyle = ToolStripGripStyle.Hidden;
            TsMenu.Items.AddRange(new ToolStripItem[] { BtnEditar, BtnCancelar, BtnPesquisar, toolStripSeparator2, CbPesquisa, TxtPesquisa });
            TsMenu.Location = new Point(0, 0);
            TsMenu.Name = "TsMenu";
            TsMenu.Padding = new Padding(5);
            TsMenu.RenderMode = ToolStripRenderMode.Professional;
            TsMenu.Size = new Size(800, 40);
            TsMenu.TabIndex = 1;
            TsMenu.Text = "toolStrip1";
            // 
            // BtnEditar
            // 
            BtnEditar.Font = new Font("Segoe UI", 10F);
            BtnEditar.Image = (Image)resources.GetObject("BtnEditar.Image");
            BtnEditar.ImageTransparentColor = Color.Magenta;
            BtnEditar.Margin = new Padding(7, 0, 5, 0);
            BtnEditar.Name = "BtnEditar";
            BtnEditar.Size = new Size(64, 30);
            BtnEditar.Text = "Editar";
            BtnEditar.Click += BtnEditar_Click;
            // 
            // BtnCancelar
            // 
            BtnCancelar.Alignment = ToolStripItemAlignment.Right;
            BtnCancelar.DisplayStyle = ToolStripItemDisplayStyle.Image;
            BtnCancelar.Image = (Image)resources.GetObject("BtnCancelar.Image");
            BtnCancelar.ImageTransparentColor = Color.Magenta;
            BtnCancelar.Margin = new Padding(0, 0, 7, 0);
            BtnCancelar.Name = "BtnCancelar";
            BtnCancelar.Size = new Size(23, 30);
            BtnCancelar.Text = "Cancelar";
            BtnCancelar.Click += BtnCancelar_Click;
            // 
            // BtnPesquisar
            // 
            BtnPesquisar.Alignment = ToolStripItemAlignment.Right;
            BtnPesquisar.DisplayStyle = ToolStripItemDisplayStyle.Image;
            BtnPesquisar.Image = (Image)resources.GetObject("BtnPesquisar.Image");
            BtnPesquisar.ImageTransparentColor = Color.Magenta;
            BtnPesquisar.Margin = new Padding(0);
            BtnPesquisar.Name = "BtnPesquisar";
            BtnPesquisar.Size = new Size(23, 30);
            BtnPesquisar.Text = "toolStripButton2";
            BtnPesquisar.Click += BtnPesquisar_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Alignment = ToolStripItemAlignment.Right;
            toolStripSeparator2.Margin = new Padding(5, 0, 5, 0);
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(6, 30);
            // 
            // CbPesquisa
            // 
            CbPesquisa.Alignment = ToolStripItemAlignment.Right;
            CbPesquisa.AutoSize = false;
            CbPesquisa.Margin = new Padding(0);
            CbPesquisa.Name = "CbPesquisa";
            CbPesquisa.Size = new Size(150, 23);
            // 
            // TxtPesquisa
            // 
            TxtPesquisa.Alignment = ToolStripItemAlignment.Right;
            TxtPesquisa.AutoSize = false;
            TxtPesquisa.Margin = new Padding(0, 0, 5, 0);
            TxtPesquisa.Name = "TxtPesquisa";
            TxtPesquisa.Size = new Size(150, 30);
            // 
            // DgvOrgaoAnuente
            // 
            DgvOrgaoAnuente.AllowUserToAddRows = false;
            DgvOrgaoAnuente.AllowUserToDeleteRows = false;
            DgvOrgaoAnuente.AllowUserToResizeColumns = false;
            DgvOrgaoAnuente.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = Color.WhiteSmoke;
            DgvOrgaoAnuente.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            DgvOrgaoAnuente.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            DgvOrgaoAnuente.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            DgvOrgaoAnuente.BackgroundColor = Color.White;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.DarkGray;
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            dataGridViewCellStyle2.ForeColor = Color.White;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            DgvOrgaoAnuente.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            DgvOrgaoAnuente.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.White;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dataGridViewCellStyle3.ForeColor = Color.Black;
            dataGridViewCellStyle3.SelectionBackColor = Color.LightBlue;
            dataGridViewCellStyle3.SelectionForeColor = Color.Black;
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            DgvOrgaoAnuente.DefaultCellStyle = dataGridViewCellStyle3;
            DgvOrgaoAnuente.EnableHeadersVisualStyles = false;
            DgvOrgaoAnuente.Location = new Point(12, 43);
            DgvOrgaoAnuente.Name = "DgvOrgaoAnuente";
            DgvOrgaoAnuente.ReadOnly = true;
            DgvOrgaoAnuente.RowHeadersVisible = false;
            DgvOrgaoAnuente.RowTemplate.Height = 25;
            DgvOrgaoAnuente.Size = new Size(776, 395);
            DgvOrgaoAnuente.TabIndex = 2;
            // 
            // FrmOrgaoAnuente
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            ControlBox = false;
            Controls.Add(DgvOrgaoAnuente);
            Controls.Add(TsMenu);
            Name = "FrmOrgaoAnuente";
            Text = "Gerenciamento Orgãos Anuentes";
            WindowState = FormWindowState.Maximized;
            Shown += FrmOrgaoAnuente_Shown;
            TsMenu.ResumeLayout(false);
            TsMenu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)DgvOrgaoAnuente).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private ToolStrip TsMenu;
        private ToolStripButton BtnEditar;
        private ToolStripButton BtnCancelar;
        private ToolStripButton BtnPesquisar;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripComboBox CbPesquisa;
        private ToolStripTextBox TxtPesquisa;
        private DataGridView DgvOrgaoAnuente;
    }
}