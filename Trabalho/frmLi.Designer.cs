namespace Trabalho
{
    partial class frmLi
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblLi;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnRemover;

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
        /// Método necessário para o Designer – não modifique
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            lblLi = new Label();
            btnOK = new Button();
            btnRemover = new Button();
            GBOrgaosAnuentes = new GroupBox();
            cbIbama = new CheckBox();
            cbInmetro = new CheckBox();
            cbDecex = new CheckBox();
            cbAnvisa = new CheckBox();
            cbMapa = new CheckBox();
            TxtLi = new TextBox();
            TxtNCM = new TextBox();
            label1 = new Label();
            dtpDataRegistroLI = new DateTimePicker();
            label2 = new Label();
            GBOrgaosAnuentes.SuspendLayout();
            SuspendLayout();
            // 
            // lblLi
            // 
            lblLi.AutoSize = true;
            lblLi.Location = new Point(12, 20);
            lblLi.Name = "lblLi";
            lblLi.Size = new Size(66, 15);
            lblLi.TabIndex = 0;
            lblLi.Text = "Número LI:";
            // 
            // btnOK
            // 
            btnOK.Location = new Point(195, 159);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(90, 30);
            btnOK.TabIndex = 7;
            btnOK.Text = "Salvar";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            // 
            // btnRemover
            // 
            btnRemover.Location = new Point(305, 159);
            btnRemover.Name = "btnRemover";
            btnRemover.Size = new Size(90, 30);
            btnRemover.TabIndex = 8;
            btnRemover.Text = "Remover";
            btnRemover.UseVisualStyleBackColor = true;
            btnRemover.Click += btnRemover_Click;
            // 
            // GBOrgaosAnuentes
            // 
            GBOrgaosAnuentes.Controls.Add(cbIbama);
            GBOrgaosAnuentes.Controls.Add(cbInmetro);
            GBOrgaosAnuentes.Controls.Add(cbDecex);
            GBOrgaosAnuentes.Controls.Add(cbAnvisa);
            GBOrgaosAnuentes.Controls.Add(cbMapa);
            GBOrgaosAnuentes.Location = new Point(12, 96);
            GBOrgaosAnuentes.Name = "GBOrgaosAnuentes";
            GBOrgaosAnuentes.Size = new Size(383, 56);
            GBOrgaosAnuentes.TabIndex = 20;
            GBOrgaosAnuentes.TabStop = false;
            GBOrgaosAnuentes.Text = "Órgãos Anuentes";
            // 
            // cbIbama
            // 
            cbIbama.AutoSize = true;
            cbIbama.Location = new Point(295, 22);
            cbIbama.Name = "cbIbama";
            cbIbama.Size = new Size(59, 19);
            cbIbama.TabIndex = 4;
            cbIbama.Tag = "IBAMA";
            cbIbama.Text = "Ibama";
            cbIbama.UseVisualStyleBackColor = true;
            cbIbama.CheckedChanged += OrgaoCheckBox_CheckedChanged;
            // 
            // cbInmetro
            // 
            cbInmetro.AutoSize = true;
            cbInmetro.Location = new Point(221, 22);
            cbInmetro.Name = "cbInmetro";
            cbInmetro.Size = new Size(68, 19);
            cbInmetro.TabIndex = 3;
            cbInmetro.Tag = "INMETRO";
            cbInmetro.Text = "Inmetro";
            cbInmetro.UseVisualStyleBackColor = true;
            cbInmetro.CheckedChanged += OrgaoCheckBox_CheckedChanged;
            // 
            // cbDecex
            // 
            cbDecex.AutoSize = true;
            cbDecex.Location = new Point(158, 22);
            cbDecex.Name = "cbDecex";
            cbDecex.Size = new Size(57, 19);
            cbDecex.TabIndex = 2;
            cbDecex.Tag = "DECEX";
            cbDecex.Text = "Decex";
            cbDecex.UseVisualStyleBackColor = true;
            cbDecex.CheckedChanged += OrgaoCheckBox_CheckedChanged;
            // 
            // cbAnvisa
            // 
            cbAnvisa.AutoSize = true;
            cbAnvisa.Location = new Point(91, 22);
            cbAnvisa.Name = "cbAnvisa";
            cbAnvisa.Size = new Size(61, 19);
            cbAnvisa.TabIndex = 1;
            cbAnvisa.Tag = "ANVISA";
            cbAnvisa.Text = "Anvisa";
            cbAnvisa.UseVisualStyleBackColor = true;
            cbAnvisa.CheckedChanged += OrgaoCheckBox_CheckedChanged;
            // 
            // cbMapa
            // 
            cbMapa.AutoSize = true;
            cbMapa.Location = new Point(29, 22);
            cbMapa.Name = "cbMapa";
            cbMapa.Size = new Size(56, 19);
            cbMapa.TabIndex = 0;
            cbMapa.Tag = "MAPA";
            cbMapa.Text = "Mapa";
            cbMapa.UseVisualStyleBackColor = true;
            cbMapa.CheckedChanged += OrgaoCheckBox_CheckedChanged;
            // 
            // TxtLi
            // 
            TxtLi.Location = new Point(84, 17);
            TxtLi.Name = "TxtLi";
            TxtLi.Size = new Size(311, 23);
            TxtLi.TabIndex = 1;
            // 
            // TxtNCM
            // 
            TxtNCM.Location = new Point(205, 67);
            TxtNCM.Name = "TxtNCM";
            TxtNCM.Size = new Size(190, 23);
            TxtNCM.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(258, 49);
            label1.Name = "label1";
            label1.Size = new Size(85, 15);
            label1.TabIndex = 376;
            label1.Text = "Número NCM:";
            // 
            // dtpDataRegistroLI
            // 
            dtpDataRegistroLI.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            dtpDataRegistroLI.Format = DateTimePickerFormat.Short;
            dtpDataRegistroLI.Location = new Point(12, 67);
            dtpDataRegistroLI.MinDate = new DateTime(2024, 10, 17, 0, 0, 0, 0);
            dtpDataRegistroLI.Name = "dtpDataRegistroLI";
            dtpDataRegistroLI.Size = new Size(172, 23);
            dtpDataRegistroLI.TabIndex = 147;
            dtpDataRegistroLI.Value = new DateTime(2024, 10, 17, 0, 0, 0, 0);
            dtpDataRegistroLI.ValueChanged += DateTimePicker_OnValueChanged;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.Location = new Point(46, 49);
            label2.Name = "label2";
            label2.Size = new Size(105, 15);
            label2.TabIndex = 147;
            label2.Text = "Data de Registro LI";
            // 
            // frmLi
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(407, 199);
            Controls.Add(label2);
            Controls.Add(dtpDataRegistroLI);
            Controls.Add(TxtNCM);
            Controls.Add(label1);
            Controls.Add(TxtLi);
            Controls.Add(GBOrgaosAnuentes);
            Controls.Add(btnRemover);
            Controls.Add(btnOK);
            Controls.Add(lblLi);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmLi";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Novo Documento";
            Load += frmLi_Load;
            GBOrgaosAnuentes.ResumeLayout(false);
            GBOrgaosAnuentes.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private GroupBox GBOrgaosAnuentes;
        private CheckBox cbIbama;
        private CheckBox cbInmetro;
        private CheckBox cbDecex;
        private CheckBox cbAnvisa;
        private CheckBox cbMapa;
        private TextBox TxtLi;
        private TextBox TxtNCM;
        private Label label1;
        private DateTimePicker dtpDataRegistroLI;
        private Label label2;
    }
}
