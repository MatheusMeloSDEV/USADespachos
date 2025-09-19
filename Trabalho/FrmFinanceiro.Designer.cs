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
            components = new System.ComponentModel.Container();
            panelFaturamento = new Panel();
            panelRecibo = new Panel();
            _bsFaturas = new BindingSource(components);
            ((System.ComponentModel.ISupportInitialize)_bsFaturas).BeginInit();
            SuspendLayout();
            // 
            // panelFaturamento
            // 
            panelFaturamento.AutoScroll = true;
            panelFaturamento.BorderStyle = BorderStyle.FixedSingle;
            panelFaturamento.Dock = DockStyle.Left;
            panelFaturamento.Location = new Point(0, 0);
            panelFaturamento.Name = "panelFaturamento";
            panelFaturamento.Size = new Size(300, 780);
            panelFaturamento.TabIndex = 0;
            // 
            // panelRecibo
            // 
            panelRecibo.AutoScroll = true;
            panelRecibo.BorderStyle = BorderStyle.FixedSingle;
            panelRecibo.Dock = DockStyle.Fill;
            panelRecibo.Location = new Point(300, 0);
            panelRecibo.Name = "panelRecibo";
            panelRecibo.Size = new Size(620, 780);
            panelRecibo.TabIndex = 1;
            // 
            // FrmFinanceiro
            // 
            ClientSize = new Size(920, 780);
            ControlBox = false;
            Controls.Add(panelRecibo);
            Controls.Add(panelFaturamento);
            Name = "FrmFinanceiro";
            Text = "Financeiro";
            WindowState = FormWindowState.Maximized;
            Shown += FrmFinanceiro_Shown;
            ((System.ComponentModel.ISupportInitialize)_bsFaturas).EndInit();
            ResumeLayout(false);

        }
        private BindingSource _bsFaturas;
    }
}
