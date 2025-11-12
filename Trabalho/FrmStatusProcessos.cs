using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CLUSA; // Models e ProcessoHelper aqui

namespace Trabalho
{
    public partial class FrmStatusProcessos : Form
    {
        private List<Processo> _processosExibidos = new List<Processo>();
        public enum StatusBloco
        {
            AguardandoCE,
            ParaRedestinar,
            Redestinados,
            AtracadosSemPresencaCarga,
            SituacaoSIGVIG,
            AtracadosComPresencaCarga,
            Deferidos,
            SolicitarNumerario,
            DIDUIMPParaDigitacao
        }

        // Otimizado: apenas info visual dos blocos
        private static readonly Dictionary<StatusBloco, (string Nome, Color Cor)> BlocoInfo =
            new()
            {
                { StatusBloco.AguardandoCE, ("Aguardando CE", Color.BlueViolet) },
                { StatusBloco.ParaRedestinar, ("Para Redestinar", Color.Red) },
                { StatusBloco.Redestinados, ("Redestinados", Color.FromArgb(0,192,192)) },
                { StatusBloco.AtracadosSemPresencaCarga, ("Atracados S/Presença de Carga", Color.Yellow) },
                { StatusBloco.SituacaoSIGVIG, ("Situação SIGVIG", Color.FromArgb(255,128,0)) },
                { StatusBloco.AtracadosComPresencaCarga, ("Atracados com Presença de Carga", Color.Black) },
                { StatusBloco.Deferidos, ("Deferidos", Color.Lime) },
                { StatusBloco.SolicitarNumerario, ("Solicitar Numerário", Color.FromArgb(255,192,192)) },
                { StatusBloco.DIDUIMPParaDigitacao, ("DI/DUIMP para Digitação", Color.FromArgb(192,0,0)) }
            };

        private List<Processo> _todosProcessos = new();

        public FrmStatusProcessos()
        {
            InitializeComponent();
            MostrarItens.Visible = false;
        }
        private async void FrmStatusProcessos_Load(object? sender, EventArgs e)
        {
            await CarregarProcessosAsync();
        }
        // Só filtra pelo status calculado
        private List<Processo> ObterProcessosPorStatus(StatusBloco status)
        {
            string statusStr = status.ToString();
            return _todosProcessos
                .Where(p => p.CondicaoProcesso == statusStr)
                .ToList();
        }

        // Chame sempre que abrir/trocar/cadastrar/editar processos
        private async Task CarregarProcessosAsync()
        {
            var processoService = new RepositorioProcesso();
            var todos = await processoService.ListarTodosAsync();
            _todosProcessos = todos.Where(p => !string.Equals(p.Status, "Finalizado", StringComparison.OrdinalIgnoreCase)).ToList();
            foreach (var p in _todosProcessos)
                ProcessoHelper.AtualizarCondicaoProcesso(p);
            AtualizarContadores();
        }


        private void MostrarItensPorStatus(StatusBloco status)
        {
            var processos = ObterProcessosPorStatus(status);
            _processosExibidos = processos;
            var blocoInfo = BlocoInfo[status];

            LblTitulo.Text = $"{blocoInfo.Nome} ({processos.Count})";
            LblTitulo.ForeColor = blocoInfo.Cor == Color.Black ? Color.White : Color.Black;
            LblTitulo.BackColor = blocoInfo.Cor;

            var dadosExibicao = processos.Select(p => new
            {
                p.Ref_USA,
                p.Importador,
                p.Exportador,
                p.Produto,
                CE = p.Capa?.CE ?? "-",
                p.Container,
                DataAtracacao = p.DataDeAtracacao?.ToString("dd/MM/yyyy") ?? "-",
                p.PresencaDeCarga,
                SIGVIG = p.Capa?.SigvigLiberado == true ? "✓ Liberado" :
                         p.Capa?.SigvigSelecionado == true ? "⚠ Selecionado" : "Pendente",
                LPCOs = ProcessoHelper.ObterResumoLPCOs(p),
                OrgaosAnuentes = p.OrgaosAnuentesString,
                Parametrizacao = p.ParametrizacaoDI,
                RascunhoDI = p.RascunhoDI,
                DataRegistroDI = p.DataRegistroDI?.ToString("dd/MM/yyyy") ?? "-",
                DataEmbarque = p.DataEmbarque?.ToString("dd/MM/yyyy") ?? "-",
                p.Status,
                p.CondicaoProcesso
            }).ToList();

            DGVSelecionado.DataSource = dadosExibicao;
            ConfigurarDataGridView();

            Blocos.Visible = false;
            MostrarItens.Visible = true;
        }

        // UI helpers
        private void ConfigurarDataGridView()
        {
            DGVSelecionado.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            DGVSelecionado.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            DGVSelecionado.MultiSelect = false;
            DGVSelecionado.ReadOnly = true;
            DGVSelecionado.AllowUserToAddRows = false;
            DGVSelecionado.RowHeadersVisible = false;
            DGVSelecionado.BackgroundColor = Color.White;
            DGVSelecionado.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);

            if (DGVSelecionado.Columns.Count > 0)
            {
                var headerStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    BackColor = Color.FromArgb(230, 230, 230),
                    ForeColor = Color.Black
                };

                foreach (DataGridViewColumn col in DGVSelecionado.Columns)
                    col.HeaderCell.Style = headerStyle;

