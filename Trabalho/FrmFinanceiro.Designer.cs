namespace Trabalho
{
    partial class FrmFinanceiro
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel panelFaturamento;
        private System.Windows.Forms.Panel panelRecibo;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            panelFaturamento = new Panel();
            tableLayoutPanel2 = new TableLayoutPanel();
            DGVFaturamento = new DataGridView();
            label1 = new Label();
            panelRecibo = new Panel();
            tableLayoutPanel3 = new TableLayoutPanel();
            DGVRecibo = new DataGridView();
            label2 = new Label();
            tableLayoutPanel1 = new TableLayoutPanel();
            panelFaturamento.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DGVFaturamento).BeginInit();
            panelRecibo.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DGVRecibo).BeginInit();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // panelFaturamento
            // 
            panelFaturamento.AutoScroll = true;
            panelFaturamento.BorderStyle = BorderStyle.FixedSingle;
            panelFaturamento.Controls.Add(tableLayoutPanel2);
            panelFaturamento.Dock = DockStyle.Fill;
            panelFaturamento.Location = new Point(3, 3);
            panelFaturamento.Name = "panelFaturamento";
            panelFaturamento.Size = new Size(607, 774);
            panelFaturamento.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 1;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Controls.Add(DGVFaturamento, 0, 1);
            tableLayoutPanel2.Controls.Add(label1, 0, 0);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(0, 0);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 2;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 5F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 95F));
            tableLayoutPanel2.Size = new Size(605, 772);
            tableLayoutPanel2.TabIndex = 0;
            // 
            // DGVFaturamento
            // 
            DGVFaturamento.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DGVFaturamento.Dock = DockStyle.Fill;
            DGVFaturamento.Location = new Point(3, 41);
            DGVFaturamento.Name = "DGVFaturamento";
            DGVFaturamento.RowTemplate.Height = 25;
            DGVFaturamento.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            DGVFaturamento.Size = new Size(599, 728);
            DGVFaturamento.TabIndex = 0;
            DGVFaturamento.CellDoubleClick += DGVFaturamento_CellDoubleClick;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Fill;
            label1.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            label1.Location = new Point(3, 0);
            label1.Name = "label1";
            label1.Size = new Size(599, 38);
            label1.TabIndex = 1;
            label1.Text = "Faturamento";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panelRecibo
            // 
            panelRecibo.AutoScroll = true;
            panelRecibo.BorderStyle = BorderStyle.FixedSingle;
            panelRecibo.Controls.Add(tableLayoutPanel3);
            panelRecibo.Dock = DockStyle.Fill;
            panelRecibo.Location = new Point(616, 3);
            panelRecibo.Name = "panelRecibo";
            panelRecibo.Size = new Size(607, 774);
            panelRecibo.TabIndex = 1;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 1;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.Controls.Add(DGVRecibo, 0, 1);
            tableLayoutPanel3.Controls.Add(label2, 0, 0);
            tableLayoutPanel3.Dock = DockStyle.Fill;
            tableLayoutPanel3.Location = new Point(0, 0);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 2;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 5F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 95F));
            tableLayoutPanel3.Size = new Size(605, 772);
            tableLayoutPanel3.TabIndex = 1;
            // 
            // DGVRecibo
            // 
            DGVRecibo.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DGVRecibo.Dock = DockStyle.Fill;
            DGVRecibo.Location = new Point(3, 41);
            DGVRecibo.Name = "DGVRecibo";
            DGVRecibo.RowTemplate.Height = 25;
            DGVRecibo.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            DGVRecibo.Size = new Size(599, 728);
            DGVRecibo.TabIndex = 0;
            DGVRecibo.CellDoubleClick += DGVRecibo_CellDoubleClick;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Dock = DockStyle.Fill;
            label2.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            label2.Location = new Point(3, 0);
            label2.Name = "label2";
            label2.Size = new Size(599, 38);
            label2.TabIndex = 1;
            label2.Text = "Recibo";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(panelRecibo, 1, 0);
            tableLayoutPanel1.Controls.Add(panelFaturamento, 0, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(1226, 780);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // FrmFinanceiro
            // 
            ClientSize = new Size(1226, 780);
            ControlBox = false;
            Controls.Add(tableLayoutPanel1);
            Name = "FrmFinanceiro";
            Text = "Financeiro";
            WindowState = FormWindowState.Maximized;
            Shown += FrmFinanceiro_Shown;
            panelFaturamento.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)DGVFaturamento).EndInit();
            panelRecibo.ResumeLayout(false);
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)DGVRecibo).EndInit();
            tableLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);

        }
        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel2;
        private DataGridView DGVFaturamento;
        private Label label1;
        private TableLayoutPanel tableLayoutPanel3;
        private DataGridView DGVRecibo;
        private Label label2;
        private BindingSource _bsRecibos;
        private BindingSource _bsFaturas;
    }
}
