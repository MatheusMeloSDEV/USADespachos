namespace Trabalho
{
    partial class frmModificaRecibo
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
            label1 = new Label();
            txtImportador = new TextBox();
            label4 = new Label();
            txtS_Ref = new TextBox();
            label3 = new Label();
            txtN_Ref = new TextBox();
            label2 = new Label();
            btnSalvar = new Button();
            txtExportador = new TextBox();
            label5 = new Label();
            txtMercadoria = new TextBox();
            label6 = new Label();
            txtNavio = new TextBox();
            label7 = new Label();
            groupBox1 = new GroupBox();
            txtTotal = new TextBox();
            label8 = new Label();
            txtExpediente = new TextBox();
            label64 = new Label();
            label65 = new Label();
            txtHonorariosDespachante = new TextBox();
            label62 = new Label();
            label63 = new Label();
            txtEmissaoLicenciamento = new TextBox();
            label60 = new Label();
            label61 = new Label();
            btnCalcular = new Button();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Bold | FontStyle.Underline);
            label1.Location = new Point(39, 42);
            label1.Name = "label1";
            label1.Size = new Size(62, 21);
            label1.TabIndex = 6;
            label1.Text = "Recibo";
            // 
            // txtImportador
            // 
            txtImportador.Location = new Point(557, 20);
            txtImportador.Name = "txtImportador";
            txtImportador.ReadOnly = true;
            txtImportador.Size = new Size(109, 23);
            txtImportador.TabIndex = 13;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(484, 23);
            label4.Name = "label4";
            label4.Size = new Size(67, 15);
            label4.TabIndex = 17;
            label4.Text = "Importador";
            // 
            // txtS_Ref
            // 
            txtS_Ref.Location = new Point(354, 20);
            txtS_Ref.Name = "txtS_Ref";
            txtS_Ref.ReadOnly = true;
            txtS_Ref.Size = new Size(78, 23);
            txtS_Ref.TabIndex = 12;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(310, 23);
            label3.Name = "label3";
            label3.Size = new Size(35, 15);
            label3.TabIndex = 16;
            label3.Text = "S/Ref";
            // 
            // txtN_Ref
            // 
            txtN_Ref.Location = new Point(179, 20);
            txtN_Ref.Name = "txtN_Ref";
            txtN_Ref.ReadOnly = true;
            txtN_Ref.Size = new Size(78, 23);
            txtN_Ref.TabIndex = 11;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(135, 23);
            label2.Name = "label2";
            label2.Size = new Size(38, 15);
            label2.TabIndex = 15;
            label2.Text = "N/Ref";
            // 
            // btnSalvar
            // 
            btnSalvar.Enabled = false;
            btnSalvar.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnSalvar.Location = new Point(697, 23);
            btnSalvar.Name = "btnSalvar";
            btnSalvar.Size = new Size(74, 27);
            btnSalvar.TabIndex = 14;
            btnSalvar.Text = "Salvar";
            btnSalvar.UseVisualStyleBackColor = true;
            btnSalvar.Click += btnSalvar_Click;
            // 
            // txtExportador
            // 
            txtExportador.Location = new Point(528, 65);
            txtExportador.Name = "txtExportador";
            txtExportador.ReadOnly = true;
            txtExportador.Size = new Size(109, 23);
            txtExportador.TabIndex = 20;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(458, 68);
            label5.Name = "label5";
            label5.Size = new Size(64, 15);
            label5.TabIndex = 23;
            label5.Text = "Exportador";
            // 
            // txtMercadoria
            // 
            txtMercadoria.Location = new Point(368, 65);
            txtMercadoria.Name = "txtMercadoria";
            txtMercadoria.ReadOnly = true;
            txtMercadoria.Size = new Size(78, 23);
            txtMercadoria.TabIndex = 19;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(295, 68);
            label6.Name = "label6";
            label6.Size = new Size(67, 15);
            label6.TabIndex = 22;
            label6.Text = "Mercadoria";
            // 
            // txtNavio
            // 
            txtNavio.Location = new Point(208, 65);
            txtNavio.Name = "txtNavio";
            txtNavio.ReadOnly = true;
            txtNavio.Size = new Size(78, 23);
            txtNavio.TabIndex = 18;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(164, 68);
            label7.Name = "label7";
            label7.Size = new Size(38, 15);
            label7.TabIndex = 21;
            label7.Text = "Navio";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(txtTotal);
            groupBox1.Controls.Add(label8);
            groupBox1.Controls.Add(txtExpediente);
            groupBox1.Controls.Add(label64);
            groupBox1.Controls.Add(label65);
            groupBox1.Controls.Add(txtHonorariosDespachante);
            groupBox1.Controls.Add(label62);
            groupBox1.Controls.Add(label63);
            groupBox1.Controls.Add(txtEmissaoLicenciamento);
            groupBox1.Controls.Add(label60);
            groupBox1.Controls.Add(label61);
            groupBox1.Location = new Point(12, 112);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(776, 96);
            groupBox1.TabIndex = 24;
            groupBox1.TabStop = false;
            groupBox1.Text = "Valores";
            // 
            // txtTotal
            // 
            txtTotal.Location = new Point(628, 50);
            txtTotal.Name = "txtTotal";
            txtTotal.ReadOnly = true;
            txtTotal.Size = new Size(100, 23);
            txtTotal.TabIndex = 84;
            txtTotal.TextChanged += AplicarMascaraMoeda;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(662, 32);
            label8.Name = "label8";
            label8.Size = new Size(33, 15);
            label8.TabIndex = 83;
            label8.Text = "Total";
            // 
            // txtExpediente
            // 
            txtExpediente.Location = new Point(280, 50);
            txtExpediente.Name = "txtExpediente";
            txtExpediente.Size = new Size(100, 23);
            txtExpediente.TabIndex = 82;
            txtExpediente.TextChanged += AplicarMascaraMoeda;
            // 
            // label64
            // 
            label64.AutoSize = true;
            label64.Location = new Point(237, 53);
            label64.Name = "label64";
            label64.Size = new Size(37, 15);
            label64.TabIndex = 75;
            label64.Text = "Preço";
            // 
            // label65
            // 
            label65.AutoSize = true;
            label65.Location = new Point(298, 32);
            label65.Name = "label65";
            label65.Size = new Size(64, 15);
            label65.TabIndex = 81;
            label65.Text = "Expediente";
            // 
            // txtHonorariosDespachante
            // 
            txtHonorariosDespachante.Location = new Point(464, 50);
            txtHonorariosDespachante.Name = "txtHonorariosDespachante";
            txtHonorariosDespachante.Size = new Size(100, 23);
            txtHonorariosDespachante.TabIndex = 80;
            txtHonorariosDespachante.TextChanged += AplicarMascaraMoeda;
            // 
            // label62
            // 
            label62.AutoSize = true;
            label62.Location = new Point(421, 53);
            label62.Name = "label62";
            label62.Size = new Size(37, 15);
            label62.TabIndex = 76;
            label62.Text = "Preço";
            // 
            // label63
            // 
            label63.AutoSize = true;
            label63.Location = new Point(446, 32);
            label63.Name = "label63";
            label63.Size = new Size(137, 15);
            label63.TabIndex = 79;
            label63.Text = "Honorários Despachante";
            // 
            // txtEmissaoLicenciamento
            // 
            txtEmissaoLicenciamento.Location = new Point(78, 50);
            txtEmissaoLicenciamento.Name = "txtEmissaoLicenciamento";
            txtEmissaoLicenciamento.Size = new Size(100, 23);
            txtEmissaoLicenciamento.TabIndex = 74;
            txtEmissaoLicenciamento.TextChanged += AplicarMascaraMoeda;
            // 
            // label60
            // 
            label60.AutoSize = true;
            label60.Location = new Point(35, 53);
            label60.Name = "label60";
            label60.Size = new Size(37, 15);
            label60.TabIndex = 78;
            label60.Text = "Preço";
            // 
            // label61
            // 
            label61.AutoSize = true;
            label61.Location = new Point(55, 32);
            label61.Name = "label61";
            label61.Size = new Size(147, 15);
            label61.TabIndex = 77;
            label61.Text = "Emissão de Licenciamento";
            // 
            // btnCalcular
            // 
            btnCalcular.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnCalcular.Location = new Point(697, 60);
            btnCalcular.Name = "btnCalcular";
            btnCalcular.Size = new Size(74, 27);
            btnCalcular.TabIndex = 25;
            btnCalcular.Text = "Calcular";
            btnCalcular.UseVisualStyleBackColor = true;
            btnCalcular.Click += btnCalcularTotal_Click;
            // 
            // frmModificaRecibo
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 220);
            Controls.Add(btnCalcular);
            Controls.Add(groupBox1);
            Controls.Add(txtExportador);
            Controls.Add(label5);
            Controls.Add(txtMercadoria);
            Controls.Add(label6);
            Controls.Add(txtNavio);
            Controls.Add(label7);
            Controls.Add(txtImportador);
            Controls.Add(label4);
            Controls.Add(txtS_Ref);
            Controls.Add(label3);
            Controls.Add(txtN_Ref);
            Controls.Add(label2);
            Controls.Add(btnSalvar);
            Controls.Add(label1);
            MaximizeBox = false;
            Name = "frmModificaRecibo";
            Text = "Recibo";
            FormClosing += frmModificaRecibo_FormClosing;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox txtImportador;
        private Label label4;
        private TextBox txtS_Ref;
        private Label label3;
        private TextBox txtN_Ref;
        private Label label2;
        private Button btnSalvar;
        private TextBox txtExportador;
        private Label label5;
        private TextBox txtMercadoria;
        private Label label6;
        private TextBox txtNavio;
        private Label label7;
        private GroupBox groupBox1;
        private TextBox txtExpediente;
        private Label label64;
        private Label label65;
        private TextBox txtHonorariosDespachante;
        private Label label62;
        private Label label63;
        private TextBox txtEmissaoLicenciamento;
        private Label label60;
        private Label label61;
        private TextBox txtTotal;
        private Label label8;
        private Button btnCalcular;
    }
}