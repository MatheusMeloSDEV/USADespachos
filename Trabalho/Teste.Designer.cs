namespace Trabalho
{
    partial class Teste
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
            poisonLabel2 = new ReaLTaiizor.Controls.PoisonLabel();
            toggleEstiloNovo = new ReaLTaiizor.Controls.ForeverToggle();
            poisonLabel1 = new ReaLTaiizor.Controls.PoisonLabel();
            toggleModoEscuro = new ReaLTaiizor.Controls.ForeverToggle();
            tcConfiguracoes.SuspendLayout();
            tpTabelas.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvConfiguracoes).BeginInit();
            tpEstilo.SuspendLayout();
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
            tcConfiguracoes.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Blue;
            tcConfiguracoes.TabIndex = 1;
            tcConfiguracoes.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Light;
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
            lblContador.Location = new Point(228, 14);
            lblContador.Name = "lblContador";
            lblContador.Size = new Size(65, 19);
            lblContador.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Blue;
            lblContador.TabIndex = 8;
            lblContador.Text = "Contador";
            lblContador.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Light;
            // 
            // btnCancelar
            // 
            btnCancelar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCancelar.BackColor = Color.Transparent;
            btnCancelar.BorderColor = Color.DodgerBlue;
            btnCancelar.EnteredBorderColor = Color.FromArgb(192, 0, 0);
            btnCancelar.EnteredColor = Color.FromArgb(32, 34, 37);
            btnCancelar.Font = new Font("Microsoft Sans Serif", 12F);
            btnCancelar.Image = null;
            btnCancelar.ImageAlign = ContentAlignment.MiddleLeft;
            btnCancelar.InactiveColor = Color.Transparent;
            btnCancelar.Location = new Point(714, 78);
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
            btnSalvar.BorderColor = Color.DodgerBlue;
            btnSalvar.EnteredBorderColor = Color.FromArgb(128, 255, 128);
            btnSalvar.EnteredColor = Color.FromArgb(32, 34, 37);
            btnSalvar.Font = new Font("Microsoft Sans Serif", 12F);
            btnSalvar.Image = null;
            btnSalvar.ImageAlign = ContentAlignment.MiddleLeft;
            btnSalvar.InactiveColor = Color.Transparent;
            btnSalvar.Location = new Point(714, 44);
            btnSalvar.Name = "btnSalvar";
            btnSalvar.PressedBorderColor = Color.FromArgb(128, 255, 128);
            btnSalvar.PressedColor = Color.FromArgb(128, 255, 128);
            btnSalvar.Size = new Size(26, 28);
            btnSalvar.TabIndex = 6;
            btnSalvar.Text = "✓";
            btnSalvar.TextAlignment = StringAlignment.Center;
            // 
            // btnBaixo
            // 
            btnBaixo.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnBaixo.BackColor = Color.Transparent;
            btnBaixo.BorderColor = Color.DodgerBlue;
            btnBaixo.EnteredBorderColor = Color.Navy;
            btnBaixo.EnteredColor = Color.FromArgb(32, 34, 37);
            btnBaixo.Font = new Font("Microsoft Sans Serif", 12F);
            btnBaixo.Image = null;
            btnBaixo.ImageAlign = ContentAlignment.MiddleLeft;
            btnBaixo.InactiveColor = Color.Transparent;
            btnBaixo.Location = new Point(714, 294);
            btnBaixo.Name = "btnBaixo";
            btnBaixo.PressedBorderColor = Color.FromArgb(128, 255, 128);
            btnBaixo.PressedColor = Color.FromArgb(128, 255, 128);
            btnBaixo.Size = new Size(26, 28);
            btnBaixo.TabIndex = 5;
            btnBaixo.Text = "↓";
            btnBaixo.TextAlignment = StringAlignment.Center;
            // 
            // btnCima
            // 
            btnCima.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCima.BackColor = Color.Transparent;
            btnCima.BorderColor = Color.DodgerBlue;
            btnCima.EnteredBorderColor = Color.Navy;
            btnCima.EnteredColor = Color.FromArgb(32, 34, 37);
            btnCima.Font = new Font("Microsoft Sans Serif", 12F);
            btnCima.Image = null;
            btnCima.ImageAlign = ContentAlignment.MiddleLeft;
            btnCima.InactiveColor = Color.Transparent;
            btnCima.Location = new Point(714, 260);
            btnCima.Name = "btnCima";
            btnCima.PressedBorderColor = Color.Navy;
            btnCima.PressedColor = Color.Navy;
            btnCima.Size = new Size(26, 28);
            btnCima.TabIndex = 1;
            btnCima.Text = "↑";
            btnCima.TextAlignment = StringAlignment.Center;
            // 
            // rbSelectAll
            // 
            rbSelectAll.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            rbSelectAll.AutoSize = true;
            rbSelectAll.Location = new Point(1171, 16);
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
            cmbGrid.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Blue;
            cmbGrid.TabIndex = 1;
            cmbGrid.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Light;
            cmbGrid.UseSelectable = true;
            // 
            // dgvConfiguracoes
            // 
            dgvConfiguracoes.AllowUserToAddRows = false;
            dgvConfiguracoes.AllowUserToDeleteRows = false;
            dgvConfiguracoes.AllowUserToResizeColumns = false;
            dgvConfiguracoes.AllowUserToResizeRows = false;
            dgvConfiguracoes.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            dgvConfiguracoes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvConfiguracoes.BackgroundColor = Color.FromArgb(255, 255, 255);
            dgvConfiguracoes.BorderStyle = BorderStyle.None;
            dgvConfiguracoes.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dgvConfiguracoes.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(0, 174, 219);
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Pixel);
            dataGridViewCellStyle1.ForeColor = Color.FromArgb(255, 255, 255);
            dataGridViewCellStyle1.SelectionBackColor = Color.FromArgb(0, 198, 247);
            dataGridViewCellStyle1.SelectionForeColor = Color.FromArgb(17, 17, 17);
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            dgvConfiguracoes.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dgvConfiguracoes.ColumnHeadersHeight = 30;
            dgvConfiguracoes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvConfiguracoes.Columns.AddRange(new DataGridViewColumn[] { colOrdem, colVisivel, colTitulo, colPropriedade, colTipo });
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(255, 255, 255);
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Pixel);
            dataGridViewCellStyle2.ForeColor = Color.FromArgb(136, 136, 136);
            dataGridViewCellStyle2.SelectionBackColor = Color.FromArgb(0, 198, 247);
            dataGridViewCellStyle2.SelectionForeColor = Color.FromArgb(17, 17, 17);
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            dgvConfiguracoes.DefaultCellStyle = dataGridViewCellStyle2;
            dgvConfiguracoes.EnableHeadersVisualStyles = false;
            dgvConfiguracoes.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Pixel);
            dgvConfiguracoes.GridColor = Color.FromArgb(255, 255, 255);
            dgvConfiguracoes.Location = new Point(9, 44);
            dgvConfiguracoes.Name = "dgvConfiguracoes";
            dgvConfiguracoes.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.FromArgb(0, 174, 219);
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Pixel);
            dataGridViewCellStyle3.ForeColor = Color.FromArgb(255, 255, 255);
            dataGridViewCellStyle3.SelectionBackColor = Color.FromArgb(0, 198, 247);
            dataGridViewCellStyle3.SelectionForeColor = Color.FromArgb(17, 17, 17);
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.True;
            dgvConfiguracoes.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            dgvConfiguracoes.RowHeadersVisible = false;
            dgvConfiguracoes.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dgvConfiguracoes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvConfiguracoes.Size = new Size(701, 278);
            dgvConfiguracoes.Style = ReaLTaiizor.Enum.Poison.ColorStyle.Blue;
            dgvConfiguracoes.TabIndex = 0;
            dgvConfiguracoes.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Light;
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
            tpEstilo.Controls.Add(poisonLabel2);
            tpEstilo.Controls.Add(toggleEstiloNovo);
            tpEstilo.Controls.Add(poisonLabel1);
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
            btnSalvarEstilo.Location = new Point(1200, 299);
            btnSalvarEstilo.Name = "btnSalvarEstilo";
            btnSalvarEstilo.PressedBorderColor = Color.FromArgb(128, 255, 128);
            btnSalvarEstilo.PressedColor = Color.FromArgb(128, 255, 128);
            btnSalvarEstilo.Size = new Size(83, 28);
            btnSalvarEstilo.TabIndex = 7;
            btnSalvarEstilo.Text = "✓ Salvar";
            btnSalvarEstilo.TextAlignment = StringAlignment.Center;
            // 
            // poisonLabel2
            // 
            poisonLabel2.AutoSize = true;
            poisonLabel2.Location = new Point(25, 31);
            poisonLabel2.Name = "poisonLabel2";
            poisonLabel2.Size = new Size(75, 19);
            poisonLabel2.TabIndex = 5;
            poisonLabel2.Text = "Estilo Novo";
            poisonLabel2.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Dark;
            // 
            // toggleEstiloNovo
            // 
            toggleEstiloNovo.BackColor = Color.Transparent;
            toggleEstiloNovo.BaseColor = Color.FromArgb(35, 168, 109);
            toggleEstiloNovo.BaseColorRed = Color.FromArgb(220, 85, 96);
            toggleEstiloNovo.BGColor = Color.Silver;
            toggleEstiloNovo.Checked = false;
            toggleEstiloNovo.Font = new Font("Segoe UI", 10F);
            toggleEstiloNovo.Location = new Point(103, 21);
            toggleEstiloNovo.Name = "toggleEstiloNovo";
            toggleEstiloNovo.Options = ReaLTaiizor.Controls.ForeverToggle._Options.Style1;
            toggleEstiloNovo.Size = new Size(76, 33);
            toggleEstiloNovo.TabIndex = 4;
            toggleEstiloNovo.Text = "foreverToggle1";
            toggleEstiloNovo.TextColor = Color.FromArgb(243, 243, 243);
            toggleEstiloNovo.ToggleColor = Color.DodgerBlue;
            // 
            // poisonLabel1
            // 
            poisonLabel1.AutoSize = true;
            poisonLabel1.Location = new Point(51, 88);
            poisonLabel1.Name = "poisonLabel1";
            poisonLabel1.Size = new Size(87, 19);
            poisonLabel1.TabIndex = 3;
            poisonLabel1.Text = "Modo Escuro";
            poisonLabel1.Theme = ReaLTaiizor.Enum.Poison.ThemeStyle.Dark;
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
            // 
            // Teste
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(tcConfiguracoes);
            Name = "Teste";
            Text = "Teste";
            tcConfiguracoes.ResumeLayout(false);
            tpTabelas.ResumeLayout(false);
            tpTabelas.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvConfiguracoes).EndInit();
            tpEstilo.ResumeLayout(false);
            tpEstilo.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private ReaLTaiizor.Controls.PoisonTabControl tcConfiguracoes;
        private TabPage tpTabelas;
        private ReaLTaiizor.Controls.PoisonLabel lblContador;
        private ReaLTaiizor.Controls.Button btnCancelar;
        private ReaLTaiizor.Controls.Button btnSalvar;
        private ReaLTaiizor.Controls.Button btnBaixo;
        private ReaLTaiizor.Controls.Button btnCima;
        private ReaLTaiizor.Controls.PoisonRadioButton rbSelectAll;
        private ReaLTaiizor.Controls.PoisonComboBox cmbGrid;
        private ReaLTaiizor.Controls.PoisonDataGridView dgvConfiguracoes;
        private DataGridViewTextBoxColumn colOrdem;
        private DataGridViewCheckBoxColumn colVisivel;
        private DataGridViewTextBoxColumn colTitulo;
        private DataGridViewTextBoxColumn colPropriedade;
        private DataGridViewTextBoxColumn colTipo;
        private TabPage tpEstilo;
        private ReaLTaiizor.Controls.Button btnSalvarEstilo;
        private ReaLTaiizor.Controls.PoisonLabel poisonLabel2;
        private ReaLTaiizor.Controls.ForeverToggle toggleEstiloNovo;
        private ReaLTaiizor.Controls.PoisonLabel poisonLabel1;
        private ReaLTaiizor.Controls.ForeverToggle toggleModoEscuro;
    }
}