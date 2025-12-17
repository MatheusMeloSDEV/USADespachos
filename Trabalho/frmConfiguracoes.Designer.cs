namespace Trabalho
{
    partial class frmConfiguracoes
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmConfiguracoes));
            toolStrip1 = new ToolStrip();
            cmbGrids = new ToolStripComboBox();
            btnMoveDireita = new ToolStripButton();
            btnMoveEsquerda = new ToolStripButton();
            lblContador = new ToolStripLabel();
            dgvColunas = new DataGridView();
            colOrdem = new DataGridViewTextBoxColumn();
            colVisivel = new DataGridViewCheckBoxColumn();
            colTitulo = new DataGridViewTextBoxColumn();
            colNomePropriedade = new DataGridViewTextBoxColumn();
            btnCancelar = new Button();
            btnSalvar = new Button();
            btnReset = new Button();
            BtnRetSelectAll = new Button();
            BtnSelectAll = new Button();
            toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvColunas).BeginInit();
            SuspendLayout();
            // 
            // toolStrip1
            // 
            toolStrip1.Items.AddRange(new ToolStripItem[] { cmbGrids, btnMoveDireita, btnMoveEsquerda, lblContador });
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(800, 25);
            toolStrip1.TabIndex = 0;
            toolStrip1.Text = "toolStrip1";
            // 
            // cmbGrids
            // 
            cmbGrids.ForeColor = SystemColors.MenuText;
            cmbGrids.Name = "cmbGrids";
            cmbGrids.Size = new Size(200, 25);
            cmbGrids.Text = "Selecione a tabela...";
            cmbGrids.SelectedIndexChanged += CmbGrids_SelectedIndexChanged;
            // 
            // btnMoveDireita
            // 
            btnMoveDireita.Alignment = ToolStripItemAlignment.Right;
            btnMoveDireita.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnMoveDireita.Image = (Image)resources.GetObject("btnMoveDireita.Image");
            btnMoveDireita.ImageTransparentColor = Color.Magenta;
            btnMoveDireita.Name = "btnMoveDireita";
            btnMoveDireita.Size = new Size(76, 22);
            btnMoveDireita.Text = "Mover baixo";
            btnMoveDireita.Click += BtnDescer_Click;
            // 
            // btnMoveEsquerda
            // 
            btnMoveEsquerda.Alignment = ToolStripItemAlignment.Right;
            btnMoveEsquerda.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnMoveEsquerda.Image = (Image)resources.GetObject("btnMoveEsquerda.Image");
            btnMoveEsquerda.ImageTransparentColor = Color.Magenta;
            btnMoveEsquerda.Name = "btnMoveEsquerda";
            btnMoveEsquerda.Size = new Size(74, 22);
            btnMoveEsquerda.Text = "Mover cima";
            btnMoveEsquerda.Click += BtnSubir_Click;
            // 
            // lblContador
            // 
            lblContador.Name = "lblContador";
            lblContador.Size = new Size(10, 22);
            lblContador.Text = ".";
            // 
            // dgvColunas
            // 
            dgvColunas.AllowUserToAddRows = false;
            dgvColunas.AllowUserToDeleteRows = false;
            dgvColunas.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            dgvColunas.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvColunas.Columns.AddRange(new DataGridViewColumn[] { colOrdem, colVisivel, colTitulo, colNomePropriedade });
            dgvColunas.Location = new Point(0, 28);
            dgvColunas.Name = "dgvColunas";
            dgvColunas.Size = new Size(800, 381);
            dgvColunas.TabIndex = 1;
            dgvColunas.CellValueChanged += DgvColunas_CellValueChanged;
            // 
            // colOrdem
            // 
            colOrdem.HeaderText = "#";
            colOrdem.Name = "colOrdem";
            // 
            // colVisivel
            // 
            colVisivel.HeaderText = "Visível";
            colVisivel.Name = "colVisivel";
            // 
            // colTitulo
            // 
            colTitulo.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colTitulo.HeaderText = "Título";
            colTitulo.Name = "colTitulo";
            // 
            // colNomePropriedade
            // 
            colNomePropriedade.HeaderText = "Propriedade";
            colNomePropriedade.Name = "colNomePropriedade";
            colNomePropriedade.Visible = false;
            // 
            // btnCancelar
            // 
            btnCancelar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCancelar.BackColor = Color.FromArgb(255, 128, 128);
            btnCancelar.FlatAppearance.BorderColor = Color.Black;
            btnCancelar.FlatStyle = FlatStyle.Flat;
            btnCancelar.Location = new Point(713, 419);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(75, 23);
            btnCancelar.TabIndex = 2;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = false;
            // 
            // btnSalvar
            // 
            btnSalvar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSalvar.BackColor = Color.PaleGreen;
            btnSalvar.FlatStyle = FlatStyle.Flat;
            btnSalvar.Location = new Point(632, 419);
            btnSalvar.Name = "btnSalvar";
            btnSalvar.Size = new Size(75, 23);
            btnSalvar.TabIndex = 3;
            btnSalvar.Text = "Salvar";
            btnSalvar.UseVisualStyleBackColor = false;
            btnSalvar.Click += BtnSalvar_Click;
            // 
            // btnReset
            // 
            btnReset.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnReset.BackColor = Color.FromArgb(255, 128, 128);
            btnReset.FlatStyle = FlatStyle.Flat;
            btnReset.Location = new Point(306, 419);
            btnReset.Name = "btnReset";
            btnReset.Size = new Size(75, 23);
            btnReset.TabIndex = 4;
            btnReset.Text = "Reset";
            btnReset.UseVisualStyleBackColor = false;
            btnReset.Click += BtnReset_Click;
            // 
            // BtnRetSelectAll
            // 
            BtnRetSelectAll.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            BtnRetSelectAll.BackColor = Color.FromArgb(128, 128, 255);
            BtnRetSelectAll.FlatStyle = FlatStyle.Flat;
            BtnRetSelectAll.Location = new Point(12, 419);
            BtnRetSelectAll.Name = "BtnRetSelectAll";
            BtnRetSelectAll.Size = new Size(134, 23);
            BtnRetSelectAll.TabIndex = 6;
            BtnRetSelectAll.Text = "Retirar todos visíveis";
            BtnRetSelectAll.UseVisualStyleBackColor = false;
            BtnRetSelectAll.Click += BtnRetSelectAll_Click;
            // 
            // BtnSelectAll
            // 
            BtnSelectAll.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            BtnSelectAll.BackColor = Color.FromArgb(255, 255, 192);
            BtnSelectAll.FlatStyle = FlatStyle.Flat;
            BtnSelectAll.Location = new Point(152, 419);
            BtnSelectAll.Name = "BtnSelectAll";
            BtnSelectAll.Size = new Size(148, 23);
            BtnSelectAll.TabIndex = 7;
            BtnSelectAll.Text = "Selecionar Todas visíveis";
            BtnSelectAll.UseVisualStyleBackColor = false;
            BtnSelectAll.Click += BtnSelectAll_Click;
            // 
            // frmConfiguracoes
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(BtnSelectAll);
            Controls.Add(BtnRetSelectAll);
            Controls.Add(btnReset);
            Controls.Add(btnSalvar);
            Controls.Add(btnCancelar);
            Controls.Add(dgvColunas);
            Controls.Add(toolStrip1);
            Name = "frmConfiguracoes";
            Text = "Configurações";
            FormClosing += frmConfiguracoes_FormClosing;
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvColunas).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ToolStrip toolStrip1;
        private ToolStripComboBox cmbGrids;
        private DataGridView dgvColunas;
        private Button btnCancelar;
        private Button btnSalvar;
        private Button btnReset;
        private ToolStripButton btnMoveDireita;
        private ToolStripButton btnMoveEsquerda;
        private DataGridViewTextBoxColumn colOrdem;
        private DataGridViewCheckBoxColumn colVisivel;
        private DataGridViewTextBoxColumn colTitulo;
        private DataGridViewTextBoxColumn colNomePropriedade;
        private Button BtnRetSelectAll;
        private Button BtnSelectAll;
        private ToolStripLabel lblContador;
    }
}