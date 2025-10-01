using CLUSA;

namespace Trabalho
{
    public partial class LIEditControl : UserControl
    {
        private LicencaImportacao _licenca;

        public LIEditControl()
        {
            InitializeComponent();
            _licenca = new LicencaImportacao();
        }

        public void VincularDados(LicencaImportacao li)
        {
            _licenca = li;

            TxtLi.DataBindings.Add("Text", _licenca, nameof(_licenca.Numero), false, DataSourceUpdateMode.OnPropertyChanged);
            TxtNCM.DataBindings.Add("Text", _licenca, nameof(_licenca.NCM), false, DataSourceUpdateMode.OnPropertyChanged);
            CbStatusLI.DataBindings.Add("Text", _licenca, nameof(_licenca.StatusLI), false, DataSourceUpdateMode.OnPropertyChanged);
            ConfigurarDatePickerNulavel(DtpDataRegistro, _licenca.DataRegistro);

            CarregarAbasLpco();

            // Conecta os eventos dos botões aos seus respectivos métodos
            BtnNovoOrgaoAnuente.Click += BtnNovoLpco_Click;
            btnExcluirLpco.Click += BtnExcluirLpco_Click;

            var itensComboBox = new List<string> { "Selecione o órgão..." };
            itensComboBox.AddRange(Enum.GetNames(typeof(TipoOrgaoAnuente)));
            CBOrgaoAnuente.DataSource = itensComboBox;
            CBOrgaoAnuente.SelectedIndex = 0;
        }

        public void SalvarAlteracoes()
        {
            // Garante que os bindings da LI principal sejam salvos
            if (this.BindingContext.Contains(_licenca))
            {
                this.BindingContext[_licenca].EndCurrentEdit();
            }
            _licenca.DataRegistro = DtpDataRegistro.Checked ? DtpDataRegistro.Value : null;

            // Manda cada controle de LPCO salvar suas próprias alterações
            foreach (TabPage aba in tabControl2.TabPages)
            {
                if (aba.Controls.OfType<LpcoEditControl>().FirstOrDefault() is LpcoEditControl lpcoControl)
                {
                    lpcoControl.SalvarAlteracoes();
                }
            }
        }

        private void CarregarAbasLpco()
        {
            tabControl2.TabPages.Clear();
            foreach (var lpco in _licenca.LPCO)
            {
                AdicionarAbaLpco(lpco);
            }
        }

        private void AdicionarAbaLpco(LpcoInfo lpco)
        {
            var tabPage = new TabPage(lpco.NomeOrgao) { Tag = lpco, BackColor = SystemColors.Control };

            // Cria a instância do UserControl especialista
            var lpcoControl = new LpcoEditControl { Dock = DockStyle.Fill };

            // Vincula os dados e adiciona à aba
            lpcoControl.VincularDados(lpco);
            tabPage.Controls.Add(lpcoControl);
            tabControl2.TabPages.Add(tabPage);
        }

        private void BtnNovoLpco_Click(object? sender, EventArgs e)
        {
            if (CBOrgaoAnuente.SelectedIndex <= 0) { /* ... */ return; }
            string nomeOrgao = CBOrgaoAnuente.SelectedItem.ToString();
            if (_licenca.LPCO.Any(l => l.NomeOrgao == nomeOrgao)) { /* ... */ return; }

            var novoLpco = new LpcoInfo { NomeOrgao = nomeOrgao };
            _licenca.LPCO.Add(novoLpco);
            AdicionarAbaLpco(novoLpco);
            tabControl2.SelectedIndex = tabControl2.TabPages.Count - 1;
        }

        private void BtnExcluirLpco_Click(object? sender, EventArgs e)
        {
            if (tabControl2.SelectedTab == null) { /* ... */ return; }
            var abaSelecionada = tabControl2.SelectedTab;
            if (abaSelecionada.Tag is not LpcoInfo lpcoParaExcluir) return;

            var resultado = MessageBox.Show($"Tem certeza que quer excluir?", "Confirmar...", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (resultado == DialogResult.Yes)
            {
                _licenca.LPCO.Remove(lpcoParaExcluir);
                tabControl2.TabPages.Remove(abaSelecionada);
            }
        }

        private void ConfigurarDatePickerNulavel(DateTimePicker dtp, DateTime? data)
        {
            dtp.ShowCheckBox = true;
            if (data.HasValue)
            {
                dtp.Checked = true;
                dtp.Value = data.Value;
                dtp.Format = DateTimePickerFormat.Short;
            }
            else
            {
                dtp.Checked = false;
                dtp.Value = DateTime.Today;
                dtp.Format = DateTimePickerFormat.Custom;
                dtp.CustomFormat = " ";
            }
            dtp.ValueChanged += (s, e) => {
                if (s is DateTimePicker picker) { picker.Format = picker.Checked ? DateTimePickerFormat.Short : DateTimePickerFormat.Custom; }
            };
        }
    }
}