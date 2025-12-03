using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Trabalho
{
    /// <summary>
    /// Gerenciador global de configuração de colunas para DataGridView.
    /// Permite que usuários personalizem quais colunas querem ver em cada grid.
    /// </summary>
    public static class GridColumnManager
    {
        private static readonly Dictionary<string, List<DefinicaoColuna>> _catalogoColunas = new();
        /// <summary>
        /// Registra um catálogo de colunas disponíveis para um grid específico.
        /// </summary>
        public static void RegistrarCatalogo(string nomeGrid, List<DefinicaoColuna> colunas)
        {
            if (string.IsNullOrWhiteSpace(nomeGrid))
                throw new ArgumentException("nomeGrid não pode ser nulo ou vazio.", nameof(nomeGrid));

            if (colunas == null)
                throw new ArgumentNullException(nameof(colunas));

            _catalogoColunas[nomeGrid] = colunas;
        }

        /// <summary>
        /// Obtém todas as colunas disponíveis para um grid.
        /// </summary>
        public static List<DefinicaoColuna> ObterCatalogo(string nomeGrid)
        {
            if (string.IsNullOrWhiteSpace(nomeGrid))
                return new List<DefinicaoColuna>();

            return _catalogoColunas.TryGetValue(nomeGrid, out var colunas)
                ? colunas
                : new List<DefinicaoColuna>();
        }

        /// <summary>
        /// Configura um DataGridView com base nas preferências do usuário.
        /// </summary>
        public static void ConfigurarGrid(
            DataGridView dgv,
            string nomeGrid,
            List<string> colunasVisiveis,
            bool aplicarEstiloPadrao = true)
        {
            if (dgv == null) throw new ArgumentNullException(nameof(dgv));

            dgv.Columns.Clear();

            // Configurações básicas
            dgv.AutoGenerateColumns = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.RowHeadersVisible = false;
            dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dgv.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            dgv.ShowCellToolTips = true;

            var todasColunas = ObterCatalogo(nomeGrid);

            // Se não tem colunas visíveis definidas, usar todas do catálogo
            if (colunasVisiveis == null || !colunasVisiveis.Any())
            {
                colunasVisiveis = todasColunas
                    .Select(c => c.NomePropriedade)
                    .ToList();
            }

            // Adicionar colunas visíveis (apenas as que existem no catálogo)
            foreach (var nomeColuna in colunasVisiveis)
            {
                var definicao = todasColunas.FirstOrDefault(c => c.NomePropriedade == nomeColuna);
                if (definicao == null) continue;

                var coluna = CriarColuna(definicao);
                dgv.Columns.Add(coluna);
            }

            // Aplicar estilo padrão se solicitado
            if (aplicarEstiloPadrao)
            {
                AplicarEstiloPadrao(dgv, todasColunas);
            }
        }

        /// <summary>
        /// Cria uma coluna DataGridView baseada na definição.
        /// </summary>
        private static DataGridViewColumn CriarColuna(DefinicaoColuna def)
        {
            DataGridViewColumn coluna;

            // Criar coluna do tipo adequado
            switch (def.TipoColuna)
            {
                case TipoColunaGrid.CheckBox:
                    coluna = new DataGridViewCheckBoxColumn();
                    break;

                case TipoColunaGrid.ComboBox:
                    var comboCol = new DataGridViewComboBoxColumn();
                    if (def.OpcoesComboBox != null)
                    {
                        comboCol.DataSource = def.OpcoesComboBox;
                    }
                    coluna = comboCol;
                    break;

                case TipoColunaGrid.Image:
                    coluna = new DataGridViewImageColumn();
                    break;

                case TipoColunaGrid.Button:
                    coluna = new DataGridViewButtonColumn();
                    break;

                case TipoColunaGrid.Link:
                    coluna = new DataGridViewLinkColumn();
                    break;

                default:
                    coluna = new DataGridViewTextBoxColumn();
                    break;
            }

            // Configurar propriedades básicas
            coluna.DataPropertyName = def.NomePropriedade;
            coluna.HeaderText = def.TituloExibicao;
            coluna.AutoSizeMode = def.AutoSizeMode;
            coluna.MinimumWidth = def.MinimumWidth;
            coluna.ReadOnly = def.SomenteLeitura;

            // Configurar FillWeight se for Fill
            if (def.AutoSizeMode == DataGridViewAutoSizeColumnMode.Fill)
            {
                coluna.FillWeight = def.FillWeight;
            }

            // Aplicar formatação
            if (!string.IsNullOrEmpty(def.Formato))
            {
                coluna.DefaultCellStyle.Format = def.Formato;
            }

            // Aplicar cor de fundo
            if (def.CorDeFundo.HasValue)
            {
                coluna.DefaultCellStyle.BackColor = def.CorDeFundo.Value;
            }

            // Aplicar cor de texto
            if (def.CorDeTexto.HasValue)
            {
                coluna.DefaultCellStyle.ForeColor = def.CorDeTexto.Value;
            }

            return coluna;
        }

        /// <summary>
        /// Aplica estilo padrão às colunas do grid.
        /// </summary>
        private static void AplicarEstiloPadrao(DataGridView dgv, List<DefinicaoColuna> todasColunas)
        {
            foreach (DataGridViewColumn coluna in dgv.Columns)
            {
                coluna.DefaultCellStyle.Font = new Font("Segoe UI", 10);
                coluna.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                coluna.SortMode = DataGridViewColumnSortMode.Programmatic;

                var definicao = todasColunas.FirstOrDefault(c => c.NomePropriedade == coluna.DataPropertyName);

                // Centralizar se for checkbox ou se a definição pedir
                if (coluna is DataGridViewCheckBoxColumn || (definicao?.Centralizar ?? false))
                {
                    coluna.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
            }
        }

        /// <summary>
        /// Registra todos os catálogos de colunas para os grids da aplicação.
        /// Chame uma vez na inicialização (ex: FrmPrincipal_Load).
        /// </summary>
        public static void RegistrarCatalogosPadrao()
        {
            RegistrarCatalogosProcesso();
            RegistrarCatalogoOrgaoAnuente();
            RegistrarCatalogoVistorias();
        }

        private static void RegistrarCatalogosProcesso()
        {
            // Catálogo base para todos os grids que usam Processo
            var colunasProcesso = new List<DefinicaoColuna>
        {
            new("Ref_USA", "Ref. USA", autoSizeMode: DataGridViewAutoSizeColumnMode.AllCells),
            new("Importador", "Importador", minimumWidth: 200),
            new("SR", "SR", autoSizeMode: DataGridViewAutoSizeColumnMode.AllCells),
            new("Produto", "Produto", minimumWidth: 200),
            new("Marca", "Marca"),
            new("Veiculo", "Veículo"),
            new("PortoDestino", "Porto Destino"),
            new("FLO", "FLO"),
            new("FreeTime", "Free Time", formato: "N0", centralizar: true),
            new("Terminal", "Terminal"),
            new("Conhecimento", "Conhecimento"),
            new("Armador", "Armador"),
            new("CE", "CE"),
            new("Container", "Container"),
            new("PresencaDeCarga", "Presença Carga", tipoColuna: TipoColunaGrid.CheckBox, centralizar: true),
            new("CapaOK", "Capa OK", tipoColuna: TipoColunaGrid.CheckBox, centralizar: true),
            new("SIGVIGLiberado", "SIGVIG Liberado", tipoColuna: TipoColunaGrid.CheckBox, centralizar: true),
            new("SIGVIGSelecionado", "SIGVIG Selecionado", tipoColuna: TipoColunaGrid.CheckBox, centralizar: true),
            new("ResultadoLab", "Resultado Lab", tipoColuna: TipoColunaGrid.CheckBox, centralizar: true),
            new("LocalDeDesembaraco", "Local Desembaraço"),
            new("DI", "DI"),
            new("RascunhoDI", "Rascunho DI"),
            new("DataRegistroDI", "Data Reg. DI", formato: "dd/MM/yyyy"),
            new("DataDesembaracoDI", "Data Desembaraço", formato: "dd/MM/yyyy"),
            new("DataCarregamentoDI", "Data Carregamento", formato: "dd/MM/yyyy"),
            new("DataMinutaDI", "Data Minuta", formato: "dd/MM/yyyy"),
            new("ParametrizacaoDI", "Parametrização DI"),
            new("DataDeAtracacao", "Atracação", formato: "dd/MM/yyyy"),
            new("Inspecao", "Inspeção", formato: "dd/MM/yyyy"),
            new("DataEmbarque", "Embarque", formato: "dd/MM/yyyy"),
            new("DataRecebOriginais", "Receb. Originais", formato: "dd/MM/yyyy"),
            new("FormaRecOriginais", "Forma Rec. Originais"),
            new("Origem", "Origem"),
            new("Amostra", "Amostra", tipoColuna: TipoColunaGrid.CheckBox, centralizar: true),
            new("Desovado", "Desovado", tipoColuna: TipoColunaGrid.CheckBox, centralizar: true),
            new("Redestinacao", "Redestinação", tipoColuna: TipoColunaGrid.CheckBox, centralizar: true),
            new("Numerario", "Numerário", tipoColuna: TipoColunaGrid.CheckBox, centralizar: true),
            new("SigVig", "SIGVIG (Processo)", tipoColuna: TipoColunaGrid.CheckBox, centralizar: true),
            new("PossuiEmbarque", "Possui Embarque", tipoColuna: TipoColunaGrid.CheckBox, centralizar: true),
            new("VencimentoFreeTime", "Venc. Free Time", formato: "dd/MM/yyyy"),
            new("VencimentoFMA", "Venc. FMA", formato: "dd/MM/yyyy"),
            new("VencimentoLI_LPCO", "Venc. LI/LPCO", formato: "dd/MM/yyyy"),
            new("HistoricoDoProcesso", "Histórico", minimumWidth: 250),
            new("Pendencia", "Pendência", minimumWidth: 200),
            new("Status", "Status"),
            new("CondicaoProcesso", "Condição Processo"),
            new("OrgaosAnuentesString", "Órgãos Anuentes")
        };

            string[] gridsProcesso =
            {
            "DGVAguardandoCE",
            "DGVParaRedestinar",
            "DGVRedestinados",
            "DGVAtracadosSemPresencaCarga",
            "DGVSituacaoSIGVIG",
            "DGVAtracadosComPresencaCarga",
            "DGVDeferidos",
            "DGVSolicitarNumerario",
            "DGVDIDUIMPParaDigitacao",
            "DGVItajai",
            "DGVSantos",
            "DGVFinalizados"
        };

            foreach (var nomeGrid in gridsProcesso)
            {
                RegistrarCatalogo(nomeGrid, colunasProcesso);
            }
        }

        private static void RegistrarCatalogoOrgaoAnuente()
        {
            var colunasOrgaoAnuente = new List<DefinicaoColuna>
        {
            new("Ref_USA", "Ref. USA", autoSizeMode: DataGridViewAutoSizeColumnMode.AllCells),
            new("Importador", "Importador", minimumWidth: 200),
            new("NumeroLI", "Número LI"),
            new("Produto", "Produto", minimumWidth: 200),
            new("Container", "Container"),
            new("Terminal", "Terminal"),
            new("Conhecimento", "Conhecimento"),
            new("Origem", "Origem"),
            new("DataChegada", "Data Chegada", formato: "dd/MM/yyyy"),
            new("Inspecao", "Inspeção", formato: "dd/MM/yyyy"),
            new("HistoricoDoProcesso", "Histórico", minimumWidth: 250),
            new("Pendencia", "Pendência", minimumWidth: 200),
            new("LPCO", "LPCO"),
            new("NomeOrgao", "Órgão"),
            new("StatusLPCO", "Status LPCO"),
            new("MotivoExigencia", "Motivo Exigência"),
            new("DataRegistroLPCO", "Data Reg. LPCO", formato: "dd/MM/yyyy"),
            new("ParametrizacaoLPCO", "Parametrização LPCO")
        };

            RegistrarCatalogo("DGVOrgaoAnuente", colunasOrgaoAnuente);
        }

        private static void RegistrarCatalogoVistorias()
        {
            var colunasVistoria = new List<DefinicaoColuna>
        {
            new("LPCO", "LPCO"),
            new("LI", "LI"),
            new("Importador", "Importador", minimumWidth: 200),
            new("Container", "Container"),
            new("Conhecimento", "Conhecimento"),
            new("Ref_USA", "Ref. USA"),
            new("Produto", "Produto", minimumWidth: 200),
            new("ParametrizacaoLPCO", "Parametrização LPCO"),
            new("Terminal", "Terminal"),
            new("Previsao", "Previsão", formato: "dd/MM/yyyy"),
            new("Notas", "Notas", minimumWidth: 250),
            new("Status", "Status", autoSizeMode: DataGridViewAutoSizeColumnMode.AllCells)
        };

            RegistrarCatalogo("DGVVistorias", colunasVistoria);
        }
    }

    /// <summary>
    /// Tipos de colunas suportados.
    /// </summary>
    public enum TipoColunaGrid
    {
        TextBox,
        CheckBox,
        ComboBox,
        Image,
        Button,
        Link
    }

    /// <summary>
    /// Definição completa de uma coluna de grid.
    /// </summary>
    public class DefinicaoColuna
    {
        public string NomePropriedade { get; set; }
        public string TituloExibicao { get; set; }
        public DataGridViewAutoSizeColumnMode AutoSizeMode { get; set; }
        public int MinimumWidth { get; set; }
        public int FillWeight { get; set; }
        public string? Formato { get; set; }
        public bool Centralizar { get; set; }
        public TipoColunaGrid TipoColuna { get; set; }
        public bool SomenteLeitura { get; set; }
        public Color? CorDeFundo { get; set; }
        public Color? CorDeTexto { get; set; }
        public List<string>? OpcoesComboBox { get; set; }

        public DefinicaoColuna(
            string nomePropriedade,
            string tituloExibicao,
            DataGridViewAutoSizeColumnMode autoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
            int minimumWidth = 100,
            int fillWeight = 100,
            string? formato = null,
            bool centralizar = false,
            TipoColunaGrid tipoColuna = TipoColunaGrid.TextBox,
            bool somenteLeitura = false,
            Color? corDeFundo = null,
            Color? corDeTexto = null,
            List<string>? opcoesComboBox = null)
        {
            NomePropriedade = nomePropriedade;
            TituloExibicao = tituloExibicao;
            AutoSizeMode = autoSizeMode;
            MinimumWidth = minimumWidth;
            FillWeight = fillWeight;
            Formato = formato;
            Centralizar = centralizar;
            TipoColuna = tipoColuna;
            SomenteLeitura = somenteLeitura;
            CorDeFundo = corDeFundo;
            CorDeTexto = corDeTexto;
            OpcoesComboBox = opcoesComboBox;
        }
    }
}