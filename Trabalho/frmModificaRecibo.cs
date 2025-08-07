using CLUSA;
using System.Data;
using System.Globalization;

namespace Trabalho
{
    public partial class frmModificaRecibo : Form
    {
        RepositorioRecibo _repositorio;
        public Recibo ReciboAtual { get; set; }
        public frmModificaRecibo(Recibo recibo)
        {
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            InitializeComponent();
            ReciboAtual = recibo;
            _repositorio = new();
            PreencherCampos();
        }
        private void AplicarMascaraMoeda(object sender, EventArgs e)
        {
            // Garante que o sender é um TextBox
            if (!(sender is TextBox txt))
                return;

            // Se o texto estiver vazio, zera e posiciona cursor
            if (string.IsNullOrWhiteSpace(txt.Text))
            {
                txt.Text = "0,00";
                txt.SelectionStart = txt.Text.Length;
                return;
            }

            // Remove tudo que não for dígito
            string somenteDigitos = new string(txt.Text.Where(char.IsDigit).ToArray());

            if (string.IsNullOrEmpty(somenteDigitos))
            {
                txt.Text = "0,00";
                txt.SelectionStart = txt.Text.Length;
                return;
            }

            // Converte para decimal considerando 2 casas (centavos)
            if (!decimal.TryParse(somenteDigitos, out decimal valor))
            {
                // Se falhar, zera
                txt.Text = "0,00";
                txt.SelectionStart = txt.Text.Length;
                return;
            }
            valor /= 100;

            // Formata no padrão pt-BR como moeda
            txt.Text = valor.ToString("C", CultureInfo.GetCultureInfo("pt-BR"));
            txt.SelectionStart = txt.Text.Length;
        }
        private void btnCalcularTotal_Click(object sender, EventArgs e)
        {
            // Cultura pt-BR para parsing e formatação
            var culture = CultureInfo.GetCultureInfo("pt-BR");

            // Função local para converter texto em R$ para decimal
            decimal Parse(string text)
            {
                if (decimal.TryParse(text, NumberStyles.Currency, culture, out var v))
                    return v;
                return 0m;
            }

            // 1) Soma de todas as variáveis terminadas em “P”
            decimal Calculo =
                  Parse(txtEmissaoLicenciamento.Text)
                + Parse(txtExpediente.Text)
                + Parse(txtHonorariosDespachante.Text);

            txtTotal.Text = Calculo.ToString("C", culture);

            btnSalvar.Enabled = true;
        }
        private void PreencherCampos()
        {
            txtN_Ref.Text = ReciboAtual.Ref_USA;
            txtS_Ref.Text = ReciboAtual.SR;
            txtImportador.Text = ReciboAtual.Importador;
            txtExportador.Text = ReciboAtual.Exportador;
            txtNavio.Text = ReciboAtual.Veiculo;
            txtMercadoria.Text = ReciboAtual.Mercadoria;
            txtEmissaoLicenciamento.Text = ReciboAtual.EmissaoLicenca.ToString();
            txtExpediente.Text = ReciboAtual.Expediente.ToString();
            txtHonorariosDespachante.Text = ReciboAtual.HonorariosDespachante.ToString();
            txtTotal.Text = ReciboAtual.Total.ToString();

        }
        private async void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                decimal ParseCurrency(TextBox tb)
                {
                    string s = tb.Text.Replace("R$", "").Trim();
                    if (decimal.TryParse(s,
                                         NumberStyles.Currency,
                                         CultureInfo.GetCultureInfo("pt-BR"),
                                         out var valor))
                    {
                        return valor;
                    }
                    return 0m;
                }

                // --- atualização do objeto ---
                ReciboAtual.Ref_USA = txtN_Ref.Text;
                ReciboAtual.SR = txtS_Ref.Text;
                ReciboAtual.Importador = txtImportador.Text;
                ReciboAtual.Veiculo = txtNavio.Text;
                ReciboAtual.Mercadoria = txtMercadoria.Text;
                ReciboAtual.Exportador = txtExportador.Text;
                ReciboAtual.EmissaoLicenca = ParseCurrency(txtEmissaoLicenciamento);
                ReciboAtual.Expediente = ParseCurrency(txtExpediente);
                ReciboAtual.HonorariosDespachante = ParseCurrency(txtHonorariosDespachante);
                ReciboAtual.Total = ParseCurrency(txtTotal);
                ReciboAtual.Datahoje = DateTime.Now.ToString("dd 'de' MMMM yyyy", new System.Globalization.CultureInfo("pt-BR"));

                // --- grava no MongoDB ---
                await _repositorio.UpdateAsync(ReciboAtual);

                MessageBox.Show("Recibo atualizado com sucesso!", "Sucesso",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar: {ex.Message}", "Erro",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
