using CLUSA;
using MongoDB.Driver;
using System.Data;

namespace Trabalho
{
    public partial class FrmModificaOrgaoAnuente : Form
    {
        public OrgaoAnuente OrgaoAnuente { get; set; }
        public Processo Processo { get; set; }
        private bool _dadosForamAlterados = false;

        public bool IsViewOnly { get; set; } = false;
        private readonly RepositorioOrgaoAnuente _repositorioOrgaoAnuente;
        private readonly RepositorioProcesso _repositorioProcesso;
        private readonly RepositorioVistorias _repositorioVistorias;

        public FrmModificaOrgaoAnuente(RepositorioOrgaoAnuente repositorioOrgaoAnuente, RepositorioProcesso repositorioProcesso)
        {
            InitializeComponent();

            var client = new MongoClient(ConfigDatabase.MongoConnectionString);
            var database = client.GetDatabase(ConfigDatabase.MongoDatabaseName);

            _repositorioVistorias = new RepositorioVistorias(database);
            _repositorioOrgaoAnuente = repositorioOrgaoAnuente;
            _repositorioProcesso = repositorioProcesso;
            OrgaoAnuente = new OrgaoAnuente();
            Processo = new Processo();
        }

        private void FrmModificaOrgaoAnuente_Load(object? sender, EventArgs e)
        {
            if (OrgaoAnuente == null || Processo == null)
            {
                MessageBox.Show("Erro: Os dados não foram carregados.", "Erro");
                this.Close();
                return;
            }
            CarregarDados();
            if (IsViewOnly)
            {
                ConfigurarModoVisualizacao();
            }
            else
            {
                AnexarEventoDeAlteracao(this);
            }
            AtualizarEstadoBotoesLpco();
        }
        private void ConfigurarModoVisualizacao()
        {
            // Altera o título do formulário
            this.Text = $"Visualizando LI - {OrgaoAnuente.Numero}";

            // Esconde os botões que permitem modificação
            BtnOK.Visible = false;
            BtnNovoOrgaoAnuente.Visible = false;
            CBOrgaoAnuente.Visible = false;
            BtnExcluirLpco.Visible = false;

            // Altera o botão Cancelar para um botão de Fechar claro
            BtnCancelar.Text = "Fechar";

            // Chama a função recursiva para desabilitar todos os controles a partir do próprio formulário
            DesabilitarControlesRecursivamente(this);
        }
        private void AtualizarEstadoBotoesLpco()
        {
            // O botão de excluir só fica ativo se houver pelo menos uma aba no controle.
            BtnExcluirLpco.Enabled = tabControl1.TabCount > 0;
        }

        /// <summary>
        /// Percorre recursivamente todos os controles de um controle pai e os desabilita.
        /// </summary>
        /// <param name="parent">O controle a partir do qual a busca e desabilitação começarão.</param>
        private void DesabilitarControlesRecursivamente(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                // Para TextBoxes, ReadOnly é melhor que Enabled=false, pois permite copiar o texto.
                if (c is TextBox box)
                {
                    box.ReadOnly = true;
                }
                // Para outros tipos de controle, desabilitamos diretamente.
                else if (c is ComboBox || c is DateTimePicker || c is CheckBox || c is NumericUpDown || c is CheckedListBox || c is Button)
                {
                    // Ignora o botão de fechar para que o usuário possa sair
                    if (c.Name != "BtnCancelar")
                    {
                        c.Enabled = false;
                    }
                }

                // Se o controle atual tiver outros controles dentro dele (como um GroupBox ou TabPage),
                // chama a função novamente para eles.
                if (c.HasChildren)
                {
                    DesabilitarControlesRecursivamente(c);
                }
            }
        }
        private void MarcarComoAlterado(object? sender, EventArgs e)
        {
            // Uma vez que algo muda, a bandeira é levantada e permanece assim até salvarmos.
            if (!_dadosForamAlterados)
            {
                _dadosForamAlterados = true;
                this.Text += "*"; // Opcional: Adiciona um "*" no título para indicar alterações
            }
        }

