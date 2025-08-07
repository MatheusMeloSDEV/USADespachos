using CLUSA;

namespace Trabalho
{
    public partial class frmLi : Form
    {
        public LiInfo Li;
        private bool _somenteVisualizacao;
        private string _numeroLi;
        // Construtor padrão para criar uma nova LI
        public frmLi()
        {
            InitializeComponent();
            Li = new LiInfo();
            InicializarDateTimePickersComCheckbox();
            CarregarDateTimePickers(Li);
        }

        public frmLi(string numeroLi, List<string> orgaos, string ncm, string lpco, DateTime? dataregistroLI, bool checkdataregistroLI,
                    DateTime? dataRegistro, bool checkDataRegistro, DateTime? dataDeferimento, bool checkDataDeferimento, string parametrizacao,
                    bool somenteVisualizacao = false)
                    : this()
        {
            _somenteVisualizacao = somenteVisualizacao;
            _numeroLi = numeroLi;

            // Preenche campos básicos
            TxtLi.Text = numeroLi;
            TxtNCM.Text = ncm;
            TXTlilpco.Text = lpco;
            cbMapa.Checked = orgaos.Contains("MAPA");
            cbAnvisa.Checked = orgaos.Contains("ANVISA");
            cbDecex.Checked = orgaos.Contains("DECEX");
            cbIbama.Checked = orgaos.Contains("IBAMA");
            cbInmetro.Checked = orgaos.Contains("INMETRO");

            // Preenche DateTimePickers
            DTPdataderegistrolilpco.Checked = checkDataRegistro;
            if (checkDataRegistro && dataRegistro.HasValue)
            {
                DTPdataderegistrolilpco.Format = DateTimePickerFormat.Short;
                DTPdataderegistrolilpco.Value = dataRegistro.Value;
            }
            else
            {
                DTPdataderegistrolilpco.Format = DateTimePickerFormat.Custom;
                DTPdataderegistrolilpco.CustomFormat = " -";
            }

            DTPdatadedeferimentolilpco.Checked = checkDataDeferimento;
            if (checkDataDeferimento && dataDeferimento.HasValue)
            {
                DTPdatadedeferimentolilpco.Format = DateTimePickerFormat.Short;
                DTPdatadedeferimentolilpco.Value = dataDeferimento.Value;
            }
            else
            {
                DTPdatadedeferimentolilpco.Format = DateTimePickerFormat.Custom;
                DTPdatadedeferimentolilpco.CustomFormat = " -";
            }
            dtpDataRegistroLI.Checked = checkdataregistroLI;
            if (checkdataregistroLI && dataregistroLI.HasValue)
            {
                dtpDataRegistroLI.Format = DateTimePickerFormat.Short;
                dtpDataRegistroLI.Value = dataregistroLI.Value;
            }
            else
            {
                dtpDataRegistroLI.Format = DateTimePickerFormat.Custom;
                dtpDataRegistroLI.CustomFormat = " -";
            }

            // Preenche textbox de parametrização
            CBparametrizacaolilpco.Text = parametrizacao;

            // Ajusta para apenas leitura se necessário
            if (_somenteVisualizacao)
            {
                TxtLi.ReadOnly = true;
                TxtNCM.ReadOnly = true;
                TXTlilpco.ReadOnly = true;
                cbMapa.Enabled = cbAnvisa.Enabled = cbDecex.Enabled = cbIbama.Enabled = cbInmetro.Enabled = false;
                DTPdataderegistrolilpco.Enabled = DTPdatadedeferimentolilpco.Enabled = dtpDataRegistroLI.Enabled = false;
                CBparametrizacaolilpco.Enabled = false;
                btnOK.Visible = false;
                btnCancelar.Text = "Remover";

                btnCancelar.Click -= btnCancelar_Click;
                btnCancelar.Click += (s, e) =>
                {
                    var resp = MessageBox.Show($"Remover LI {_numeroLi}?", "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (resp == DialogResult.Yes && Owner is ILiHandler handler)
                    {
                        btnOK.Enabled = false;
                        handler.RemoverLi(_numeroLi);
                        this.Close();
                    }
                };
            }
        }
        private void InicializarDateTimePickersComCheckbox()
        {
            // Liste aqui todos os seus DateTimePickers que devem ter checkbox interno
            var dtps = new[]
            {
            DTPdataderegistrolilpco,
            DTPdatadedeferimentolilpco,
            dtpDataRegistroLI
            };

            foreach (var dtp in dtps)
            {
                dtp.ShowCheckBox = true;
                dtp.ValueChanged += DateTimePicker_OnValueChanged;
                // caso queira capturar também o uncheck via clique:
                dtp.MouseUp += (s, e2) => DateTimePicker_OnValueChanged(s!, EventArgs.Empty);
            }
        }
        private void DateTimePicker_OnValueChanged(object? sender, EventArgs e)
        {
            if (sender is not DateTimePicker picker)
                return;

            // Extrai o sufixo do nome para construir o nome da propriedade
            var campo = picker.Name!.Substring(3);

            // Ajusta o formato de acordo com o Checked
            if (picker.Checked)
            {
                picker.Format = DateTimePickerFormat.Short;
            }
            else
            {
                picker.Format = DateTimePickerFormat.Custom;
                picker.CustomFormat = "' -'";
            }

            DateTime? valor = picker.Checked ? picker.Value : (DateTime?)null;

            // Atualiza a propriedade DateTime? (DataX) no objeto LiInfo
            var nomePropData = campo switch
            {
                "dataderegistrolilpco" => "DataRegistroLPCO",
                "datadedeferimentolilpco" => "DataDeferimentoLPCO",
                "DataRegistroLI" => "DataRegistroLI",
                _ => null
            };
            if (nomePropData != null)
            {
                var propData = typeof(LiInfo).GetProperty(nomePropData);
                if (propData?.PropertyType == typeof(DateTime?))
                    propData.SetValue(Li, valor);

                var propCheck = typeof(LiInfo).GetProperty("Check" + nomePropData);
                if (propCheck?.PropertyType == typeof(bool))
                    propCheck.SetValue(Li, picker.Checked);
            }
        }
        private void CarregarDateTimePickers(LiInfo li)
        {
            // Mapeamento de cada DTP ao par (data, flag)
            var mapeamento = new Dictionary<DateTimePicker, (DateTime? data, bool has)>()
            {
                { DTPdataderegistrolilpco,    (li.DataRegistroLPCO,      li.CheckDataRegistroLPCO) },
                { DTPdatadedeferimentolilpco, (li.DataDeferimentoLPCO,   li.CheckDataDeferimentoLPCO) },
                { dtpDataRegistroLI,          (li.DataRegistroLI,        li.CheckDataRegistroLI) }
            };

            foreach (var kv in mapeamento)
            {
                var dtp = kv.Key;
                var (date, has) = kv.Value;

                // 1) Se quiser exibir checkbox interno (opcional)
                dtp.ShowCheckBox = true;

                // 2) Sincroniza o Checked com o banco
                dtp.Checked = has;

                // 3) Se tiver data, formata e atribui; senão, mantém em branco
                if (has && date.HasValue)
                {
                    dtp.Format = DateTimePickerFormat.Short;
                    dtp.Value = date.Value;
                }
                else
                {
                    dtp.Format = DateTimePickerFormat.Custom;
                    dtp.CustomFormat = " -";
                }
            }
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            var numeroLi = TxtLi.Text.Trim();
            if (string.IsNullOrWhiteSpace(numeroLi))
            {
                MessageBox.Show("Informe o número da LI.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var orgaosSelecionados = new List<string>();
            if (cbMapa.Checked) orgaosSelecionados.Add("MAPA");
            if (cbAnvisa.Checked) orgaosSelecionados.Add("ANVISA");
            if (cbDecex.Checked) orgaosSelecionados.Add("DECEX");
            if (cbIbama.Checked) orgaosSelecionados.Add("IBAMA");
            if (cbInmetro.Checked) orgaosSelecionados.Add("INMETRO");

            var ncm = TxtNCM.Text.Trim();
            var lpco = TXTlilpco.Text.Trim();
            var dataReg = DTPdataderegistrolilpco.Checked ? DTPdataderegistrolilpco.Value : (DateTime?)null;
            var chkReg = DTPdataderegistrolilpco.Checked;
            var dataDef = DTPdatadedeferimentolilpco.Checked ? DTPdatadedeferimentolilpco.Value : (DateTime?)null;
            var chkDef = DTPdatadedeferimentolilpco.Checked;
            var dataRegLI = dtpDataRegistroLI.Checked ? dtpDataRegistroLI.Value : (DateTime?)null;
            var chkRegLI = dtpDataRegistroLI.Checked;
            var param = CBparametrizacaolilpco.Text.Trim();

            var novaLi = new LiInfo(numeroLi, orgaosSelecionados, ncm, lpco, dataRegLI, chkRegLI, dataReg, chkReg, dataDef, chkDef, param);

            if (Owner is ILiHandler handler)
            {
                // Se já existe, atualiza; se não, adiciona
                if (handler.ContainsLi(numeroLi))
                {
                    handler.AtualizarLi(numeroLi, novaLi);
                    MessageBox.Show("LI atualizada com sucesso.", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    handler.AdicionarLi(novaLi);
                    MessageBox.Show("LI adicionada com sucesso.", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                this.Close();
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
