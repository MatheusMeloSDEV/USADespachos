using CLUSA.Helpers;
using CLUSA.Models;
using CLUSA.Repositories;
using CLUSA.Services;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.Data;
using System.Diagnostics;
using System.Reflection;

namespace Trabalho
{
    public enum OrigemProcesso
    {
        Santos,
        Itajai
    }
    public partial class FrmModificaProcesso : Form
    {
        public Processo processo { get; set; } = null!;
        private Processo _processoOriginal = null!;
        public Logado UsuarioLogado { get; set; }
        public string Modo { get; set; } = "Adicionar";
        public bool Visualização { get; set; } = false;
        private bool _atualizandoGridCatalogo = false;
        public OrigemProcesso Origem { get; set; }
        private FrmLoadingOverlay? _overlay;
        private readonly RepositorioProcesso _repositorio;
        private readonly RepositorioNotifUrgente _repoNotifUrgente;
        private readonly RepositorioNotificacao _notificacaoRepo;
        private readonly RepositorioLog _logRepo;
        private bool _dadosForamAlterados = false;

        public FrmModificaProcesso()
        {
            InitializeComponent();
            _repositorio = new RepositorioProcesso();
            _notificacaoRepo = new RepositorioNotificacao();
            _repoNotifUrgente = new RepositorioNotifUrgente();
            _logRepo = new RepositorioLog();
        }

