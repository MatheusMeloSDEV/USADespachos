using CLUSA;
using System.Data;
using System.Globalization;

namespace Trabalho
{
    public partial class frmModificaRecibo : Form
    {
        RepositorioRecibo _repositorio;
        public Recibo ReciboAtual { get; set; }
        private bool _dadosForamAlterados = false;
        public frmModificaRecibo(Recibo recibo)
        {
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            InitializeComponent();
            ReciboAtual = recibo;
            _repositorio = new();
            PreencherCampos();
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
        private void frmModificaRecibo_FormClosing(object? sender, FormClosingEventArgs e)
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

                _dadosForamAlterados = false;
                this.Text = this.Text.Replace("*", "");

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar: {ex.Message}", "Erro",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
