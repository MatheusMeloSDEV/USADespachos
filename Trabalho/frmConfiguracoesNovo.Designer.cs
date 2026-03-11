namespace Trabalho
{
    partial class frmConfiguracoesNovo
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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            tcConfiguracoes = new ReaLTaiizor.Controls.PoisonTabControl();
            tpTabelas = new TabPage();
            lblContador = new ReaLTaiizor.Controls.PoisonLabel();
            btnCancelar = new ReaLTaiizor.Controls.Button();
            btnSalvar = new ReaLTaiizor.Controls.Button();
            btnBaixo = new ReaLTaiizor.Controls.Button();
            btnCima = new ReaLTaiizor.Controls.Button();
            rbSelectAll = new ReaLTaiizor.Controls.PoisonRadioButton();
            cmbGrid = new ReaLTaiizor.Controls.PoisonComboBox();
            dgvConfiguracoes = new ReaLTaiizor.Controls.PoisonDataGridView();
            colOrdem = new DataGridViewTextBoxColumn();
            colVisivel = new DataGridViewCheckBoxColumn();
            colTitulo = new DataGridViewTextBoxColumn();
            colPropriedade = new DataGridViewTextBoxColumn();
            colTipo = new DataGridViewTextBoxColumn();
            tpEstilo = new TabPage();
            btnSalvarEstilo = new ReaLTaiizor.Controls.Button();
            lblEstilo = new ReaLTaiizor.Controls.PoisonLabel();
            toggleEstiloNovo = new ReaLTaiizor.Controls.ForeverToggle();
            lblModoEscuro = new ReaLTaiizor.Controls.PoisonLabel();
            toggleModoEscuro = new ReaLTaiizor.Controls.ForeverToggle();
            poisonStyleManager1 = new ReaLTaiizor.Manager.PoisonStyleManager(components);
            poisonStyleExtender1 = new ReaLTaiizor.Controls.PoisonStyleExtender(components);
            metroControlBox1 = new ReaLTaiizor.Controls.MetroControlBox();
            tcConfiguracoes.SuspendLayout();
            tpTabelas.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvConfiguracoes).BeginInit();
            tpEstilo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)poisonStyleManager1).BeginInit();
            SuspendLayout();
            // 
            // tcConfiguracoes
            // 
            tcConfiguracoes.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tcConfiguracoes.Controls.Add(tpTabelas);
            tcConfiguracoes.Controls.Add(tpEstilo);
            tcConfiguracoes.ItemSize = new Size(80, 26);
            tcConfiguracoes.Location = new Point(23, 63);
            tcConfiguracoes.Name = "tcConfiguracoes";
            tcConfiguracoes.Padding = new Point(8, 4);
            tcConfiguracoes.SelectedIndex = 0;
            tcConfiguracoes.Size = new Size(754, 364);
            tcConfiguracoes.Style = ReaLTaiizor.Enum.Poison.ColorStyle.White;
            tcConfiguracoes.TabIndex = 0;
            tcConfiguracoes.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Dark;
            tcConfiguracoes.UseSelectable = true;
            // 
            // tpTabelas
            // 
            tpTabelas.BackColor = Color.Transparent;
            tpTabelas.Controls.Add(lblContador);
            tpTabelas.Controls.Add(btnCancelar);
            tpTabelas.Controls.Add(btnSalvar);
            tpTabelas.Controls.Add(btnBaixo);
            tpTabelas.Controls.Add(btnCima);
            tpTabelas.Controls.Add(rbSelectAll);
            tpTabelas.Controls.Add(cmbGrid);
            tpTabelas.Controls.Add(dgvConfiguracoes);
            tpTabelas.Location = new Point(4, 30);
            tpTabelas.Name = "tpTabelas";
            tpTabelas.Padding = new Padding(3);
            tpTabelas.Size = new Size(746, 330);
            tpTabelas.TabIndex = 0;
            tpTabelas.Text = "Tabelas";
            // 
            // lblContador
            // 
            lblContador.AutoSize = true;
            lblContador.Font = new Font("Segoe UI Light", 14F, FontStyle.Regular, GraphicsUnit.Pixel);
            lblContador.Location = new Point(225, 11);
            lblContador.Name = "lblContador";
            lblContador.Size = new Size(65, 19);
            lblContador.TabIndex = 8;
            lblContador.Text = "Contador";
            lblContador.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Dark;
            // 
            // btnCancelar
            // 
            btnCancelar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCancelar.BackColor = Color.Transparent;
            btnCancelar.BorderColor = Color.FromArgb(32, 34, 37);
            btnCancelar.EnteredBorderColor = Color.FromArgb(192, 0, 0);
            btnCancelar.EnteredColor = Color.FromArgb(32, 34, 37);
            btnCancelar.Font = new Font("Microsoft Sans Serif", 12F);
            btnCancelar.Image = null;
            btnCancelar.ImageAlign = ContentAlignment.MiddleLeft;
            btnCancelar.InactiveColor = Color.FromArgb(32, 34, 37);
            btnCancelar.Location = new Point(714, 75);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.PressedBorderColor = Color.FromArgb(192, 0, 0);
            btnCancelar.PressedColor = Color.FromArgb(192, 0, 0);
            btnCancelar.Size = new Size(26, 28);
            btnCancelar.TabIndex = 7;
            btnCancelar.Text = "❌";
            btnCancelar.TextAlignment = StringAlignment.Center;
            // 
            // btnSalvar
            // 
            btnSalvar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSalvar.BackColor = Color.Transparent;
            btnSalvar.BorderColor = Color.FromArgb(32, 34, 37);
            btnSalvar.EnteredBorderColor = Color.FromArgb(128, 255, 128);
            btnSalvar.EnteredColor = Color.FromArgb(32, 34, 37);
            btnSalvar.Font = new Font("Microsoft Sans Serif", 12F);
            btnSalvar.Image = null;
            btnSalvar.ImageAlign = ContentAlignment.MiddleLeft;
            btnSalvar.InactiveColor = Color.FromArgb(32, 34, 37);
            btnSalvar.Location = new Point(714, 41);
            btnSalvar.Name = "btnSalvar";
            btnSalvar.PressedBorderColor = Color.FromArgb(128, 255, 128);
            btnSalvar.PressedColor = Color.FromArgb(128, 255, 128);
            btnSalvar.Size = new Size(26, 28);
            btnSalvar.TabIndex = 6;
            btnSalvar.Text = "✓";
            btnSalvar.TextAlignment = StringAlignment.Center;
            btnSalvar.Click += btnSalvar_Click;
            // 
            // btnBaixo
            // 
            btnBaixo.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnBaixo.BackColor = Color.Transparent;
            btnBaixo.BorderColor = Color.FromArgb(32, 34, 37);
            btnBaixo.EnteredBorderColor = Color.Navy;
            btnBaixo.EnteredColor = Color.FromArgb(32, 34, 37);
            btnBaixo.Font = new Font("Microsoft Sans Serif", 12F);
            btnBaixo.Image = null;
            btnBaixo.ImageAlign = ContentAlignment.MiddleLeft;
            btnBaixo.InactiveColor = Color.FromArgb(32, 34, 37);
            btnBaixo.Location = new Point(714, 296);
            btnBaixo.Name = "btnBaixo";
            btnBaixo.PressedBorderColor = Color.FromArgb(128, 255, 128);
            btnBaixo.PressedColor = Color.FromArgb(128, 255, 128);
            btnBaixo.Size = new Size(26, 28);
            btnBaixo.TabIndex = 5;
            btnBaixo.Text = "↓";
            btnBaixo.TextAlignment = StringAlignment.Center;
            btnBaixo.Click += btnBaixo_Click;
            // 
            // btnCima
            // 
            btnCima.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCima.BackColor = Color.Transparent;
            btnCima.BorderColor = Color.FromArgb(32, 34, 37);
            btnCima.EnteredBorderColor = Color.Navy;
            btnCima.EnteredColor = Color.FromArgb(32, 34, 37);
            btnCima.Font = new Font("Microsoft Sans Serif", 12F);
            btnCima.Image = null;
            btnCima.ImageAlign = ContentAlignment.MiddleLeft;
            btnCima.InactiveColor = Color.FromArgb(32, 34, 37);
            btnCima.Location = new Point(714, 262);
            btnCima.Name = "btnCima";
            btnCima.PressedBorderColor = Color.Navy;
            btnCima.PressedColor = Color.Navy;
            btnCima.Size = new Size(26, 28);
            btnCima.TabIndex = 1;
            btnCima.Text = "↑";
            btnCima.TextAlignment = StringAlignment.Center;
            btnCima.Click += btnCima_Click;
            // 
            // rbSelectAll
            // 
            rbSelectAll.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            rbSelectAll.AutoSize = true;
            rbSelectAll.Location = new Point(628, 13);
            rbSelectAll.Name = "rbSelectAll";
            rbSelectAll.Size = new Size(112, 15);
            rbSelectAll.Style = ReaLTaiizor.Enum.Poison.ColorStyle.White;
            rbSelectAll.TabIndex = 4;
            rbSelectAll.Text = "Selecionar Todos";
            rbSelectAll.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Dark;
            rbSelectAll.UseSelectable = true;
            // 
            // cmbGrid
            // 
            cmbGrid.FormattingEnabled = true;
            cmbGrid.ItemHeight = 23;
            cmbGrid.Location = new Point(6, 6);
            cmbGrid.Name = "cmbGrid";
            cmbGrid.Size = new Size(201, 29);
            cmbGrid.Style = ReaLTaiizor.Enum.Poison.ColorStyle.White;
            cmbGrid.TabIndex = 1;
            cmbGrid.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Dark;
            cmbGrid.UseSelectable = true;
            cmbGrid.SelectedIndexChanged += cmbGrid_SelectedIndexChanged;
            // 
            // dgvConfiguracoes
            // 
            dgvConfiguracoes.AllowUserToAddRows = false;
            dgvConfiguracoes.AllowUserToDeleteRows = false;
            dgvConfiguracoes.AllowUserToResizeColumns = false;
            dgvConfiguracoes.AllowUserToResizeRows = false;
            dgvConfiguracoes.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            dgvConfiguracoes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvConfiguracoes.BackgroundColor = Color.FromArgb(17, 17, 17);
            dgvConfiguracoes.BorderStyle = BorderStyle.None;
            dgvConfiguracoes.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dgvConfiguracoes.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(85, 85, 85);
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Pixel);
            dataGridViewCellStyle1.ForeColor = Color.FromArgb(17, 17, 17);
            dataGridViewCellStyle1.SelectionBackColor = Color.FromArgb(102, 102, 102);
            dataGridViewCellStyle1.SelectionForeColor = Color.FromArgb(17, 17, 17);
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            dgvConfiguracoes.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dgvConfiguracoes.ColumnHeadersHeight = 30;
            dgvConfiguracoes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvConfiguracoes.Columns.AddRange(new DataGridViewColumn[] { colOrdem, colVisivel, colTitulo, colPropriedade, colTipo });
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(17, 17, 17);
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Pixel);
            dataGridViewCellStyle2.ForeColor = Color.FromArgb(109, 109, 109);
            dataGridViewCellStyle2.SelectionBackColor = Color.FromArgb(102, 102, 102);
            dataGridViewCellStyle2.SelectionForeColor = Color.FromArgb(17, 17, 17);
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            dgvConfiguracoes.DefaultCellStyle = dataGridViewCellStyle2;
            dgvConfiguracoes.EnableHeadersVisualStyles = false;
            dgvConfiguracoes.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Pixel);
            dgvConfiguracoes.GridColor = Color.FromArgb(17, 17, 17);
            dgvConfiguracoes.Location = new Point(6, 41);
            dgvConfiguracoes.Name = "dgvConfiguracoes";
            dgvConfiguracoes.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.FromArgb(85, 85, 85);
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Pixel);
            dataGridViewCellStyle3.ForeColor = Color.FromArgb(17, 17, 17);
            dataGridViewCellStyle3.SelectionBackColor = Color.FromArgb(102, 102, 102);
            dataGridViewCellStyle3.SelectionForeColor = Color.FromArgb(17, 17, 17);
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.True;
            dgvConfiguracoes.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            dgvConfiguracoes.RowHeadersVisible = false;
            dgvConfiguracoes.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dgvConfiguracoes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvConfiguracoes.Size = new Size(701, 283);
            dgvConfiguracoes.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Silver;
            dgvConfiguracoes.TabIndex = 0;
            dgvConfiguracoes.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Dark;
            // 
            // colOrdem
            // 
            colOrdem.HeaderText = "Ordem";
            colOrdem.Name = "colOrdem";
            // 
            // colVisivel
            // 
            colVisivel.HeaderText = "Visível";
            colVisivel.Name = "colVisivel";
            // 
            // colTitulo
            // 
            colTitulo.HeaderText = "Título";
            colTitulo.Name = "colTitulo";
            // 
            // colPropriedade
            // 
            colPropriedade.HeaderText = "Propriedade";
            colPropriedade.Name = "colPropriedade";
            // 
            // colTipo
            // 
            colTipo.HeaderText = "Tipo";
            colTipo.Name = "colTipo";
            // 
            // tpEstilo
            // 
            tpEstilo.BackColor = Color.Transparent;
            tpEstilo.Controls.Add(btnSalvarEstilo);
            tpEstilo.Controls.Add(lblEstilo);
            tpEstilo.Controls.Add(toggleEstiloNovo);
            tpEstilo.Controls.Add(lblModoEscuro);
            tpEstilo.Controls.Add(toggleModoEscuro);
            tpEstilo.Location = new Point(4, 30);
            tpEstilo.Name = "tpEstilo";
            tpEstilo.Padding = new Padding(3);
            tpEstilo.Size = new Size(746, 330);
            tpEstilo.TabIndex = 1;
            tpEstilo.Text = "Estilo";
            // 
            // btnSalvarEstilo
            // 
            btnSalvarEstilo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSalvarEstilo.BackColor = Color.Transparent;
            btnSalvarEstilo.BorderColor = Color.FromArgb(32, 34, 37);
            btnSalvarEstilo.EnteredBorderColor = Color.FromArgb(128, 255, 128);
            btnSalvarEstilo.EnteredColor = Color.FromArgb(32, 34, 37);
            btnSalvarEstilo.Font = new Font("Microsoft Sans Serif", 12F);
            btnSalvarEstilo.Image = null;
            btnSalvarEstilo.ImageAlign = ContentAlignment.MiddleLeft;
            btnSalvarEstilo.InactiveColor = Color.FromArgb(32, 34, 37);
            btnSalvarEstilo.Location = new Point(657, 296);
            btnSalvarEstilo.Name = "btnSalvarEstilo";
            btnSalvarEstilo.PressedBorderColor = Color.FromArgb(128, 255, 128);
            btnSalvarEstilo.PressedColor = Color.FromArgb(128, 255, 128);
            btnSalvarEstilo.Size = new Size(83, 28);
            btnSalvarEstilo.TabIndex = 7;
            btnSalvarEstilo.Text = "✓ Salvar";
            btnSalvarEstilo.TextAlignment = StringAlignment.Center;
            btnSalvarEstilo.Click += btnSalvarEstilo_Click;
            // 
            // lblEstilo
            // 
            lblEstilo.AutoSize = true;
            lblEstilo.Location = new Point(22, 28);
            lblEstilo.Name = "lblEstilo";
            lblEstilo.Size = new Size(75, 19);
            lblEstilo.TabIndex = 5;
            lblEstilo.Text = "Estilo Novo";
            lblEstilo.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Dark;
            // 
            // toggleEstiloNovo
            // 
            toggleEstiloNovo.BackColor = Color.Transparent;
            toggleEstiloNovo.BaseColor = Color.FromArgb(35, 168, 109);
            toggleEstiloNovo.BaseColorRed = Color.FromArgb(220, 85, 96);
            toggleEstiloNovo.BGColor = Color.FromArgb(84, 85, 86);
            toggleEstiloNovo.Checked = false;
            toggleEstiloNovo.Font = new Font("Segoe UI", 10F);
            toggleEstiloNovo.Location = new Point(103, 21);
            toggleEstiloNovo.Name = "toggleEstiloNovo";
            toggleEstiloNovo.Options = ReaLTaiizor.Controls.ForeverToggle._Options.Style1;
            toggleEstiloNovo.Size = new Size(76, 33);
            toggleEstiloNovo.TabIndex = 4;
            toggleEstiloNovo.Text = "foreverToggle1";
            toggleEstiloNovo.TextColor = Color.FromArgb(243, 243, 243);
            toggleEstiloNovo.ToggleColor = Color.FromArgb(45, 47, 49);
            toggleEstiloNovo.CheckedChanged += toggleEstiloNovo_CheckedChanged;
            // 
            // lblModoEscuro
            // 
            lblModoEscuro.AutoSize = true;
            lblModoEscuro.Location = new Point(48, 85);
            lblModoEscuro.Name = "lblModoEscuro";
            lblModoEscuro.Size = new Size(87, 19);
            lblModoEscuro.TabIndex = 3;
            lblModoEscuro.Text = "Modo Escuro";
            lblModoEscuro.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Dark;
            // 
            // toggleModoEscuro
            // 
            toggleModoEscuro.BackColor = Color.Transparent;
            toggleModoEscuro.BaseColor = Color.FromArgb(35, 168, 109);
            toggleModoEscuro.BaseColorRed = Color.FromArgb(220, 85, 96);
            toggleModoEscuro.BGColor = Color.FromArgb(84, 85, 86);
            toggleModoEscuro.Checked = false;
            toggleModoEscuro.Font = new Font("Segoe UI", 10F);
            toggleModoEscuro.Location = new Point(141, 78);
            toggleModoEscuro.Name = "toggleModoEscuro";
            toggleModoEscuro.Options = ReaLTaiizor.Controls.ForeverToggle._Options.Style1;
            toggleModoEscuro.Size = new Size(76, 33);
            toggleModoEscuro.TabIndex = 2;
            toggleModoEscuro.Text = "foreverToggle1";
            toggleModoEscuro.TextColor = Color.FromArgb(243, 243, 243);
            toggleModoEscuro.ToggleColor = Color.FromArgb(45, 47, 49);
            toggleModoEscuro.CheckedChanged += toggleModoEscuro_CheckedChanged;
            // 
            // poisonStyleManager1
            // 
            poisonStyleManager1.Owner = this;
            poisonStyleManager1.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Silver;
            poisonStyleManager1.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Dark;
            // 
            // poisonStyleExtender1
            // 
            poisonStyleExtender1.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Dark;
            // 
            // metroControlBox1
            // 
            metroControlBox1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            metroControlBox1.CloseHoverBackColor = Color.FromArgb(183, 40, 40);
            metroControlBox1.CloseHoverForeColor = Color.White;
            metroControlBox1.CloseNormalForeColor = Color.Gray;
            metroControlBox1.DefaultLocation = ReaLTaiizor.Enum.Metro.LocationType.Normal;
            metroControlBox1.DisabledForeColor = Color.Silver;
            metroControlBox1.IsDerivedStyle = true;
            metroControlBox1.Location = new Point(694, 11);
            metroControlBox1.MaximizeBox = true;
            metroControlBox1.MaximizeHoverBackColor = Color.FromArgb(238, 238, 238);
            metroControlBox1.MaximizeHoverForeColor = Color.Gray;
            metroControlBox1.MaximizeNormalForeColor = Color.Gray;
            metroControlBox1.MinimizeBox = true;
            metroControlBox1.MinimizeHoverBackColor = Color.FromArgb(238, 238, 238);
            metroControlBox1.MinimizeHoverForeColor = Color.Gray;
            metroControlBox1.MinimizeNormalForeColor = Color.Gray;
            metroControlBox1.Name = "metroControlBox1";
            metroControlBox1.Size = new Size(100, 25);
            metroControlBox1.Style = ReaLTaiizor.Enum.Metro.Style.Dark;
            metroControlBox1.StyleManager = null;
            metroControlBox1.TabIndex = 1;
            metroControlBox1.Text = "metroControlBox1";
            metroControlBox1.ThemeAuthor = "Taiizor";
            metroControlBox1.ThemeName = "MetroDark";
            // 
            // frmConfiguracoesNovo
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            ControlBox = false;
            Controls.Add(metroControlBox1);
            Controls.Add(tcConfiguracoes);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmConfiguracoesNovo";
            Style = ReaLTaiizor.Enum.Poison.ColorStyle.White;
            Text = "Configuração";
            Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Dark;
            tcConfiguracoes.ResumeLayout(false);
            tpTabelas.ResumeLayout(false);
            tpTabelas.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvConfiguracoes).EndInit();
            tpEstilo.ResumeLayout(false);
            tpEstilo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)poisonStyleManager1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private ReaLTaiizor.Controls.PoisonTabControl tcConfiguracoes;
        private TabPage tpTabelas;
        private TabPage tpEstilo;
        private ReaLTaiizor.Controls.PoisonDataGridView dgvConfiguracoes;
        private ReaLTaiizor.Controls.PoisonComboBox cmbGrid;
        private ReaLTaiizor.Manager.PoisonStyleManager poisonStyleManager1;
        private ReaLTaiizor.Controls.PoisonStyleExtender poisonStyleExtender1;
        private ReaLTaiizor.Controls.PoisonRadioButton rbSelectAll;
        private ReaLTaiizor.Controls.Button btnCima;
        private ReaLTaiizor.Controls.Button btnCancelar;
        private ReaLTaiizor.Controls.Button btnSalvar;
        private ReaLTaiizor.Controls.Button btnBaixo;
        private ReaLTaiizor.Controls.PoisonLabel lblModoEscuro;
        private ReaLTaiizor.Controls.ForeverToggle toggleModoEscuro;
        private ReaLTaiizor.Controls.PoisonLabel lblContador;
        private DataGridViewTextBoxColumn colOrdem;
        private DataGridViewCheckBoxColumn colVisivel;
        private DataGridViewTextBoxColumn colTitulo;
        private DataGridViewTextBoxColumn colPropriedade;
        private DataGridViewTextBoxColumn colTipo;
        private ReaLTaiizor.Controls.MetroControlBox metroControlBox1;
        private ReaLTaiizor.Controls.PoisonLabel lblEstilo;
        private ReaLTaiizor.Controls.ForeverToggle toggleEstiloNovo;
        private ReaLTaiizor.Controls.Button btnSalvarEstilo;
    }
}