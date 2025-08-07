using CLUSA;

namespace Trabalho
{
    public partial class UCOrgaoAnuente : UserControl
    {
        // Essa lista guarda as LIs em memória (será preenchida pelo Form genérico)
        private List<LiInfo> listaLis = new List<LiInfo>();

        // Define se o controle está em modo de visualização (sem edição)
        public bool Visualizacao { get; set; }

        public UCOrgaoAnuente()
        {
            InitializeComponent();
        }

        // Chame este método para “injetar” no UserControl a entidade (T) que vem do Form genérico
        public void CarregarCamposBase(OrgaoAnuente entidade)
        {
            // Entidade já estiver populada pelo Form, então apenas reflete os campos:
            TXTnr.Text = entidade.Ref_USA;
            TXTimportador.Text = entidade.Importador;
            TXTsr.Text = entidade.SR;
            TXTexportador.Text = entidade.Exportador;
            TXTterminal.Text = entidade.Terminal;
            TXTNavio.Text = entidade.Veiculo;
            TXTProduto.Text = entidade.Produto;
            TXTorigem.Text = entidade.Origem;
            TXTstatusdoprocesso.Text = entidade.StatusDoProcesso;
            TXTpendencia.Text = entidade.Pendencia;
            CBamostra.Checked = entidade.Amostra;

            DTPdatadeatracacao.Value = entidade.DataDeAtracacao ?? DateTime.Now;
            DTPdatadeatracacao.Checked = entidade.CheckDataDeAtracacao;
            DTPdatadeembarque.Value = entidade.DataEmbarque ?? DateTime.Now;
            DTPdatadeembarque.Checked = entidade.CheckDataEmbarque;
            DTPdatadeinspecao.Value = entidade.Inspecao ?? DateTime.Now;
            DTPdatadeinspecao.Checked = entidade.CheckInspecao;
        }

        // Chame este método para popular a lista de LIs no controle
        public void CarregarLisBase(OrgaoAnuente entidade)
        {
            // Obtém o nome da classe da entidade (ex: "MAPA", "ANVISA", etc.)
            var nomeOrgao = entidade.GetType().Name.ToUpperInvariant();

            // Filtra as LIs que possuem o órgão atual na lista de órgãos anuentes
            listaLis = entidade.LI?
                .Where(li => li.OrgaosAnuentes != null &&
                             li.OrgaosAnuentes.Any(orgao =>
                                 string.Equals(orgao, nomeOrgao, StringComparison.OrdinalIgnoreCase)))
                .ToList() ?? new List<LiInfo>();

            AtualizarPainelLi();
        }

        private void AtualizarPainelLi()
        {
            flpLis.Controls.Clear();
            flpLis.FlowDirection = FlowDirection.LeftToRight;
            flpLis.WrapContents = true;
            flpLis.AutoScroll = true;

            int panelWidth = (flpLis.ClientSize.Width - SystemInformation.VerticalScrollBarWidth) / 2 - 4;
            int panelHeight = 40;

            foreach (var li in listaLis)
            {
                var panel = new Panel
                {
                    Size = new Size(panelWidth, panelHeight),
                    BorderStyle = BorderStyle.FixedSingle,
                    Margin = new Padding(2)
                };

                var lbl = new Label
                {
                    Text = $"LI: {li.Numero}",
                    AutoSize = true,
                    Location = new Point(5, 10)
                };

                var btnVisualizar = new Button
                {
                    Text = Visualizacao ? "Visualizar" : "Editar",
                    Size = new Size(75, 25),
                    Anchor = AnchorStyles.Top | AnchorStyles.Right
                };
                // Ajusta a posição após o AutoSize do painel
                btnVisualizar.Location = new Point(panel.Width - 80, 7);

                btnVisualizar.Click += (s, e) =>
                {
                    var frm = new frmLi(
                        li.Numero,
                        li.OrgaosAnuentes,
                        li.NCM,
                        li.LPCO,
                        li.DataRegistroLI,
                        li.CheckDataRegistroLI,
                        li.DataRegistroLPCO,
                        li.CheckDataRegistroLPCO,
                        li.DataDeferimentoLPCO,
                        li.CheckDataDeferimentoLPCO,
                        li.ParametrizacaoLPCO,
                        Visualizacao
                    );
                    frm.Owner = this.FindForm(); // o Form genérico será o dono
                    frm.ShowDialog();
                };

                panel.Controls.Add(lbl);
                panel.Controls.Add(btnVisualizar);
                flpLis.Controls.Add(panel);
            }
        }

        // Chame este método para ler os valores do UserControl e popular a Entidade
        public void ExtrairParaEntidade(OrgaoAnuente entidade)
        {
            entidade.Ref_USA = TXTnr.Text;
            entidade.Importador = TXTimportador.Text;
            entidade.SR = TXTsr.Text;
            entidade.Exportador = TXTexportador.Text;
            entidade.Terminal = TXTterminal.Text;
            entidade.Veiculo = TXTNavio.Text;
            entidade.Produto = TXTProduto.Text;
            entidade.Origem = TXTorigem.Text;
            entidade.StatusDoProcesso = TXTstatusdoprocesso.Text;
            entidade.Pendencia = TXTpendencia.Text;
            entidade.Amostra = CBamostra.Checked;

            entidade.DataDeAtracacao = DTPdatadeatracacao.Checked ? DTPdatadeatracacao.Value : (DateTime?)null;
            entidade.CheckDataDeAtracacao = DTPdatadeatracacao.Checked;

            entidade.DataEmbarque = DTPdatadeembarque.Checked ? DTPdatadeembarque.Value : (DateTime?)null;
            entidade.CheckDataEmbarque = DTPdatadeembarque.Checked;

            entidade.Inspecao = DTPdatadeinspecao.Checked ? DTPdatadeinspecao.Value : (DateTime?)null;
            entidade.CheckInspecao = DTPdatadeinspecao.Checked;

            entidade.LI = listaLis.ToList();
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            // Quando o botão "Salvar" for clicado, apenas dispara um evento para o form pai:
            OnConfirmar?.Invoke(this, EventArgs.Empty);
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            // Dispara evento de cancelamento para o form pai
            OnCancelar?.Invoke(this, EventArgs.Empty);
        }

        private void TErro_Tick(object sender, EventArgs e)
        {
            // Lógica de timer, se necessário
        }

        // Eventos que o Form genérico irá escutar
        public event EventHandler OnConfirmar;
        public event EventHandler OnCancelar;
    }
}
