using CLUSA;
using System.Data;
using System.Diagnostics;

namespace Trabalho
{
    public partial class FrmModificaProcesso : Form
    {
        public Processo processo { get; set; }
        public string Modo { get; set; } = "Adicionar"; // Valor padrão
        public bool Visualização { get; set; } = false;
        private bool _dadosForamAlterados = false;

        public FrmModificaProcesso()
        {
            InitializeComponent();
            processo = new(); // Garante que 'processo' nunca seja nulo
        }

        private void FrmModificaProcesso_Load(object? sender, EventArgs e)
        {
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            CarregarDadosNosControles();
            ConfigurarFormularioPeloModo();
            AnexarEventoDeAlteracao(this);
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
            // Se a bandeira não estiver levantada, o formulário fecha normalmente sem perguntar nada.
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
                // Inicializa o processo com uma LI em branco para o usuário preencher
                if (!processo.LI.Any())
                {
                    processo.LI.Add(new LicencaImportacao());
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

            processo.HistoricoDoProcesso = TXTstatusdoprocesso.Text;
            processo.Pendencia = TXTpendencia.Text;

            // --- (NOVO) Salva os dados das abas dinâmicas de LI e LPCO ---
            foreach (TabPage abaLi in TCLi.TabPages)
            {
                if (abaLi.Tag is not LicencaImportacao li) continue;

                // O DataBinding para TxtLi e TxtNCM geralmente funciona, mas vamos garantir.
                if (abaLi.Controls.Find("TxtLi", true).FirstOrDefault() is TextBox txtLi) li.Numero = txtLi.Text;
                if (abaLi.Controls.Find("TxtNCM", true).FirstOrDefault() is TextBox txtNcm) li.NCM = txtNcm.Text;
                if (abaLi.Controls.Find("DtpDataRegistro", true).FirstOrDefault() is DateTimePicker dtpLi)
                {
                    li.DataRegistro = dtpLi.Checked ? dtpLi.Value : null;
                }

                // Encontra o TabControl aninhado dos LPCOs
                var tabControlLpco = abaLi.Controls.OfType<TabControl>().FirstOrDefault();
                if (tabControlLpco != null)
                {
                    foreach (TabPage abaLpco in tabControlLpco.TabPages)
                    {
                        if (abaLpco.Tag is not LpcoInfo lpco) continue;

                        // AGORA ESTA PARTE VAI FUNCIONAR CORRETAMENTE
                        if (abaLpco.Controls.Find("txtLpcoNum", true).FirstOrDefault() is TextBox txtLpcoNum)
                            lpco.LPCO = txtLpcoNum.Text;

                        if (abaLpco.Controls.Find("cmbParam", true).FirstOrDefault() is ComboBox cmbParam)
                            lpco.ParametrizacaoLPCO = cmbParam.Text;

                        if (abaLpco.Controls.Find("dtpDataReg", true).FirstOrDefault() is DateTimePicker dtpLpcoReg)
                            lpco.DataRegistroLPCO = dtpLpcoReg.Checked ? dtpLpcoReg.Value : null;

                        if (abaLpco.Controls.Find("dtpDataDef", true).FirstOrDefault() is DateTimePicker dtpLpcoDef)
                            lpco.DataDeferimentoLPCO = dtpLpcoDef.Checked ? dtpLpcoDef.Value : null;
                    }
                }
            }
        }

        private void btnAdiciona_Click(object? sender, EventArgs e)
        {
            SalvarDadosDosControles();
            // Lógica de cálculo dos vencimentos
            if (processo.DataDeAtracacao.HasValue)
            {
                processo.VencimentoFreeTime = processo.DataDeAtracacao.Value.AddDays(processo.FreeTime);
                processo.VencimentoFMA = processo.DataDeAtracacao.Value.AddDays(85); // Exemplo
            }
            if (processo.LI.FirstOrDefault()?.DataRegistro.HasValue ?? false)
            {
                processo.VencimentoLI_LPCO = processo.LI.First().DataRegistro.Value.AddDays(85); // Exemplo
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
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
            var tabPageLi = new TabPage($"LI - {li.Numero}") { Tag = li, BackColor = SystemColors.Control };

            var cbOrgaos = new ComboBox { Location = new Point(415, 6), Width = 184, DropDownStyle = ComboBoxStyle.DropDownList };
            cbOrgaos.Items.AddRange(Enum.GetNames(typeof(TipoOrgaoAnuente)));
            var btnNovoLpco = new Button { Text = "Novo LPCO", Location = new Point(604, 6), Width = 129 };

            var tabControlLpco = new TabControl { Location = new Point(6, 3), Size = new Size(730, 137), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            btnNovoLpco.Click += (s, e) => BtnNovoLpco_Click(li, cbOrgaos, tabControlLpco);

            var groupBoxDadosLi = new GroupBox { Text = "Dados da LI", Location = new Point(6, 145), Size = new Size(730, 83), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            var lblLiNum = new Label { Text = "LI", Font = new Font("Segoe UI", 12F), Location = new Point(193, 19), AutoSize = true, Parent = groupBoxDadosLi };
            var txtLi = new TextBox { Name = "TxtLi", Location = new Point(137, 43), Width = 134, Parent = groupBoxDadosLi };
            var lblNcm = new Label { Text = "NCM", Font = new Font("Segoe UI", 12F), Location = new Point(335, 19), AutoSize = true, Parent = groupBoxDadosLi };
            var txtNcm = new TextBox { Name = "TxtNCM", Location = new Point(285, 43), Width = 147, Parent = groupBoxDadosLi };
            var lblDataRegLi = new Label { Text = "Data Registro", Font = new Font("Segoe UI", 12F), Location = new Point(463, 19), AutoSize = true, Parent = groupBoxDadosLi };
            var dtpRegistroLi = new DateTimePicker { Name = "DtpDataRegistro", Location = new Point(446, 43), Width = 135, Format = DateTimePickerFormat.Short, Parent = groupBoxDadosLi };

            txtLi.DataBindings.Add("Text", li, nameof(li.Numero), false, DataSourceUpdateMode.OnPropertyChanged);
            txtNcm.DataBindings.Add("Text", li, nameof(li.NCM), false, DataSourceUpdateMode.OnPropertyChanged);

            // MUDANÇA: Passando o valor da data diretamente.
            ConfigurarDatePickerNulavel(dtpRegistroLi, li.DataRegistro);

            txtLi.TextChanged += (s, e) => { tabPageLi.Text = $"LI - {txtLi.Text}"; };

            tabPageLi.Controls.AddRange(new Control[] { cbOrgaos, btnNovoLpco, tabControlLpco, groupBoxDadosLi });

            foreach (var lpco in li.LPCO)
            {
                AdicionarAbaLpco(tabControlLpco, lpco);
            }

            TCLi.TabPages.Add(tabPageLi);
        }

        private void AdicionarAbaLpco(TabControl parentTabControl, LpcoInfo lpco)
        {
            var tabPageLpco = new TabPage(lpco.NomeOrgao) { Tag = lpco, BackColor = SystemColors.Control };

            var lblLpcoNum = new Label { Text = "LPCO", Location = new Point(46, 28), AutoSize = true, Parent = tabPageLpco };
            var txtLpcoNum = new TextBox { Name = "txtLpcoNum", Location = new Point(90, 25), Width = 261, Parent = tabPageLpco };
            var lblParam = new Label { Text = "Parametrização", Location = new Point(46, 62), AutoSize = true, Parent = tabPageLpco };
            var cmbParam = new ComboBox { Name = "cmbParam", Location = new Point(140, 59), Width = 211, DropDownStyle = ComboBoxStyle.DropDownList, Parent = tabPageLpco };
            cmbParam.Items.AddRange(new string[] { "", "Documental", "Exame Físico", "Conferência Física", "Coleta de Amostra", "Inspeção Física" });

            var lblDataReg = new Label { Text = "Data Registro", Font = new Font("Segoe UI", 11F), Location = new Point(399, 34), AutoSize = true, Parent = tabPageLpco };
            var dtpDataReg = new DateTimePicker { Name = "dtpDataReg", Location = new Point(382, 56), Width = 135, Format = DateTimePickerFormat.Short, Parent = tabPageLpco };
            var lblDataDef = new Label { Text = "Data Deferimento", Font = new Font("Segoe UI", 11F), Location = new Point(534, 34), AutoSize = true, Parent = tabPageLpco };
            var dtpDataDef = new DateTimePicker { Name = "dtpDataDef", Location = new Point(531, 56), Width = 135, Format = DateTimePickerFormat.Short, Parent = tabPageLpco };

            txtLpcoNum.DataBindings.Add("Text", lpco, nameof(lpco.LPCO), false, DataSourceUpdateMode.OnPropertyChanged);
            cmbParam.DataBindings.Add("Text", lpco, nameof(lpco.ParametrizacaoLPCO), false, DataSourceUpdateMode.OnPropertyChanged);

            ConfigurarDatePickerNulavel(dtpDataReg, lpco.DataRegistroLPCO);
            ConfigurarDatePickerNulavel(dtpDataDef, lpco.DataDeferimentoLPCO);

            parentTabControl.TabPages.Add(tabPageLpco);
        }

        private void BtnLI_Click(object? sender, EventArgs e)
        {
            var novaLi = new LicencaImportacao { Numero = "Nova LI" };
            processo.LI.Add(novaLi);
            AdicionarAbaLi(novaLi);
            TCLi.SelectedIndex = TCLi.TabPages.Count - 1;
        }

        private void BtnNovoLpco_Click(LicencaImportacao liPai, ComboBox cbOrgaos, TabControl tabControlLpco)
        {
            string? nomeOrgao = cbOrgaos.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(nomeOrgao)) { MessageBox.Show("Selecione um órgão anuente."); return; }
            if (liPai.LPCO.Any(l => l.NomeOrgao == nomeOrgao)) { MessageBox.Show($"Já existe um LPCO para o órgão '{nomeOrgao}' nesta LI."); return; }

            var novoLpco = new LpcoInfo { NomeOrgao = nomeOrgao };
            liPai.LPCO.Add(novoLpco);
            AdicionarAbaLpco(tabControlLpco, novoLpco);
            tabControlLpco.SelectTab(tabControlLpco.TabPages.Count - 1);
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
        }

        private void ConfigurarDatePickerNulavel(DateTimePicker dtp, DateTime? data)
        {
            dtp.ShowCheckBox = true;

            // Carrega o valor inicial
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

            dtp.ValueChanged -= Dtp_ValueChanged; 
            dtp.ValueChanged += Dtp_ValueChanged;
        }

        // Este evento agora só cuida da FORMATAÇÃO VISUAL.
        private void Dtp_ValueChanged(object? sender, EventArgs e)
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
            BtnNovoOrgaoAnuente.Enabled = false; // Exemplo de botão no designer
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