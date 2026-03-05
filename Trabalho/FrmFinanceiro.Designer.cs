namespace Trabalho
{
    partial class FrmFinanceiro
    {
        private System.ComponentModel.IContainer components = null;

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
            DGVFaturamento = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)DGVFaturamento).BeginInit();
            SuspendLayout();
            // 
            // DGVFaturamento
            // 
            DGVFaturamento.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DGVFaturamento.Location = new Point(0, 34);
            DGVFaturamento.Name = "DGVFaturamento";
            DGVFaturamento.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            DGVFaturamento.Size = new Size(1226, 746);
            DGVFaturamento.TabIndex = 0;
            DGVFaturamento.CellDoubleClick += DGVFaturamento_CellDoubleClick;
            // 
            // FrmFinanceiro
            // 
            ClientSize = new Size(1226, 780);
            ControlBox = false;
            Controls.Add(DGVFaturamento);
            Name = "FrmFinanceiro";
            Text = "Financeiro";
            WindowState = FormWindowState.Maximized;
            Shown += FrmFinanceiro_Shown;
            ((System.ComponentModel.ISupportInitialize)DGVFaturamento).EndInit();
            ResumeLayout(false);

        }
        private BindingSource _bsRecibos;
        private BindingSource _bsFaturas;
        private DataGridView DGVFaturamento;
    }
}
