namespace Trabalho
{
    partial class FrmStatusProcessos
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
            tableLayoutPanel1 = new TableLayoutPanel();
            lblEmAndamento = new Label();
            label2 = new Label();
            label5 = new Label();
            label6 = new Label();
            label4 = new Label();
            label3 = new Label();
            tableLayoutPanel2 = new TableLayoutPanel();
            label1 = new Label();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel1.Controls.Add(lblEmAndamento, 0, 0);
            tableLayoutPanel1.Controls.Add(label2, 0, 1);
            tableLayoutPanel1.Controls.Add(label5, 1, 1);
            tableLayoutPanel1.Controls.Add(label6, 2, 1);
            tableLayoutPanel1.Controls.Add(label4, 1, 0);
            tableLayoutPanel1.Controls.Add(label3, 2, 0);
            tableLayoutPanel1.Location = new Point(133, 73);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.Size = new Size(970, 281);
            tableLayoutPanel1.TabIndex = 11;
            // 
            // lblEmAndamento
            // 
            lblEmAndamento.BackColor = Color.FromArgb(255, 255, 192);
            lblEmAndamento.Font = new Font("Segoe UI", 22F);
            lblEmAndamento.Location = new Point(25, 25);
            lblEmAndamento.Margin = new Padding(25);
            lblEmAndamento.Name = "lblEmAndamento";
            lblEmAndamento.Size = new Size(275, 91);
            lblEmAndamento.TabIndex = 3;
            lblEmAndamento.Text = "0 Em Andamento";
            lblEmAndamento.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            label2.BackColor = Color.FromArgb(255, 255, 192);
            label2.Font = new Font("Segoe UI", 22F);
            label2.Location = new Point(25, 166);
            label2.Margin = new Padding(25);
            label2.Name = "label2";
            label2.Size = new Size(275, 91);
            label2.TabIndex = 4;
            label2.Text = "0 Em Andamento";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            label5.BackColor = Color.FromArgb(255, 255, 192);
            label5.Font = new Font("Segoe UI", 22F);
            label5.Location = new Point(350, 166);
            label5.Margin = new Padding(25);
            label5.Name = "label5";
            label5.Size = new Size(275, 91);
            label5.TabIndex = 7;
            label5.Text = "0 Em Andamento";
            label5.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            label6.BackColor = Color.FromArgb(255, 255, 192);
            label6.Font = new Font("Segoe UI", 22F);
            label6.Location = new Point(675, 166);
            label6.Margin = new Padding(25);
            label6.Name = "label6";
            label6.Size = new Size(275, 91);
            label6.TabIndex = 8;
            label6.Text = "0 Em Andamento";
            label6.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            label4.BackColor = Color.FromArgb(255, 255, 192);
            label4.Font = new Font("Segoe UI", 22F);
            label4.Location = new Point(350, 25);
            label4.Margin = new Padding(25);
            label4.Name = "label4";
            label4.Size = new Size(275, 91);
            label4.TabIndex = 6;
            label4.Text = "0 Em Andamento";
            label4.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            label3.BackColor = Color.FromArgb(255, 255, 192);
            label3.Font = new Font("Segoe UI", 22F);
            label3.Location = new Point(675, 25);
            label3.Margin = new Padding(25);
            label3.Name = "label3";
            label3.Size = new Size(275, 91);
            label3.TabIndex = 5;
            label3.Text = "0 Em Andamento";
            label3.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.Controls.Add(label1, 0, 0);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(0, 0);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 3;
            tableLayoutPanel2.RowStyles.Add(new RowStyle());
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 343F));
            tableLayoutPanel2.Size = new Size(1264, 681);
            tableLayoutPanel2.TabIndex = 12;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 20F);
            label1.Location = new Point(3, 0);
            label1.Name = "label1";
            label1.Size = new Size(200, 37);
            label1.TabIndex = 0;
            label1.Text = "Em Andamento";
            label1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // FrmStatusProcessos
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1264, 681);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(tableLayoutPanel2);
            Name = "FrmStatusProcessos";
            Text = "FrmStatusProcessos";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private Label lblEmAndamento;
        private Label label2;
        private Label label5;
        private Label label6;
        private Label label4;
        private Label label3;
        private TableLayoutPanel tableLayoutPanel2;
        private Label label1;
    }
}