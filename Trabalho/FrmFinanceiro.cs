using CLUSA;
using System.Diagnostics;

namespace Trabalho
{
    public partial class FrmFinanceiro : Form
    {
        private RepositorioFatura _repositorioFatura;
        private RepositorioRecibo _repositorioRecibo;
        public FrmFinanceiro()
        {
            InitializeComponent();
            _repositorioFatura = new();
            _repositorioRecibo = new();
            CriarBotoes(panelFaturamento);
            CriarBotoes(panelRecibo);
        }
        private void CriarBotoes(Panel panel)
        {
            var faturas = _repositorioFatura.FindRef();
            var recibos = _repositorioRecibo.FindRef();
            PreencherPainelComFaturas(panelFaturamento, faturas);
            PreencherPainelComRecibos(panelRecibo, recibos);
        }

        private void PreencherPainelComFaturas(Panel panel, List<Fatura> faturas)
        {
            int colunas = 3;
            int espacoX = 10, espacoY = 10;
            int btnWidth = 250, btnHeight = 40;
            int panelWidth = btnWidth + 40, panelHeight = btnHeight + 60;

            panel.Width = (panelWidth + espacoX) * colunas;
            panel.Controls.Clear();

            Label titulo = new Label
            {
                Text = "Faturamento",
                Font = new Font("Arial", 12, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 30
            };
            panel.Controls.Add(titulo);

            for (int i = 0; i < faturas.Count; i++)
            {
                string referencia = faturas[i].Ref_USA ?? "Sem referência";
                string importador = faturas[i].Importador ?? "Desconhecido";

                Label lblReferencia = new Label
                {
                    Text = referencia,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Height = 20,
                    Dock = DockStyle.Top,
                    Margin = new Padding(0, 0, 0, 5)
                };

                Button btnImportador = new Button
                {
                    Text = importador,
                    Tag = (referencia, importador),
                    Width = btnWidth - 5,
                    Height = btnHeight,
                    MaximumSize = new Size(btnWidth, btnHeight),
                    Margin = new Padding(0, 0, 0, 5),
                    Anchor = AnchorStyles.None
                };

                btnImportador.Click += (s, e) =>
                {
                    btnImportador.Click += (sender, e) =>
                    {
                        if (sender is Button btn && btn.Tag is (string refClicked, string impClicked))
                        {
                            using var detalhesForm = new DetalhesFaturaForm(refClicked, impClicked);
                            var resultado = detalhesForm.ShowDialog();

                            if (resultado == DialogResult.OK)
                                MessageBox.Show("Lógica para Exportar foi executada.");
                            else if (resultado == DialogResult.Ignore)
                                MessageBox.Show("Lógica para Editar foi executada.");
                        }
                        else
                        {
                            Debug.Fail("Clique em controle inesperado ou Tag não configurada corretamente.");
                        }
                    };
                };

                Panel itemPanel = new Panel
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
        }
        private void PreencherPainelComRecibos(Panel panel, List<Recibo> recibos)
        {
            int colunas = 3;
            int espacoX = 10, espacoY = 10;
            int btnWidth = 250, btnHeight = 40;
            int panelWidth = btnWidth + 40, panelHeight = btnHeight + 60;

            panel.Width = (panelWidth + espacoX) * colunas;
            panel.Controls.Clear();

            Label titulo = new Label
            {
                Text = "Recibos",
                Font = new Font("Arial", 12, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 30
            };
            panel.Controls.Add(titulo);

            for (int i = 0; i < recibos.Count; i++)
            {
                string referencia = recibos[i].Ref_USA ?? "Sem referência";
                string importador = recibos[i].Importador ?? "Desconhecido";

                Label lblReferencia = new Label
                {
                    Text = referencia,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Height = 20,
                    Dock = DockStyle.Top,
                    Margin = new Padding(0, 0, 0, 5)
                };

                Button btnImportador = new Button
                {
                    Text = importador,
                    Tag = (referencia, importador),
                    Width = btnWidth - 5,
                    Height = btnHeight,
                    MaximumSize = new Size(btnWidth, btnHeight),
                    Margin = new Padding(0, 0, 0, 5),
                    Anchor = AnchorStyles.None
                };

                btnImportador.Click += (sender, e) =>
                {
                    if (sender is Button btn && btn.Tag is (string refClicked, string impClicked))
                    {
                        using var detalhesForm = new DetalhesReciboForm(refClicked, impClicked);
                        var resultado = detalhesForm.ShowDialog();

                        if (resultado == DialogResult.OK)
                            MessageBox.Show("Lógica para Exportar foi executada.");
                        else if (resultado == DialogResult.Ignore)
                            MessageBox.Show("Lógica para Editar foi executada.");
                    }
                    else
                    {
                        Debug.Fail("Clique em controle inesperado ou Tag não configurada corretamente.");
                    }
                };

                Panel itemPanel = new Panel
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
        }
    }
}
