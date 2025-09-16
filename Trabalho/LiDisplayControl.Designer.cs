namespace Trabalho
{
    partial class LiDisplayControl
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
            lblNumeroLi = new Label();
            lblDataRegistro = new Label();
            lblNcm = new Label();
            SuspendLayout();
            // 
            // lblNumeroLi
            // 
            lblNumeroLi.AutoSize = true;
            lblNumeroLi.Location = new Point(24, 9);
            lblNumeroLi.Name = "lblNumeroLi";
            lblNumeroLi.Size = new Size(38, 15);
            lblNumeroLi.TabIndex = 0;
            lblNumeroLi.Text = "label1";
            // 
            // lblDataRegistro
            // 
            lblDataRegistro.AutoSize = true;
            lblDataRegistro.Location = new Point(183, 9);
            lblDataRegistro.Name = "lblDataRegistro";
            lblDataRegistro.Size = new Size(38, 15);
            lblDataRegistro.TabIndex = 1;
            lblDataRegistro.Text = "label2";
            // 
            // lblNcm
            // 
            lblNcm.AutoSize = true;
            lblNcm.Location = new Point(100, 9);
            lblNcm.Name = "lblNcm";
            lblNcm.Size = new Size(38, 15);
            lblNcm.TabIndex = 2;
            lblNcm.Text = "label3";
            // 
            // LiDisplayControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(323, 35);
            Controls.Add(lblNcm);
            Controls.Add(lblDataRegistro);
            Controls.Add(lblNumeroLi);
            Name = "LiDisplayControl";
            Text = "LiDisplayControl";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblNumeroLi;
        private Label lblDataRegistro;
        private Label lblNcm;
    }
}