using CLUSA;
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
        // Propriedades genéricas para guardar os dados e repositórios
        private IEntidadeBase? _documentoAtual;
        private readonly RepositorioFatura _repoFatura;
        private readonly RepositorioRecibo _repoRecibo;

        // Propriedades para identificar o documento
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

            // Desabilita os botões até os dados serem carregados
            btnEditar.Enabled = false;
            btnExportar.Enabled = false;

            // Define o título da janela
            this.Text = $"Detalhes do {_tipoDocumento}";

            lblInfo.Text = $"Número de Referência: {_referencia}\n" +
                           $"Nome do Importador: {_importador}";

        }
        private async void DetalhesForm_Load(object? sender, EventArgs e)
        {
            try
            {
                if (_tipoDocumento == TipoDocumentoFinanceiro.Fatura)
                {
                    _documentoAtual = await _repoFatura.ObterPorRefUSAAsync(_referencia);
                }
                else
                {
                    _documentoAtual = await _repoRecibo.ObterPorRefUSAAsync(_referencia);
                }

                // Se o documento foi encontrado, habilita os botões
                if (_documentoAtual != null)
                {
                    btnEditar.Enabled = true;
                    btnExportar.Enabled = true;
                }
                else
                {
                    MessageBox.Show($"{_tipoDocumento} não encontrado.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    // Desabilita os botões se o documento não for encontrado
                    btnEditar.Enabled = false;
                    btnExportar.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocorreu um erro ao carregar os dados: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnExportar_Click(object? sender, EventArgs e)
        {
            if (_documentoAtual == null) return;

            var progressForm = new ProgressForm();
            progressForm.Show(this);
            btnExportar.Enabled = false;
            btnEditar.Enabled = false;

            Task.Run(() =>
            {
                string pdfPath = "";
                string? mensagemErro = null;

                try
                {
                    if (_tipoDocumento == TipoDocumentoFinanceiro.Fatura)
                    {
                        pdfPath = PythonRunner.ExecutarFaturamento(_referencia, _importador).Trim();
                    }
                    else // É um Recibo
                    {
                        pdfPath = PythonRunner.ExecutarRecibo(_referencia, _importador).Trim();
                    }
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
                        this.Close(); // Fecha em caso de erro
                        return;
                    }

                    var resp = MessageBox.Show("Exportação concluída. Deseja abrir o PDF?", "Resultado", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (resp == DialogResult.Yes && !string.IsNullOrEmpty(pdfPath) && File.Exists(pdfPath))
                    {
                        Process.Start(new ProcessStartInfo { FileName = pdfPath, UseShellExecute = true });
                    }
                    this.Close();
                }));
            });
        }

        private void btnEditar_Click(object? sender, EventArgs e)
        {
            if (_documentoAtual == null)
            {
                MessageBox.Show("Nenhum documento carregado para edição.", "Aviso");
                return;
            }

            // Lógica para abrir o formulário de edição correto
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