        private void AnexarEventoDeAlteracao(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                switch (c)
                {
                    case TextBox box: box.TextChanged += MarcarComoAlterado; break;
                    case ComboBox box: box.SelectedIndexChanged += MarcarComoAlterado; break;
                    case DateTimePicker dtp: dtp.ValueChanged += MarcarComoAlterado; break;
                    case CheckBox chk: chk.CheckedChanged += MarcarComoAlterado; break;
                    case NumericUpDown num: num.ValueChanged += MarcarComoAlterado; break;
                    case CheckedListBox clb: clb.ItemCheck += (s, e) => MarcarComoAlterado(s, e); break;
                }

                // Faz o mesmo para controles dentro de outros containers (ex: GroupBox)
                if (c.HasChildren)
                {
                    AnexarEventoDeAlteracao(c);
                }
            }
        }
        private void frmModificaOrgaoAnuente_FormClosing(object? sender, FormClosingEventArgs e)
        {
            // Se a bandeira de alterações estiver levantada...
            if (_dadosForamAlterados)
            {
                // ...pergunta ao usuário o que fazer.
                var resultado = MessageBox.Show(
                    "Você tem alterações não salvas. Deseja fechar e descartar as alterações?",
                    "Atenção",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                // Se o usuário escolher "Não", nós cancelamos o fechamento do formulário.
                if (resultado == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
            else { this.DialogResult = DialogResult.OK; }
            // Se a bandeira não estiver levantada, o formulário fecha normalmente sem perguntar nada.
        }
        private void CarregarDados()
        {
            this.Text = string.IsNullOrWhiteSpace(OrgaoAnuente.Numero)
                ? "Detalhes da LI"
        : $"LI - {OrgaoAnuente.Numero}";

            // --- Carrega dados de CONTEXTO (somente leitura) ---
            TXTnr.Text = Processo.Ref_USA;
            TXTsr.Text = Processo.SR;
            TXTimportador.Text = Processo.Importador;
            TXTexportador.Text = Processo.Exportador;
            TXTProduto.Text = Processo.Produto;
            TXTstatusdoprocesso.Text = Processo.HistoricoDoProcesso;
            TXTpendencia.Text = Processo.Pendencia;
            TxtTerminal.Text = Processo.Terminal;

            // Trava os campos de contexto
            new List<TextBox> { TXTnr, TXTsr, TXTimportador, TXTexportador, TXTProduto, TxtTerminal }.ForEach(txt => txt.ReadOnly = true);

            // --- Carrega dados EDITÁVEIS do OrgaoAnuente (a LI) ---
            TXTpendencia.DataBindings.Add("Text", OrgaoAnuente, nameof(OrgaoAnuente.Pendencia), false, DataSourceUpdateMode.OnPropertyChanged);
            TXTstatusdoprocesso.DataBindings.Add("Text", OrgaoAnuente, nameof(OrgaoAnuente.HistoricoDoProcesso), false, DataSourceUpdateMode.OnPropertyChanged);

            DTPdatadeinspecao.Enabled = false;
            ConfigurarDatePickerNulavel(DTPdatadeinspecao, OrgaoAnuente.Inspecao);

            // --- Carrega dados da LI nos campos do GroupBox "Dados Li" ---
            TxtLi.DataBindings.Add("Text", OrgaoAnuente, nameof(OrgaoAnuente.Numero), false, DataSourceUpdateMode.OnPropertyChanged);
            TxtNCM.DataBindings.Add("Text", OrgaoAnuente, nameof(OrgaoAnuente.NCM), false, DataSourceUpdateMode.OnPropertyChanged);
            ConfigurarDatePickerNulavel(DtpDataRegistro, OrgaoAnuente.DataRegistro);

            TxtLi.ReadOnly = true;
            TxtNCM.ReadOnly = true;
            // --- Carrega as abas de LPCO ---
            CarregarAbasLpco();
        }
        private void AdicionarAbaLpco(LpcoInfo lpco)
        {
            var tabPage = new TabPage(lpco.NomeOrgao)
            {
                Name = $"{lpco.NomeOrgao}",
                Tag = lpco, // Guarda o objeto de dados
                BackColor = SystemColors.Control
            };

            // 1. Cria uma instância do seu novo UserControl.
            var editorLpco = new LpcoEditControl
            {
                Dock = DockStyle.Fill,
                Tag = lpco
            };

            // 2. Chama o método para vincular os dados.
            editorLpco.VincularDados(lpco);

            // 3. Adiciona o UserControl à aba.
            tabPage.Controls.Add(editorLpco);

            // 4. Adiciona a aba ao TabControl.
            tabControl1.TabPages.Add(tabPage);
        }
        private void BtnNovoOrgaoAnuente_Click(object? sender, EventArgs e)
        {
            if (CBOrgaoAnuente.SelectedIndex != 0)
            {
                string? nomeOrgao = CBOrgaoAnuente.SelectedItem?.ToString();
                if (string.IsNullOrEmpty(nomeOrgao)) { MessageBox.Show("Selecione um órgão."); return; }
                if (OrgaoAnuente.LPCO.Any(l => l.NomeOrgao == nomeOrgao)) { MessageBox.Show($"Já existe um LPCO para o órgão '{nomeOrgao}'."); return; }

                var novoLpco = new LpcoInfo { NomeOrgao = nomeOrgao };
                OrgaoAnuente.LPCO.Add(novoLpco);
                AdicionarAbaLpco(novoLpco);
                tabControl1.SelectTab(tabControl1.TabPages.Count - 1);
                AtualizarEstadoBotoesLpco();
            }
            else
            {
                MessageBox.Show("Selecione um órgão."); return;
            }
            _dadosForamAlterados = true;
            this.Text += "*";
        }
        private void BtnExcluirLpco_Click(object sender, EventArgs e)
        {
            // 1. Verifica se existe alguma aba selecionada para excluir.
            if (tabControl1.TabCount == 0 || tabControl1.SelectedTab == null)
            {
                MessageBox.Show("Nenhum LPCO selecionado para excluir.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 2. Pega a aba e o objeto LpcoInfo associado a ela (guardado na propriedade Tag).
            var abaSelecionada = tabControl1.SelectedTab;
            if (abaSelecionada.Tag is not LpcoInfo lpcoParaExcluir)
            {
                MessageBox.Show("Erro ao identificar os dados do LPCO selecionado.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 3. Pede confirmação ao usuário ANTES de excluir.
            var resultado = MessageBox.Show(
                $"Tem certeza que deseja excluir o LPCO para o órgão '{lpcoParaExcluir.NomeOrgao}'?\n\nEsta ação não poderá ser desfeita.",
                "Confirmar Exclusão",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            // Se o usuário clicar em "Não", a função para aqui.
            if (resultado == DialogResult.No)
            {
                return;
            }

            // 4. Se o usuário confirmou, executa a exclusão.
            // Remove da lista de dados...
            OrgaoAnuente.LPCO.Remove(lpcoParaExcluir);

            // ...e remove da interface gráfica.
            tabControl1.TabPages.Remove(abaSelecionada);

            // 5. Marca que houve uma alteração nos dados para o sistema de "salvar ao fechar".
            MarcarComoAlterado(sender, e);

            AtualizarEstadoBotoesLpco();
            _dadosForamAlterados = true;
            this.Text += "*";
        }
        private async Task SalvarDadosDosControlesAsync()
        {
            this.ValidateChildren(); // Força a atualização de todos os bindings

            OrgaoAnuente.Inspecao = DTPdatadeinspecao.Checked ? DTPdatadeinspecao.Value : null;
            OrgaoAnuente.DataRegistro = DtpDataRegistro.Checked ? DtpDataRegistro.Value : null;

            foreach (TabPage abaLpco in tabControl1.TabPages)
            {
                if (abaLpco.Tag is not LpcoInfo lpco) continue;

                // AGORA ESTA PARTE VAI FUNCIONAR CORRETAMENTE
                if (abaLpco.Controls.Find("TxtLPCO", true).FirstOrDefault() is TextBox txtLpcoNum)
                    lpco.LPCO = txtLpcoNum.Text;

                if (abaLpco.Controls.Find("CbParametrizacao", true).FirstOrDefault() is ComboBox cmbParam)
                    lpco.ParametrizacaoLPCO = cmbParam.Text;

                if (abaLpco.Controls.Find("CbEmExigencia", true).FirstOrDefault() is CheckBox CbEmExigencia)
                    lpco.EmExigencia = CbEmExigencia.Checked;

                if (abaLpco.Controls.Find("CbMotivoExigencia", true).FirstOrDefault() is ComboBox CbMotivoExigencia)
                    lpco.MotivoExigencia = CbMotivoExigencia.Text;

                if (abaLpco.Controls.Find("DtpDataRegistroLPCO", true).FirstOrDefault() is DateTimePicker dtpLpcoReg)
                    lpco.DataRegistroLPCO = dtpLpcoReg.Checked ? dtpLpcoReg.Value : null;

                if (abaLpco.Controls.Find("DtpDataDeferimentoLPCO", true).FirstOrDefault() is DateTimePicker dtpLpcoDef)
                    lpco.DataDeferimentoLPCO = dtpLpcoDef.Checked ? dtpLpcoDef.Value : null;

                if (lpco.MotivoExigencia.ToUpperInvariant() == "CANCELADA" && !string.IsNullOrWhiteSpace(lpco.LPCO))
                {
                    var v = await _repositorioVistorias.GetByLPCOAsync(lpco.LPCO);
                    if (v != null)
                    {
                        await _repositorioVistorias.DeleteByLpcoAsync(lpco.LPCO);
                    }
                }
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
                dtp.Format = DateTimePickerFormat.Custom;
                dtp.CustomFormat = " ";
            }
            dtp.ValueChanged += (s, e) =>
            {
                if (s is DateTimePicker picker)
                {
                    picker.Format = picker.Checked ? DateTimePickerFormat.Short : DateTimePickerFormat.Custom;
                }
            };
        }
        private async void BtnOK_Click(object? sender, EventArgs e)
        {
            try
            {
                await SalvarDadosDosControlesAsync();

                await _repositorioOrgaoAnuente.UpdateAsync(OrgaoAnuente);

                var liNoProcesso = Processo.LI.FirstOrDefault(li => li.Numero == OrgaoAnuente.Numero);
                if (liNoProcesso != null)
                {
                    liNoProcesso.NCM = OrgaoAnuente.NCM;
                    liNoProcesso.DataRegistro = OrgaoAnuente.DataRegistro;
                    liNoProcesso.LPCO = OrgaoAnuente.LPCO;
                }

                // Atualiza campos do Processo que podem ter sido editados nesta tela
                Processo.HistoricoDoProcesso = OrgaoAnuente.HistoricoDoProcesso;
                Processo.Pendencia = OrgaoAnuente.Pendencia;

                // 4. Salva o objeto Processo principal, agora com a LI interna e outros dados atualizados.
                await _repositorioProcesso.UpdateAsync(Processo);

                // 5. Finaliza a operação com sucesso
                _dadosForamAlterados = false;
                this.Text = this.Text.Replace("*", "");

                MessageBox.Show("Alterações salvas com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocorreu um erro ao salvar as alterações: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void CarregarAbasLpco()
        {
            tabControl1.TabPages.Clear(); // Limpa as abas de exemplo do designer
            foreach (var lpco in OrgaoAnuente.LPCO)
            {
                AdicionarAbaLpco(lpco);
            }
        }

        private void BtnCancelar_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}