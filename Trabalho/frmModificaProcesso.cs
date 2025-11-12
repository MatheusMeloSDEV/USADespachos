using CLUSA;
using System.Data;
using System.Diagnostics;

namespace Trabalho
{
    public enum OrigemProcesso
    {
        Santos, // Para processos gerais
        Itajai  // Para processos com sufixo ITJ
    }
    public partial class FrmModificaProcesso : Form
    {
        public Processo processo { get; set; }
        public string Modo { get; set; } = "Adicionar"; // Valor padrão
        public bool Visualização { get; set; } = false;
        public OrigemProcesso Origem { get; set; }
        private readonly RepositorioProcesso _repositorio;
        private bool _dadosForamAlterados = false;

        public FrmModificaProcesso()
        {
            InitializeComponent();
            _repositorio = new RepositorioProcesso();
        }

        private void FrmModificaProcesso_Load(object? sender, EventArgs e)
        {
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            switch (Origem)
            {
                case OrigemProcesso.Itajai:
                    // 'L' significa Letra, 'I' e 'T' são letras fixas, 'J' é opcional
                    TXTnr.Mask = @"0000/00\I\TJ";
                    break;
                case OrigemProcesso.Santos:
                default:
                    TXTnr.Mask = "0000/00";
                    break;
            }
            CarregarDadosNosControles();
            ConfigurarFormularioPeloModo();
            AnexarEventoDeAlteracao(this);
            AtualizarEstadoBotoesLI();
        }
        private void AtualizarEstadoBotoesLI()
        {
            // O botão de excluir só fica ativo se houver pelo menos uma aba no controle.
            BtnExcluirLI.Enabled = TCLi.TabCount > 0;
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
        private void frmModificaProcesso_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (_dadosForamAlterados)
            {
                var resultado = MessageBox.Show(
                    "Você tem alterações não salvas. Deseja fechar e descartar?",
                    "Atenção",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (resultado == DialogResult.No)
                {
                    e.Cancel = true; // Cancela o fechamento
                }
            }
        }   
        #region "Configuração, Carregamento e Salvamento"

        private void ConfigurarFormularioPeloModo()
        {
            this.Text = $"{Modo} Processo";
            if (Modo == "Editar")
            {
                TXTnr.Enabled = false; // Não permite editar a Ref. USA
            }
            else if (Modo == "Adicionar")
            {
                btnCapa.Enabled = false;
                btnRelatorio.Enabled = false;
                if (processo.LI == null || !processo.LI.Any())
                {
                    if (processo.LI == null)
                    {
                        processo.LI = new List<LicencaImportacao>();
                    }
                    processo.LI.Add(new LicencaImportacao { Numero = "Nova LI" });
                }
            }

            if (Visualização)
            {
                SetCamposSomenteLeitura(this);
                btnAdiciona.Visible = false;
            }
        }

        private void CarregarDadosNosControles()
        {
            BsModificaProcesso.DataSource = processo;
            CarregarControlesDeData();
            CarregarCheckedListBoxes();
            PopularMarca();
            CarregarAbasLi();
        }

        private void SalvarDadosDosControles()
        {
            BsModificaProcesso.EndEdit();
            this.ValidateChildren();

            // --- Salva dados gerais ---
            processo.DocRecebidos = ObterItensSelecionados(checkedListBox1);
            processo.FormaRecOriginais = checkedListBox2.CheckedItems.Count > 0 ? checkedListBox2.CheckedItems[0]?.ToString() ?? "" : "";
            processo.Marca = (new[] { "Sacos", "Caixas", "Pallets" }.Contains(cbMarca.Text))
                ? $"{numMarca.Value} {cbMarca.Text}"
                : $"{numMarca.Value} x {cbMarca.Text}";

            // --- Salva as datas do processo principal ---
            processo.DataRegistroDI = DTPdataderegistrodi.Checked ? DTPdataderegistrodi.Value : null;
            processo.DataDesembaracoDI = DTPdatadedesembaracodi.Checked ? DTPdatadedesembaracodi.Value : null;
            processo.DataCarregamentoDI = DTPdatadecarregamentodi.Checked ? DTPdatadecarregamentodi.Value : null;
            processo.Inspecao = DTPdatadeinspecao.Checked ? DTPdatadeinspecao.Value : null;
            processo.DataDeAtracacao = DTPdatadeatracacao.Checked ? DTPdatadeatracacao.Value : null;
            processo.DataEmbarque = DTPdatadeembarque.Checked ? DTPdatadeembarque.Value : null;
            processo.DataRecebOriginais = DTPDataRecOriginais.Checked ? DTPDataRecOriginais.Value : null;
            processo.DataMinutaDI = dtpDataMinuta.Checked ? dtpDataMinuta.Value : null;

            if (DTPdatadeatracacao.Checked)
            {
                processo.VencimentoFMA = DataHelper.CalcularVencimento(DTPdatadeatracacao.Value, 85);
                dtpVencimentoFMA.Value = processo.VencimentoFMA ?? dtpVencimentoFMA.Value;
            }
            else
            {
                processo.VencimentoFMA = null;
            }

            if (DTPdatadeatracacao.Checked)
            {
                processo.VencimentoFreeTime = DataHelper.CalcularVencimento(DTPdatadeatracacao.Value, Convert.ToInt32(NUMfreetime.Value));
                dtpVencimentoFreeTime.Value = processo.VencimentoFreeTime ?? dtpVencimentoFreeTime.Value;
            }
            else
            {
                processo.VencimentoFreeTime = null;
            }

            DateTime? dataMaisAntiga = null;

            if (processo.LI != null && processo.LI.Count > 0)
            {
                dataMaisAntiga = processo.LI
                    .Where(li => li.DataRegistro.HasValue)
                    .Min(li => li.DataRegistro);
            }

            if (dataMaisAntiga.HasValue)
            {
                processo.VencimentoLI_LPCO = DataHelper.CalcularVencimento(dataMaisAntiga.Value, 80);
                if (processo.VencimentoLI_LPCO.HasValue)
                {
                    dtpVencimentoLI_LPCO.Value = processo.VencimentoLI_LPCO.Value;
                }
                else
                {
                    dtpVencimentoLI_LPCO.Value = DateTime.Today;
                    dtpVencimentoLI_LPCO.Format = DateTimePickerFormat.Custom;
                    dtpVencimentoLI_LPCO.CustomFormat = " ";
                }
            }
            else
            {
                processo.VencimentoLI_LPCO = null;
            }

            processo.HistoricoDoProcesso = TXTstatusdoprocesso.Text;
            processo.Pendencia = TXTpendencia.Text;

            if (processo.Capa == null)
            {
                processo.Capa = new Capa();
            }

            // Copia os valores dos campos principais do Processo para os campos correspondentes na Capa.
            processo.Capa.Container = processo.Container;
            processo.Capa.Master = processo.Veiculo;
            processo.Capa.SigvigSelecionado = processo.SIGVIGSelecionado;
            processo.Capa.SigvigLiberado = processo.SIGVIGLiberado;

            // --- Salva os dados das abas dinâmicas de LI e LPCO ---
            foreach (TabPage abaLi in TCLi.TabPages)
            {
                if (abaLi.Controls.OfType<LIEditControl>().FirstOrDefault() is LIEditControl liControl)
                {
                    liControl.SalvarAlteracoes();
                }
            }

            processo.LI.RemoveAll(li => string.IsNullOrWhiteSpace(li.Numero) || li.Numero == "Nova LI");

            // 🆕 ATUALIZA A CONDIÇÃO DO PROCESSO AUTOMATICAMENTE
            ProcessoHelper.AtualizarCondicaoProcesso(processo);
        }
        private async void btnAdiciona_Click(object? sender, EventArgs e)
        {
            try
            {
                // --- VALIDAÇÃO: Verificar se Ref_USA já existe (APENAS no modo Adicionar) ---
                if (Modo == "Adicionar" && !string.IsNullOrWhiteSpace(TXTnr.Text))
                {
                    bool refUsaExiste = await _repositorio.VerificarRefUsaExisteAsync(TXTnr.Text);

                    if (refUsaExiste)
                    {
                        MessageBox.Show(
                            $"A Ref_USA '{TXTnr.Text}' já existe no banco de dados!\n\nPor favor, utilize uma referência diferente.",
                            "Ref_USA Duplicada",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );

                        TXTnr.Clear(); // Limpa o campo
                        TXTnr.Focus(); // Coloca o foco no campo para nova digitação
                        return; // Para a execução aqui
                    }
                }

                // Se passou na validação, continua salvando
                SalvarDadosDosControles();

                if (Modo == "Adicionar")
                {
                    await _repositorio.CreateAsync(processo);
                    Modo = "Editar"; // Depois de criar, o modo muda para edição
                    TXTnr.Enabled = false;
                }
                else // Modo "Editar"
                {
                    await _repositorio.UpdateAsync(processo);
                }

                btnCapa.Enabled = true;
                btnRelatorio.Enabled = true;

                _dadosForamAlterados = false;
                this.Text = this.Text.Replace("*", "");
                MessageBox.Show("Processo salvo com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar o processo: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        #endregion

        #region "Gerenciamento Dinâmico de LI/LPCO"

        private void CarregarAbasLi()
        {
            TCLi.TabPages.Clear();
            foreach (var li in processo.LI)
            {
                AdicionarAbaLi(li);
            }
        }

        private void AdicionarAbaLi(LicencaImportacao li)
        {
            // Cria a aba principal da LI.
            var tabPageLi = new TabPage($"LI - {li.Numero}")
            {
                Tag = li,
                BackColor = SystemColors.Control
            };

            // 1. Cria uma instância do nosso novo UserControl.
            var editorLi = new LIEditControl
            {
                Dock = DockStyle.Fill // Faz ele preencher a aba
            };

            // 2. Chama o método do UserControl para vincular os dados da LI.
            editorLi.VincularDados(li);

            // MUDANÇA: Atualiza o texto da aba de forma segura.
            var txtLiControl = editorLi.Controls.Find("TxtLi", true).FirstOrDefault() as TextBox;
            if (txtLiControl != null)
            {
                txtLiControl.TextChanged += (s, e) => {
                    tabPageLi.Text = $"LI - {txtLiControl.Text}";
                };
            }

            // 3. Adiciona o UserControl à aba.
            tabPageLi.Controls.Add(editorLi);

            // 4. Adiciona a aba de LI ao TabControl principal.
            TCLi.TabPages.Add(tabPageLi);
        }

        private void BtnLI_Click(object? sender, EventArgs e)
        {
            var novaLi = new LicencaImportacao { Numero = "Nova LI" };
            processo.LI.Add(novaLi);
            AdicionarAbaLi(novaLi);
            TCLi.SelectedIndex = TCLi.TabPages.Count - 1;
            _dadosForamAlterados = true;
            this.Text += "*";
        }

        private void BtnExcluirLi_Click(object sender, EventArgs e)
        {
            // 1. Verifica se existe alguma aba de LI selecionada para excluir.
            if (TCLi.TabCount == 0 || TCLi.SelectedTab == null)
            {
                MessageBox.Show("Nenhuma LI selecionada para excluir.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 2. Pega a aba e o objeto LicencaImportacao associado a ela.
            var abaSelecionada = TCLi.SelectedTab;
            if (abaSelecionada.Tag is not LicencaImportacao liParaExcluir)
            {
                MessageBox.Show("Erro ao identificar os dados da LI selecionada.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 3. Pede confirmação ao usuário.
            var resultado = MessageBox.Show(
                $"Tem certeza que deseja excluir a LI '{liParaExcluir.Numero}' e todos os seus LPCOs associados?",
                "Confirmar Exclusão de LI",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (resultado == DialogResult.No)
            {
                return;
            }

            // 4. Executa a exclusão nos dados e na interface.
            processo.LI.Remove(liParaExcluir);    // Remove da lista de dados do Processo
            TCLi.TabPages.Remove(abaSelecionada); // Remove a aba da tela

            // 5. Marca que houve uma alteração nos dados
            // MarcarComoAlterado(sender, e);
            MessageBox.Show("LI removida. As alterações serão salvas quando você salvar o processo.", "LI Removida", MessageBoxButtons.OK, MessageBoxIcon.Information);
            AtualizarEstadoBotoesLI();
            _dadosForamAlterados = true;
            this.Text += "*";
        }

        #endregion

        #region "Métodos Auxiliares"

        private void CarregarControlesDeData()
        {
            // Mapeia o controle à sua propriedade correspondente no objeto 'processo'
            ConfigurarDatePickerNulavel(dtpDataMinuta, processo.DataMinutaDI);
            ConfigurarDatePickerNulavel(DTPdataderegistrodi, processo.DataRegistroDI);
            ConfigurarDatePickerNulavel(DTPdatadedesembaracodi, processo.DataDesembaracoDI);
            ConfigurarDatePickerNulavel(DTPdatadecarregamentodi, processo.DataCarregamentoDI);
            ConfigurarDatePickerNulavel(DTPdatadeinspecao, processo.Inspecao);
            ConfigurarDatePickerNulavel(DTPdatadeatracacao, processo.DataDeAtracacao);
            ConfigurarDatePickerNulavel(DTPdatadeembarque, processo.DataEmbarque);
            ConfigurarDatePickerNulavel(DTPDataRecOriginais, processo.DataRecebOriginais);
            ConfigurarDatePickerNulavel(dtpVencimentoFMA, processo.VencimentoFMA);
            ConfigurarDatePickerNulavel(dtpVencimentoFreeTime, processo.VencimentoFreeTime);
            ConfigurarDatePickerNulavel(dtpVencimentoLI_LPCO, processo.VencimentoLI_LPCO);
        }

        private void ConfigurarDatePickerNulavel(DateTimePicker dtp, DateTime? data)
        {
            dtp.ShowCheckBox = true;

            // Se já existe uma data salva no banco, usa ela.
            if (data.HasValue)
            {
                dtp.Checked = true;
                dtp.Value = data.Value;
                dtp.Format = DateTimePickerFormat.Short;
            }
            else // Se for um objeto novo (data == null)
            {
                dtp.Checked = false; // Começa desmarcado

                // MUDANÇA PRINCIPAL: Define o valor subjacente para a data de hoje.
                // Assim, se o usuário marcar a caixa, a data que aparecerá será a de hoje,
                // e não uma data antiga do designer.
                dtp.Value = DateTime.Today;

                dtp.Format = DateTimePickerFormat.Custom;
                dtp.CustomFormat = " "; // Deixa visualmente em branco
            }

            // O evento para formatar a aparência continua o mesmo.
            dtp.ValueChanged -= Dtp_ValueChanged_Format;
            dtp.ValueChanged += Dtp_ValueChanged_Format;
        }

        // Este evento agora só cuida da FORMATAÇÃO VISUAL.
        private void Dtp_ValueChanged_Format(object? sender, EventArgs e)
        {
            if (sender is DateTimePicker picker)
            {
                picker.Format = picker.Checked ? DateTimePickerFormat.Short : DateTimePickerFormat.Custom;
            }
        }

        private void CarregarCheckedListBoxes()
        {
            // 1. Lida com o CheckedListBox de multi-seleção ("Docs Recebidos")
            if (processo.DocRecebidos != null)
            {
                // Desmarca todos os itens primeiro para garantir uma carga limpa
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    checkedListBox1.SetItemChecked(i, false);
                }

                // Marca os itens que estão na lista do processo
                foreach (var item in processo.DocRecebidos)
                {
                    int index = checkedListBox1.Items.IndexOf(item);
                    if (index != -1)
                    {
                        checkedListBox1.SetItemChecked(index, true);
                    }
                }
            }

            // 2. Lida com o CheckedListBox de seleção única ("Forma Rec.")
            if (!string.IsNullOrEmpty(processo.FormaRecOriginais))
            {
                int index = checkedListBox2.Items.IndexOf(processo.FormaRecOriginais);
                if (index != -1)
                {
                    // Marca apenas o item correspondente
                    checkedListBox2.SetItemChecked(index, true);
                }
            }
        }
        private string[] ObterItensSelecionados(CheckedListBox clb) => clb.CheckedItems.OfType<string>().ToArray();
        private void PopularMarca()
        {
            string marcaCompleta = processo.Marca ?? string.Empty;
            string[] modulosEspacos = new[] { "Sacos", "Caixas", "Pallets" };

            string numMarcaStr = "";
            string textoMarca = "";

            if (modulosEspacos.Any(m => marcaCompleta.EndsWith(" " + m)))
            {
                // Formato "10 Sacos"
                int indexEspaco = marcaCompleta.LastIndexOf(' ');
                if (indexEspaco > 0)
                {
                    numMarcaStr = marcaCompleta.Substring(0, indexEspaco);
                    textoMarca = marcaCompleta.Substring(indexEspaco + 1);
                }
            }
            else if (marcaCompleta.Contains(" x "))
            {
                // Formato "2 x 40 HC"
                int indexX = marcaCompleta.IndexOf(" x ");
                if (indexX > 0)
                {
                    numMarcaStr = marcaCompleta.Substring(0, indexX);
                    textoMarca = marcaCompleta.Substring(indexX + 3);
                }
            }
            else
            {
                // Se não encontrar um padrão, assume que é tudo texto
                textoMarca = marcaCompleta;
            }

            // Atribui os valores aos controles
            if (decimal.TryParse(numMarcaStr, out decimal num))
            {
                numMarca.Value = Math.Clamp(num, numMarca.Minimum, numMarca.Maximum);
            }
            cbMarca.Text = textoMarca;
        }
        private void SetCamposSomenteLeitura(Control parent)
        {
            // Desabilita botões de ação que não fazem sentido no modo de visualização
            BtnLI.Enabled = false; // Exemplo de botão no designer
                                                 // Adicione outros botões que precisam ser desabilitados

            foreach (Control control in parent.Controls)
            {
                switch (control)
                {
                    case TextBox box: box.ReadOnly = true; break;
                    case MaskedTextBox box: box.ReadOnly = true; break;
                    case ComboBox box: box.Enabled = false; break;
                    case CheckBox box: box.Enabled = false; break;
                    case DateTimePicker picker: picker.Enabled = false; break;
                    case NumericUpDown num: num.Enabled = false; break;
                    case CheckedListBox list: list.Enabled = false; break;
                    case Button btn when btn.Name != "btnCancelar" && btn.Name != "btnRelatorio" && btn.Name != "btnCapa":
                        btn.Enabled = false; // Desabilita outros botões, exceto os de navegação/relatório
                        break;
                }

                // Chamada recursiva para controles dentro de GroupBoxes, Panels, TabPages, etc.
                if (control.HasChildren)
                {
                    SetCamposSomenteLeitura(control);
                }
            }
        }
        private void btnCapa_Click(object? sender, EventArgs e)
        {
            using var frm = new FrmModificaCapa
            {
                capa = processo.Capa ?? new Capa(),
                Modo = this.Modo,
                ref_usa = processo.Ref_USA,
                Visualizacao = this.Visualização
            };

            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                processo.Capa = frm.capa;
            }
            _dadosForamAlterados = true;
            this.Text += "*";
        }
        private void btnRelatorio_Click(object? sender, EventArgs e)
        {
            string referencia = TXTnr.Text;
            var progressForm = new ProgressForm();
            progressForm.Show(this);

            Task.Run(() =>
            {
                string? pdfPath = null;
                string? mensagemErro = null;
                try
                {
                    pdfPath = PythonRunner.ExecutarRelatorio(referencia).Trim();
                }
                catch (Exception ex)
                {
                    mensagemErro = $"Erro durante exportação: {ex.Message}";
                }

                Invoke(new Action(() =>
                {
                    progressForm.Close();
                    if (mensagemErro != null)
                    {
                        MessageBox.Show(mensagemErro, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    var resp = MessageBox.Show("Exportação concluída. Deseja abrir o PDF?", "Resultado", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (resp == DialogResult.Yes && !string.IsNullOrEmpty(pdfPath) && File.Exists(pdfPath))
                    {
                        Process.Start(new ProcessStartInfo { FileName = pdfPath, UseShellExecute = true });
                    }
                }));
            });
        }
        private void checkedListBox2_ItemCheck(object? sender, ItemCheckEventArgs e)
        {
            // Se o usuário está marcando um novo item...
            if (e.NewValue == CheckState.Checked)
            {
                // ...percorre todos os outros itens e os desmarca.
                for (int i = 0; i < checkedListBox2.Items.Count; i++)
                {
                    if (i != e.Index)
                    {
                        checkedListBox2.SetItemChecked(i, false);
                    }
                }
            }
        }

        #endregion

    }
}