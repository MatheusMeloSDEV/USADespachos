using CLUSA;
using CLUSA.Interfaces;
using CLUSA.Models;
using CLUSA.Repositories;
using CLUSA.Services;
using System.Diagnostics;

namespace Trabalho
{
    public enum TipoDocumentoFinanceiro
    {
        Fatura,
        Recibo
    }
    public partial class DetalhesForm : Form
    {
        private IEntidadeBase? _documentoAtual;
        private readonly RepositorioFatura _repoFatura;
        private readonly RepositorioRecibo _repoRecibo;
        private readonly string _referencia;
        private readonly string _importador;
        private readonly TipoDocumentoFinanceiro _tipoDocumento;

        public DetalhesForm(string referencia, string importador, TipoDocumentoFinanceiro tipo)
        {
            InitializeComponent();
            _referencia = referencia;
            _importador = importador;
            _tipoDocumento = tipo;

            _repoFatura = new RepositorioFatura();
            _repoRecibo = new RepositorioRecibo();

            btnEditar.Enabled = false;
            btnExportar.Enabled = false;
            this.Text = $"Detalhes do {_tipoDocumento}";
            lblInfo.Text = $"Número de Referência: {_referencia}\nNome do Importador: {_importador}";
        }

        private async void DetalhesForm_Load(object? sender, EventArgs e)
        {
            try
            {
                if (_tipoDocumento == TipoDocumentoFinanceiro.Fatura)
                    _documentoAtual = await _repoFatura.ObterPorRefUSAAsync(_referencia);
                else
                    _documentoAtual = await _repoRecibo.ObterPorRefUSAAsync(_referencia);

                if (_documentoAtual != null)
                {
                    btnEditar.Enabled = true;
                    btnExportar.Enabled = true;
                }
                else
                {
                    MessageBox.Show($"{_tipoDocumento} não encontrado.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- MÉTODO ATUALIZADO AQUI ---
        private async void btnExportar_Click(object? sender, EventArgs e)
        {
            if (_documentoAtual == null) return;

            // 1. Configura UI
            var progressForm = new ProgressForm();
            progressForm.Show(this);
            btnExportar.Enabled = false;
            btnEditar.Enabled = false;

            try
            {
                string pdfPath = "";

                // 2. Executa a exportação (Em Task.Run para manter o GIF de loading rodando liso)
                await Task.Run(async () =>
                {
                    if (_tipoDocumento == TipoDocumentoFinanceiro.Fatura)
                    {
                        var service = new FaturamentoService();
                        // O C# busca o importador e dados internamente, só precisa da referência
                        pdfPath = await service.GerarFaturamentoAsync(_referencia);
                    }
                    else // Recibo
                    {
                        var service = new ReciboService();
                        pdfPath = await service.GerarReciboAsync(_referencia);
                    }
                });

                // 3. Sucesso (Volta pra UI thread automaticamente por causa do await)
                progressForm.Close();

                var resp = MessageBox.Show("Exportação concluída. Deseja abrir o PDF?", "Resultado", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (resp == DialogResult.Yes && !string.IsNullOrEmpty(pdfPath) && File.Exists(pdfPath))
                {
                    Process.Start(new ProcessStartInfo { FileName = pdfPath, UseShellExecute = true });
                }

                this.Close();
            }
            catch (Exception ex)
            {
                // 4. Erro
                progressForm.Close();
                MessageBox.Show($"Erro durante exportação: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Reabilita botões caso queira tentar de novo (ou fecha o form se preferir)
                btnExportar.Enabled = true;
                btnEditar.Enabled = true;
            }
        }

        private void btnEditar_Click(object? sender, EventArgs e)
        {
            if (_documentoAtual == null) return;

            if (_documentoAtual is Fatura fatura)
            {
                using var frmEdicao = new frmModificaFatura(fatura);
                frmEdicao.ShowDialog();
            }
            else if (_documentoAtual is Recibo recibo)
            {
                using var frmEdicao = new frmModificaRecibo(recibo);
                frmEdicao.ShowDialog();
            }
        }
    }
}