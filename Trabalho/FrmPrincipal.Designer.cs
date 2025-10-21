namespace Trabalho
{
    partial class FrmPrincipal
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmPrincipal));
            menuStrip1 = new MenuStrip();
            MenuItemNotifications = new ToolStripMenuItem();
            MenuItemHome = new ToolStripMenuItem();
            MenuItemEmAndamento = new ToolStripMenuItem();
            MenuItemMaximizar = new ToolStripMenuItem();
            MenuItemMinimizar = new ToolStripMenuItem();
            MenuItemExit = new ToolStripMenuItem();
            planilhasToolStripMenuItem = new ToolStripMenuItem();
            santosToolStripMenuItem = new ToolStripMenuItem();
            MenuItemProcessoSantos = new ToolStripMenuItem();
            MenuItemOrgaoAnuente = new ToolStripMenuItem();
            itajaíToolStripMenuItem = new ToolStripMenuItem();
            MenuItemProcessosItajai = new ToolStripMenuItem();
            MenuItemOrgaoAnuente1 = new ToolStripMenuItem();
            MenuItemAdmin = new ToolStripMenuItem();
            MenuItemVistoria = new ToolStripMenuItem();
            MenuitemFinanceiro = new ToolStripMenuItem();
            timerReleaseExit = new System.Windows.Forms.Timer(components);
            pictureBox1 = new PictureBox();
            panel1 = new Panel();
            menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { MenuItemNotifications, MenuItemHome, planilhasToolStripMenuItem, MenuItemAdmin, MenuItemVistoria, MenuitemFinanceiro });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1264, 24);
            menuStrip1.TabIndex = 1;
            menuStrip1.Text = "menuStrip1";
            // 
            // MenuItemNotifications
            // 
            MenuItemNotifications.Image = (Image)resources.GetObject("MenuItemNotifications.Image");
            MenuItemNotifications.Name = "MenuItemNotifications";
            MenuItemNotifications.Size = new Size(101, 20);
            MenuItemNotifications.Text = "Notificações";
            // 
            // MenuItemHome
            // 
            MenuItemHome.DropDownItems.AddRange(new ToolStripItem[] { MenuItemEmAndamento, MenuItemMaximizar, MenuItemMinimizar, MenuItemExit });
            MenuItemHome.Name = "MenuItemHome";
            MenuItemHome.Size = new Size(50, 20);
            MenuItemHome.Text = "Menu";
            MenuItemHome.DoubleClick += MenuItemHome_DoubleClick;
            // 
            // MenuItemEmAndamento
            // 
            MenuItemEmAndamento.Name = "MenuItemEmAndamento";
            MenuItemEmAndamento.Size = new Size(210, 22);
            MenuItemEmAndamento.Text = "Processos em andamento";
            // 
            // MenuItemMaximizar
            // 
            MenuItemMaximizar.Name = "MenuItemMaximizar";
            MenuItemMaximizar.Size = new Size(210, 22);
            MenuItemMaximizar.Text = "Maximizar";
            MenuItemMaximizar.Click += MenuItemMaximize_Click;
            // 
            // MenuItemMinimizar
            // 
            MenuItemMinimizar.Name = "MenuItemMinimizar";
            MenuItemMinimizar.Size = new Size(210, 22);
            MenuItemMinimizar.Text = "Minimizar";
            MenuItemMinimizar.Click += MenuItemMinimize_Click;
            // 
            // MenuItemExit
            // 
            MenuItemExit.Name = "MenuItemExit";
            MenuItemExit.Size = new Size(210, 22);
            MenuItemExit.Text = "Sair";
            MenuItemExit.Click += MenuItemExit_Click;
            // 
            // planilhasToolStripMenuItem
            // 
            planilhasToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { santosToolStripMenuItem, itajaíToolStripMenuItem });
            planilhasToolStripMenuItem.Name = "planilhasToolStripMenuItem";
            planilhasToolStripMenuItem.Size = new Size(66, 20);
            planilhasToolStripMenuItem.Text = "Planilhas";
            // 
            // santosToolStripMenuItem
            // 
            santosToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { MenuItemProcessoSantos, MenuItemOrgaoAnuente });
            santosToolStripMenuItem.Name = "santosToolStripMenuItem";
            santosToolStripMenuItem.Size = new Size(109, 22);
            santosToolStripMenuItem.Text = "Santos";
            // 
            // MenuItemProcessoSantos
            // 
            MenuItemProcessoSantos.Name = "MenuItemProcessoSantos";
            MenuItemProcessoSantos.Size = new Size(155, 22);
            MenuItemProcessoSantos.Text = "Processos";
            MenuItemProcessoSantos.Click += MenuItemProcessoSantos_Click;
            // 
            // MenuItemOrgaoAnuente
            // 
            MenuItemOrgaoAnuente.Name = "MenuItemOrgaoAnuente";
            MenuItemOrgaoAnuente.Size = new Size(155, 22);
            MenuItemOrgaoAnuente.Text = "Orgão Anuente";
            MenuItemOrgaoAnuente.Click += MenuItemOrgaoAnuente_Click;
            // 
            // itajaíToolStripMenuItem
            // 
            itajaíToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { MenuItemProcessosItajai, MenuItemOrgaoAnuente1 });
            itajaíToolStripMenuItem.Name = "itajaíToolStripMenuItem";
            itajaíToolStripMenuItem.Size = new Size(109, 22);
            itajaíToolStripMenuItem.Text = "Itajaí";
            // 
            // MenuItemProcessosItajai
            // 
            MenuItemProcessosItajai.Name = "MenuItemProcessosItajai";
            MenuItemProcessosItajai.Size = new Size(155, 22);
            MenuItemProcessosItajai.Text = "Processos";
            MenuItemProcessosItajai.Click += MenuItemProcessosItajai_Click;
            // 
            // MenuItemOrgaoAnuente1
            // 
            MenuItemOrgaoAnuente1.Name = "MenuItemOrgaoAnuente1";
            MenuItemOrgaoAnuente1.Size = new Size(155, 22);
            MenuItemOrgaoAnuente1.Text = "Orgão Anuente";
            MenuItemOrgaoAnuente1.Click += MenuItemOrgaoAnuente_Click;
            // 
            // MenuItemAdmin
            // 
            MenuItemAdmin.Alignment = ToolStripItemAlignment.Right;
            MenuItemAdmin.Name = "MenuItemAdmin";
            MenuItemAdmin.Size = new Size(95, 20);
            MenuItemAdmin.Text = "Administrador";
            MenuItemAdmin.Click += MenuItemAdmin_Click;
            // 
            // MenuItemVistoria
            // 
            MenuItemVistoria.Name = "MenuItemVistoria";
            MenuItemVistoria.Size = new Size(58, 20);
            MenuItemVistoria.Text = "Vistoria";
            MenuItemVistoria.Click += MenuItemVistoria_Click;
            // 
            // MenuitemFinanceiro
            // 
            MenuitemFinanceiro.Name = "MenuitemFinanceiro";
            MenuitemFinanceiro.Size = new Size(74, 20);
            MenuitemFinanceiro.Text = "Financeiro";
            MenuitemFinanceiro.Click += MenuItemFinanceiro_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pictureBox1.BackColor = Color.Transparent;
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(382, 90);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(500, 500);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 3;
            pictureBox1.TabStop = false;
            // 
            // panel1
            // 
            panel1.BackColor = Color.Transparent;
            panel1.Controls.Add(pictureBox1);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(1264, 681);
            panel1.TabIndex = 5;
            // 
            // FrmPrincipal
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1264, 681);
            Controls.Add(menuStrip1);
            Controls.Add(panel1);
            IsMdiContainer = true;
            MainMenuStrip = menuStrip1;
            Name = "FrmPrincipal";
            Text = "Processos em Andamento";
            WindowState = FormWindowState.Maximized;
            FormClosing += FrmPrincipal_FormClosing;
            Load += FrmPrincipal_Load;
            Shown += FrmPrincipal_Shown;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            panel1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem MenuItemNotifications;
        private ToolStripMenuItem MenuItemHome;
        private ToolStripMenuItem planilhasToolStripMenuItem;
        private ToolStripMenuItem santosToolStripMenuItem;
        private ToolStripMenuItem MenuItemProcessoSantos;
        private ToolStripMenuItem MenuItemOrgaoAnuente;
        private ToolStripMenuItem itajaíToolStripMenuItem;
        private ToolStripMenuItem MenuItemProcessosItajai;
        private ToolStripMenuItem MenuItemOrgaoAnuente1;
        private ToolStripMenuItem MenuItemAdmin;
        private ToolStripMenuItem MenuitemFinanceiro;
        private ToolStripMenuItem MenuItemMaximizar;
        private ToolStripMenuItem MenuItemMinimizar;
        private ToolStripMenuItem MenuItemExit;
        private ToolStripMenuItem MenuItemEmAndamento;
        private System.Windows.Forms.Timer timerReleaseExit;
        private ToolStripMenuItem MenuItemVistoria;
        private PictureBox pictureBox1;
        private Panel panel1;
    }
}