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
            Menu = new MenuStrip();
            MenuItemHome = new ToolStripMenuItem();
            MenuItemMenu = new ToolStripMenuItem();
            MenuItemEmAndamento = new ToolStripMenuItem();
            MenuItemMaximizar = new ToolStripMenuItem();
            MenuItemMinimizar = new ToolStripMenuItem();
            MenuItemExit = new ToolStripMenuItem();
            MenuItemNotifications = new ToolStripMenuItem();
            planilhasToolStripMenuItem = new ToolStripMenuItem();
            santosToolStripMenuItem = new ToolStripMenuItem();
            MenuItemProcessoSantos = new ToolStripMenuItem();
            MenuItemOrgaoAnuente = new ToolStripMenuItem();
            itajaíToolStripMenuItem = new ToolStripMenuItem();
            MenuItemProcessosItajai = new ToolStripMenuItem();
            MenuItemOrgaoAnuente1 = new ToolStripMenuItem();
            MenuItemAdmin = new ToolStripMenuItem();
            MenuItemVistoria = new ToolStripMenuItem();
            vencimentosToolStripMenuItem = new ToolStripMenuItem();
            MenuitemFinanceiro = new ToolStripMenuItem();
            timerReleaseExit = new System.Windows.Forms.Timer(components);
            pictureBox1 = new PictureBox();
            panel1 = new Panel();
            panel2 = new Panel();
            tableLayoutPanel1 = new TableLayoutPanel();
            toolStrip1 = new ToolStrip();
            toolStripLabel1 = new ToolStripLabel();
            BtnDone = new ToolStripButton();
            BtnAddNotifUrg = new ToolStripButton();
            contextMenuStripNotifications = new ContextMenuStrip(components);
            Menu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // Menu
            // 
            Menu.Items.AddRange(new ToolStripItem[] { MenuItemHome, MenuItemNotifications, planilhasToolStripMenuItem, MenuItemAdmin, MenuItemVistoria, vencimentosToolStripMenuItem, MenuitemFinanceiro });
            Menu.Location = new Point(0, 0);
            Menu.Name = "Menu";
            Menu.Size = new Size(1264, 34);
            Menu.TabIndex = 1;
            Menu.Text = "menuStrip1";
            // 
            // MenuItemHome
            // 
            MenuItemHome.DisplayStyle = ToolStripItemDisplayStyle.Image;
            MenuItemHome.DropDownItems.AddRange(new ToolStripItem[] { MenuItemMenu, MenuItemEmAndamento, MenuItemMaximizar, MenuItemMinimizar, MenuItemExit });
            MenuItemHome.Image = (Image)resources.GetObject("MenuItemHome.Image");
            MenuItemHome.Margin = new Padding(0, 5, 0, 5);
            MenuItemHome.Name = "MenuItemHome";
            MenuItemHome.Size = new Size(28, 20);
            MenuItemHome.Text = "Menu";
            MenuItemHome.DoubleClick += MenuItemHome_DoubleClick;
            // 
            // MenuItemMenu
            // 
            MenuItemMenu.Name = "MenuItemMenu";
            MenuItemMenu.Size = new Size(210, 22);
            MenuItemMenu.Text = "Menu";
            MenuItemMenu.Click += MenuItemMenu_Click;
            // 
            // MenuItemEmAndamento
            // 
            MenuItemEmAndamento.Name = "MenuItemEmAndamento";
            MenuItemEmAndamento.Size = new Size(210, 22);
            MenuItemEmAndamento.Text = "Processos em andamento";
            MenuItemEmAndamento.Click += MenuItemEmAndamento_Click;
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
            // MenuItemNotifications
            // 
            MenuItemNotifications.Image = (Image)resources.GetObject("MenuItemNotifications.Image");
            MenuItemNotifications.Name = "MenuItemNotifications";
            MenuItemNotifications.Size = new Size(101, 30);
            MenuItemNotifications.Text = "Notificações";
            MenuItemNotifications.Click += menuItemNotificacoes_Click;
            // 
            // planilhasToolStripMenuItem
            // 
            planilhasToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { santosToolStripMenuItem, itajaíToolStripMenuItem });
            planilhasToolStripMenuItem.Name = "planilhasToolStripMenuItem";
            planilhasToolStripMenuItem.Size = new Size(66, 30);
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
            MenuItemAdmin.Size = new Size(95, 30);
            MenuItemAdmin.Text = "Administrador";
            MenuItemAdmin.Click += MenuItemAdmin_Click;
            // 
            // MenuItemVistoria
            // 
            MenuItemVistoria.Name = "MenuItemVistoria";
            MenuItemVistoria.Size = new Size(58, 30);
            MenuItemVistoria.Text = "Vistoria";
            MenuItemVistoria.Click += MenuItemVistoria_Click;
            // 
            // vencimentosToolStripMenuItem
            // 
            vencimentosToolStripMenuItem.Name = "vencimentosToolStripMenuItem";
            vencimentosToolStripMenuItem.Size = new Size(87, 30);
            vencimentosToolStripMenuItem.Text = "Vencimentos";
            // 
            // MenuitemFinanceiro
            // 
            MenuitemFinanceiro.Alignment = ToolStripItemAlignment.Right;
            MenuitemFinanceiro.Name = "MenuitemFinanceiro";
            MenuitemFinanceiro.Size = new Size(74, 30);
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
            panel1.BackColor = SystemColors.Control;
            panel1.Controls.Add(panel2);
            panel1.Controls.Add(pictureBox1);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(1264, 681);
            panel1.TabIndex = 5;
            // 
            // panel2
            // 
            panel2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            panel2.BorderStyle = BorderStyle.FixedSingle;
            panel2.Controls.Add(tableLayoutPanel1);
            panel2.Controls.Add(toolStrip1);
            panel2.Location = new Point(22, 61);
            panel2.Name = "panel2";
            panel2.Size = new Size(339, 590);
            panel2.TabIndex = 4;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Location = new Point(0, 28);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 537F));
            tableLayoutPanel1.Size = new Size(338, 561);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // toolStrip1
            // 
            toolStrip1.Items.AddRange(new ToolStripItem[] { toolStripLabel1, BtnDone, BtnAddNotifUrg });
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(337, 25);
            toolStrip1.TabIndex = 0;
            toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            toolStripLabel1.Font = new Font("Segoe UI", 12F);
            toolStripLabel1.Name = "toolStripLabel1";
            toolStripLabel1.Size = new Size(162, 22);
            toolStripLabel1.Text = "Notificações Urgentes";
            // 
            // BtnDone
            // 
            BtnDone.Alignment = ToolStripItemAlignment.Right;
            BtnDone.DisplayStyle = ToolStripItemDisplayStyle.Image;
            BtnDone.Image = (Image)resources.GetObject("BtnDone.Image");
            BtnDone.ImageTransparentColor = Color.Magenta;
            BtnDone.Name = "BtnDone";
            BtnDone.Size = new Size(23, 22);
            BtnDone.Text = "toolStripButton2";
            // 
            // BtnAddNotifUrg
            // 
            BtnAddNotifUrg.Alignment = ToolStripItemAlignment.Right;
            BtnAddNotifUrg.DisplayStyle = ToolStripItemDisplayStyle.Image;
            BtnAddNotifUrg.Image = (Image)resources.GetObject("BtnAddNotifUrg.Image");
            BtnAddNotifUrg.ImageTransparentColor = Color.Magenta;
            BtnAddNotifUrg.Name = "BtnAddNotifUrg";
            BtnAddNotifUrg.Size = new Size(23, 22);
            BtnAddNotifUrg.Text = "toolStripButton2";
            // 
            // contextMenuStripNotifications
            // 
            contextMenuStripNotifications.AutoClose = false;
            contextMenuStripNotifications.Name = "contextMenuStrip1";
            contextMenuStripNotifications.ShowImageMargin = false;
            contextMenuStripNotifications.Size = new Size(36, 4);
            // 
            // FrmPrincipal
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ButtonFace;
            ClientSize = new Size(1264, 681);
            Controls.Add(Menu);
            Controls.Add(panel1);
            IsMdiContainer = true;
            MainMenuStrip = Menu;
            Name = "FrmPrincipal";
            Text = "Página Principal";
            WindowState = FormWindowState.Maximized;
            FormClosing += FrmPrincipal_FormClosing;
            Load += FrmPrincipal_Load;
            Shown += FrmPrincipal_Shown;
            Menu.ResumeLayout(false);
            Menu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            panel1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip Menu;
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
        private ToolStripMenuItem MenuItemMenu;
        private ToolStripMenuItem vencimentosToolStripMenuItem;
        private ContextMenuStrip contextMenuStripNotifications;
        private Panel panel2;
        private ToolStrip toolStrip1;
        private ToolStripLabel toolStripLabel1;
        private ToolStripButton BtnDone;
        private ToolStripButton BtnAddNotifUrg;
        private TableLayoutPanel tableLayoutPanel1;
    }
}