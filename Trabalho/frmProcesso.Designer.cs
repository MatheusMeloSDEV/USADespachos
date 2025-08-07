using System.Windows.Forms;

namespace Trabalho
{
    partial class FrmProcesso
    {
        private System.ComponentModel.IContainer components = null;

        private ToolStrip toolStrip1;
        private ToolStripButton BtnAdicionar;
        private ToolStripButton BtnRemover;
        private ToolStripButton BtnExportar;
        private ToolStripButton BtnEditar;
        private ToolStripButton BtnCancelar;
        private ToolStripButton BtnPesquisar;
        private ToolStripButton BtnReload;
        private ToolStripTextBox TxtPesquisar;
        private ToolStripComboBox CmbPesquisar;
        private DataGridView DataGridView1;
        private BindingSource BsProcesso;
        /// <summary>
        /// Liberar recursos utilizados.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
                BsProcesso.Dispose();
            }
            base.Dispose(disposing);
        }


        private void InitializeComponent()
        {
            var components = new System.ComponentModel.Container();
            BsProcesso = new BindingSource(components);

            #region ToolStrip Config

            toolStrip1 = new ToolStrip
            {
                GripStyle = ToolStripGripStyle.Hidden,
                RenderMode = ToolStripRenderMode.Professional,
                BackColor = Color.WhiteSmoke,
                AutoSize = false,
                Height = 40,
                Padding = new Padding(5, 5, 5, 5),
            };

            // Definir uma fonte padrão
            Font defaultFont = new Font("Segoe UI", 10, FontStyle.Regular);

            BtnAdicionar = new ToolStripButton
            {
                Text = "Adicionar",
                Font = defaultFont,
                Margin = Margin = new Padding(0, 0, 5, 0),
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                ImageScaling = ToolStripItemImageScaling.SizeToFit
            };
            BtnAdicionar.Click += BtnAdicionar_Click;

            BtnEditar = new ToolStripButton
            {
                Text = "Editar",
                Font = defaultFont,
                Margin = Margin = new Padding(0, 0, 5, 0),
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                ImageScaling = ToolStripItemImageScaling.SizeToFit
            };
            BtnEditar.Click += BtnEditar_Click;

            BtnRemover = new ToolStripButton
            {
                Text = "Excluir",
                Font = defaultFont,
                Margin = Margin = new Padding(0, 0, 5, 0),
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                ImageScaling = ToolStripItemImageScaling.SizeToFit
            };
            BtnRemover.Click += BtnExcluir_Click;

            BtnExportar = new ToolStripButton
            {
                Text = "Exportar",
                Font = defaultFont,
                Margin = Margin = new Padding(0, 0, 5, 0),
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                ImageScaling = ToolStripItemImageScaling.SizeToFit
            };
            BtnExportar.Click += BtnExportar_Click;

            new ToolStripSeparator();

            BtnCancelar = new ToolStripButton
            {
                Alignment = ToolStripItemAlignment.Right,
                Text = "Cancelar",
                Font = defaultFont,
                Margin = Margin = new Padding(5, 0, 0, 0),
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                ImageScaling = ToolStripItemImageScaling.SizeToFit
            };
            BtnCancelar.Click += BtnCancelar_Click;

            BtnReload = new ToolStripButton
            {
                Alignment = ToolStripItemAlignment.Right,
                Text = "Recarregar",
                Font = defaultFont,
                Margin = Margin = new Padding(5, 0, 0, 0),
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                ImageScaling = ToolStripItemImageScaling.SizeToFit
            };
            BtnReload.Click += BtnReload_Click;

            new ToolStripSeparator();

            BtnPesquisar = new ToolStripButton
            {
                Alignment = ToolStripItemAlignment.Right,
                Text = "Pesquisar",
                Font = defaultFont,
                Margin = Margin = new Padding(5, 0, 0, 0),
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                ImageScaling = ToolStripItemImageScaling.SizeToFit
            };
            BtnPesquisar.Click += BtnPesquisar_Click;

            CmbPesquisar = new ToolStripComboBox
            {
                Alignment = ToolStripItemAlignment.Right,
                Font = defaultFont,
                Margin = Margin = new Padding(5, 0, 5, 0),
                Size = new Size(150, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            CmbPesquisar.SelectedIndexChanged += CmbPesquisar_SelectedIndexChanged;

            TxtPesquisar = new ToolStripTextBox
            {
                Alignment = ToolStripItemAlignment.Right,
                Font = defaultFont,
                Margin = Margin = new Padding(0, 0, 5, 0),
                Size = new Size(200, 25),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Agora adicione na ordem inversa desejada para itens à direita:
            ToolStripSeparator ToolStripRight = new ToolStripSeparator
            {
                Alignment = ToolStripItemAlignment.Right,
            };
            ToolStripSeparator ToolStripRight1 = new ToolStripSeparator
            {
                Alignment = ToolStripItemAlignment.Right,
            };

            toolStrip1.Items.Add(BtnAdicionar);
            toolStrip1.Items.Add(BtnEditar);
            toolStrip1.Items.Add(BtnRemover);
            toolStrip1.Items.Add(BtnExportar);
            toolStrip1.Items.Add(BtnCancelar);
            toolStrip1.Items.Add(ToolStripRight1);
            toolStrip1.Items.Add(BtnReload);
            toolStrip1.Items.Add(ToolStripRight);
            toolStrip1.Items.Add(BtnPesquisar);
            toolStrip1.Items.Add(CmbPesquisar);
            toolStrip1.Items.Add(TxtPesquisar);

            #endregion

            #region DataGridView Configuração

            DataGridView1 = new DataGridView
            {
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                AutoGenerateColumns = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                EnableHeadersVisualStyles = false,
                Location = new Point(12, 50),
                Name = "dataGridView1",
                ReadOnly = true,
                RowHeadersVisible = false,
                RowTemplate = { Height = 25 },
                Size = new Size(776, 388),
                BorderStyle = BorderStyle.FixedSingle,
                BackgroundColor = Color.White,
            };

            DataGridView1.CellDoubleClick += DataGridView1_CellDoubleClick;

            DataGridView1.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(50, 50, 50),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                WrapMode = DataGridViewTriState.True
            };

            DataGridView1.DefaultCellStyle = new DataGridViewCellStyle
            {
                Font = new Font("Segoe UI", 10),
                BackColor = Color.White,
                ForeColor = Color.Black,
                SelectionBackColor = Color.LightBlue,
                SelectionForeColor = Color.Black,
                Alignment = DataGridViewContentAlignment.MiddleLeft
            };

            DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            DataGridView1.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.WhiteSmoke
            };

            #endregion

            #region Form Configuração

            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(DataGridView1);
            Controls.Add(toolStrip1);
            Text = "Gerenciamento de Processos";
            WindowState = FormWindowState.Maximized;
            ControlBox = false;

            Load += FrmProcesso_Load;

            #endregion
        }

    }
}
