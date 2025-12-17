namespace Trabalho
{
    partial class FrmModificaVencimentos
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
            btnEnviar = new Button();
            dtpRadar = new DateTimePicker();
            cbImportador = new ComboBox();
            chkRadar = new CheckBox();
            chkProcuracao = new CheckBox();
            dtpProcuracao = new DateTimePicker();
            chkSigvig = new CheckBox();
            dtpSigvig = new DateTimePicker();
            chkEcac = new CheckBox();
            dtpEcac = new DateTimePicker();
            label1 = new Label();
            chkLecom = new CheckBox();
            dtpLecom = new DateTimePicker();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            SuspendLayout();
            // 
            // btnEnviar
            // 
            btnEnviar.Location = new Point(354, 12);
            btnEnviar.Name = "btnEnviar";
            btnEnviar.Size = new Size(75, 23);
            btnEnviar.TabIndex = 3;
            btnEnviar.Text = "Enviar";
            btnEnviar.UseVisualStyleBackColor = true;
            btnEnviar.Click += btnEnviar_Click;
            // 
            // dtpRadar
            // 
            dtpRadar.Format = DateTimePickerFormat.Short;
            dtpRadar.Location = new Point(42, 58);
            dtpRadar.Name = "dtpRadar";
            dtpRadar.Size = new Size(177, 23);
            dtpRadar.TabIndex = 4;
            // 
            // cbImportador
            // 
            cbImportador.FormattingEnabled = true;
            cbImportador.Items.AddRange(new object[] { "ACCIO", "ALICE ALIMENTOS", "AURORA", "BRASCOD", "CASA FLORA", "COPY DATA", "DAMPER", "ELTO COMERCIAL", "FMG", "FREEWAY", "FRUGAL", "KUKAMAR", "LEITESOL", "LIBRA", "MARHUA", "MARCOL", "MARNOBRE", "MGA", "NOR IMPORT", "REBELA", "SEIKO", "VANUCCI", "VILA SIMPATIA", "ZARAGOZA" });
            cbImportador.Location = new Point(135, 12);
            cbImportador.Name = "cbImportador";
            cbImportador.Size = new Size(197, 23);
            cbImportador.TabIndex = 5;
            cbImportador.Text = "Nome do Importador";
            // 
            // chkRadar
            // 
            chkRadar.Location = new Point(22, 58);
            chkRadar.Name = "chkRadar";
            chkRadar.Size = new Size(14, 23);
            chkRadar.TabIndex = 7;
            chkRadar.UseVisualStyleBackColor = true;
            // 
            // chkProcuracao
            // 
            chkProcuracao.Location = new Point(248, 58);
            chkProcuracao.Name = "chkProcuracao";
            chkProcuracao.Size = new Size(14, 23);
            chkProcuracao.TabIndex = 9;
            chkProcuracao.UseVisualStyleBackColor = true;
            // 
            // dtpProcuracao
            // 
            dtpProcuracao.Format = DateTimePickerFormat.Short;
            dtpProcuracao.Location = new Point(268, 58);
            dtpProcuracao.Name = "dtpProcuracao";
            dtpProcuracao.Size = new Size(177, 23);
            dtpProcuracao.TabIndex = 8;
            // 
            // chkSigvig
            // 
            chkSigvig.Location = new Point(248, 103);
            chkSigvig.Name = "chkSigvig";
            chkSigvig.Size = new Size(14, 23);
            chkSigvig.TabIndex = 13;
            chkSigvig.UseVisualStyleBackColor = true;
            // 
            // dtpSigvig
            // 
            dtpSigvig.Format = DateTimePickerFormat.Short;
            dtpSigvig.Location = new Point(268, 103);
            dtpSigvig.Name = "dtpSigvig";
            dtpSigvig.Size = new Size(177, 23);
            dtpSigvig.TabIndex = 12;
            // 
            // chkEcac
            // 
            chkEcac.Location = new Point(22, 103);
            chkEcac.Name = "chkEcac";
            chkEcac.Size = new Size(14, 23);
            chkEcac.TabIndex = 11;
            chkEcac.UseVisualStyleBackColor = true;
            // 
            // dtpEcac
            // 
            dtpEcac.Format = DateTimePickerFormat.Short;
            dtpEcac.Location = new Point(42, 103);
            dtpEcac.Name = "dtpEcac";
            dtpEcac.Size = new Size(177, 23);
            dtpEcac.TabIndex = 10;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(63, 40);
            label1.Name = "label1";
            label1.Size = new Size(103, 15);
            label1.TabIndex = 14;
            label1.Text = "Vencimento Radar";
            // 
            // chkLecom
            // 
            chkLecom.Location = new Point(126, 149);
            chkLecom.Name = "chkLecom";
            chkLecom.Size = new Size(14, 23);
            chkLecom.TabIndex = 18;
            chkLecom.UseVisualStyleBackColor = true;
            // 
            // dtpLecom
            // 
            dtpLecom.Format = DateTimePickerFormat.Short;
            dtpLecom.Location = new Point(146, 149);
            dtpLecom.Name = "dtpLecom";
            dtpLecom.Size = new Size(177, 23);
            dtpLecom.TabIndex = 17;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(300, 40);
            label2.Name = "label2";
            label2.Size = new Size(133, 15);
            label2.TabIndex = 19;
            label2.Text = "Vencimento Procuração";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(300, 85);
            label3.Name = "label3";
            label3.Size = new Size(108, 15);
            label3.TabIndex = 20;
            label3.Text = "Vencimento SIGVIG";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(178, 131);
            label4.Name = "label4";
            label4.Size = new Size(113, 15);
            label4.TabIndex = 21;
            label4.Text = "Vencimento LECOM";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(63, 85);
            label5.Name = "label5";
            label5.Size = new Size(103, 15);
            label5.TabIndex = 22;
            label5.Text = "Vencimento ECAC";
            // 
            // FrmModificaVencimentos
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(469, 184);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(chkLecom);
            Controls.Add(dtpLecom);
            Controls.Add(label1);
            Controls.Add(chkSigvig);
            Controls.Add(dtpSigvig);
            Controls.Add(chkEcac);
            Controls.Add(dtpEcac);
            Controls.Add(chkProcuracao);
            Controls.Add(dtpProcuracao);
            Controls.Add(chkRadar);
            Controls.Add(cbImportador);
            Controls.Add(dtpRadar);
            Controls.Add(btnEnviar);
            Name = "FrmModificaVencimentos";
            Text = "Edita/Adiciona Vencimento";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button btnEnviar;
        private DateTimePicker dtpRadar;
        private ComboBox cbImportador;
        private CheckBox chkRadar;
        private CheckBox chkProcuracao;
        private DateTimePicker dtpProcuracao;
        private CheckBox chkSigvig;
        private DateTimePicker dtpSigvig;
        private CheckBox chkEcac;
        private DateTimePicker dtpEcac;
        private Label label1;
        private CheckBox chkLecom;
        private DateTimePicker dtpLecom;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
    }
}