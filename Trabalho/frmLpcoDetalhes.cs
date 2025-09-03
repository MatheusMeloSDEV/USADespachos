using CLUSA;

namespace Trabalho
{
    public partial class frmLpcoDetalhes : Form
    {
        public LpcoInfo Lpco { get; private set; }
        private readonly bool _somenteVisualizacao;

        // O construtor recebe o objeto LpcoInfo para editar
        public frmLpcoDetalhes(LpcoInfo lpcoParaEditar, bool somenteVisualizacao = false)
        {
            InitializeComponent();
            Lpco = lpcoParaEditar;
            _somenteVisualizacao = somenteVisualizacao; // <-- Adicione esta linha
            InicializarDateTimePickersComCheckbox();
        }

        private void frmLpcoDetalhes_Load(object sender, EventArgs e)
        {
            // Carrega os dados do objeto nos campos da tela
            this.Text = $"Detalhes do LPCO para {Lpco.NomeOrgao}";
            txtLpco.Text = Lpco.LPCO;
            cmbParametrizacao.Text = Lpco.ParametrizacaoLPCO;

            // Carrega as datas (semelhante ao que fizemos antes)
            if (Lpco.CheckDataRegistroLPCO && Lpco.DataRegistroLPCO.HasValue)
            {
                dtpDataRegistro.Checked = true;
                dtpDataRegistro.Value = Lpco.DataRegistroLPCO.Value;
                dtpDataRegistro.Format = DateTimePickerFormat.Short;
            }
            else
            {
                dtpDataRegistro.Checked = false;
                dtpDataRegistro.Format = DateTimePickerFormat.Custom;
                dtpDataRegistro.CustomFormat = " ";
            }

            if (Lpco.CheckDataDeferimentoLPCO && Lpco.DataDeferimentoLPCO.HasValue)
            {
                dtpDataDeferimento.Checked = true;
                dtpDataDeferimento.Value = Lpco.DataDeferimentoLPCO.Value;
                dtpDataDeferimento.Format = DateTimePickerFormat.Short;
            }
            else
            {
                dtpDataDeferimento.Checked = false;
                dtpDataDeferimento.Format = DateTimePickerFormat.Custom;
                dtpDataDeferimento.CustomFormat = " ";
            }

            if (_somenteVisualizacao)
            {
                this.Text = $"Visualizando LPCO de {Lpco.NomeOrgao}";
                txtLpco.ReadOnly = true;
                dtpDataRegistro.Enabled = false;
                dtpDataDeferimento.Enabled = false;
                cmbParametrizacao.Enabled = false;
                button1.Visible = false;
            }
        }
        private void DateTimePicker_OnValueChanged(object? sender, EventArgs e)
        {
            if (sender is not DateTimePicker picker) return;

            if (picker.Checked)
            {
                picker.Format = DateTimePickerFormat.Short;
            }
            else
            {
                picker.Format = DateTimePickerFormat.Custom;
                picker.CustomFormat = " ";
            }

            if (picker.Name == "dtpDataDeferimento")
            {
                Lpco.CheckDataDeferimentoLPCO = picker.Checked;
                Lpco.DataDeferimentoLPCO = picker.Checked ? picker.Value : null;
            }
            if (picker.Name == "dtpDataRegistro")
            {
                Lpco.CheckDataRegistroLPCO = picker.Checked;
                Lpco.DataRegistroLPCO = picker.Checked ? picker.Value : null;
            }
        }

        private void InicializarDateTimePickersComCheckbox()
        {
            // Liste aqui todos os seus DateTimePickers que devem ter checkbox interno
            var dtps = new[]
            {
                dtpDataDeferimento,
                dtpDataRegistro
            };

            foreach (var dtp in dtps)
            {
                dtp.ShowCheckBox = true;
                dtp.Checked = false;

                dtp.Format = DateTimePickerFormat.Custom;
                dtp.CustomFormat = " ";

                dtp.ValueChanged += DateTimePicker_OnValueChanged;
                dtp.MouseUp += (s, e2) => DateTimePicker_OnValueChanged(s, null);
            }
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            // Salva os dados da tela de volta para o objeto
            Lpco.LPCO = txtLpco.Text.Trim();
            Lpco.ParametrizacaoLPCO = cmbParametrizacao.Text.Trim();

            Lpco.CheckDataRegistroLPCO = dtpDataRegistro.Checked;
            Lpco.DataRegistroLPCO = dtpDataRegistro.Checked ? dtpDataRegistro.Value : null;

            Lpco.CheckDataDeferimentoLPCO = dtpDataDeferimento.Checked;
            Lpco.DataDeferimentoLPCO = dtpDataDeferimento.Checked ? dtpDataDeferimento.Value : null;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void frmLpcoDetalhes_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.DialogResult == DialogResult.None)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }
    }
}