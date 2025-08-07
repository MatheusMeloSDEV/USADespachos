using System.Data;
using System.Globalization;

namespace Trabalho
{
    public partial class DetalhesAgencia : Form
    {
        public string NumeroAgencia => txtAgencia.Text.Trim();
        public decimal PrecoCusto => ParseCurrency(precoCusto);

        public DetalhesAgencia()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAgencia.Text))
            {
                MessageBox.Show("Informe o número da agência.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        decimal ParseCurrency(TextBox tb)
        {
            // Tenta converter toda a string, incluindo "R$", parênteses, sinal, separadores
            if (decimal.TryParse(
                    tb.Text,
                    NumberStyles.Currency,
                    CultureInfo.GetCultureInfo("pt-BR"),
                    out var valor))
            {
                return valor;
            }
            return 0m;
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
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
