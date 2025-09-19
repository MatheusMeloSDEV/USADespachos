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

        private void FrmModificaCapa_Load(object sender, EventArgs e)
        {
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            // Preenche os controles com os valores do objeto capa
            txtMaster.Text = capa.Master ?? "";
            txtContainer.Text = capa.Container ?? "";
            cbSigvig.SelectedItem = capa.Sigvig ?? "Selecionado";
            DTPSigvig.Value = capa.SigvigData ?? DateTime.Today;
            TxtIncoterm.Text = capa.Incoterm ?? "";
            txtDTA.Text = capa.DTA ?? "";
            txtMarinha.Text = capa.Marinha ?? "";
            txtCE.Text = capa.CE ?? "";
            cbArmazenagem.Checked = capa.Armazenagem; 
            cbArmFaturado.Checked = capa.Faturado;

            if (capa.Numerario != null)
            {
                foreach (var item in capa.Numerario)
                {
                    int idx = cbNumerario.Items.IndexOf(item);
                    if (idx >= 0)
                        Impostos.SetItemChecked(idx, true);
                }
            }

            if (capa.Imposto != null)
            {
                foreach (var item in capa.Imposto)
                {
                    int idx = Impostos.Items.IndexOf(item);
                    if (idx >= 0)
                        Impostos.SetItemChecked(idx, true);
                }
            }

            // Bools
            // No FrmModificaCapa_Load, adicione:
            ItensAdicionais.SetItemChecked(ItensAdicionais.Items.IndexOf("Tela do Canal"), capa.TelaDoCanal);
            ItensAdicionais.SetItemChecked(ItensAdicionais.Items.IndexOf("Lançado"), capa.Lancado);
            ItensAdicionais.SetItemChecked(ItensAdicionais.Items.IndexOf("Consulta SEFAZ"), capa.ConsultaSEFAZ);
            ItensAdicionais.SetItemChecked(ItensAdicionais.Items.IndexOf("DAT & LI Deferida"), capa.DAT_IIDeferida);
            ItensAdicionais.SetItemChecked(ItensAdicionais.Items.IndexOf("DANFE"), capa.DANFE);

            // Datas
            DTPAverbar.Value = capa.AverbarData ?? DateTime.Today;
            DTPLiberarBL.Value = capa.LiberarBLData ?? DateTime.Today;
            DTPIsencaoMarinha.Value = capa.MarinhaMercante_IsencaoData ?? DateTime.Today;
            DTPICMS.Value = capa.ICMS_ExoneracaoData ?? DateTime.Today;
            DTPSisCarga.Value = capa.SISCargaLiberadoData ?? DateTime.Today;
            txtPagoPor.Text = capa.PagoPor ?? "";
            DTPEntTransporte.Value = capa.ENTTransporteData ?? DateTime.Today;
            txtTransporte.Text = capa.ENTTransporteN ?? "";
            DTPEntAlfandega.Value = capa.ENTAlfandegaData ?? DateTime.Today;
            txtDOSSIE.Text = capa.ENTAlfandegaDossie ?? "";
            DTPConferenciaFisica.Value = capa.ConferenciaFisicaData ?? DateTime.Today;

            txtObservacao.Text = capa.Observacoes ?? "";

            btnExportar.Enabled = false;
            if (Visualizacao) SetCamposSomenteLeitura(this);
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

        private void CBOSigvig_SelectedIndexChanged(object sender, EventArgs e)
        {
            DTPSigvig.Enabled = cbSigvig.SelectedItem?.ToString() == "Liberado";
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

        private void btnSalvar_Click_1(object sender, EventArgs e)
        {
            // Preencher objeto capa com os valores dos controles
            capa.Master = txtMaster.Text;
            capa.Container = txtContainer.Text;
            capa.Sigvig = cbSigvig.SelectedItem?.ToString();
            capa.SigvigData = (capa.Sigvig == "Liberado") ? DTPSigvig.Value : null;
            capa.Incoterm = TxtIncoterm.Text;
            capa.Numerario = Impostos.CheckedItems.OfType<string>().ToArray();
            capa.DTA = txtDTA.Text;
            capa.Marinha = txtMarinha.Text;
            capa.CE = txtCE.Text;
            capa.Imposto = Impostos.CheckedItems.OfType<string>().ToArray();

            capa.TelaDoCanal = ItensAdicionais.CheckedItems.Contains("Tela do Canal");
            capa.Lancado = ItensAdicionais.CheckedItems.Contains("Lançado");
            capa.ConsultaSEFAZ = ItensAdicionais.CheckedItems.Contains("Consulta SEFAZ");
            capa.DAT_IIDeferida = ItensAdicionais.CheckedItems.Contains("DAT & LI Deferida");
            capa.DANFE = ItensAdicionais.CheckedItems.Contains("DANFE");
            capa.Armazenagem = cbArmazenagem.Checked;
            capa.Faturado = cbArmFaturado.Checked;

            capa.AverbarData = DTPAverbar.Value;
            capa.LiberarBLData = DTPLiberarBL.Value;
            capa.MarinhaMercante_IsencaoData = DTPIsencaoMarinha.Value;
            capa.ICMS_ExoneracaoData = DTPICMS.Value;
            capa.SISCargaLiberadoData = DTPSisCarga.Value;
            capa.PagoPor = txtPagoPor.Text;
            capa.ENTTransporteData = DTPEntTransporte.Value;
            capa.ENTTransporteN = txtTransporte.Text;
            capa.ENTAlfandegaData = DTPEntAlfandega.Value;
            capa.ENTAlfandegaDossie = txtDOSSIE.Text;
            capa.ConferenciaFisicaData = DTPConferenciaFisica.Value;

            capa.Observacoes = txtObservacao.Text;

            // Confirmação de exportação
            var resp = MessageBox.Show(
                "Deseja exportar a capa agora?",
                "Exportar Capa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (resp == DialogResult.Yes)
            {
                btnExportar_Click(btnExportar, EventArgs.Empty);
            }
            else
            {
                btnExportar.Enabled = true;
            }
        }

        private void btnCancelar_Click_1(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}