using CLUSA;
using System.Data;

namespace Trabalho
{
    public partial class FrmModificaOrgaoAnuente : Form
    {
        // Propriedades para receber os dados da tela anterior
        public OrgaoAnuente? OrgaoAnuente { get; set; }
        public Processo? Processo { get; set; }

        private readonly RepositorioOrgaoAnuente _repositorioOrgaoAnuente;
        private readonly RepositorioProcesso _repositorioProcesso;

        public FrmModificaOrgaoAnuente()
        {
            InitializeComponent();

            _repositorioOrgaoAnuente = new RepositorioOrgaoAnuente();
            _repositorioProcesso = new RepositorioProcesso();

            this.Load += FrmModificaOrgaoAnuente_Load;
        }

        private void FrmModificaOrgaoAnuente_Load(object? sender, EventArgs e)
        {
            if (OrgaoAnuente == null || Processo == null)
            {
                MessageBox.Show("Erro: Os dados do órgão anuente ou do processo não foram carregados.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }
            tabControl1.TabPages.Clear();
            CarregarDados();
        }
        private void CarregarDados()
        {
            // Vincula dados do Processo (somente leitura)
            TXTnr.Text = Processo.Ref_USA;
            TXTsr.Text = Processo.SR;
            TXTimportador.Text = Processo.Importador;
            TXTexportador.Text = Processo.Exportador;
            TXTProduto.Text = Processo.Produto;
            TXTnr.ReadOnly = true;
            TXTsr.ReadOnly = true;
            TXTimportador.ReadOnly = true;
            TXTexportador.ReadOnly = true;
            TXTProduto.ReadOnly = true;

            // Vincula dados do OrgaoAnuente (editáveis)
            TXTpendencia.DataBindings.Add("Text", OrgaoAnuente, "Pendencia", false, DataSourceUpdateMode.OnPropertyChanged);
            TXTstatusdoprocesso.DataBindings.Add("Text", OrgaoAnuente, "StatusDoProcesso", false, DataSourceUpdateMode.OnPropertyChanged);
            SetupDatePickerBinding(DTPdatadeinspecao, OrgaoAnuente, nameof(OrgaoAnuente.Inspecao));

            // Vincula dados da Licença de Importação principal
            TxtLi.DataBindings.Add("Text", OrgaoAnuente.Licenca, "Numero", false, DataSourceUpdateMode.OnPropertyChanged);
            TxtNCM.DataBindings.Add("Text", OrgaoAnuente.Licenca, "NCM", false, DataSourceUpdateMode.OnPropertyChanged);
            SetupDatePickerBinding(DtpDataRegistro, OrgaoAnuente.Licenca, nameof(OrgaoAnuente.Licenca.DataRegistro));

            // Carrega as abas de LPCO existentes
            foreach (var lpco in OrgaoAnuente.Licenca.LPCO)
            {
                AdicionarAbaLpco(lpco);
            }
        }
        private void AdicionarAbaLpco(LpcoInfo lpco)
        {
            var tabPage = new TabPage(lpco.NomeOrgao) { Name = $"tabPage_{lpco.NomeOrgao}", Tag = lpco };
            var lblLpco = new Label { Text = "LPCO:", Location = new Point(15, 20), AutoSize = true };
            var txtLpco = new TextBox { Name = "txtLpco", Location = new Point(120, 17), Width = 150 };
            var lblParam = new Label { Text = "Parametrização:", Location = new Point(15, 50), AutoSize = true };
            var cmbParam = new ComboBox
            {
                Name = "cmbParam",
                Location = new Point(120, 47),
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList // Garante que o usuário só possa escolher as opções
            };
            // Adiciona as opções ao ComboBox
            cmbParam.Items.AddRange(new string[] { "", "Documental", "Exame Físico" });
            var lblDataReg = new Label { Text = "Data Registro:", Location = new Point(300, 20), AutoSize = true };
            var dtpDataReg = new DateTimePicker { Name = "dtpDataReg", Location = new Point(410, 17), Width = 120, Format = DateTimePickerFormat.Short };
            var lblDataDef = new Label { Text = "Data Deferimento:", Location = new Point(300, 50), AutoSize = true };
            var dtpDataDef = new DateTimePicker { Name = "dtpDataDef", Location = new Point(410, 47), Width = 120, Format = DateTimePickerFormat.Short };

            tabPage.Controls.AddRange(new Control[] { lblLpco, txtLpco, lblParam, cmbParam, lblDataReg, dtpDataReg, lblDataDef, dtpDataDef });

            txtLpco.DataBindings.Add("Text", lpco, nameof(LpcoInfo.LPCO), false, DataSourceUpdateMode.OnPropertyChanged);
            cmbParam.DataBindings.Add("Text", lpco, nameof(LpcoInfo.ParametrizacaoLPCO), false, DataSourceUpdateMode.OnPropertyChanged);
            SetupDatePickerBinding(dtpDataReg, lpco, nameof(LpcoInfo.DataRegistroLPCO));
            SetupDatePickerBinding(dtpDataDef, lpco, nameof(LpcoInfo.DataDeferimentoLPCO));

            tabControl1.TabPages.Add(tabPage);
        }
        private void BtnNovoOrgaoAnuente_Click(object? sender, EventArgs e)
        {
            if (CBOrgaoAnuente.SelectedItem == null)
            {
                MessageBox.Show("Selecione um órgão anuente na lista.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string nomeOrgao = CBOrgaoAnuente.SelectedItem.ToString();
            if (tabControl1.TabPages.ContainsKey($"tabPage_{nomeOrgao}"))
            {
                MessageBox.Show($"Já existe uma aba para o órgão '{nomeOrgao}'.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                tabControl1.SelectTab($"tabPage_{nomeOrgao}");
                return;
            }
            var novoLpco = new LpcoInfo { NomeOrgao = nomeOrgao };
            OrgaoAnuente.Licenca.LPCO.Add(novoLpco);
            AdicionarAbaLpco(novoLpco);
            tabControl1.SelectTab($"tabPage_{nomeOrgao}");
        }

        private void SalvarDadosDosControles()
        {
            if (OrgaoAnuente == null) return;

            // --- Salva dados dos campos principais do OrgaoAnuente ---
            OrgaoAnuente.Pendencia = TXTpendencia.Text;
            OrgaoAnuente.StatusDoProcesso = TXTstatusdoprocesso.Text;

            // Para o DateTimePicker, verificamos se ele foi habilitado pelo usuário
            if (this.Controls.Find($"chk_{DTPdatadeinspecao.Name}", true).FirstOrDefault() is CheckBox chkInspecao)
            {
                OrgaoAnuente.Inspecao = chkInspecao.Checked ? DTPdatadeinspecao.Value : null;
            }

            // --- Salva dados da Licença de Importação principal ---
            OrgaoAnuente.Licenca.Numero = TxtLi.Text;
            OrgaoAnuente.Licenca.NCM = TxtNCM.Text;
            if (this.Controls.Find($"chk_{DtpDataRegistro.Name}", true).FirstOrDefault() is CheckBox chkDataReg)
            {
                OrgaoAnuente.Licenca.DataRegistro = chkDataReg.Checked ? DtpDataRegistro.Value : null;
            }
            // Supondo que o 'checkBox1' que você tinha antes era para a Amostra
            // OrgaoAnuente.Licenca.Amostra = checkBox1.Checked;


            // --- Salva dados das abas de LPCO criadas dinamicamente ---
            foreach (TabPage tabPage in tabControl1.TabPages)
            {
                if (tabPage.Tag is not LpcoInfo lpco) continue; // Pula se a aba não tiver um LpcoInfo associado

                // Encontra os controles dentro da aba pelo nome que demos a eles
                var txtLpco = tabPage.Controls.Find("txtLpco", true).FirstOrDefault() as TextBox;
                var cmbParam = tabPage.Controls.Find("cmbParam", true).FirstOrDefault() as ComboBox;
                var dtpDataReg = tabPage.Controls.Find("dtpDataReg", true).FirstOrDefault() as DateTimePicker;
                var dtpDataDef = tabPage.Controls.Find("dtpDataDef", true).FirstOrDefault() as DateTimePicker;

                // Atribui os valores dos controles de volta para o objeto LpcoInfo
                if (txtLpco != null) lpco.LPCO = txtLpco.Text;
                if (cmbParam != null) lpco.ParametrizacaoLPCO = cmbParam.Text;

                // Para as datas, usamos o mesmo truque do CheckBox invisível para saber se foram preenchidas
                if (this.Controls.Find($"chk_{dtpDataReg.Name}", true).FirstOrDefault() is CheckBox chkLpcoReg)
                {
                    lpco.DataRegistroLPCO = chkLpcoReg.Checked ? dtpDataReg.Value : null;
                }
                if (this.Controls.Find($"chk_{dtpDataDef.Name}", true).FirstOrDefault() is CheckBox chkLpcoDef)
                {
                    lpco.DataDeferimentoLPCO = chkLpcoDef.Checked ? dtpDataDef.Value : null;
                }
            }
        }

        private async void BtnOK_Click(object? sender, EventArgs e)
        {
            if (OrgaoAnuente == null || Processo == null) return;

            try
            {
                // 1. Chama o método para garantir que os objetos C# estão atualizados com a tela
                SalvarDadosDosControles();

                // 2. Atualiza o documento na coleção OrgaosAnuentes
                await _repositorioOrgaoAnuente.UpdateAsync(OrgaoAnuente.Ref_USA, OrgaoAnuente.Tipo, OrgaoAnuente);

                // 3. Busca a versão mais recente do processo principal
                var processoParaAtualizar = await _repositorioProcesso.GetByRefUsaAsync(OrgaoAnuente.Ref_USA);
                if (processoParaAtualizar != null)
                {
                    // 4. Sincroniza as informações alteradas
                    processoParaAtualizar.Pendencia = OrgaoAnuente.Pendencia;
                    processoParaAtualizar.HistóricoDoProcesso = OrgaoAnuente.StatusDoProcesso;

                    // Encontra a LI correspondente no processo e a atualiza
                    var liNoProcesso = processoParaAtualizar.LI.FirstOrDefault(li => li.Numero == OrgaoAnuente.Licenca.Numero);
                    if (liNoProcesso != null)
                    {
                        // Atualiza as propriedades da LI dentro da lista do processo
                        liNoProcesso.NCM = OrgaoAnuente.Licenca.NCM;
                        liNoProcesso.DataRegistro = OrgaoAnuente.Licenca.DataRegistro;
                        liNoProcesso.Amostra = OrgaoAnuente.Licenca.Amostra;
                        liNoProcesso.LPCO = OrgaoAnuente.Licenca.LPCO;
                    }

                    // 5. Salva as alterações no processo principal
                    await _repositorioProcesso.UpdateAsync(processoParaAtualizar);
                }

                MessageBox.Show("Alterações salvas com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar as alterações: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancelar_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void SetupDatePickerBinding(DateTimePicker dtp, object dataSource, string dataMember)
        {
            var chk = new CheckBox { Visible = false };
            this.Controls.Add(chk);
            var binding = new Binding("Value", dataSource, dataMember, true, DataSourceUpdateMode.OnPropertyChanged);
            binding.Format += (s, args) => { if (args.Value == null) args.Value = DateTime.Now; };
            dtp.DataBindings.Add(binding);
            dtp.DataBindings.Add("Enabled", chk, "Checked");
            chk.DataBindings.Add("Checked", dataSource, dataMember, true, DataSourceUpdateMode.OnPropertyChanged);
            dtp.Format = DateTimePickerFormat.Custom;
            dtp.CustomFormat = " ";
            dtp.ValueChanged += (s, args) => { dtp.CustomFormat = dtp.Enabled ? "dd/MM/yyyy" : " "; };
            if (chk.Checked) dtp.CustomFormat = "dd/MM/yyyy";
        }
    }
}