using CLUSA;
using System.Diagnostics;

namespace Trabalho
{
    public partial class FrmFinanceiro : Form
    {
        private readonly RepositorioFatura _repositorioFatura;
        private readonly RepositorioRecibo _repositorioRecibo;

        public FrmFinanceiro()
        {
            InitializeComponent();
            _repositorioFatura = new();
            _repositorioRecibo = new();

            // MUDANÇA: O carregamento de dados foi movido do construtor para o evento Shown.
            this.Shown += FrmFinanceiro_Shown;
        }

        // MUDANÇA: O carregamento agora é assíncrono.
        private async void FrmFinanceiro_Shown(object? sender, EventArgs e)
        {
            await CarregarDadosAsync();
        }

        private async Task CarregarDadosAsync()
        {
            try
            {
                // MUDANÇA: Usando os métodos assíncronos com 'await'.
                var faturas = await _repositorioFatura.FindRefAsync();
                var recibos = await _repositorioRecibo.FindRefAsync();

                // MUDANÇA: Chamando o novo método genérico para preencher os painéis.
                PreencherPainelGenerico(panelFaturamento, "Faturamento", faturas, AbrirDetalhesFatura);
                PreencherPainelGenerico(panelRecibo, "Recibos", recibos, AbrirDetalhesRecibo);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar dados financeiros: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // MUDANÇA: Um único método genérico para preencher qualquer painel.
        private void PreencherPainelGenerico<T>(Panel panel, string titulo, List<T> itens, Action<T> onButtonClick) where T : IEntidadeBase
        {
            panel.Controls.Clear();
            panel.SuspendLayout(); // Otimização para adicionar múltiplos controles

            // --- Configurações de Layout ---
            const int colunas = 3;
            const int espacoX = 10, espacoY = 10;
            const int btnWidth = 250, btnHeight = 40;
            const int panelWidth = btnWidth + 40, panelHeight = btnHeight + 60;

            panel.Width = (panelWidth + espacoX) * colunas;

            // --- Título do Painel ---
            Label lblTitulo = new Label
            {
                Text = titulo,
                Font = new Font("Arial", 12, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 30
            };
            panel.Controls.Add(lblTitulo);

            // --- Cria os Itens ---
            for (int i = 0; i < itens.Count; i++)
            {
                var item = itens[i];
                string referencia = item.Ref_USA ?? "Sem referência";
                // Assumindo que a entidade T terá uma propriedade Importador.
                string importador = (item.GetType().GetProperty("Importador")?.GetValue(item) as string) ?? "Desconhecido";

                var lblReferencia = new Label
                {
                    Text = referencia,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Top,
                    Height = 20,
                    Margin = new Padding(0, 0, 0, 5)
                };

                var btnImportador = new Button
                {
                    Text = importador,
                    Tag = item, // Armazena o objeto inteiro (Fatura ou Recibo)
                    Width = btnWidth - 5,
                    Height = btnHeight,
                    Anchor = AnchorStyles.None
                };

                // MUDANÇA: Evento de clique corrigido e simplificado.
                btnImportador.Click += (sender, e) =>
                {
                    if (sender is Button { Tag: T clickedItem })
                    {
                        onButtonClick(clickedItem);
                    }
                };

                var itemPanel = new Panel
                {
                    Width = panelWidth - 15,
                    Height = panelHeight,
                    BorderStyle = BorderStyle.FixedSingle,
                    Margin = new Padding(2)
                };

                itemPanel.Controls.Add(lblReferencia);
                itemPanel.Controls.Add(btnImportador);
                btnImportador.Top = lblReferencia.Bottom + 5;
                btnImportador.Left = (itemPanel.Width - btnImportador.Width) / 2;

                int x = (i % colunas) * (panelWidth + espacoX);
                int y = (i / colunas) * (panelHeight + espacoY);
                itemPanel.Location = new Point(x + 10, y + 35);

                panel.Controls.Add(itemPanel);
            }
            panel.ResumeLayout();
        }

        // --- Métodos de Ação (chamados pelo método genérico) ---

        private void AbrirDetalhesFatura(Fatura fatura)
        {
            using var detalhesForm = new DetalhesFaturaForm(fatura.Ref_USA, fatura.Importador);
            var resultado = detalhesForm.ShowDialog();

            if (resultado == DialogResult.OK)
                MessageBox.Show("Lógica para Exportar Fatura foi executada.");
            else if (resultado == DialogResult.Ignore)
                MessageBox.Show("Lógica para Editar Fatura foi executada.");
        }

        private void AbrirDetalhesRecibo(Recibo recibo)
        {
            using var detalhesForm = new DetalhesReciboForm(recibo.Ref_USA, recibo.Importador);
            var resultado = detalhesForm.ShowDialog();

            if (resultado == DialogResult.OK)
                MessageBox.Show("Lógica para Exportar Recibo foi executada.");
            else if (resultado == DialogResult.Ignore)
                MessageBox.Show("Lógica para Editar Recibo foi executada.");
        }
    }
}