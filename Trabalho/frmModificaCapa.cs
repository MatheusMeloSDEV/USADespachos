using CLUSA;
using System.Diagnostics;

namespace Trabalho
{
    public partial class FrmModificaCapa : Form
    {
        public Capa capa;
        public string? Modo;
        public bool Visualizacao;
        public string ref_usa;
        private bool _dadosForamAlterados = false;
        // Inicialize todos os checkboxes


        public FrmModificaCapa()
        {
            InitializeComponent();
            capa = new();
        }

        private void FrmModificaCapa_Load(object? sender, EventArgs e)
        {
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            CarregarDados();
            AnexarEventoDeAlteracao(this);

            if (Visualizacao)
            {
                SetCamposSomenteLeitura(this);
            }
        }
        private void CarregarDados()
        {
            txtMaster.Text = capa.Master;
            txtContainer.Text = capa.Container;
            CbSelecionado.Checked = capa.SigvigSelecionado;
            CbLiberado.Checked = capa.SigvigLiberado;
            TxtIncoterm.Text = capa.Incoterm;
            txtDTA.Text = capa.DTA;
            txtMarinha.Text = capa.Marinha;
            txtCE.Text = capa.CE;
            txtPagoPor.Text = capa.PagoPor;
            txtTransporte.Text = capa.ENTTransporteN;
            txtDOSSIE.Text = capa.ENTAlfandegaDossie;
            txtObservacao.Text = capa.Observacoes;

            cbArmazenagem.Checked = capa.Armazenagem;
            cbArmFaturado.Checked = capa.Faturado;

            // Carrega o estado do CheckedListBox 'ItensAdicionais'
            MarcarItem(ItensAdicionais, "Tela do Canal", capa.TelaDoCanal);
            MarcarItem(ItensAdicionais, "Lançado", capa.Lancado);
            MarcarItem(ItensAdicionais, "Consulta SEFAZ", capa.ConsultaSEFAZ);
            MarcarItem(ItensAdicionais, "DAT & LI Deferida", capa.DAT_IIDeferida);
            MarcarItem(ItensAdicionais, "DANFE", capa.DANFE);
            MarcarItem(ItensAdicionais, "SISCarga Liberado", capa.SISCargaLiberado);
            MarcarItem(ItensAdicionais, "Pago", capa.Pago);
            MarcarItem(ItensAdicionais, "ENT Transporte", capa.ENTTransporte);
            MarcarItem(ItensAdicionais, "ENT Alfandega", capa.ENTAlfandega);
            MarcarItem(ItensAdicionais, "Conferência Física", capa.ConferenciaFisica);
            MarcarItem(ItensAdicionais, "Averbar", capa.Averbar);
            MarcarItem(ItensAdicionais, "Liberar BL", capa.LiberarBL);
            MarcarItem(ItensAdicionais, "Marinha Mercante - Isenção", capa.MarinhaMercante_Isencao);
            MarcarItem(ItensAdicionais, "ICMS - Exoneração", capa.ICMS_Exoneracao);

            if (capa.Numerario != null)
            {
                // Percorre os itens que já estão salvos no objeto 'capa'
                foreach (var itemSalvo in capa.Numerario)
                {
                    // Procura o índice do item na lista do controle
                    int index = cbNumerario.Items.IndexOf(itemSalvo);
                    if (index != -1)
                    {
                        // Se encontrar, marca o item
                        cbNumerario.SetItemChecked(index, true);
                    }
                }
            }

            ConfigurarDatePickerNulavel(DTPSigvig, capa.SigvigData);
            ConfigurarDatePickerNulavel(DTPAverbar, capa.AverbarData);
            ConfigurarDatePickerNulavel(DTPLiberarBL, capa.LiberarBLData);
            ConfigurarDatePickerNulavel(DTPIsencaoMarinha, capa.MarinhaMercante_IsencaoData);
            ConfigurarDatePickerNulavel(DTPICMS, capa.ICMS_ExoneracaoData);
            ConfigurarDatePickerNulavel(DTPSisCarga, capa.SISCargaLiberadoData);
            ConfigurarDatePickerNulavel(DTPEntTransporte, capa.ENTTransporteData);
            ConfigurarDatePickerNulavel(DTPEntAlfandega, capa.ENTAlfandegaData);
            ConfigurarDatePickerNulavel(DTPConferenciaFisica, capa.ConferenciaFisicaData);

            // Habilita/desabilita o DTP do Sigvig com base na seleção inicial
            DTPSigvig.Enabled = CbLiberado.Checked;
        }
        private void MarcarItem(CheckedListBox clb, string itemTexto, bool deveMarcar)
        {
            int index = clb.Items.IndexOf(itemTexto);
            if (index != -1) // <-- A VERIFICAÇÃO MÁGICA!
            {
                clb.SetItemChecked(index, deveMarcar);
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
            dtp.ValueChanged += (s, e) => {
                if (s is DateTimePicker picker)
                {
                    picker.Format = picker.Checked ? DateTimePickerFormat.Short : DateTimePickerFormat.Custom;
                }
            };
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
        private void frmModificaCapa_FormClosing(object? sender, FormClosingEventArgs e)
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
        private void SetCamposSomenteLeitura(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                switch (control)
                {
                    case TextBox textBox:
                        textBox.ReadOnly = true;
                        break;
                    case DateTimePicker:
                    case CheckBox:
                    case ComboBox:
                    case NumericUpDown:
                    case CheckedListBox:
                        control.Enabled = false;
                        break;
                }
                if (control.HasChildren)
                    SetCamposSomenteLeitura(control);
            }
            btnExportar.Enabled = false;
        }

        private void CbLiberado_CheckedChanged(object? sender, EventArgs e)
        {
            // Habilita o seletor de data apenas se "Liberado" estiver marcado
            DTPSigvig.Enabled = CbLiberado.Checked;

            // Se o usuário desmarcar "Liberado", a data também é desmarcada
            if (!CbLiberado.Checked)
            {
                DTPSigvig.Checked = false;
            }
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            // 1) Cria e exibe o formulário de progresso
            var progressForm = new ProgressForm();
            progressForm.Show(this);

            Task.Run(() =>
            {
                string pdfPath = null;
                string mensagemErro = null;

                try
                {
                    // 1. Executa o script e armazena a saída (que pode ser um caminho ou um erro)
                    string resultadoPython = PythonRunner.ExecutarCapa(ref_usa);

                    // 2. CORREÇÃO: VERIFICA O RESULTADO
                    // Se a string retornada termina com .pdf E o arquivo realmente existe, é sucesso.
                    if (!string.IsNullOrWhiteSpace(resultadoPython) &&
                        resultadoPython.Trim().EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) &&
                        File.Exists(resultadoPython.Trim()))
                    {
                        // SUCESSO: O resultado é um caminho de PDF válido.
                        pdfPath = resultadoPython.Trim();
                    }
                    else
                    {
                        // FALHA: O resultado não é um PDF válido, então consideramos que é a mensagem de erro.
                        mensagemErro = resultadoPython;
                        if (string.IsNullOrWhiteSpace(mensagemErro))
                        {
                            mensagemErro = "Ocorreu um erro desconhecido durante a execução do script. A saída estava vazia.";
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Este catch agora pega erros da própria lógica C# (ex: permissões, etc.)
                    mensagemErro = $"Erro inesperado na aplicação: {ex.Message}";
                }

                // 3. Atualiza a interface do usuário com o resultado
                Invoke(new Action(() =>
                {
                    progressForm.Close();
                    progressForm.Dispose();

                    // Agora, se mensagemErro não for nula, ela contém o erro do Python
                    if (mensagemErro != null)
                    {
                        MessageBox.Show(mensagemErro, "Erro na Exportação", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // O resto do código de sucesso permanece o mesmo
                    var resp = MessageBox.Show(
                        "Exportação concluída. Deseja abrir o PDF?",
                        "Sucesso",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );

                    if (resp == DialogResult.Yes && File.Exists(pdfPath))
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = pdfPath,
                            UseShellExecute = true
                        });
                    }
                    DialogResult = DialogResult.OK; 
                }));
            });
        }
        private void SalvarDados()
        {
            capa.Master = txtMaster.Text;
            capa.Container = txtContainer.Text;
            capa.SigvigSelecionado = CbSelecionado.Checked;
            capa.SigvigLiberado = CbLiberado.Checked;
            capa.Incoterm = TxtIncoterm.Text;
            capa.DTA = txtDTA.Text;
            capa.Marinha = txtMarinha.Text;
            capa.CE = txtCE.Text;
            capa.PagoPor = txtPagoPor.Text;
            capa.ENTTransporteN = txtTransporte.Text;
            capa.ENTAlfandegaDossie = txtDOSSIE.Text;
            capa.Observacoes = txtObservacao.Text;

            // --- Salva campos booleanos ---
            capa.Armazenagem = cbArmazenagem.Checked;
            capa.Faturado = cbArmFaturado.Checked;

            // MUDANÇA: Usando o novo método seguro para salvar os itens do CheckedListBox
            capa.TelaDoCanal = IsItemChecked(ItensAdicionais, "Tela do Canal");
            capa.Lancado = IsItemChecked(ItensAdicionais, "Lançado");
            capa.ConsultaSEFAZ = IsItemChecked(ItensAdicionais, "Consulta SEFAZ");
            capa.DAT_IIDeferida = IsItemChecked(ItensAdicionais, "DAT & LI Deferida");
            capa.DANFE = IsItemChecked(ItensAdicionais, "DANFE");
            capa.SISCargaLiberado = IsItemChecked(ItensAdicionais, "SISCarga Liberado");
            capa.Pago = IsItemChecked(ItensAdicionais, "Pago");
            capa.ENTTransporte = IsItemChecked(ItensAdicionais, "ENT Transporte");
            capa.ENTAlfandega = IsItemChecked(ItensAdicionais, "ENT Alfandega");
            capa.ConferenciaFisica = IsItemChecked(ItensAdicionais, "Conferência Física");
            capa.Averbar = IsItemChecked(ItensAdicionais, "Averbar");
            capa.LiberarBL = IsItemChecked(ItensAdicionais, "Liberar BL");
            capa.MarinhaMercante_Isencao = IsItemChecked(ItensAdicionais, "Marinha Mercante - Isenção");
            capa.ICMS_Exoneracao = IsItemChecked(ItensAdicionais, "ICMS - Exoneração");

            capa.Numerario = cbNumerario.CheckedItems.OfType<string>().ToArray();

            capa.SigvigData = DTPSigvig.Checked ? DTPSigvig.Value : null;
            capa.AverbarData = DTPAverbar.Checked ? DTPAverbar.Value : null;
            capa.LiberarBLData = DTPLiberarBL.Checked ? DTPLiberarBL.Value : null;
            capa.MarinhaMercante_IsencaoData = DTPIsencaoMarinha.Checked ? DTPIsencaoMarinha.Value : null;
            capa.ICMS_ExoneracaoData = DTPICMS.Checked ? DTPICMS.Value : null;
            capa.SISCargaLiberadoData = DTPSisCarga.Checked ? DTPSisCarga.Value : null;
            capa.ENTTransporteData = DTPEntTransporte.Checked ? DTPEntTransporte.Value : null;
            capa.ENTAlfandegaData = DTPEntAlfandega.Checked ? DTPEntAlfandega.Value : null;
            capa.ConferenciaFisicaData = DTPConferenciaFisica.Checked ? DTPConferenciaFisica.Value : null;
        }
        private bool IsItemChecked(CheckedListBox clb, string itemTexto)
        {
            int index = clb.Items.IndexOf(itemTexto);

            // Se o item não foi encontrado (index = -1), ele não pode estar marcado.
            if (index == -1)
            {
                // Opcional: Adiciona um aviso no console de depuração para o desenvolvedor.
                Debug.WriteLine($"Aviso: O item '{itemTexto}' não foi encontrado na CheckedListBox '{clb.Name}'.");
                return false;
            }

            // Se encontrou, retorna o estado real (marcado ou desmarcado).
            return clb.GetItemChecked(index);
        }
        private void btnSalvar_Click_1(object? sender, EventArgs e)
        {
            SalvarDados();
            _dadosForamAlterados = false;
            this.Text = this.Text.Replace("*", "");

            MessageBox.Show("Capa salva com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            // Removida a pergunta de exportar para simplificar o botão Salvar.
        }

        private void btnCancelar_Click_1(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}