        private void FrmModificaProcesso_Load(object? sender, EventArgs e)
        {
            if (Modo == "Editar" && processo != null)
            {
                var settings = new JsonSerializerSettings();
                settings.Converters.Add(new ObjectIdConverter());

                var json = JsonConvert.SerializeObject(processo, settings);
                _processoOriginal = JsonConvert.DeserializeObject<Processo>(json, settings) ?? new Processo();
            }
            else
            {
                // Se for novo, o original é vazio
                _processoOriginal = new Processo();
            }
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            switch (Origem)
            {
                case OrigemProcesso.Itajai:
                    // 'L' significa Letra, 'I' e 'T' são letras fixas, 'J' é opcional
                    TXTnr.Mask = @"0000/00\I\TJ";
                    break;
                case OrigemProcesso.Santos:
                default:
                    TXTnr.Mask = "0000/00";
                    break;
            }
            MostrarLoading($"Carregando Dados...");
            CarregarDadosNosControles();
            EsconderLoading();
            MostrarLoading($"Carregando Sugestões...");
            _ = Task.Run(() => ConfigurarAutoCompletarAsync());
            EsconderLoading();
            ConfigurarFormularioPeloModo();
            AnexarEventoDeAlteracao(this);
            AtualizarEstadoBotoesLI();
        }
        private void AtualizarEstadoBotoesLI()
        {
            // O botão de excluir só fica ativo se houver pelo menos uma aba no controle.
            bool possuiLIsReais = processo.LI != null && processo.LI.Any(li => !string.IsNullOrWhiteSpace(li.Numero) && li.Numero != "Nova LI");
            if (possuiLIsReais)
            {
                // Se tem dados de LI salvos, foca na aba de LI/LPCO
                tcCat_LI.SelectedTab = TabLI_LPCO;
            }
            else
            {
                tcCat_LI.SelectedTab = tabCatalogo;
            }
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
        private string GerarLogCompletoDeAlteracoes()
        {
            var mudancas = new List<string>();

            // 1. Compara todas as propriedades simples do PROCESSO (String, Int, Date, Bool...)
            // Ignora: Listas e Objetos complexos que faremos separadamente
            // IMPORTANTE: "Catalogos" foi adicionado aqui para o Reflection não bugar
            var ignorar = new HashSet<string> { "Id", "_id", "LI", "Capa", "DocRecebidos", "Catalogos" };
            mudancas.AddRange(CompararPropriedades(_processoOriginal, processo, ignorar));

            // 2. Compara a CAPA (se existir)
            if (_processoOriginal.Capa != null && processo.Capa != null)
            {
                var mudancasCapa = CompararPropriedades(_processoOriginal.Capa, processo.Capa, new HashSet<string>());
                if (mudancasCapa.Any())
                {
                    mudancas.Add("[Alterações na CAPA]:");
                    mudancas.AddRange(mudancasCapa);
                }
            }

            // 2.5 Compara a Lista de CATÁLOGOS
            int qtdCatAntigo = _processoOriginal.Catalogos?.Count ?? 0;
            int qtdCatNovo = processo.Catalogos?.Count ?? 0;

            if (qtdCatAntigo != qtdCatNovo)
            {
                mudancas.Add($"Qtd de Catálogos: {qtdCatAntigo} -> {qtdCatNovo}");
            }
            else if (qtdCatNovo > 0)
            {
                // Se a quantidade é a mesma, verifica se os dados internos mudaram (usando JSON)
                string jsonAntigo = JsonConvert.SerializeObject(_processoOriginal.Catalogos);
                string jsonNovo = JsonConvert.SerializeObject(processo.Catalogos);

                if (jsonAntigo != jsonNovo)
                {
                    mudancas.Add("Dados internos do(s) Catálogo(s) atualizados");
                }
            }

            // 3. Compara Listas Específicas (LIs)
            CompararListaLIs(mudancas);

            // 4. Compara CheckedListBox (Docs Recebidos)
            CompararArrays(mudancas, "Documentos", _processoOriginal.DocRecebidos, processo.DocRecebidos);

            if (mudancas.Count == 0) return "Sem alterações detectadas.";

            return string.Join("; ", mudancas);
        }

        /// <summary>
        /// Mágica do Reflection: Compara propriedade por propriedade
        /// </summary>
        private List<string> CompararPropriedades(object antigo, object novo, HashSet<string> ignorar)
        {
            var diferencas = new List<string>();

            if (antigo == null || novo == null) return diferencas;

            PropertyInfo[] propriedades = antigo.GetType().GetProperties();

            foreach (var prop in propriedades)
            {
                if (ignorar.Contains(prop.Name)) continue;

                // CORREÇÃO AQUI: 'object?' permite que o valor seja nulo
                object? valorAntigo = prop.GetValue(antigo);
                object? valorNovo = prop.GetValue(novo);

                // O compilador agora aceita passar 'object?' porque ajustamos o FormatarValor abaixo
                string sAntigo = FormatarValor(valorAntigo);
                string sNovo = FormatarValor(valorNovo);

                if (sAntigo != sNovo)
                {
                    diferencas.Add($"{prop.Name}: '{sAntigo}' -> '{sNovo}'");
                }
            }

            return diferencas;
        }

        private string FormatarValor(object val)
        {
            return val switch
            {
                null => "",
                DateTime dt => dt.ToShortDateString(),
                decimal dec => dec.ToString("N2"),
                bool b => b ? "Sim" : "Não",
                _ => val.ToString() ?? ""
            };
        }

        private void CompararListaLIs(List<string> logs)
        {
            var listaAntiga = _processoOriginal.LI ?? new List<LicencaImportacao>();
            var listaNova = processo.LI ?? new List<LicencaImportacao>();

            int qtdOriginal = listaAntiga.Count;
            int qtdAtual = listaNova.Count;

            if (qtdOriginal != qtdAtual)
            {
                logs.Add($"Quantidade de LIs: {qtdOriginal} -> {qtdAtual}");
            }
            else
            {
                for (int i = 0; i < qtdAtual; i++)
                {
                    var liAntiga = listaAntiga[i];
                    var liNova = listaNova[i];

                    // Usado para identificar a LI no texto do Log
                    string identificadorLi = string.IsNullOrWhiteSpace(liNova.Numero) ? $"[{i + 1}]" : liNova.Numero;

                    // 1. Compara os dados da própria LI
                    if (liAntiga.Numero != liNova.Numero)
                        logs.Add($"LI {identificadorLi} (Número): '{liAntiga.Numero}' -> '{liNova.Numero}'");

                    if (liAntiga.NCM != liNova.NCM)
                        logs.Add($"LI {identificadorLi} (NCM): '{liAntiga.NCM}' -> '{liNova.NCM}'");

                    if (liAntiga.Amostra != liNova.Amostra)
                        logs.Add($"LI {identificadorLi} (Amostra): '{(liAntiga.Amostra ? "Sim" : "Não")}' -> '{(liNova.Amostra ? "Sim" : "Não")}'");

                    if (liAntiga.DataRegistro != liNova.DataRegistro)
                        logs.Add($"LI {identificadorLi} (Data Registro): '{FormatarValor(liAntiga.DataRegistro)}' -> '{FormatarValor(liNova.DataRegistro)}'");

                    // 2. Compara a lista de LPCOs dentro desta LI
                    var lpcosAntigos = liAntiga.LPCO ?? new List<LpcoInfo>();
                    var lpcosNovos = liNova.LPCO ?? new List<LpcoInfo>();

                    if (lpcosAntigos.Count != lpcosNovos.Count)
                    {
                        logs.Add($"LI {identificadorLi} (Qtd LPCOs): {lpcosAntigos.Count} -> {lpcosNovos.Count}");
                    }
                    else
                    {
                        // Compara campo a campo de cada LPCO
                        for (int j = 0; j < lpcosNovos.Count; j++)
                        {
                            var lAntigo = lpcosAntigos[j];
                            var lNovo = lpcosNovos[j];

                            string idLpco = string.IsNullOrWhiteSpace(lNovo.LPCO) ? $"[{j + 1}]" : lNovo.LPCO;

                            if (lAntigo.NomeOrgao != lNovo.NomeOrgao)
                                logs.Add($"LI {identificadorLi} - LPCO {idLpco} (Órgão): '{lAntigo.NomeOrgao}' -> '{lNovo.NomeOrgao}'");

                            if (lAntigo.LPCO != lNovo.LPCO)
                                logs.Add($"LI {identificadorLi} - LPCO {idLpco} (Número): '{lAntigo.LPCO}' -> '{lNovo.LPCO}'");

                            if (lAntigo.StatusLPCO != lNovo.StatusLPCO)
                                logs.Add($"LI {identificadorLi} - LPCO {idLpco} (Status): '{lAntigo.StatusLPCO}' -> '{lNovo.StatusLPCO}'");

                            if (lAntigo.ParametrizacaoLPCO != lNovo.ParametrizacaoLPCO)
                                logs.Add($"LI {identificadorLi} - LPCO {idLpco} (Canal): '{lAntigo.ParametrizacaoLPCO}' -> '{lNovo.ParametrizacaoLPCO}'");

                            if (lAntigo.EmExigencia != lNovo.EmExigencia)
                                logs.Add($"LI {identificadorLi} - LPCO {idLpco} (Exigência): '{(lAntigo.EmExigencia ? "Sim" : "Não")}' -> '{(lNovo.EmExigencia ? "Sim" : "Não")}'");

                            if (lAntigo.MotivoExigencia != lNovo.MotivoExigencia)
                                logs.Add($"LI {identificadorLi} - LPCO {idLpco} (Motivo Exig.): '{lAntigo.MotivoExigencia}' -> '{lNovo.MotivoExigencia}'");

                            if (lAntigo.DataRegistroLPCO != lNovo.DataRegistroLPCO)
                                logs.Add($"LI {identificadorLi} - LPCO {idLpco} (Registro): '{FormatarValor(lAntigo.DataRegistroLPCO)}' -> '{FormatarValor(lNovo.DataRegistroLPCO)}'");

                            if (lAntigo.DataDeferimentoLPCO != lNovo.DataDeferimentoLPCO)
                                logs.Add($"LI {identificadorLi} - LPCO {idLpco} (Deferimento): '{FormatarValor(lAntigo.DataDeferimentoLPCO)}' -> '{FormatarValor(lNovo.DataDeferimentoLPCO)}'");
                        }
                    }
                }
            }
        }

        private void CompararArrays(List<string> logs, string nomeCampo, string[] antigo, string[] novo)
        {
            string strAntigo = antigo == null ? "" : string.Join(", ", antigo);
            string strNovo = novo == null ? "" : string.Join(", ", novo);

            if (strAntigo != strNovo)
            {
                logs.Add($"{nomeCampo}: [{strAntigo}] -> [{strNovo}]");
            }
        }
        private void frmModificaProcesso_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (_dadosForamAlterados)
            {
                var resultado = MessageBox.Show(
                    "Você tem alterações não salvas. Deseja fechar e descartar?",
                    "Atenção",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (resultado == DialogResult.No)
                {
                    e.Cancel = true; // Cancela o fechamento
                }
            }
        }
        #region "Configuração, Carregamento e Salvamento"

        private void ConfigurarFormularioPeloModo()
        {
            this.Text = $"{Modo} Processo";
            if (Modo == "Editar")
            {
                TXTnr.Enabled = false; // Não permite editar a Ref. USA
            }
            else if (Modo == "Adicionar")
            {
                btnCapa.Enabled = false;
                btnRelatorio.Enabled = false;
                if (processo.LI == null || !processo.LI.Any())
                {
                    if (processo.LI == null)
                    {
                        processo.LI = new List<LicencaImportacao>();
                    }
                    processo.LI.Add(new LicencaImportacao { Numero = "Nova LI" });
                }
            }

            if (Visualização)
            {
                SetCamposSomenteLeitura(this);
                btnAdiciona.Visible = false;
            }
        }

        private void CarregarDadosNosControles()
        {
            BsModificaProcesso.DataSource = processo;
            CarregarControlesDeData();
            CarregarCheckedListBoxes();
            PopularMarca();
            CarregarAbasLi();

            if (processo.Catalogos == null) processo.Catalogos = new List<Catalogo>();
            RbRegistroRegistrado.Checked = processo.RegistroRegistrado;
            RbRegistroPendente.Checked = processo.RegistroPendente;
            AtualizarGridCatalogos();
        }

        private void SalvarDadosDosControles()
        {
            BsModificaProcesso.EndEdit();
            this.ValidateChildren();

            // 1. Salva dados gerais (Capa, Datas, CheckBoxes)
            processo.DocRecebidos = ObterItensSelecionados(checkedListBox1);
            processo.FormaRecOriginais = checkedListBox2.CheckedItems.Count > 0 ? checkedListBox2.CheckedItems[0]?.ToString() ?? "" : "";
            processo.Marca = (new[] { "Sacos", "Caixas", "Pallets" }.Contains(cbMarca.Text))
                ? $"{numMarca.Value} {cbMarca.Text}"
                : $"{numMarca.Value} x {cbMarca.Text}";

            processo.DataRegistroDI = DTPdataderegistrodi.Checked ? DTPdataderegistrodi.Value : null;
            processo.DataDesembaracoDI = DTPdatadedesembaracodi.Checked ? DTPdatadedesembaracodi.Value : null;
            processo.DataCarregamentoDI = DTPdatadecarregamentodi.Checked ? DTPdatadecarregamentodi.Value : null;
            processo.Inspecao = DTPdatadeinspecao.Checked ? DTPdatadeinspecao.Value : null;
            processo.DataDeAtracacao = DTPdatadeatracacao.Checked ? DTPdatadeatracacao.Value : null;
            processo.DataEmbarque = DTPdatadeembarque.Checked ? DTPdatadeembarque.Value : null;
            processo.DataRecebOriginais = DTPDataRecOriginais.Checked ? DTPDataRecOriginais.Value : null;
            processo.DataMinutaDI = dtpDataMinuta.Checked ? dtpDataMinuta.Value : null;
            processo.Capa.CE = txtCE.Text;

            // Cálculo de Vencimentos
            if (DTPdatadeatracacao.Checked)
            {
                processo.VencimentoFMA = DataHelper.CalcularVencimento(DTPdatadeatracacao.Value, 85);
                dtpVencimentoFMA.Value = processo.VencimentoFMA ?? dtpVencimentoFMA.Value;

                processo.VencimentoFreeTime = DataHelper.CalcularVencimento(DTPdatadeatracacao.Value, Convert.ToInt32(NUMfreetime.Value) - 1);
                dtpVencimentoFreeTime.Value = processo.VencimentoFreeTime ?? dtpVencimentoFreeTime.Value;
            }
            else
            {
                processo.VencimentoFMA = null;
                processo.VencimentoFreeTime = null;
            }

            // Vencimento LI/LPCO
            DateTime? dataMaisAntiga = null;
            if (processo.LI != null && processo.LI.Count > 0)
            {
                dataMaisAntiga = processo.LI
                    .Where(li => li.DataRegistro.HasValue)
                    .Min(li => li.DataRegistro);
            }

            if (dataMaisAntiga.HasValue)
            {
                processo.VencimentoLI_LPCO = DataHelper.CalcularVencimento(dataMaisAntiga.Value, 80);
                if (processo.VencimentoLI_LPCO.HasValue)
                    dtpVencimentoLI_LPCO.Value = processo.VencimentoLI_LPCO.Value;
            }
            else
            {
                processo.VencimentoLI_LPCO = null;
            }

            processo.RegistroPendente = RbRegistroPendente.Checked;
            processo.RegistroRegistrado = RbRegistroRegistrado.Checked;

            processo.HistoricoDoProcesso = TXTstatusdoprocesso.Text;
            processo.Pendencia = TXTpendencia.Text;

            if (processo.Capa == null) processo.Capa = new Capa();
            processo.Capa.Container = processo.Container;
            processo.Capa.Master = processo.Veiculo;
            processo.Capa.SigvigSelecionado = processo.SIGVIGSelecionado;
            processo.Capa.SigvigLiberado = processo.SIGVIGLiberado;


            foreach (TabPage abaLi in TCLi.TabPages)
            {
                if (abaLi.Controls.OfType<LIEditControl>().FirstOrDefault() is LIEditControl liControl)
                {
                    liControl.SalvarAlteracoes();
                }
            }

            processo.LI.RemoveAll(li => string.IsNullOrWhiteSpace(li.Numero) || li.Numero == "Nova LI");

            var duplicadas = processo.LI
                .GroupBy(li => li.Numero)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicadas.Any())
            {
                string msg = $"O sistema encontrou LIs com números iguais!\n" +
                             $"Duplicados: {string.Join(", ", duplicadas)}\n\n" +
                             $"Por favor, corrija os números ou apague as abas extras antes de salvar.";

                throw new Exception(msg);
            }


            ProcessoHelper.AtualizarCondicaoProcesso(processo);
        }
        private async void btnAdiciona_Click(object? sender, EventArgs e)
        {
            try
            {
                string logDetalhado = "";

                if (Modo == "Adicionar" && !string.IsNullOrWhiteSpace(TXTnr.Text))
                {
                    bool refUsaExiste = await _repositorio.VerificarRefUsaExisteAsync(TXTnr.Text);

                    if (refUsaExiste)
                    {
                        MessageBox.Show(
                            $"A Ref_USA '{TXTnr.Text}' já existe no banco de dados!\n\nPor favor, utilize uma referência diferente.",
                            "Ref_USA Duplicada",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );

                        TXTnr.Clear();
                        TXTnr.Focus();
                        return;
                    }
                }

                SalvarDadosDosControles();

                await VerificarMudancaOriginaisAsync();

                if (Modo == "Adicionar")
                {
                    await _repositorio.CreateAsync(processo);

                    await _logRepo.RegistrarLogAsync("Criação", UsuarioLogado?.Usuario, $"Novo processo: {processo.Ref_USA}");

                    var settings = new Newtonsoft.Json.JsonSerializerSettings();
                    settings.Converters.Add(new ObjectIdConverter());
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(processo, settings);
                    _processoOriginal = Newtonsoft.Json.JsonConvert.DeserializeObject<Processo>(json, settings) ?? new Processo();

                    Modo = "Editar";
                    TXTnr.Enabled = false;

                }
                else // MODO EDIÇÃO
                {
                    logDetalhado = GerarLogCompletoDeAlteracoes();

                    // GERA A LISTA DE CAMPOS QUE REALMENTE MUDARAM
                    var atualizacoes = GerarAtualizacoesParciaisParaBanco();

                    if (atualizacoes.Count > 0)
                    {
                        // Salva NO BANCO apenas as partes alteradas!
                        await _repositorio.UpdateParcialAsync(processo.Id, atualizacoes);

                        // Grava no log do sistema
                        await _logRepo.RegistrarLogAsync(
                            "Edição", UsuarioLogado?.Usuario,
                            $"Atualização em {processo.Ref_USA}",
                            logDetalhado
                        );
                    }

                    // Atualiza a cópia original local
                    var settings = new Newtonsoft.Json.JsonSerializerSettings();
                    settings.Converters.Add(new ObjectIdConverter());
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(processo, settings);
                    _processoOriginal = Newtonsoft.Json.JsonConvert.DeserializeObject<Processo>(json, settings) ?? new Processo();
                }

                await SincronizarOrgaoAnuenteAsync();

                await ExcluirNotificacoesAutomaticamente(processo);

                btnCapa.Enabled = true;
                btnRelatorio.Enabled = true;

                _dadosForamAlterados = false;
                this.Text = this.Text.Replace("*", "");

                // --- NOVA LÓGICA: FOLLOW UP AUTOMÁTICO PARA LEITESOL ---
                if (!string.IsNullOrEmpty(processo.Importador) && processo.Importador.Equals("LEITESOL", StringComparison.OrdinalIgnoreCase))
                {
                    bool historicoMudou = logDetalhado.Contains("HistoricoDoProcesso");

                    // Se for um novo processo (Modo Adicionar), consideramos que "mudou" (criou)
                    // Como a variável Modo muda para "Editar" no bloco de adição acima, 
                    // usamos a variável historicoMudou também se o log estiver vazio e for um processo novo
                    if (string.IsNullOrEmpty(logDetalhado)) historicoMudou = true;

                    using (var frmSucesso = new FrmDialogoSucesso(historicoMudou))
                    {
                        frmSucesso.ShowDialog(this);

                        if (frmSucesso.EnviarEmail)
                        {
                            try
                            {
                                string assunto = $"Atualização processo SRef: {processo.SR}";
                                string corpo = "";

                                // ATENÇÃO: Se o seu FollowUpService exigir a logo no construtor (como no Program.cs),
                                // você precisará passar aqui: new CLUSA.Services.FollowUpService(Trabalho.Properties.Resources.FollowUpLogo);
                                var followUpService = new CLUSA.Services.FollowUpService();

                                string nomeImportador = processo.Importador;
                                byte[] pdfBytes = await followUpService.GerarPdfBytesAsync(nomeImportador);
                                string nomeArquivoAnexo = $"FollowUp_{nomeImportador.Replace(" ", "_")}.pdf";

                                await CLUSA.Services.EmailService.EnviarFollowUpAsync(
                                    assunto,
                                    corpo,
                                    pdfBytes,
                                    nomeArquivoAnexo
                                );

                                MessageBox.Show("Salvo com sucesso e E-mail enviado com o PDF anexo!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            catch (Exception exEmail)
                            {
                                MessageBox.Show($"Salvo, mas houve erro ao enviar e-mail: {exEmail.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
                else
                {
                    // Fluxo normal para outros importadores
                    MessageBox.Show("Salvo com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                // --------------------------------------------------------
            }
            catch (Exception ex)
            {
                // Pega o nome do usuário. Se por algum motivo vier vazio, salva como "Sistema"
                string nomeAutor = UsuarioLogado?.Usuario ?? "Sistema";

                await _logRepo.RegistrarLogAsync(
                    "Erro",                                       // Tipo
                    nomeAutor,                                    // Autor
                    $"Falha ao salvar processo {processo?.Ref_USA}", // Mensagem
                    ex.Message                                    // Detalhes
                );

                MessageBox.Show($"Erro ao salvar o processo: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async Task ConfigurarAutoCompletarAsync()
        {
            // Mapeie aqui os seus TextBoxes e o nome exato da coluna no MongoDB
            // Substitua os nomes (ex: TXTimportador) pelos nomes reais dos seus controles
            var mapeamento = new Dictionary<TextBox, string>
            {
                { TXTimportador, "Importador" },
                { TXTexportador, "Exportador" },
                { TXTProduto, "Produto" },
                { txtArmador, "Armador" },
                { txtOrigem, "Origem" },
                { txtTerminal, "Terminal" },
                { txtVeiculo, "Veiculo"  },
                { TXTportodedestino, "PortoDestino" },
                { txtLocalDeDesembaraco, "LocalDeDesembaraco" }
            };

            foreach (var item in mapeamento)
            {
                try
                {
                    // Busca apenas os valores únicos daquela coluna no banco (Super rápido)
                    var valores = await _repositorio.ObterValoresUnicosAsync(item.Value);

                    var colecao = new AutoCompleteStringCollection();
                    colecao.AddRange(valores.Where(v => !string.IsNullOrWhiteSpace(v)).ToArray());

                    // Configura o TextBox na Thread da UI
                    this.Invoke(() =>
                    {
                        item.Key.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                        item.Key.AutoCompleteSource = AutoCompleteSource.CustomSource;
                        item.Key.AutoCompleteCustomSource = colecao;
                    });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Erro autocomplete {item.Value}: {ex.Message}");
                }
            }
            this.Invoke(() =>
            {
                var colecaoContainer = new AutoCompleteStringCollection();
                colecaoContainer.Add("Carga Solta");

                TxtContainer.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                TxtContainer.AutoCompleteSource = AutoCompleteSource.CustomSource;
                TxtContainer.AutoCompleteCustomSource = colecaoContainer;
            });
        }
        private async Task VerificarMudancaOriginaisAsync()
        {
            // Lógica: Se antes era NULL e agora TEM VALOR
            bool estavaVazio = !_processoOriginal.DataRecebOriginais.HasValue;
            bool agoraTemValor = processo.DataRecebOriginais.HasValue;

            if (estavaVazio && agoraTemValor)
            {
                var novaNotificacao = new NotifUrgente
                {
                    Id = ObjectId.GenerateNewId(),
                    // TODO: Substituir pelo ID do usuário logado na sessão atual
                    UsuarioOrigemId = new ObjectId("69162b44257801e12e9e3dfe"),

                    // TODO: Substituir pelo ID do usuário que deve receber (ex: Gerente ou setor financeiro)
                    UsuarioDestinoId = new ObjectId("68b86f90ab6f33bd01680922"),

                    Mensagem = $"PROTOCOLAR CSI ORIGINAL NO MAPA - Processo {processo.Ref_USA} recebidos em {processo.DataRecebOriginais.Value:dd/MM/yyyy}!",
                    DataEnvio = DateTime.Now,
                    Done = false
                };

                await _repoNotifUrgente.InsertAsync(novaNotificacao);

                MessageBox.Show(
            $"Notificação de URGÊNCIA gerada com sucesso!\n\nO sistema detectou a chegada de originais e notificou o setor responsável.",
            "Alerta Automático",
            MessageBoxButtons.OK,
            MessageBoxIcon.Exclamation
        );
            }
        }

        private List<UpdateDefinition<Processo>> GerarAtualizacoesParciaisParaBanco()
        {
            var updates = new List<UpdateDefinition<Processo>>();
            var builder = Builders<Processo>.Update;

            // 1. Verifica propriedades simples via Reflection
            // IMPORTANTE: Adicionei "Catalogos" na lista de ignorar para o Reflection não bugar
            var ignorar = new HashSet<string> { "Id", "_id", "LI", "Capa", "DocRecebidos", "OrgaosAnuentesString", "Catalogos" };
            PropertyInfo[] propriedades = _processoOriginal.GetType().GetProperties();

            foreach (var prop in propriedades)
            {
                if (ignorar.Contains(prop.Name) || !prop.CanWrite) continue;

                object? valorAntigo = prop.GetValue(_processoOriginal);
                object? valorNovo = prop.GetValue(processo);

                // Se o valor mudou, adiciona na lista de campos a serem atualizados
                if (!Equals(valorAntigo, valorNovo))
                {
                    updates.Add(builder.Set(prop.Name, valorNovo));
                }
            }

            // 2. Compara a Capa (Se mudou qualquer coisa nela, enviamos o objeto Capa atualizado)
            bool capaMudou = CompararPropriedades(_processoOriginal.Capa, processo.Capa, new HashSet<string>()).Any();
            if (capaMudou) updates.Add(builder.Set(p => p.Capa, processo.Capa));

            // 3. Compara os Documentos Recebidos (Array)
            string strAntigo = _processoOriginal.DocRecebidos == null ? "" : string.Join(",", _processoOriginal.DocRecebidos);
            string strNovo = processo.DocRecebidos == null ? "" : string.Join(",", processo.DocRecebidos);
            if (strAntigo != strNovo) updates.Add(builder.Set(p => p.DocRecebidos, processo.DocRecebidos));

            // 4. Compara LIs
            var logsLi = new List<string>();
            CompararListaLIs(logsLi);
            if (logsLi.Any()) updates.Add(builder.Set(p => p.LI, processo.LI));

            // 5. COMPARAÇÃO DA NOVA LISTA DE CATÁLOGOS
            var listaCatAntiga = _processoOriginal.Catalogos ?? new List<Catalogo>();
            var listaCatNova = processo.Catalogos ?? new List<Catalogo>();

            bool catalogosMudou = false;

            // Se a quantidade de catálogos for diferente, já sabemos que mudou (adicionou/excluiu)
            if (listaCatAntiga.Count != listaCatNova.Count)
            {
                catalogosMudou = true;
            }
            else if (listaCatNova.Count > 0)
            {
                // Se a quantidade é a mesma, alguém pode ter editado um catálogo (alterado NCM, adicionado Órgão, etc).
                // Usamos JSON para comparar o conteúdo de tudo de forma rápida.
                string jsonAntigo = JsonConvert.SerializeObject(listaCatAntiga);
                string jsonNovo = JsonConvert.SerializeObject(listaCatNova);

                if (jsonAntigo != jsonNovo)
                {
                    catalogosMudou = true;
                }
            }

            if (catalogosMudou)
            {
                updates.Add(builder.Set(p => p.Catalogos, processo.Catalogos));
            }

            return updates;
        }
        private async Task SincronizarOrgaoAnuenteAsync()
        {
            await _repositorio.SincronizarLicencas(processo);
        }

        #endregion

        #region "Gerenciamento Dinâmico de LI/LPCO"

        private void CarregarAbasLi()
        {
            TCLi.TabPages.Clear();
            foreach (var li in processo.LI)
            {
                AdicionarAbaLi(li);
            }
        }

        private void AdicionarAbaLi(LicencaImportacao li)
        {
            // Cria a aba principal da LI.
            var tabPageLi = new TabPage($"LI - {li.Numero}")
            {
                Tag = li,
                BackColor = SystemColors.Control
            };

            var editorLi = new LIEditControl
            {
                Dock = DockStyle.Fill
            };

            editorLi.VincularDados(li);

            // MUDANÇA: Atualiza o texto da aba de forma segura.
            var txtLiControl = editorLi.Controls.Find("TxtLi", true).FirstOrDefault() as TextBox;
            if (txtLiControl != null)
            {
                txtLiControl.TextChanged += (s, e) =>
                {
                    tabPageLi.Text = $"LI - {txtLiControl.Text}";
                };
            }

            // 3. Adiciona o UserControl à aba.
            tabPageLi.Controls.Add(editorLi);

            // 4. Adiciona a aba de LI ao TabControl principal.
            TCLi.TabPages.Add(tabPageLi);
        }

        private void BtnLI_Click(object? sender, EventArgs e)
        {
            var novaLi = new LicencaImportacao { Numero = "Nova LI" };
            processo.LI.Add(novaLi);
            AdicionarAbaLi(novaLi);
            TCLi.SelectedIndex = TCLi.TabPages.Count - 1;
            _dadosForamAlterados = true;
            this.Text += "*";
        }

        private async void BtnExcluirLi_Click(object sender, EventArgs e)
        {
            // 1. Verifica se há abas e se alguma está selecionada
            if (TCLi.TabCount == 0 || TCLi.SelectedIndex < 0)
            {
                MessageBox.Show("Nenhuma LI selecionada para excluir.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 2. Obtém o índice da aba selecionada
            int indice = TCLi.SelectedIndex;

            // Recupera o objeto LI para exibir o número na mensagem de confirmação (opcional)
            var aba = TCLi.TabPages[indice];
            var liParaExcluir = aba.Tag as LicencaImportacao;
            string numeroLi = liParaExcluir?.Numero ?? "Desconhecida";

            // 3. Pede confirmação
            var resultado = MessageBox.Show(
                $"Tem certeza que deseja excluir a LI '{numeroLi}' e todos os seus LPCOs associados?",
                "Confirmar Exclusão de LI",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (resultado == DialogResult.No) return;

            // 4. REMOÇÃO SEGURA POR ÍNDICE
            // Remove do objeto de dados (processo.LI) se o índice for válido
            if (indice < processo.LI.Count)
            {
                processo.LI.RemoveAt(indice);
            }

            // Remove a aba visualmente
            TCLi.TabPages.RemoveAt(indice);

            // 5. Atualiza estado
            AtualizarEstadoBotoesLI();

            await _logRepo.RegistrarLogAsync(
                "Exclusão", UsuarioLogado?.Usuario,
                $"LI {numeroLi} removida do processo {processo.Ref_USA}",
                $"Usuário removeu a aba da LI"
            );

            _dadosForamAlterados = true;
            this.Text += "*";

            MessageBox.Show("LI removida com sucesso.", "LI Removida", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion

        #region "Métodos Auxiliares"

        private void CarregarControlesDeData()
        {
            // Mapeia o controle à sua propriedade correspondente no objeto 'processo'
            ConfigurarDatePickerNulavel(dtpDataMinuta, processo.DataMinutaDI);
            ConfigurarDatePickerNulavel(DTPdataderegistrodi, processo.DataRegistroDI);
            ConfigurarDatePickerNulavel(DTPdatadedesembaracodi, processo.DataDesembaracoDI);
            ConfigurarDatePickerNulavel(DTPdatadecarregamentodi, processo.DataCarregamentoDI);
            ConfigurarDatePickerNulavel(DTPdatadeinspecao, processo.Inspecao);
            ConfigurarDatePickerNulavel(DTPdatadeatracacao, processo.DataDeAtracacao);
            ConfigurarDatePickerNulavel(DTPdatadeembarque, processo.DataEmbarque);
            ConfigurarDatePickerNulavel(DTPDataRecOriginais, processo.DataRecebOriginais);
            ConfigurarDatePickerNulavel(dtpVencimentoFMA, processo.VencimentoFMA);
            ConfigurarDatePickerNulavel(dtpVencimentoFreeTime, processo.VencimentoFreeTime);
            ConfigurarDatePickerNulavel(dtpVencimentoLI_LPCO, processo.VencimentoLI_LPCO);
        }

        private void ConfigurarDatePickerNulavel(DateTimePicker dtp, DateTime? data)
        {
            dtp.ShowCheckBox = true;

            if (data.HasValue)
            {
                dtp.Checked = true;
                dtp.Value = data.Value;

                // LÓGICA NOVA PARA EXIBIR HORA AO CARREGAR
                if (dtp.Name == "DTPdatadeatracacao")
                {
                    dtp.Format = DateTimePickerFormat.Custom;
                    dtp.CustomFormat = "dd/MM/yyyy HH:mm";
                }
                else
                {
                    dtp.Format = DateTimePickerFormat.Short;
                }
            }
            else
            {
                dtp.Checked = false;
                dtp.Value = DateTime.Today; // Data base para quando o usuário clicar
                dtp.Format = DateTimePickerFormat.Custom;
                dtp.CustomFormat = " ";
            }

            // Reinscrição do evento (mantém igual ao seu código original)
            dtp.ValueChanged -= Dtp_ValueChanged_Format;
            dtp.ValueChanged += Dtp_ValueChanged_Format;
        }

        // Este evento agora só cuida da FORMATAÇÃO VISUAL.
        private void Dtp_ValueChanged_Format(object? sender, EventArgs e)
        {
            if (sender is DateTimePicker picker)
            {
                if (picker.Checked)
                {
                    // Verifique se o controle atual é o que deve mostrar as horas
                    // (Substitua "DTPDatadeChegada" pelo nome exato do seu componente no Designer)
                    if (picker.Name == "DTPdatadeatracacao")
                    {
                        picker.Format = DateTimePickerFormat.Custom;
                        picker.CustomFormat = "dd/MM/yyyy HH:mm"; // Mostra Dia, Mês, Ano, Hora e Minuto
                    }
                    else
                    {
                        picker.Format = DateTimePickerFormat.Short; // Padrão (apenas data) para os outros
                    }
                }
                else
                {
                    // Se estiver desmarcado (nulo visualmente)
                    picker.Format = DateTimePickerFormat.Custom;
                    picker.CustomFormat = " ";
                }
            }
        }
        private void CarregarCheckedListBoxes()
        {
            // 1. Lida com o CheckedListBox de multi-seleção ("Docs Recebidos")
            if (processo.DocRecebidos != null)
            {
                // Desmarca todos os itens primeiro para garantir uma carga limpa
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    checkedListBox1.SetItemChecked(i, false);
                }

                // Marca os itens que estão na lista do processo
                foreach (var item in processo.DocRecebidos)
                {
                    int index = checkedListBox1.Items.IndexOf(item);
                    if (index != -1)
                    {
                        checkedListBox1.SetItemChecked(index, true);
                    }
                }
            }

            // 2. Lida com o CheckedListBox de seleção única ("Forma Rec.")
            if (!string.IsNullOrEmpty(processo.FormaRecOriginais))
            {
                int index = checkedListBox2.Items.IndexOf(processo.FormaRecOriginais);
                if (index != -1)
                {
                    // Marca apenas o item correspondente
                    checkedListBox2.SetItemChecked(index, true);
                }
            }
        }
        private string[] ObterItensSelecionados(CheckedListBox clb) => clb.CheckedItems.OfType<string>().ToArray();
        private void PopularMarca()
        {
            string marcaCompleta = processo.Marca ?? string.Empty;
            string[] modulosEspacos = new[] { "Sacos", "Caixas", "Pallets" };

            string numMarcaStr = "";
            string textoMarca = "";

            if (modulosEspacos.Any(m => marcaCompleta.EndsWith(" " + m)))
            {
                // Formato "10 Sacos"
                int indexEspaco = marcaCompleta.LastIndexOf(' ');
                if (indexEspaco > 0)
                {
                    numMarcaStr = marcaCompleta.Substring(0, indexEspaco);
                    textoMarca = marcaCompleta.Substring(indexEspaco + 1);
                }
            }
            else if (marcaCompleta.Contains(" x "))
            {
                // Formato "2 x 40 HC"
                int indexX = marcaCompleta.IndexOf(" x ");
                if (indexX > 0)
                {
                    numMarcaStr = marcaCompleta.Substring(0, indexX);
                    textoMarca = marcaCompleta.Substring(indexX + 3);
                }
            }
            else
            {
                // Se não encontrar um padrão, assume que é tudo texto
                textoMarca = marcaCompleta;
            }

            // Atribui os valores aos controles
            if (decimal.TryParse(numMarcaStr, out decimal num))
            {
                numMarca.Value = Math.Clamp(num, numMarca.Minimum, numMarca.Maximum);
            }
            cbMarca.Text = textoMarca;
        }
        private void SetCamposSomenteLeitura(Control parent)
        {
            BtnLI.Enabled = false;

            foreach (Control control in parent.Controls)
            {
                switch (control)
                {
                    case TextBox box: box.ReadOnly = true; break;
                    case MaskedTextBox box: box.ReadOnly = true; break;
                    case ComboBox box: box.Enabled = false; break;
                    case CheckBox box: box.Enabled = false; break;
                    case DateTimePicker picker: picker.Enabled = false; break;
                    case NumericUpDown num: num.Enabled = false; break;
                    case CheckedListBox list: list.Enabled = false; break;
                    case Button btn when btn.Name != "btnCancelar" && btn.Name != "btnRelatorio" && btn.Name != "btnCapa":
                        btn.Enabled = false; // Desabilita outros botões, exceto os de navegação/relatório
                        break;
                }

                // Chamada recursiva para controles dentro de GroupBoxes, Panels, TabPages, etc.
                if (control.HasChildren)
                {
                    SetCamposSomenteLeitura(control);
                }
            }
        }
        private void btnCapa_Click(object? sender, EventArgs e)
        {
            using var frm = new FrmModificaCapa
            {
                capa = processo.Capa ?? new Capa(),
                Modo = this.Modo,
                ref_usa = processo.Ref_USA ?? string.Empty,
                Visualizacao = this.Visualização
            };

            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                processo.Capa = frm.capa ?? new Capa();
            }
            _dadosForamAlterados = true;
            this.Text += "*";
        }
        private async void btnRelatorio_Click(object? sender, EventArgs e)
        {
            // 1. Pega a referência
            string referencia = TXTnr.Text;

            if (string.IsNullOrWhiteSpace(referencia))
            {
                MessageBox.Show("Por favor, digite a referência.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Mostra o Loading
            var progressForm = new ProgressForm();
            progressForm.Show(this);

            try
            {
                // 3. Instancia e chama o serviço (Rodamos em Task.Run para garantir que a UI não trave nada)
                var service = new RelatorioService();

                // AQUI ESTÁ A MUDANÇA: Chamamos o C# em vez do PythonRunner
                string pdfPath = await Task.Run(() => service.GerarRelatorioAsync(referencia));

                // 4. Fecha o Loading (Já estamos na Thread UI graças ao await, não precisa de Invoke)
                progressForm.Close();

                // 5. Pergunta se quer abrir
                var resp = MessageBox.Show("Relatório gerado com sucesso!\nDeseja abrir o PDF?",
                    "Concluído", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (resp == DialogResult.Yes && !string.IsNullOrEmpty(pdfPath) && File.Exists(pdfPath))
                {
                    Process.Start(new ProcessStartInfo { FileName = pdfPath, UseShellExecute = true });
                }
            }
            catch (Exception ex)
            {
                progressForm.Close();
                MessageBox.Show($"Erro ao gerar relatório: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void checkedListBox2_ItemCheck(object? sender, ItemCheckEventArgs e)
        {
            // Se o usuário está marcando um novo item...
            if (e.NewValue == CheckState.Checked)
            {
                // ...percorre todos os outros itens e os desmarca.
                for (int i = 0; i < checkedListBox2.Items.Count; i++)
                {
                    if (i != e.Index)
                    {
                        checkedListBox2.SetItemChecked(i, false);
                    }
                }
            }
        }

        #endregion

        #region "Exclusão automática notificações

        private async Task ExcluirNotificacoesAutomaticamente(Processo p)
        {
            if (string.IsNullOrWhiteSpace(p.Ref_USA)) return;
            if (p.Redestinacao == true)
            {
                await ExcluirNotificacaoPorMensagemExataAsync(
                    p.Ref_USA,
                    $"Processo {p.Ref_USA}: Redestinar container ao terminal"
                );
            }
            if (p.DataRegistroDI.HasValue)
            {
                await ExcluirNotificacoesPorTipoAsync(p.Ref_USA, "Vencimento FMA");
                await ExcluirNotificacoesPorTipoAsync(p.Ref_USA, "Vencimento LI/LPCO");
            }
        }
        private async Task ExcluirNotificacaoPorMensagemExataAsync(string refUsa, string mensagemExata)
        {
            await _notificacaoRepo.ExcluirPorMensagemExataAsync(refUsa, mensagemExata);
        }
        private async Task ExcluirNotificacoesPorTipoAsync(string refUsa, string tipoNotificacao)
        {
            await _notificacaoRepo.ExcluirPorTipoNaMensagemAsync(refUsa, tipoNotificacao);
        }

        #endregion

        private void MostrarLoading(string mensagem)
        {
            if (_overlay != null) return;
            _overlay = new FrmLoadingOverlay { Opacity = 0.60 };
            _overlay.lblLoading.Text = mensagem;
            var rect = this.RectangleToScreen(this.ClientRectangle);
            _overlay.StartPosition = FormStartPosition.Manual;
            _overlay.Location = rect.Location;
            _overlay.Size = rect.Size;
            _overlay.Show(this);
            _overlay.BringToFront();
        }

        private void EsconderLoading()
        {
            _overlay?.Close();
            _overlay?.Dispose();
            _overlay = null;
        }

        private void BtnAdicionarCatalogo_Click(object sender, EventArgs e)
        {
            try
            {
                using var frm = new FrmModificaCatalogo();
                frm.StartPosition = FormStartPosition.CenterParent;
                frm.Modo = FrmModificaCatalogo.ModoFormulario.Adicionar;

                var result = frm.ShowDialog(this);
                if (result == DialogResult.OK && frm.CatalogoInicial != null)
                {
                    if (processo.Catalogos == null) processo.Catalogos = new List<Catalogo>();

                    processo.Catalogos.Add(frm.CatalogoInicial);
                    AtualizarGridCatalogos();

                    // --- LÓGICA DE AUTOMATIZAÇÃO DO HISTÓRICO (ADICIONAR) ---
                    string dataAtual = DateTime.Now.ToString("dd/MM");

                    // 1. Adiciona a linha do NCM e cClassTrib
                    string logNcm = $"{dataAtual} - NCM: {frm.CatalogoInicial.NCM} - cClassTrib: {frm.CatalogoInicial.cClassTrib}.";
                    TXTstatusdoprocesso.Text = $"{logNcm}\r\n{TXTstatusdoprocesso.Text}".Trim();

                    // 2. Se o usuário já adicionou ANVISA com comunicado logo na criação, também avisa
                    var anvisa = frm.CatalogoInicial.Orgaos?.FirstOrDefault(o => o.OrgaoId != null && o.OrgaoId.Contains("ANVISA", StringComparison.OrdinalIgnoreCase));
                    if (anvisa != null && !string.IsNullOrWhiteSpace(anvisa.Comunicado))
                    {
                        string logAnvisa = $"{dataAtual} - Comunicado ANVISA: {anvisa.Comunicado}.";
                        TXTstatusdoprocesso.Text = $"{logAnvisa}\r\n{TXTstatusdoprocesso.Text}".Trim();
                    }
                    // --------------------------------------------------------

                    _dadosForamAlterados = true;
                    this.Text += this.Text.EndsWith("*") ? "" : "*";

                    MessageBox.Show("Catálogo adicionado ao processo.", "Concluído", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao abrir o formulário de catálogo: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEditarCatalogo_Click(object sender, EventArgs e)
        {
            try
            {
                var catalogoSelecionado = DGVCatalogo.CurrentRow?.DataBoundItem as Catalogo;
                if (catalogoSelecionado == null)
                {
                    MessageBox.Show("Selecione um catálogo na lista para editar.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using var frm = new FrmModificaCatalogo();
                frm.StartPosition = FormStartPosition.CenterParent;
                frm.Modo = FrmModificaCatalogo.ModoFormulario.Editar;
                frm.CatalogoInicial = catalogoSelecionado;

                var result = frm.ShowDialog(this);

                if (result == DialogResult.OK && frm.CatalogoInicial != null)
                {
                    // --- LÓGICA DE AUTOMATIZAÇÃO DO HISTÓRICO (EDITAR ANVISA) ---
                    var anvisaAntigo = catalogoSelecionado.Orgaos?.FirstOrDefault(o => o.OrgaoId != null && o.OrgaoId.Contains("ANVISA", StringComparison.OrdinalIgnoreCase));
                    var anvisaNovo = frm.CatalogoInicial.Orgaos?.FirstOrDefault(o => o.OrgaoId != null && o.OrgaoId.Contains("ANVISA", StringComparison.OrdinalIgnoreCase));

                    // Se o ANVISA existe no novo objeto
                    if (anvisaNovo != null)
                    {
                        string comunicadoAntigo = anvisaAntigo?.Comunicado ?? "";
                        string comunicadoNovo = anvisaNovo.Comunicado ?? "";

                        // Verifica se a palavra mudou e se não está vazio
                        if (comunicadoAntigo != comunicadoNovo && !string.IsNullOrWhiteSpace(comunicadoNovo))
                        {
                            string dataAtual = DateTime.Now.ToString("dd/MM");
                            string logAnvisa = $"{dataAtual} - Comunicado ANVISA: {comunicadoNovo}.";
                            TXTstatusdoprocesso.Text = $"{logAnvisa}\r\n{TXTstatusdoprocesso.Text}".Trim();
                        }
                    }
                    // -----------------------------------------------------------

                    int index = processo.Catalogos.IndexOf(catalogoSelecionado);
                    if (index >= 0)
                    {
                        processo.Catalogos[index] = frm.CatalogoInicial;
                    }

                    AtualizarGridCatalogos();

                    _dadosForamAlterados = true;
                    this.Text += this.Text.EndsWith("*") ? "" : "*";
                    MessageBox.Show("Catálogo atualizado.", "Concluído", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao editar o catálogo: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnExcluirCatalogo_Click(object sender, EventArgs e)
        {
            var catalogoSelecionado = DGVCatalogo.CurrentRow?.DataBoundItem as Catalogo;
            if (catalogoSelecionado == null)
            {
                MessageBox.Show("Selecione um catálogo para excluir.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var resp = MessageBox.Show("Tem certeza que deseja excluir o catálogo selecionado?", "Confirmar Exclusão", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (resp == DialogResult.Yes)
            {
                processo.Catalogos.Remove(catalogoSelecionado);

                AtualizarGridCatalogos();
                DGVOrgaoCatalogo.DataSource = null;

                _dadosForamAlterados = true;
                this.Text += this.Text.EndsWith("*") ? "" : "*";
            }
        }

        // Este evento deve ser atrelado ao evento 'SelectionChanged' do seu DGVCatalogo
        private void DGVCatalogo_SelectionChanged(object sender, EventArgs e)
        {
            // === 1. GUARDA DE SEGURANÇA CONTRA ERRO DE CURRENCYMANAGER ===
            if (DGVCatalogo.DataSource == null || DGVCatalogo.Rows.Count == 0)
            {
                DGVOrgaoCatalogo.DataSource = null;
                return; // Sai do método antes de tentar ler o CurrentRow e causar o erro
            }

            // === 2. LÓGICA NORMAL PROTEGIDA ===
            if (DGVCatalogo.CurrentRow != null && DGVCatalogo.CurrentRow.Index >= 0)
            {
                var catalogoSelecionado = DGVCatalogo.CurrentRow.DataBoundItem as Catalogo;

                if (catalogoSelecionado != null)
                {
                    var listaOrgaos = catalogoSelecionado.Orgaos;
                    DGVOrgaoCatalogo.DataSource = null;

                    if (listaOrgaos != null && listaOrgaos.Count > 0)
                    {
                        DGVOrgaoCatalogo.DataSource = listaOrgaos;

                        // --- FORMATAÇÃO VISUAL DO DGVOrgaoCatalogo ---
                        if (DGVOrgaoCatalogo.Columns.Contains("Inspecao"))
                        {
                            DGVOrgaoCatalogo.Columns["Inspecao"].DefaultCellStyle.Format = "d";
                            DGVOrgaoCatalogo.Columns["Inspecao"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        }

                        if (DGVOrgaoCatalogo.Columns.Contains("Coleta"))
                        {
                            DGVOrgaoCatalogo.Columns["Coleta"].DefaultCellStyle.Format = "d";
                            DGVOrgaoCatalogo.Columns["Coleta"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        }

                        if (DGVOrgaoCatalogo.Columns.Contains("Parametrizacao"))
                        {
                            DGVOrgaoCatalogo.Columns["Parametrizacao"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        }
                    }
                }
                else
                {
                    DGVOrgaoCatalogo.DataSource = null;
                }
            }
            else
            {
                DGVOrgaoCatalogo.DataSource = null;
            }
        }
        private void AtualizarGridCatalogos()
        {
            // 1. Levanta a "bandeira" avisando que a grid está em manutenção
            _atualizandoGridCatalogo = true;

            try
            {
                DGVCatalogo.DataSource = null;

                if (processo.Catalogos != null && processo.Catalogos.Count > 0)
                {
                    // O .ToList() é o segredo aqui: Ele força a grid a ler uma nova memória, 
                    // fugindo do erro de Index do CurrencyManager.
                    DGVCatalogo.DataSource = processo.Catalogos.ToList();

                    // --- FORMATAÇÃO VISUAL DO DGVCatalogo ---
                    if (DGVCatalogo.Columns.Contains("Id"))
                        DGVCatalogo.Columns["Id"].Visible = false;

                    if (DGVCatalogo.Columns.Contains("NCM"))
                        DGVCatalogo.Columns["NCM"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                    if (DGVCatalogo.Columns.Contains("cClassTrib"))
                        DGVCatalogo.Columns["cClassTrib"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
                else
                {
                    DGVOrgaoCatalogo.DataSource = null;
                }
            }
            finally
            {
                // 2. Abaixa a bandeira aconteça o que acontecer
                _atualizandoGridCatalogo = false;
            }

            // 3. Força a leitura da seleção manualmente agora que a grid está estável
            DGVCatalogo_SelectionChanged(DGVCatalogo, EventArgs.Empty);
        }

        private void RbRegistroRegistrado_CheckedChanged(object sender, EventArgs e)
        {
            // O .Focused garante que isso só dispare pelo clique do usuário, e não no Load da tela
            if (RbRegistroRegistrado.Checked && RbRegistroRegistrado.Focused)
            {
                string codigo = MostrarInputBox("Digite o código interno gerado no registro:", "Código do Registro");

                // Se o usuário digitou algo e clicou em OK
                if (!string.IsNullOrWhiteSpace(codigo))
                {
                    string dataAtual = DateTime.Now.ToString("dd/MM");
                    string novaLinha = $"{dataAtual} - Registro Catálogo de Produtos: Registrado ({codigo}).";

                    // Adiciona no topo do TextBox do histórico
                    TXTstatusdoprocesso.Text = $"{novaLinha}\r\n{TXTstatusdoprocesso.Text}".Trim();
                }
                else
                {
                    // Se ele cancelar a caixinha ou deixar vazio, voltamos a marcação pro Pendente
                    RbRegistroPendente.Checked = true;
                }
            }
        }

        private void DGVOrgaoCatalogo_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Se o valor for uma data...
            if (e.Value is DateTime data)
            {
                // Se for nulo ou ano 0001, transforma em vazio ("") visualmente
                if (data == DateTime.MinValue || data.Year < 1753)
                {
                    e.Value = "";
                    e.FormattingApplied = true;
                }
            }
        }

        private void RbRegistroPendente_CheckedChanged(object sender, EventArgs e)
        {
            // O .Focused garante que isso só dispare pelo clique do usuário
            if (RbRegistroPendente.Checked && RbRegistroPendente.Focused)
            {
                string dataAtual = DateTime.Now.ToString("dd/MM");
                string novaLinha = $"{dataAtual} - Registro Catálogo de Produtos: Pendente.";

                // Adiciona no topo do TextBox do histórico
                TXTstatusdoprocesso.Text = $"{novaLinha}\r\n{TXTstatusdoprocesso.Text}".Trim();
            }
        }

        private string MostrarInputBox(string mensagem, string titulo)
        {
            using Form prompt = new Form()
            {
                Width = 400,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = titulo,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false
            };

            Label textLabel = new Label() { Left = 20, Top = 20, Text = mensagem, Width = 340 };
            TextBox textBox = new TextBox() { Left = 20, Top = 45, Width = 340 };
            Button confirmation = new Button() { Text = "OK", Left = 260, Top = 75, Width = 100, DialogResult = DialogResult.OK };

            // Ao apertar ENTER no TextBox, ele clica no OK
            prompt.AcceptButton = confirmation;

            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);

            return prompt.ShowDialog(this) == DialogResult.OK ? textBox.Text : string.Empty;
        }

        private void DGVCatalogo_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // === 1. GUARDA DE SEGURANÇA CONTRA ERRO DE CURRENCYMANAGER ===
            if (DGVCatalogo.DataSource == null || DGVCatalogo.Rows.Count == 0)
            {
                DGVOrgaoCatalogo.DataSource = null;
                return; // Sai do método antes de tentar ler o CurrentRow e causar o erro
            }

            // === 2. LÓGICA NORMAL PROTEGIDA ===
            if (DGVCatalogo.CurrentRow != null && DGVCatalogo.CurrentRow.Index >= 0)
            {
                var catalogoSelecionado = DGVCatalogo.CurrentRow.DataBoundItem as Catalogo;

                if (catalogoSelecionado != null)
                {
                    var listaOrgaos = catalogoSelecionado.Orgaos;
                    DGVOrgaoCatalogo.DataSource = null;

                    if (listaOrgaos != null && listaOrgaos.Count > 0)
                    {
                        DGVOrgaoCatalogo.DataSource = listaOrgaos;

                        // --- FORMATAÇÃO VISUAL DO DGVOrgaoCatalogo ---
                        if (DGVOrgaoCatalogo.Columns.Contains("Inspecao"))
                        {
                            DGVOrgaoCatalogo.Columns["Inspecao"].DefaultCellStyle.Format = "d";
                            DGVOrgaoCatalogo.Columns["Inspecao"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        }

                        if (DGVOrgaoCatalogo.Columns.Contains("Coleta"))
                        {
                            DGVOrgaoCatalogo.Columns["Coleta"].DefaultCellStyle.Format = "d";
                            DGVOrgaoCatalogo.Columns["Coleta"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        }

                        if (DGVOrgaoCatalogo.Columns.Contains("Parametrizacao"))
                        {
                            DGVOrgaoCatalogo.Columns["Parametrizacao"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        }
                    }
                }
                else
                {
                    DGVOrgaoCatalogo.DataSource = null;
                }
            }
            else
            {
                DGVOrgaoCatalogo.DataSource = null;
            }
        }
    }
}