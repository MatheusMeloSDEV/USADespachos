namespace Trabalho
{
    partial class LIEditControl
    {
        /// <summary> 
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Designer de Componentes

        /// <summary> 
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            btnExcluirLpco = new Button();
            BtnNovoOrgaoAnuente = new Button();
            groupBox5 = new GroupBox();
            tableLayoutPanel1 = new TableLayoutPanel();
            label40 = new Label();
            TxtLi = new TextBox();
            DtpDataRegistro = new DateTimePicker();
            label39 = new Label();
            label41 = new Label();
            TxtNCM = new TextBox();
            CBOrgaoAnuente = new ComboBox();
            tabControl2 = new TabControl();
            MAPA = new TabPage();
            lpcoEditControl1 = new LpcoEditControl();
            label28 = new Label();
            dateTimePicker3 = new DateTimePicker();
            label29 = new Label();
            dateTimePicker2 = new DateTimePicker();
            ANVISA = new TabPage();
            DECEX = new TabPage();
            IBAMA = new TabPage();
            IMETRO = new TabPage();
            groupBox5.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            tabControl2.SuspendLayout();
            MAPA.SuspendLayout();
            SuspendLayout();
            // 
            // btnExcluirLpco
            // 
            btnExcluirLpco.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnExcluirLpco.Location = new Point(610, 113);
            btnExcluirLpco.Name = "btnExcluirLpco";
            btnExcluirLpco.Size = new Size(129, 23);
            btnExcluirLpco.TabIndex = 823;
            btnExcluirLpco.Text = "Excluir LPCO";
            btnExcluirLpco.UseVisualStyleBackColor = true;
            // 
            // BtnNovoOrgaoAnuente
            // 
            BtnNovoOrgaoAnuente.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            BtnNovoOrgaoAnuente.Location = new Point(610, 70);
            BtnNovoOrgaoAnuente.Name = "BtnNovoOrgaoAnuente";
            BtnNovoOrgaoAnuente.Size = new Size(129, 23);
            BtnNovoOrgaoAnuente.TabIndex = 820;
            BtnNovoOrgaoAnuente.Text = "Novo LPCO";
            BtnNovoOrgaoAnuente.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            groupBox5.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox5.Controls.Add(tableLayoutPanel1);
            groupBox5.Location = new Point(3, 142);
            groupBox5.Name = "groupBox5";
            groupBox5.Size = new Size(736, 83);
            groupBox5.TabIndex = 819;
            groupBox5.TabStop = false;
            groupBox5.Text = "Dados LI";
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tableLayoutPanel1.Controls.Add(label40, 0, 0);
            tableLayoutPanel1.Controls.Add(TxtLi, 0, 1);
            tableLayoutPanel1.Controls.Add(DtpDataRegistro, 2, 1);
            tableLayoutPanel1.Controls.Add(label39, 2, 0);
            tableLayoutPanel1.Controls.Add(label41, 1, 0);
            tableLayoutPanel1.Controls.Add(TxtNCM, 1, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(3, 19);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.Padding = new Padding(2);
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 43.85965F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 56.14035F));
            tableLayoutPanel1.Size = new Size(730, 61);
            tableLayoutPanel1.TabIndex = 424;
            // 
            // label40
            // 
            label40.AutoSize = true;
            label40.Dock = DockStyle.Fill;
            label40.Font = new Font("Segoe UI", 12F);
            label40.Location = new Point(5, 2);
            label40.Name = "label40";
            label40.Size = new Size(236, 25);
            label40.TabIndex = 420;
            label40.Text = "LI";
            label40.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // TxtLi
            // 
            TxtLi.Dock = DockStyle.Fill;
            TxtLi.Location = new Point(7, 32);
            TxtLi.Margin = new Padding(5);
            TxtLi.Name = "TxtLi";
            TxtLi.Size = new Size(232, 23);
            TxtLi.TabIndex = 418;
            // 
            // DtpDataRegistro
            // 
            DtpDataRegistro.Dock = DockStyle.Fill;
            DtpDataRegistro.Format = DateTimePickerFormat.Short;
            DtpDataRegistro.Location = new Point(491, 32);
            DtpDataRegistro.Margin = new Padding(5);
            DtpDataRegistro.Name = "DtpDataRegistro";
            DtpDataRegistro.Size = new Size(232, 23);
            DtpDataRegistro.TabIndex = 418;
            // 
            // label39
            // 
            label39.AutoSize = true;
            label39.Dock = DockStyle.Fill;
            label39.Font = new Font("Segoe UI", 11F);
            label39.Location = new Point(489, 2);
            label39.Name = "label39";
            label39.Size = new Size(236, 25);
            label39.TabIndex = 419;
            label39.Text = "Data Registro";
            label39.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label41
            // 
            label41.AutoSize = true;
            label41.Dock = DockStyle.Fill;
            label41.Font = new Font("Segoe UI", 12F);
            label41.Location = new Point(247, 2);
            label41.Name = "label41";
            label41.Size = new Size(236, 25);
            label41.TabIndex = 421;
            label41.Text = "NCM";
            label41.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // TxtNCM
            // 
            TxtNCM.Dock = DockStyle.Fill;
            TxtNCM.Location = new Point(249, 32);
            TxtNCM.Margin = new Padding(5);
            TxtNCM.Name = "TxtNCM";
            TxtNCM.Size = new Size(232, 23);
            TxtNCM.TabIndex = 419;
            // 
            // CBOrgaoAnuente
            // 
            CBOrgaoAnuente.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            CBOrgaoAnuente.FormattingEnabled = true;
            CBOrgaoAnuente.Items.AddRange(new object[] { "MAPA", "DECEX", "ANVISA", "IBAMA", "INMETRO" });
            CBOrgaoAnuente.Location = new Point(610, 27);
            CBOrgaoAnuente.Name = "CBOrgaoAnuente";
            CBOrgaoAnuente.Size = new Size(129, 23);
            CBOrgaoAnuente.TabIndex = 821;
            // 
            // tabControl2
            // 
            tabControl2.Controls.Add(MAPA);
            tabControl2.Controls.Add(ANVISA);
            tabControl2.Controls.Add(DECEX);
            tabControl2.Controls.Add(IBAMA);
            tabControl2.Controls.Add(IMETRO);
            tabControl2.Location = new Point(3, 3);
            tabControl2.Name = "tabControl2";
            tabControl2.SelectedIndex = 0;
            tabControl2.Size = new Size(601, 137);
            tabControl2.TabIndex = 822;
            // 
            // MAPA
            // 
            MAPA.Controls.Add(lpcoEditControl1);
            MAPA.Controls.Add(label28);
            MAPA.Controls.Add(dateTimePicker3);
            MAPA.Controls.Add(label29);
            MAPA.Controls.Add(dateTimePicker2);
            MAPA.Location = new Point(4, 24);
            MAPA.Name = "MAPA";
            MAPA.Size = new Size(593, 109);
            MAPA.TabIndex = 0;
            MAPA.Text = "MAPA";
            MAPA.UseVisualStyleBackColor = true;
            // 
            // lpcoEditControl1
            // 
            lpcoEditControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            lpcoEditControl1.Location = new Point(0, 0);
            lpcoEditControl1.Name = "lpcoEditControl1";
            lpcoEditControl1.Size = new Size(593, 109);
            lpcoEditControl1.TabIndex = 426;
            // 
            // label28
            // 
            label28.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label28.AutoSize = true;
            label28.Font = new Font("Segoe UI", 11F);
            label28.Location = new Point(2642, 30);
            label28.Name = "label28";
            label28.Size = new Size(129, 20);
            label28.TabIndex = 425;
            label28.Text = "Data Deferimento";
            // 
            // dateTimePicker3
            // 
            dateTimePicker3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            dateTimePicker3.Format = DateTimePickerFormat.Short;
            dateTimePicker3.Location = new Point(2639, 54);
            dateTimePicker3.Name = "dateTimePicker3";
            dateTimePicker3.Size = new Size(135, 23);
            dateTimePicker3.TabIndex = 424;
            // 
            // label29
            // 
            label29.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label29.AutoSize = true;
            label29.Font = new Font("Segoe UI", 11F);
            label29.Location = new Point(2507, 30);
            label29.Name = "label29";
            label29.Size = new Size(100, 20);
            label29.TabIndex = 423;
            label29.Text = "Data Registro";
            // 
            // dateTimePicker2
            // 
            dateTimePicker2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            dateTimePicker2.Format = DateTimePickerFormat.Short;
            dateTimePicker2.Location = new Point(2490, 54);
            dateTimePicker2.Name = "dateTimePicker2";
            dateTimePicker2.Size = new Size(135, 23);
            dateTimePicker2.TabIndex = 422;
            // 
            // ANVISA
            // 
            ANVISA.Location = new Point(4, 24);
            ANVISA.Name = "ANVISA";
            ANVISA.Size = new Size(593, 109);
            ANVISA.TabIndex = 1;
            ANVISA.Text = "ANVISA";
            ANVISA.UseVisualStyleBackColor = true;
            // 
            // DECEX
            // 
            DECEX.Location = new Point(4, 24);
            DECEX.Name = "DECEX";
            DECEX.Size = new Size(593, 109);
            DECEX.TabIndex = 2;
            DECEX.Text = "DECEX";
            DECEX.UseVisualStyleBackColor = true;
            // 
            // IBAMA
            // 
            IBAMA.Location = new Point(4, 24);
            IBAMA.Name = "IBAMA";
            IBAMA.Size = new Size(593, 109);
            IBAMA.TabIndex = 3;
            IBAMA.Text = "IBAMA";
            IBAMA.UseVisualStyleBackColor = true;
            // 
            // IMETRO
            // 
            IMETRO.Location = new Point(4, 24);
            IMETRO.Name = "IMETRO";
            IMETRO.Size = new Size(593, 109);
            IMETRO.TabIndex = 4;
            IMETRO.Text = "IMETRO";
            IMETRO.UseVisualStyleBackColor = true;
            // 
            // LIEditControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(btnExcluirLpco);
            Controls.Add(BtnNovoOrgaoAnuente);
            Controls.Add(groupBox5);
            Controls.Add(CBOrgaoAnuente);
            Controls.Add(tabControl2);
            Name = "LIEditControl";
            Size = new Size(746, 230);
            groupBox5.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            tabControl2.ResumeLayout(false);
            MAPA.ResumeLayout(false);
            MAPA.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Button btnExcluirLpco;
        private Button BtnNovoOrgaoAnuente;
        private GroupBox groupBox5;
        private Label label39;
        private DateTimePicker DtpDataRegistro;
        private Label label40;
        private Label label41;
        private TextBox TxtNCM;
        private TextBox TxtLi;
        private ComboBox CBOrgaoAnuente;
        private TabControl tabControl2;
        private TabPage MAPA;
        private LpcoEditControl lpcoEditControl1;
        private Label label28;
        private DateTimePicker dateTimePicker3;
        private Label label29;
        private DateTimePicker dateTimePicker2;
        private TabPage ANVISA;
        private TabPage DECEX;
        private TabPage IBAMA;
        private TabPage IMETRO;
        private TableLayoutPanel tableLayoutPanel1;
    }
}
