namespace Trabalho
{
    partial class FrmAddNotifUrgente
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
            comboBoxDestino = new ComboBox();
            label1 = new Label();
            label2 = new Label();
            txtMensagem = new TextBox();
            BtnEnviar = new Button();
            SuspendLayout();
            // 
            // comboBoxDestino
            // 
            comboBoxDestino.FormattingEnabled = true;
            comboBoxDestino.Location = new Point(32, 41);
            comboBoxDestino.Name = "comboBoxDestino";
            comboBoxDestino.Size = new Size(143, 23);
            comboBoxDestino.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Underline);
            label1.Location = new Point(59, 23);
            label1.Name = "label1";
            label1.Size = new Size(89, 15);
            label1.TabIndex = 1;
            label1.Text = "Usuário destino";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Underline);
            label2.Location = new Point(232, 23);
            label2.Name = "label2";
            label2.Size = new Size(66, 15);
            label2.TabIndex = 2;
            label2.Text = "Mensagem";
            // 
            // txtMensagem
            // 
            txtMensagem.Location = new Point(213, 41);
            txtMensagem.Name = "txtMensagem";
            txtMensagem.Size = new Size(546, 23);
            txtMensagem.TabIndex = 3;
            // 
            // BtnEnviar
            // 
            BtnEnviar.Location = new Point(363, 70);
            BtnEnviar.Name = "BtnEnviar";
            BtnEnviar.Size = new Size(75, 23);
            BtnEnviar.TabIndex = 4;
            BtnEnviar.Text = "Enviar";
            BtnEnviar.UseVisualStyleBackColor = true;
            BtnEnviar.Click += BtnEnviar_Click;
            // 
            // FrmAddNotifUrgente
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 104);
            Controls.Add(BtnEnviar);
            Controls.Add(txtMensagem);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(comboBoxDestino);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FrmAddNotifUrgente";
            Text = "Nova mensagem";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox comboBoxDestino;
        private Label label1;
        private Label label2;
        private TextBox txtMensagem;
        private Button BtnEnviar;
    }
}