namespace Trabalho
{
    partial class FrmModificaVencimentos
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
            btnEnviar = new Button();
            cbImportador = new ComboBox();
            cbTagEvento = new ComboBox();
            dtpDataEvento = new DateTimePicker();
            btnAdicionarEvento = new Button();
            dgvEventos = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)dgvEventos).BeginInit();
            SuspendLayout();
            // 
            // btnEnviar
            // 
            btnEnviar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnEnviar.Location = new Point(298, 12);
            btnEnviar.Name = "btnEnviar";
            btnEnviar.Size = new Size(75, 23);
            btnEnviar.TabIndex = 3;
            btnEnviar.Text = "Enviar";
            btnEnviar.UseVisualStyleBackColor = true;
            btnEnviar.Click += btnEnviar_Click;
            // 
            // cbImportador
            // 
            cbImportador.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cbImportador.FormattingEnabled = true;
            cbImportador.Items.AddRange(new object[] { "ACCIO", "ALICE ALIMENTOS", "AURORA", "BRASCOD", "CASA FLORA", "COPY DATA", "DAMPER", "ELTO COMERCIAL", "FMG", "FREEWAY", "FRUGAL", "KUKAMAR", "LEITESOL", "LIBRA", "MARHUA", "MARCOL", "MARNOBRE", "MGA", "NOR IMPORT", "REBELA", "SEIKO", "VANUCCI", "VILA SIMPATIA", "ZARAGOZA" });
            cbImportador.Location = new Point(95, 12);
            cbImportador.Name = "cbImportador";
            cbImportador.Size = new Size(197, 23);
            cbImportador.TabIndex = 5;
            cbImportador.Text = "Nome do Importador";
            // 
            // cbTagEvento
            // 
            cbTagEvento.FormattingEnabled = true;
            cbTagEvento.Location = new Point(12, 53);
            cbTagEvento.Name = "cbTagEvento";
            cbTagEvento.Size = new Size(165, 23);
            cbTagEvento.TabIndex = 6;
            // 
            // dtpDataEvento
            // 
            dtpDataEvento.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            dtpDataEvento.Format = DateTimePickerFormat.Short;
            dtpDataEvento.Location = new Point(183, 53);
            dtpDataEvento.Name = "dtpDataEvento";
            dtpDataEvento.Size = new Size(138, 23);
            dtpDataEvento.TabIndex = 7;
            // 
            // btnAdicionarEvento
            // 
            btnAdicionarEvento.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAdicionarEvento.Location = new Point(327, 53);
            btnAdicionarEvento.Name = "btnAdicionarEvento";
            btnAdicionarEvento.Size = new Size(130, 23);
            btnAdicionarEvento.TabIndex = 8;
            btnAdicionarEvento.Text = "Adicionar Evento";
            btnAdicionarEvento.UseVisualStyleBackColor = true;
            btnAdicionarEvento.Click += btnAdicionarEvento_Click;
            // 
            // dgvEventos
            // 
            dgvEventos.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvEventos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvEventos.Location = new Point(12, 85);
            dgvEventos.Name = "dgvEventos";
            dgvEventos.Size = new Size(445, 150);
            dgvEventos.TabIndex = 9;
            // 
            // FrmModificaVencimentos
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(469, 247);
            Controls.Add(dgvEventos);
            Controls.Add(btnAdicionarEvento);
            Controls.Add(dtpDataEvento);
            Controls.Add(cbTagEvento);
            Controls.Add(cbImportador);
            Controls.Add(btnEnviar);
            Name = "FrmModificaVencimentos";
            Text = "Edita/Adiciona Vencimento";
            ((System.ComponentModel.ISupportInitialize)dgvEventos).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private Button btnEnviar;
        private ComboBox cbImportador;
        private ComboBox cbTagEvento;
        private DateTimePicker dtpDataEvento;
        private Button btnAdicionarEvento;
        private DataGridView dgvEventos;
    }
}