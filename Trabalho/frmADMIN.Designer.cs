namespace Trabalho
{
    partial class FrmAdmin
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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            DGVAdmin = new DataGridView();
            BSAdmin = new BindingSource(components);
            btnAdcionar = new Button();
            btnExcluir = new Button();
            btnEditar = new Button();
            dgvLogs = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)DGVAdmin).BeginInit();
            ((System.ComponentModel.ISupportInitialize)BSAdmin).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvLogs).BeginInit();
            SuspendLayout();
            // 
            // DGVAdmin
            // 
            DGVAdmin.AllowUserToAddRows = false;
            DGVAdmin.AllowUserToDeleteRows = false;
            DGVAdmin.AllowUserToResizeColumns = false;
            DGVAdmin.AllowUserToResizeRows = false;
            DGVAdmin.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            DGVAdmin.AutoGenerateColumns = false;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = SystemColors.Control;
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            DGVAdmin.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            DGVAdmin.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DGVAdmin.DataSource = BSAdmin;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = SystemColors.Window;
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle2.NullValue = null;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            DGVAdmin.DefaultCellStyle = dataGridViewCellStyle2;
            DGVAdmin.Location = new Point(12, 41);
            DGVAdmin.Name = "DGVAdmin";
            DGVAdmin.ReadOnly = true;
            DGVAdmin.RowHeadersVisible = false;
            DGVAdmin.Size = new Size(179, 397);
            DGVAdmin.TabIndex = 3;
            // 
            // btnAdcionar
            // 
            btnAdcionar.Location = new Point(12, 12);
            btnAdcionar.Name = "btnAdcionar";
            btnAdcionar.Size = new Size(66, 23);
            btnAdcionar.TabIndex = 4;
            btnAdcionar.Text = "Adicionar";
            btnAdcionar.UseVisualStyleBackColor = true;
            btnAdcionar.Click += BtnAdicionar_Click;
            // 
            // btnExcluir
            // 
            btnExcluir.Location = new Point(84, 12);
            btnExcluir.Name = "btnExcluir";
            btnExcluir.Size = new Size(53, 23);
            btnExcluir.TabIndex = 5;
            btnExcluir.Text = "Excluir";
            btnExcluir.UseVisualStyleBackColor = true;
            btnExcluir.Click += BtnExcluir_Click;
            // 
            // btnEditar
            // 
            btnEditar.Location = new Point(143, 12);
            btnEditar.Name = "btnEditar";
            btnEditar.Size = new Size(48, 23);
            btnEditar.TabIndex = 6;
            btnEditar.Text = "Editar";
            btnEditar.UseVisualStyleBackColor = true;
            btnEditar.Click += BtnEditar_Click;
            // 
            // dgvLogs
            // 
            dgvLogs.AllowUserToAddRows = false;
            dgvLogs.AllowUserToDeleteRows = false;
            dgvLogs.AllowUserToResizeColumns = false;
            dgvLogs.AllowUserToResizeRows = false;
            dgvLogs.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvLogs.AutoGenerateColumns = false;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = SystemColors.Control;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle3.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.True;
            dgvLogs.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            dgvLogs.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvLogs.DataSource = BSAdmin;
            dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = SystemColors.Window;
            dataGridViewCellStyle4.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle4.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle4.NullValue = null;
            dataGridViewCellStyle4.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = DataGridViewTriState.False;
            dgvLogs.DefaultCellStyle = dataGridViewCellStyle4;
            dgvLogs.Location = new Point(197, 12);
            dgvLogs.Name = "dgvLogs";
            dgvLogs.ReadOnly = true;
            dgvLogs.RowHeadersVisible = false;
            dgvLogs.Size = new Size(591, 426);
            dgvLogs.TabIndex = 7;
            // 
            // FrmAdmin
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            ControlBox = false;
            Controls.Add(dgvLogs);
            Controls.Add(btnEditar);
            Controls.Add(btnExcluir);
            Controls.Add(btnAdcionar);
            Controls.Add(DGVAdmin);
            Name = "FrmAdmin";
            Text = "ADMIN";
            WindowState = FormWindowState.Maximized;
            Load += FrmADMIN_Load;
            ((System.ComponentModel.ISupportInitialize)DGVAdmin).EndInit();
            ((System.ComponentModel.ISupportInitialize)BSAdmin).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvLogs).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private DataGridView DGVAdmin;
        private Button btnAdcionar;
        private Button btnExcluir;
        private Button btnEditar;
        private BindingSource BSAdmin;
        private DataGridViewTextBoxColumn iDDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn usernameDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn passwordDataGridViewTextBoxColumn;
        private DataGridViewCheckBoxColumn adminDataGridViewCheckBoxColumn;
        private DataGridView dgvLogs;
    }
}