                if (DGVSelecionado.Columns.Contains("Ref_USA"))
                    DGVSelecionado.Columns["Ref_USA"].HeaderText = "Ref. USA";

                if (DGVSelecionado.Columns.Contains("CE"))
                    DGVSelecionado.Columns["CE"].HeaderText = "CE (Conhecimento)";

                if (DGVSelecionado.Columns.Contains("DataAtracacao"))
                    DGVSelecionado.Columns["DataAtracacao"].HeaderText = "Data Atracação";

                if (DGVSelecionado.Columns.Contains("PresencaDeCarga"))
                    DGVSelecionado.Columns["PresencaDeCarga"].HeaderText = "Presença Carga";

                if (DGVSelecionado.Columns.Contains("LPCOs"))
                    DGVSelecionado.Columns["LPCOs"].HeaderText = "LPCOs (Deferidos/Total)";

                if (DGVSelecionado.Columns.Contains("OrgaosAnuentes"))
                    DGVSelecionado.Columns["OrgaosAnuentes"].HeaderText = "Órgãos Anuentes";

                if (DGVSelecionado.Columns.Contains("Parametrizacao"))
                    DGVSelecionado.Columns["Parametrizacao"].HeaderText = "Parametrização";

                if (DGVSelecionado.Columns.Contains("RascunhoDI"))
                    DGVSelecionado.Columns["RascunhoDI"].HeaderText = "Rascunho DI";

                if (DGVSelecionado.Columns.Contains("DataRegistroDI"))
                    DGVSelecionado.Columns["DataRegistroDI"].HeaderText = "Data Registro DI";

                if (DGVSelecionado.Columns.Contains("DataEmbarque"))
                    DGVSelecionado.Columns["DataEmbarque"].HeaderText = "Data Embarque";

                if (DGVSelecionado.Columns.Contains("CondicaoProcesso"))
                    DGVSelecionado.Columns["CondicaoProcesso"].HeaderText = "Condição Atual";
            }
        }

        private void AtualizarContadores()
        {
            foreach (StatusBloco status in Enum.GetValues(typeof(StatusBloco)))
            {
                var count = ObterProcessosPorStatus(status).Count;
                var label = ObterLabelPorStatus(status);
                var textoBase = BlocoInfo[status].Nome;
                if (label != null)
                    label.Text = count > 0 ? $"{textoBase}\n({count})" : textoBase;
            }
        }

        // Métodos de vinculação de UI
        private Label ObterLabelPorStatus(StatusBloco status)
        {
            return status switch
            {
                StatusBloco.AguardandoCE => BtnAguardandoCE,
                StatusBloco.ParaRedestinar => BtnParaRedestinar,
                StatusBloco.Redestinados => BtnRedestinados,
                StatusBloco.AtracadosSemPresencaCarga => BtnAtracadosSPresencaDeCarga,
                StatusBloco.SituacaoSIGVIG => BtnSituacaoSIGVIG,
                StatusBloco.AtracadosComPresencaCarga => BtnAtracadosCPresencaDeCarga,
                StatusBloco.Deferidos => BtnDeferidos,
                StatusBloco.SolicitarNumerario => BtnSolicitarNumerario,
                StatusBloco.DIDUIMPParaDigitacao => BtnDIDUIMPParaDigitacao,
                _ => throw new ArgumentException($"Status inválido: {status}", nameof(status))
            };
        }

        // Event handlers por status
        private void BtnAguardandoCE_Click(object sender, EventArgs e) =>
            MostrarItensPorStatus(StatusBloco.AguardandoCE);

        private void BtnParaRedestinar_Click(object sender, EventArgs e) =>
            MostrarItensPorStatus(StatusBloco.ParaRedestinar);

        private void BtnRedestinados_Click(object sender, EventArgs e) =>
            MostrarItensPorStatus(StatusBloco.Redestinados);

        private void BtnAtracadosSPresencaDeCarga_Click(object sender, EventArgs e) =>
            MostrarItensPorStatus(StatusBloco.AtracadosSemPresencaCarga);

        private void BtnSituacaoSIGVIG_Click(object sender, EventArgs e) =>
            MostrarItensPorStatus(StatusBloco.SituacaoSIGVIG);

        private void BtnAtracadosCPresencaDeCarga_Click(object sender, EventArgs e) =>
            MostrarItensPorStatus(StatusBloco.AtracadosComPresencaCarga);

        private void BtnDeferidos_Click(object sender, EventArgs e) =>
            MostrarItensPorStatus(StatusBloco.Deferidos);

        private void BtnSolicitarNumerario_Click(object sender, EventArgs e) =>
            MostrarItensPorStatus(StatusBloco.SolicitarNumerario);

        private void BtnDIDUIMPParaDigitacao_Click(object sender, EventArgs e) =>
            MostrarItensPorStatus(StatusBloco.DIDUIMPParaDigitacao);
        private async void DGVSelecionado_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _processosExibidos.Count) return;

            var processoSelecionado = _processosExibidos[e.RowIndex];

            using var frm = new FrmModificaProcesso { processo = processoSelecionado, Modo = "Editar" };
            frm.ShowDialog();

            // Se quiser que a atualização já seja async:
            await CarregarProcessosAsync();
        }
        private void BtnVoltar_Click(object sender, EventArgs e)
        {
            MostrarItens.Visible = false;
            Blocos.Visible = true;
            AtualizarContadores();
        }
    }
}
