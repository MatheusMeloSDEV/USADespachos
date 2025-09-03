namespace Trabalho
{
    partial class frmLpcoDetalhes
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
            GroupBox3 = new GroupBox();
            Label22 = new Label();
            Label20 = new Label();
            Label18 = new Label();
            cmbParametrizacao = new ComboBox();
            dtpDataDeferimento = new DateTimePicker();
            dtpDataRegistro = new DateTimePicker();
            txtLpco = new TextBox();
            Label26 = new Label();
            button1 = new Button();
            GroupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // GroupBox3
            // 
            GroupBox3.Controls.Add(Label22);
            GroupBox3.Controls.Add(Label20);
            GroupBox3.Controls.Add(Label18);
            GroupBox3.Controls.Add(cmbParametrizacao);
            GroupBox3.Controls.Add(dtpDataDeferimento);
            GroupBox3.Controls.Add(dtpDataRegistro);
            GroupBox3.Controls.Add(txtLpco);
            GroupBox3.Controls.Add(Label26);
            GroupBox3.Location = new Point(12, 12);
            GroupBox3.Name = "GroupBox3";
            GroupBox3.Size = new Size(383, 118);
            GroupBox3.TabIndex = 4;
            GroupBox3.TabStop = false;
            GroupBox3.Text = "LPCO";
            // 
            // Label22
            // 
            Label22.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Label22.AutoSize = true;
            Label22.Font = new Font("Segoe UI", 8.25F);
            Label22.Location = new Point(56, 64);
            Label22.Name = "Label22";
            Label22.Size = new Size(84, 13);
            Label22.TabIndex = 146;
            Label22.Text = "Parametrização";
            // 
            // Label20
            // 
            Label20.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Label20.AutoSize = true;
            Label20.Font = new Font("Segoe UI", 8.25F);
            Label20.Location = new Point(217, 64);
            Label20.Name = "Label20";
            Label20.Size = new Size(114, 13);
            Label20.TabIndex = 145;
            Label20.Text = "Data de Deferimento";
            // 
            // Label18
            // 
            Label18.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Label18.AutoSize = true;
            Label18.Font = new Font("Segoe UI", 8.25F);
            Label18.Location = new Point(225, 19);
            Label18.Name = "Label18";
            Label18.Size = new Size(93, 13);
            Label18.TabIndex = 145;
            Label18.Text = "Data de Registro";
            // 
            // cmbParametrizacao
            // 
            cmbParametrizacao.FormattingEnabled = true;
            cmbParametrizacao.Items.AddRange(new object[] { "Documental", "Conferência Física", "Exame Físico", "Coleta de Amostra" });
            cmbParametrizacao.Location = new Point(34, 80);
            cmbParametrizacao.Name = "cmbParametrizacao";
            cmbParametrizacao.Size = new Size(128, 23);
            cmbParametrizacao.TabIndex = 4;
            // 
            // dtpDataDeferimento
            // 
            dtpDataDeferimento.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            dtpDataDeferimento.Format = DateTimePickerFormat.Short;
            dtpDataDeferimento.Location = new Point(193, 80);
            dtpDataDeferimento.MinDate = new DateTime(2024, 10, 17, 0, 0, 0, 0);
            dtpDataDeferimento.Name = "dtpDataDeferimento";
            dtpDataDeferimento.Size = new Size(163, 23);
            dtpDataDeferimento.TabIndex = 6;
            dtpDataDeferimento.Value = new DateTime(2024, 10, 17, 0, 0, 0, 0);
            // 
            // dtpDataRegistro
            // 
            dtpDataRegistro.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            dtpDataRegistro.Format = DateTimePickerFormat.Short;
            dtpDataRegistro.Location = new Point(190, 35);
            dtpDataRegistro.MinDate = new DateTime(2024, 10, 17, 0, 0, 0, 0);
            dtpDataRegistro.Name = "dtpDataRegistro";
            dtpDataRegistro.Size = new Size(163, 23);
            dtpDataRegistro.TabIndex = 5;
            dtpDataRegistro.Value = new DateTime(2024, 10, 17, 0, 0, 0, 0);
            // 
            // txtLpco
            // 
            txtLpco.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            txtLpco.Location = new Point(34, 35);
            txtLpco.Name = "txtLpco";
            txtLpco.Size = new Size(128, 23);
            txtLpco.TabIndex = 3;
            // 
            // Label26
            // 
            Label26.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Label26.AutoSize = true;
            Label26.Font = new Font("Segoe UI", 8.25F);
            Label26.Location = new Point(89, 19);
            Label26.Name = "Label26";
            Label26.Size = new Size(19, 13);
            Label26.TabIndex = 85;
            Label26.Text = "N°";
            // 
            // button1
            // 
            button1.Location = new Point(319, 136);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 5;
            button1.Text = "Salvar";
            button1.UseVisualStyleBackColor = true;
            button1.Click += btnOK_Click;
            // 
            // frmLpcoDetalhes
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(406, 171);
            Controls.Add(button1);
            Controls.Add(GroupBox3);
            Name = "frmLpcoDetalhes";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "frmLpcoDetalhes";
            FormClosing += frmLpcoDetalhes_FormClosing;
            Load += frmLpcoDetalhes_Load;
            GroupBox3.ResumeLayout(false);
            GroupBox3.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox GroupBox3;
        private Label Label22;
        private Label Label20;
        private Label Label18;
        private ComboBox cmbParametrizacao;
        private DateTimePicker dtpDataDeferimento;
        private DateTimePicker dtpDataRegistro;
        private TextBox txtLpco;
        private Label Label26;
        private Button button1;
    }
}