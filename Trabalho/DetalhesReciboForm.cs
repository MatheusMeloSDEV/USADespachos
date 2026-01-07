using CLUSA;
using CLUSA.Models;
using CLUSA.Repositories;
using System.Diagnostics;

namespace Trabalho
{
    public partial class DetalhesReciboForm : Form
    {
        public Recibo? ReciboAtual { get; private set; }
        private readonly RepositorioRecibo _repositorio;
        private readonly string _referencia;
        private readonly string _importador;

        public DetalhesReciboForm(string referencia, string importador)
        {
            InitializeComponent();
            _repositorio = new RepositorioRecibo();
            _referencia = referencia;
            _importador = importador;

            // 1) Desabilita já de cara
            btnEditar.Enabled = false;

            lblInfo.Text = $"Número de Referência: {_referencia}\n" +
                           $"Nome do Importador: {_importador}";

            // 2) No Load, carrega ReciboAtual e só então habilita
            this.Load += async (_, __) =>
            {
                ReciboAtual = await FindReciboAsync();
                if (ReciboAtual is not null)
                {
                    lblInfo.Text += $"\nVeículo: {ReciboAtual.Veiculo}";
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
                string pdfPath = "";
                string mensagemErro = null;

                try
                {
                    pdfPath = PythonRunner.ExecutarRecibo(referencia, importador).Trim();
                }
                catch (Exception ex)
                {
                    mensagemErro = $"Erro durante exportação: {ex.Message}";
                }

                Invoke(new Action(() =>
                {
                    progressForm.Close();
                    progressForm.Dispose();
                    btnExportar.Enabled = true;

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

                    this.Close();
                }));
            });

        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            // 3) safe-guard: só cria o form se ReciboAtual não for nulo
            if (ReciboAtual is null)
            {
                MessageBox.Show("Ainda carregando recibo…", "Aguarde",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var frmEdicao = new frmModificaRecibo(ReciboAtual);
            frmEdicao.ShowDialog();
        }
        private async Task<Recibo?> FindReciboAsync()
        {
            try
            {
                var r = await _repositorio.ObterPorRefUSAAsync(_referencia);
                if (r is null)
                    MessageBox.Show("Recibo não encontrado.", "Aviso",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return r;
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
