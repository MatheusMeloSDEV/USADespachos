using CLUSA;
using System.Diagnostics;

namespace Trabalho
{
    public partial class DetalhesFaturaForm : Form
    {
        public Fatura? FaturaAtual { get; private set; }
        private readonly RepositorioFatura _repositorio;
        private readonly string _referencia;
        private readonly string _importador;

        public DetalhesFaturaForm(string referencia, string importador)
        {
            InitializeComponent();
            _repositorio = new RepositorioFatura();
            _referencia = referencia;
            _importador = importador;

            // 1) Desabilita já de cara
            btnEditar.Enabled = false;

            lblInfo.Text = $"Número de Referência: {_referencia}\n" +
                           $"Nome do Importador: {_importador}";

            // 2) No Load, carrega ReciboAtual e só então habilita
            this.Load += async (_, __) =>
            {
                FaturaAtual = await FindFaturaAsync();
                if (FaturaAtual is not null)
                {
                    lblInfo.Text += $"\nVeículo: {FaturaAtual.Veiculo}";
                    btnEditar.Enabled = true;      // habilita somente com objeto válido
                }
            };
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            string importador = _importador;
            string referencia = _referencia;

            // 1) Cria sem using
            var progressForm = new ProgressForm();
            progressForm.Show(this);       // exibe modeless, com o próprio Form como owner

            // opcional: desabilita o botão pra não clicar de novo
            btnExportar.Enabled = false;

            Task.Run(() =>
            {
                // executa faturamento Python em background
                string pdfPath = PythonRunner
                    .ExecutarFaturamento(referencia, importador)
                    .Trim();

                // 2) Quando terminar, volta pro UI thread
                Invoke(new Action(() =>
                {
                    // fecha e libera o form de progresso
                    progressForm.Close();
                    progressForm.Dispose();

                    btnExportar.Enabled = true;

                    // pergunta se quer abrir PDF
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

                    this.Close();
                }));
            });
        }


        private void btnEditar_Click(object sender, EventArgs e)
        {
            // 3) safe-guard: só cria o form se ReciboAtual não for nulo
            if (FaturaAtual is null)
            {
                MessageBox.Show("Ainda carregando recibo…", "Aguarde",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var frmEdicao = new frmModificaFatura(FaturaAtual);
            frmEdicao.ShowDialog();
        }
        private async Task<Fatura?> FindFaturaAsync()
        {
            try
            {
                var f = await _repositorio.ObterPorRefUSAAsync(_referencia);
                if (f is null)
                    MessageBox.Show("Recibo não encontrado.", "Aviso",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return f;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar recibo: {ex.Message}", "Erro",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
    }


}
