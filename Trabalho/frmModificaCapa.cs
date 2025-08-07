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
            cbNumerario.SelectedItem = capa.Numerario ?? "";
            txtDTA.Text = capa.DTA ?? "";
            txtMarinha.Text = capa.Marinha ?? "";
            txtCE.Text = capa.CE ?? "";

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
            ItensAdicionais.SetItemChecked(ItensAdicionais.Items.IndexOf("Armazenagem"), capa.Armazenagem);
            ItensAdicionais.SetItemChecked(ItensAdicionais.Items.IndexOf("Faturado"), capa.Faturado);

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

            // 1) Cria sem using
            var progressForm = new ProgressForm();
            progressForm.Show(this);       // exibe modeless, com o próprio Form como owner

            Task.Run(() =>
            {
                string pdfPath = "";
                string mensagemErro = null;

                try
                {
                    pdfPath = PythonRunner.ExecutarCapa(ref_usa).Trim();
                }
                catch (Exception ex)
                {
                    mensagemErro = $"Erro durante exportação: {ex.Message}";
                }

                Invoke(new Action(() =>
                {
                    progressForm.Close();
                    progressForm.Dispose();

                    if (mensagemErro != null)
                    {
                        MessageBox.Show(mensagemErro, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    var resp = MessageBox.Show(
                        "Exportação concluída. Deseja abrir o PDF?",
                        "Resultado",
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
            capa.Numerario = cbNumerario.SelectedItem?.ToString();
            capa.DTA = txtDTA.Text;
            capa.Marinha = txtMarinha.Text;
            capa.CE = txtCE.Text;
            capa.Imposto = Impostos.CheckedItems.OfType<string>().ToArray();

            capa.TelaDoCanal = ItensAdicionais.CheckedItems.Contains("Tela do Canal");
            capa.Lancado = ItensAdicionais.CheckedItems.Contains("Lançado");
            capa.ConsultaSEFAZ = ItensAdicionais.CheckedItems.Contains("Consulta SEFAZ");
            capa.DAT_IIDeferida = ItensAdicionais.CheckedItems.Contains("DAT & LI Deferida");
            capa.DANFE = ItensAdicionais.CheckedItems.Contains("DANFE");
            capa.Armazenagem = ItensAdicionais.CheckedItems.Contains("Armazenagem");
            capa.Faturado = ItensAdicionais.CheckedItems.Contains("Faturado");

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