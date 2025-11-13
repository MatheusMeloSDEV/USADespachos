namespace Trabalho
{
    partial class FrmMudarSenha
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
            BtnSalvar = new Button();
            TxtOldPassword = new TextBox();
            TxtNewPassword = new TextBox();
            SuspendLayout();
            // 
            // BtnSalvar
            // 
            BtnSalvar.Location = new Point(292, 12);
            BtnSalvar.Name = "BtnSalvar";
            BtnSalvar.Size = new Size(111, 23);
            BtnSalvar.TabIndex = 0;
            BtnSalvar.Text = "Salvar";
            BtnSalvar.UseVisualStyleBackColor = true;
            BtnSalvar.Click += BtnSalvar_Click;
            // 
            // TxtOldPassword
            // 
            TxtOldPassword.Location = new Point(12, 12);
            TxtOldPassword.Name = "TxtOldPassword";
            TxtOldPassword.PlaceholderText = "Senha atual";
            TxtOldPassword.Size = new Size(111, 23);
            TxtOldPassword.TabIndex = 1;
            // 
            // TxtNewPassword
            // 
            TxtNewPassword.Location = new Point(152, 12);
            TxtNewPassword.Name = "TxtNewPassword";
            TxtNewPassword.PlaceholderText = "Nova senha";
            TxtNewPassword.Size = new Size(111, 23);
            TxtNewPassword.TabIndex = 2;
            // 
            // FrmMudarSenha
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(421, 47);
            Controls.Add(TxtNewPassword);
            Controls.Add(TxtOldPassword);
            Controls.Add(BtnSalvar);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FrmMudarSenha";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Mudar senha";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button BtnSalvar;
        private TextBox TxtOldPassword;
        private TextBox TxtNewPassword;
    }
}