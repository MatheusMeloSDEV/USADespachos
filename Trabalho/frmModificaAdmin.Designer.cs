namespace Trabalho
{
    partial class FrmModificaAdmin 
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmModificaAdmin));
            BsModificaAdmin = new BindingSource(components);
            btnEnviar = new Button();
            txtPassword = new TextBox();
            txtUsername = new TextBox();
            label1 = new Label();
            label2 = new Label();
            cbAdmin = new CheckBox();
            tErro = new System.Windows.Forms.Timer(components);
            ((System.ComponentModel.ISupportInitialize)BsModificaAdmin).BeginInit();
            SuspendLayout();
            // 
            // BsModificaAdmin
            // 
            BsModificaAdmin.DataSource = typeof(CLUSA.Models.Users);
            // 
            // btnEnviar
            // 
            resources.ApplyResources(btnEnviar, "btnEnviar");
            btnEnviar.Name = "btnEnviar";
            btnEnviar.UseVisualStyleBackColor = true;
            btnEnviar.Click += BtnEnviar_Click;
            // 
            // txtPassword
            // 
            txtPassword.BackColor = Color.White;
            txtPassword.Cursor = Cursors.IBeam;
            txtPassword.DataBindings.Add(new Binding("Text", BsModificaAdmin, "Password", true));
            txtPassword.ForeColor = SystemColors.Window;
            resources.ApplyResources(txtPassword, "txtPassword");
            txtPassword.Name = "txtPassword";
            // 
            // txtUsername
            // 
            txtUsername.BackColor = Color.White;
            txtUsername.Cursor = Cursors.IBeam;
            txtUsername.DataBindings.Add(new Binding("Text", BsModificaAdmin, "Username", true));
            resources.ApplyResources(txtUsername, "txtUsername");
            txtUsername.Name = "txtUsername";
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(label2, "label2");
            label2.Name = "label2";
            // 
            // cbAdmin
            // 
            resources.ApplyResources(cbAdmin, "cbAdmin");
            cbAdmin.Cursor = Cursors.Hand;
            cbAdmin.DataBindings.Add(new Binding("Checked", BsModificaAdmin, "Admin", true));
            cbAdmin.Name = "cbAdmin";
            cbAdmin.UseVisualStyleBackColor = true;
            // 
            // tErro
            // 
            // 
            // frmModificaAdmin
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(cbAdmin);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(txtUsername);
            Controls.Add(txtPassword);
            Controls.Add(btnEnviar);
            Name = "frmModificaAdmin";
            Load += FrmModificaAdmin_Load;
            ((System.ComponentModel.ISupportInitialize)BsModificaAdmin).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private BindingSource BsModificaAdmin;
        private Button btnEnviar;
        private TextBox txtPassword;
        private TextBox txtUsername;
        private Label label1;
        private Label label2;
        private CheckBox cbAdmin;
        private System.Windows.Forms.Timer tErro;
    }
}