using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CLUSA
{
    #region "Interface e Base"

    /// <summary>
    /// Interface base para todas as entidades do MongoDB
    /// </summary>
    public interface IEntidadeBase
    {
        ObjectId Id { get; set; }
    }

    /// <summary>
    /// Repositório base genérico para operações CRUD no MongoDB
    /// </summary>
    public abstract class RepositorioBase<T> where T : IEntidadeBase
    {
        protected readonly IMongoCollection<T> _colecao;

        protected RepositorioBase(string nomeColecao, IMongoDatabase? database = null)
        {
            var db = database ?? ConfigDatabase.GetDatabase();
            _colecao = db.GetCollection<T>(nomeColecao);
        }

        // CREATE
        public virtual async Task InsertAsync(T entidade)
        {
            if (entidade.Id == ObjectId.Empty)
            {
                entidade.Id = ObjectId.GenerateNewId();
            }
            await _colecao.InsertOneAsync(entidade);
        }

        public virtual async Task InsertManyAsync(List<T> entidades)
        {
            foreach (var entidade in entidades)
            {
                if (entidade.Id == ObjectId.Empty)
                {
                    entidade.Id = ObjectId.GenerateNewId();
                }
            }
            await _colecao.InsertManyAsync(entidades);
        }

        // READ
        public virtual async Task<List<T>> ListarTodosAsync()
        {
            return await _colecao.Find(FilterDefinition<T>.Empty).ToListAsync();
        }

        public virtual async Task<T?> ObterPorIdAsync(ObjectId id)
        {
            var filter = Builders<T>.Filter.Eq(x => x.Id, id);
            return await _colecao.Find(filter).FirstOrDefaultAsync();
        }

        // UPDATE
        public virtual async Task UpdateAsync(T entidade)
        {
            var filter = Builders<T>.Filter.Eq(x => x.Id, entidade.Id);
            await _colecao.ReplaceOneAsync(filter, entidade);
        }

        // DELETE
        public virtual async Task DeleteAsync(ObjectId id)
        {
            var filter = Builders<T>.Filter.Eq(x => x.Id, id);
            await _colecao.DeleteOneAsync(filter);
        }

        public virtual async Task DeleteManyAsync(FilterDefinition<T> filter)
        {
            await _colecao.DeleteManyAsync(filter);
        }

        // MÉTODOS AUXILIARES ESPECÍFICOS PARA PROCESSOS
        public virtual async Task DeletePorRefUsaAsync(string refUsa)
        {
            var filter = Builders<T>.Filter.Eq("Ref_USA", refUsa);
            await _colecao.DeleteManyAsync(filter);
        }

        // CONTAGEM
        public virtual async Task<long> ContarAsync(FilterDefinition<T>? filter = null)
        {
            filter ??= FilterDefinition<T>.Empty;
            return await _colecao.CountDocumentsAsync(filter);
        }

        // BUSCA CUSTOMIZADA
        public virtual async Task<List<T>> BuscarAsync(FilterDefinition<T> filter)
        {
            return await _colecao.Find(filter).ToListAsync();
        }

        public virtual async Task<T?> BuscarUmAsync(FilterDefinition<T> filter)
        {
            return await _colecao.Find(filter).FirstOrDefaultAsync();
        }
    }

    #endregion

    #region "Fatura Model"
    public class Fatura : IEntidadeBase
    {
        public Fatura() { }
        public Fatura(Processo processo)
        {
            Ref_USA = processo.Ref_USA;
            SR = processo.SR;
            Importador = processo.Importador;
            Veiculo = processo.Veiculo;
            FLO = processo.FLO;
            Mercadoria = processo.Produto;
            Marca = processo.Marca;
            DataAtracacao = processo.DataDeAtracacao;
            DI = processo.DI;
            DataDesembaracoDI = processo.DataDesembaracoDI;
            DAtaDI = processo.DataRegistroDI;
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        public string Ref_USA { get; set; } = string.Empty;
        public string SR { get; set; } = string.Empty;
        public string Importador { get; set; } = string.Empty;
        public string Endereco_Importador { get; set; } = string.Empty;
        public string FLO { get; set; } = string.Empty;
        public DateTime? DataAtracacao { get; set; } = (DateTime?)null;
        public string Veiculo { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public float Quantidade { get; set; } = 0;
        public string Mercadoria { get; set; } = string.Empty;
        public decimal ValRecebidos { get; set; } = 0;
        public DateTime? DataRecebimento { get; set; } = (DateTime?)null;
        public string DI { get; set; } = string.Empty;
        public DateTime? DataDesembaracoDI { get; set; } = (DateTime?)null;
        public DateTime? DAtaDI { get; set; } = (DateTime?)null;
        public decimal ImpostoImportacao { get; set; } = 0;
        public decimal IPI { get; set; } = 0;
        public decimal DI_ADICAO { get; set; } = 0;
        public decimal PIS_PASEP { get; set; } = 0;
        public decimal COFINS { get; set; } = 0;
        public decimal MULTA_LI { get; set; } = 0;
        public decimal ICMS { get; set; } = 0;
        public List<Agencia> Agencias { get; set; } = new();
        public string ArmazenagemN { get; set; } = string.Empty;
        public decimal ArmazenagemP { get; set; } = 0;
        public string FreteMaritimoN { get; set; } = string.Empty;
        public decimal FreteMaritimoP { get; set; } = 0;
        public string Marinha_MercanteN { get; set; } = string.Empty;
        public decimal Marinha_MercanteP { get; set; } = 0;
        public string GRUANVISAN { get; set; } = string.Empty;
        public decimal GRUANVISAP { get; set; } = 0;
        public string LiCancelada_IndeferidaN { get; set; } = string.Empty;
        public decimal LiCancelada_IndeferidaP { get; set; } = 0;
        public string ExpedienteLiCanceladaN { get; set; } = string.Empty;
        public decimal ExpedienteLiCanceladaP { get; set; } = 0;
        public string EncaminhamentoAmostrasN { get; set; } = string.Empty;
        public decimal EncaminhamentoAmostrasP { get; set; } = 0;
        public string DarfAnvisaN { get; set; } = string.Empty;
        public decimal DarfAnvisaP { get; set; } = 0;
        public string MotoboyN { get; set; } = string.Empty;
        public decimal MotoboyP { get; set; } = 0;
        public decimal LiP { get; set; } = 0;
        public decimal Expediente { get; set; } = 0;
        public string DespesasDesembaracoN { get; set; } = string.Empty;
        public decimal DespesasDesembaracoP { get; set; } = 0;
        public decimal HD { get; set; } = 0;
        public decimal Cartorio { get; set; } = 0;
        public string[] NomesDocumentosAnexos { get; set; } = Array.Empty<string>();
        public string[] NumeroDocumentosAnexos { get; set; } = Array.Empty<string>();
        public decimal TotalDespesas { get; set; } = 0;
        public decimal NComissao { get; set; } = 0;
        public decimal SubTotal { get; set; } = 0;
        public decimal Adiantamento { get; set; } = 0;
        public decimal Saldo { get; set; } = 0;
        public string TipoFinalizacao { get; set; } = string.Empty;
    }
    #endregion
   #region "Repositorio Fatura"
    public class RepositorioFatura : RepositorioBase<Fatura>
    {
        public RepositorioFatura(IMongoDatabase? database = null)
            : base("Fatura", database) { }

        public async Task<List<Fatura>> FindRefAsync()
        {
            var filter = Builders<Fatura>.Filter.And(
                Builders<Fatura>.Filter.Ne(f => f.Ref_USA, null),
                Builders<Fatura>.Filter.Ne(f => f.Importador, null)
            );
            return await _colecao.Find(filter).ToListAsync();
        }
        public async Task<Fatura?> ObterPorRefUSAAsync(string refUsa)
        {
            if (string.IsNullOrWhiteSpace(refUsa))
                return null;

            var filter = Builders<Fatura>.Filter.Eq(f => f.Ref_USA, refUsa);
            return await _colecao.Find(filter).FirstOrDefaultAsync();
        }
    }

    #endregion

   #region "Recibo Model"
    public class Recibo : IEntidadeBase
    {
        public Recibo() { }
        public Recibo(Processo processo)
        {
            Ref_USA = processo.Ref_USA;
            SR = processo.SR;
            Importador = processo.Importador;
            Exportador = processo.Exportador;
            Veiculo = processo.Veiculo;
            Mercadoria = processo.Produto;
        }
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        public string Ref_USA { get; set; } = string.Empty;
        public string SR { get; set; } = string.Empty;
        public string Importador { get; set; } = string.Empty;
        public string Exportador { get; set; } = string.Empty;
        public string Endereco_Importador { get; set; } = string.Empty;
        public string Veiculo { get; set; } = string.Empty;
        public string Mercadoria { get; set; } = string.Empty;
        public decimal EmissaoLicenca { get; set; } = 0;
        public decimal Expediente { get; set; } = 0;
        public decimal HonorariosDespachante { get; set; } = 0;
        public decimal Total { get; set; } = 0;
        public string Datahoje { get; set; } = DateTime.Now.ToString("dd 'de' MMMM yyyy", new System.Globalization.CultureInfo("pt-BR"));
    }
    #endregion
   #region "Repositorio Recibo"
    public class RepositorioRecibo : RepositorioBase<Recibo>
    {
        public RepositorioRecibo(IMongoDatabase? database = null)
            : base("Recibo", database) { }

        public async Task<List<Recibo>> FindRefAsync()
        {
            var filter = Builders<Recibo>.Filter.And(
                Builders<Recibo>.Filter.Ne(f => f.Ref_USA, null),
                Builders<Recibo>.Filter.Ne(f => f.Importador, null)
            );
            return await _colecao.Find(filter).ToListAsync();
        }
        public async Task<Recibo?> ObterPorRefUSAAsync(string refUsa)
        {
            if (string.IsNullOrWhiteSpace(refUsa))
                return null;

            var filter = Builders<Recibo>.Filter.Eq(f => f.Ref_USA, refUsa);
            return await _colecao.Find(filter).FirstOrDefaultAsync();
        }
    }
    #endregion

   #region "Orgão Anuente Model"
    public enum TipoOrgaoAnuente { MAPA, ANVISA, DECEX, IBAMA, INMETRO }

    [BsonIgnoreExtraElements]
    public class OrgaoAnuente : IEntidadeBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

        // Propriedades da Licença de Importação (LI)
        public string Numero { get; set; } = string.Empty;
        public string NCM { get; set; } = string.Empty;
        public DateTime? DataRegistro { get; set; }


        // Lista de LPCOs DENTRO desta LI
        public List<LpcoInfo> LPCO { get; set; } = new();

        // Dados de status específicos desta LI/Órgão

        // Dados de contexto (copiados do Processo)
        //public TipoOrgaoAnuente Tipo { get; set; } // O órgão principal desta LI
        public string Ref_USA { get; set; } = string.Empty;
        public string Importador { get; set; } = string.Empty;
        public string Container { get; set; } = string.Empty;
        public string Origem { get; set; } = string.Empty;
        public string Conhecimento { get; set; } = string.Empty;
        public string Terminal { get; set; } = string.Empty;
        public string Produto { get; set; } = string.Empty;
        public DateTime? Inspecao { get; set; }
        public DateTime? DataChegada { get; set; }
        public string Pendencia { get; set; } = string.Empty;
        public string HistoricoDoProcesso { get; set; } = string.Empty;

        public OrgaoAnuente() { }
    }
    public class LpcoViewModel
    {
        // ID para identificar o registro original no banco ao clicar em Editar
        public object OrgaoAnuenteId { get; set; }

        // Dados Gerais (vindos da LI/Processo)
        public string Ref_USA { get; set; }
        public string Importador { get; set; }
        public string NumeroLI { get; set; }
        public string Produto { get; set; }
        public string Container { get; set; }
        public string Terminal { get; set; }
        public string Conhecimento { get; set; }
        public string Origem { get; set; }

        // Datas
        public DateTime? DataChegada { get; set; }
        public DateTime? Inspecao { get; set; }

        // Status e Controle
        public string HistoricoDoProcesso { get; set; }
        public string Pendencia { get; set; }

        // Dados Específicos do LPCO (da sublista de LPCOs)
        public string LPCO { get; set; } // Número do LPCO
        public string NomeOrgao { get; set; }
        public string StatusLPCO { get; set; }
        public string MotivoExigencia { get; set; }

        // Datas específicas do LPCO
        public DateTime? DataRegistroLPCO { get; set; }
        public string ParametrizacaoLPCO { get; set; }
    }
    #endregion
   #region "Repositorio Orgão Anuente"
    public class RepositorioOrgaoAnuente : RepositorioBase<OrgaoAnuente>
    {
        public RepositorioOrgaoAnuente(IMongoDatabase? database = null)
            : base("OrgaosAnuentes", database) { }

        public async Task<List<OrgaoAnuente>> GetAllAsync() => await ListarTodosAsync();

        public async Task<OrgaoAnuente?> GetByIdAsync(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId)) return null;
            return await ObterPorIdAsync(objectId);
        }

        public async Task<OrgaoAnuente?> GetByNumeroAsync(string numero)
        {
            var filter = Builders<OrgaoAnuente>.Filter.Eq(x => x.Numero, numero);
            return await _colecao.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<List<OrgaoAnuente>> ListByRefUsaAsync(string refUsa) => await GetListByRefUsaAsync(refUsa);

        public async Task<List<OrgaoAnuente>> GetListByRefUsaAsync(string refUsa)
        {
            var filter = Builders<OrgaoAnuente>.Filter.Eq(x => x.Ref_USA, refUsa);
            return await _colecao.Find(filter).ToListAsync();
        }


        public async Task<List<OrgaoAnuente>> SearchAsync(string field, string value)
        {
            var filter = Builders<OrgaoAnuente>.Filter.Regex(field, new BsonRegularExpression(new Regex(value, RegexOptions.IgnoreCase)));
            return await _colecao.Find(filter).ToListAsync();
        }

        public async Task DeleteByIdAsync(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId)) return;
            await DeleteAsync(objectId);
        }

        public async Task DeleteAllByRefUsaAsync(string refUsa) => await DeletePorRefUsaAsync(refUsa);

    }
    #endregion

   #region "Vistoria Models"
    public enum StatusVistoria
    {
        AguardandoChegadaParaAgendar,
        SolicitarDataVistoria,
        VistoriaAgendada,
        AguardandoDeferimento,
        AguardandoLaudo,
        ProcessoDadoEntrada
    }

    public class Vistoria
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

        // Chave única para evitar duplicatas
        public string LPCO { get; set; } = string.Empty;

        // Dados copiados para exibição
        public string LI { get; set; } = string.Empty;
        public string Importador { get; set; } = string.Empty;
        public string Container { get; set; } = string.Empty;
        public string Conhecimento { get; set; } = string.Empty;
        public string Ref_USA { get; set; } = string.Empty;
        public string Produto { get; set; } = string.Empty;
        public string ParametrizacaoLPCO { get; set; } = string.Empty;
        public string Terminal { get; set; } = string.Empty;
        public DateTime? Previsao { get; set; } = null;

        // Dados específicos da Vistoria (editáveis)
        public string Notas { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.String)]
        public StatusVistoria Status { get; set; } = StatusVistoria.AguardandoChegadaParaAgendar;
    }
    #endregion
   #region "Vistoria Service"
    public class VistoriaService
    {
        private readonly RepositorioOrgaoAnuente _repoOrgaoAnuente;
        private readonly RepositorioVistorias _repoVistorias;
        private readonly RepositorioProcesso _repoProcesso;

        // Parametrizações que exigem vistoria física no MAPA
        private readonly HashSet<string> _parametrizacoesMapaAlvo = new()
        {
            "EXAME FÍSICO",
            "CONFERÊNCIA FÍSICA",
            "COLETA DE AMOSTRA",
            "INSPEÇÃO FÍSICA"
        };

        public VistoriaService(IMongoDatabase database)
        {
            _repoOrgaoAnuente = new RepositorioOrgaoAnuente();
            _repoVistorias = new RepositorioVistorias(database);
            _repoProcesso = new RepositorioProcesso();
        }

        public async Task<List<string>> SincronizarVistoriasAsync()
        {
            var listaAlteracoes = new List<string>();

            // 1. Carregamento de Dados
            // Traz APENAS processos ativos para otimizar
            var todosProcessos = await _repoProcesso.ListarProcessosAtivosParaStatusAsync();
            var processosDict = todosProcessos.ToDictionary(p => p.Ref_USA);

            var todasAsLIs = await _repoOrgaoAnuente.GetAllAsync();

            // Dicionário de vistorias existentes indexado pelo LPCO
            var vistoriasExistentes = (await _repoVistorias.GetAllAsync())
                                      .Where(v => !string.IsNullOrEmpty(v.LPCO))
                                      .ToDictionary(v => v.LPCO);

            // 2. Loop Único de Processamento
            foreach (var orgaoAnuente in todasAsLIs)
            {
                if (orgaoAnuente.LPCO == null || !orgaoAnuente.LPCO.Any()) continue;

                processosDict.TryGetValue(orgaoAnuente.Ref_USA, out var processoPai);

                foreach (var lpcoInfo in orgaoAnuente.LPCO)
                {
                    // Verificação de nulidade da propriedade string
                    if (string.IsNullOrEmpty(lpcoInfo.LPCO)) continue;

                    // --- ANÁLISE DA REGRA DE NEGÓCIO ---
                    // CORREÇÃO: lpcoInfo é do tipo LpcoInfo, não LPCO
                    var (deveTerVistoria, statusSugerido) = AnalisarLpco(lpcoInfo);

                    // Cenário A: O LPCO deve ter uma vistoria ativa
                    if (deveTerVistoria)
                    {
                        var novaVistoria = new Vistoria
                        {
                            // CORREÇÃO: orgaoAnuente.Numero é int, Vistoria.LI é string. Precisa do ToString()
                            LI = orgaoAnuente.Numero.ToString(),
                            LPCO = lpcoInfo.LPCO,
                            Importador = orgaoAnuente.Importador,
                            Container = orgaoAnuente.Container,
                            Conhecimento = orgaoAnuente.Conhecimento,
                            Ref_USA = orgaoAnuente.Ref_USA,
                            Produto = orgaoAnuente.Produto,
                            ParametrizacaoLPCO = lpcoInfo.ParametrizacaoLPCO ?? "",
                            Terminal = processoPai?.Terminal ?? string.Empty,
                            Previsao = processoPai?.DataDeAtracacao,
                            Status = statusSugerido
                        };

                        if (vistoriasExistentes.TryGetValue(lpcoInfo.LPCO, out var vistoriaDb))
                        {
                            // Atualização
                            bool mudouParametrizacao = vistoriaDb.ParametrizacaoLPCO != novaVistoria.ParametrizacaoLPCO;
                            novaVistoria.Id = vistoriaDb.Id;

                            if (!mudouParametrizacao)
                            {
                                novaVistoria.Status = vistoriaDb.Status;
                            }

                            if (mudouParametrizacao ||
                                vistoriaDb.Terminal != novaVistoria.Terminal ||
                                vistoriaDb.Previsao != novaVistoria.Previsao)
                            {
                                await _repoVistorias.UpsertAsync(novaVistoria);
                                listaAlteracoes.Add($"Atualizado LPCO {lpcoInfo.LPCO}");
                            }
                        }
                        else
                        {
                            // Inserção
                            await _repoVistorias.UpsertAsync(novaVistoria);
                            listaAlteracoes.Add($"Novo LPCO adicionado: {lpcoInfo.LPCO}");
                        }
                    }
                    // Cenário B: Remover se existir
                    else if (vistoriasExistentes.ContainsKey(lpcoInfo.LPCO))
                    {
                        await _repoVistorias.DeleteByLpcoAsync(lpcoInfo.LPCO);
                        listaAlteracoes.Add($"Vistoria removida: {lpcoInfo.LPCO}");
                    }
                }
            }

            return listaAlteracoes;
        }

        /// <summary>
        /// Analisa um LPCO e determina se ele deve gerar uma vistoria.
        /// CORREÇÃO: Tipo do parâmetro alterado de LPCO para LpcoInfo
        /// </summary>
        /// <summary>
        /// Analisa um LPCO e determina se ele deve gerar uma vistoria.
        /// </summary>
        private (bool DeveTerVistoria, StatusVistoria Status) AnalisarLpco(LpcoInfo lpco)
        {
            var nomeOrgao = (lpco.NomeOrgao ?? "").ToUpperInvariant();
            var motivo = (lpco.MotivoExigencia ?? "").ToUpperInvariant();
            var parametrizacao = (lpco.ParametrizacaoLPCO ?? "").ToUpperInvariant();

            // CORREÇÃO: Usamos lpco.StatusLPCO agora, pois StatusLI não existe mais no pai
            var statusLpcoNormalizado = (lpco.StatusLPCO ?? "").ToUpperInvariant();

            // 1. Regra Global: Se Deferido ou Cancelado, nunca gera vistoria
            if (motivo == "DEFERIDO" || motivo == "CANCELADA")
            {
                return (false, StatusVistoria.AguardandoChegadaParaAgendar);
            }

            // 2. REGRA ANVISA
            if (nomeOrgao.Contains("ANVISA"))
            {
                // Verifica o status no LPCO
                if ((string.IsNullOrEmpty(parametrizacao) || parametrizacao == "DOCUMENTAL")
                    && statusLpcoNormalizado == "ENTRADA CONCLUÍDA")
                {
                    return (true, StatusVistoria.ProcessoDadoEntrada);
                }

                return (false, StatusVistoria.AguardandoChegadaParaAgendar);
            }

            // 3. REGRA MAPA
            if (nomeOrgao == "MAPA")
            {
                if (!string.IsNullOrEmpty(parametrizacao) && _parametrizacoesMapaAlvo.Contains(parametrizacao))
                {
                    return (true, StatusVistoria.AguardandoChegadaParaAgendar);
                }

                // Verifica o status no LPCO
                if (string.IsNullOrEmpty(parametrizacao) && statusLpcoNormalizado == "ENTRADA CONCLUÍDA")
                {
                    return (true, StatusVistoria.ProcessoDadoEntrada);
                }

                return (false, StatusVistoria.AguardandoChegadaParaAgendar);
            }

            return (false, StatusVistoria.AguardandoChegadaParaAgendar);
        }
    }
    #endregion
   #region "Repositorio Vistorias"
    public class RepositorioVistorias
    {
        private readonly IMongoCollection<Vistoria> _colecao;

        public RepositorioVistorias(IMongoDatabase? database = null)
        {
            var db = database ?? ConfigDatabase.GetDatabase();
            _colecao = db.GetCollection<Vistoria>("Vistorias");
        }

        public async Task<List<Vistoria>> GetAllAsync()
        {
            return await _colecao.Find(FilterDefinition<Vistoria>.Empty).ToListAsync();
        }

        public async Task UpsertAsync(Vistoria vistoria)
        {
            var filter = Builders<Vistoria>.Filter.Eq(v => v.LPCO, vistoria.LPCO);

            // Busca o ID existente para garantir que o replace seja feito corretamente.
            var existing = await _colecao.Find(filter).FirstOrDefaultAsync();

            if (existing != null)
            {
                vistoria.Id = existing.Id;
                await _colecao.ReplaceOneAsync(v => v.Id == existing.Id, vistoria);
            }
            else
            {
                await _colecao.InsertOneAsync(vistoria);
            }
        }

        public async Task<List<Vistoria>> GetByRefUsaAsync(string refUsa)
        {
            var filter = Builders<Vistoria>.Filter.Eq(v => v.Ref_USA, refUsa);
            return await _colecao.Find(filter).ToListAsync();
        }

        public async Task InsertAsync(Vistoria vistoria)
        {
            if (vistoria.Id == default || vistoria.Id == ObjectId.Empty)
                vistoria.Id = MongoDB.Bson.ObjectId.GenerateNewId();
            await _colecao.InsertOneAsync(vistoria);
        }

        public async Task<Vistoria?> GetByLPCOAsync(string lpco)
        {
            var filter = Builders<Vistoria>.Filter.Eq(v => v.LPCO, lpco ?? "");
            return await _colecao.Find(filter).FirstOrDefaultAsync();
        }

        public async Task DeleteByLpcoAsync(string numeroLpco)
        {
            if (string.IsNullOrEmpty(numeroLpco)) return;
            var filter = Builders<Vistoria>.Filter.Eq(v => v.LPCO, numeroLpco);
            await _colecao.DeleteOneAsync(filter);
        }

        public async Task DeleteAsync(ObjectId id)
        {
            var filter = Builders<Vistoria>.Filter.Eq(x => x.Id, id);
            await _colecao.DeleteOneAsync(filter);
        }
    }
    #endregion

   #region "User Models"
    public class Users
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool Admin { get; set; } = false;

        public Dictionary<string, List<string>> PreferenciasGrids { get; set; } = new();
    }
    public class Logado
    {
        public ObjectId Id { get; set; }
        public bool admin = false;
        public bool log = false;
        public string Usuario = string.Empty;
    }
    #endregion
   #region "Repositorio Users"
    public class RepositorioUsers
    {
        // A variável privada se chama _Users
        private readonly IMongoCollection<Users> _Users;

        public RepositorioUsers(IMongoDatabase database = null)
        {
            var db = database ?? ConfigDatabase.GetDatabase();

            _Users = db.GetCollection<Users>("Users");
        }
        public async Task<Users> GetByIdAsync(ObjectId id)
        {
            var filtro = Builders<Users>.Filter.Eq(u => u.Id, id);

            return await _Users.Find(filtro).FirstOrDefaultAsync();
        }
        public async Task<List<Users>> FindAllAsync()
        {
            var filter = Builders<Users>.Filter.Empty;
            return await _Users.Find(filter).ToListAsync();
        }

        public async Task CreateAsync(Users user)
        {
            await _Users.InsertOneAsync(user);
        }

        public async Task UpdateAsync(Users user)
        {
            var filter = Builders<Users>.Filter.Eq(u => u.Id, user.Id);
            var update = Builders<Users>.Update
                .Set(u => u.Username, user.Username)
                .Set(u => u.Password, user.Password)
                .Set(u => u.Admin, user.Admin);

            await _Users.UpdateOneAsync(filter, update);
        }

        public async Task DeleteAsync(Users user)
        {
            var filter = Builders<Users>.Filter.Eq(u => u.Id, user.Id);
            await _Users.DeleteOneAsync(filter);
        }

        public Logado Login(Users user)
        {
            Logado log = new();
            var filter = Builders<Users>.Filter.Eq(g => g.Username, user.Username);
            var usuarioEncontrado = _Users.Find(filter).FirstOrDefault();

            if (usuarioEncontrado == null)
                return log;

            if (usuarioEncontrado.Password == user.Password)
            {
                log.log = true;
                log.admin = usuarioEncontrado.Admin;
                log.Usuario = usuarioEncontrado.Username;
                log.Id = usuarioEncontrado.Id;
            }

            return log;
        }
    }
    #endregion

   #region "Notificacao Models"
    public class Notificacao
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string? RefUsa { get; set; }
        public string? Mensagem { get; set; }
        public DateTime DataCriacao { get; set; }
        public bool Visualizado { get; set; }
    }
    public class NotifUrgente
    {
        public ObjectId Id { get; set; }
        public ObjectId UsuarioOrigemId { get; set; }
        public ObjectId UsuarioDestinoId { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public DateTime DataEnvio { get; set; }
        public bool Done { get; set; }
    }
    public class UsuarioDestinoItem
    {
        public ObjectId Id { get; set; }
        public string NomeUsuario { get; set; } = "";
    }
    #endregion
   #region "Repositorio Notificação"
    public class RepositorioNotificacao
    {
        private readonly IMongoCollection<Notificacao> _colecao;

        public RepositorioNotificacao(IMongoDatabase? database = null)
        {
            var db = database ?? ConfigDatabase.GetDatabase();
            _colecao = db.GetCollection<Notificacao>("Notificacao");
        }

        public async Task InsertManyAsync(List<Notificacao> notificacoes)
        {
            if (notificacoes == null || !notificacoes.Any()) return;
            await _colecao.InsertManyAsync(notificacoes);
        }

        public async Task ExcluirPorRefUsaAsync(string refUsa)
        {
            var filtro = Builders<Notificacao>.Filter.Eq(n => n.RefUsa, refUsa);
            await _colecao.DeleteManyAsync(filtro);
        }

        public async Task<bool> ExisteNotificacaoAsync(string refUsa, string mensagem)
        {
            var filtro = Builders<Notificacao>.Filter.And(
                Builders<Notificacao>.Filter.Eq(n => n.RefUsa, refUsa),
                Builders<Notificacao>.Filter.Eq(n => n.Mensagem, mensagem)
            );
            return await _colecao.Find(filtro).AnyAsync();
        }
        public async Task ExcluirPorMensagemExataAsync(string refUsa, string mensagem)
        {
            var filtro = Builders<Notificacao>.Filter.And(
                Builders<Notificacao>.Filter.Eq(n => n.RefUsa, refUsa),
                Builders<Notificacao>.Filter.Eq(n => n.Mensagem, mensagem)
            );

            await _colecao.DeleteManyAsync(filtro);
        }

        // 2. Exclui se a RefUsa bater E a mensagem contiver o texto (tipo)
        public async Task ExcluirPorTipoNaMensagemAsync(string refUsa, string trechoMensagem)
        {
            // Usa Regex para simular um "Contains" (SQL LIKE %texto%)
            // O "i" no BsonRegularExpression torna a busca Case Insensitive (ignora maiúsculas/minúsculas)
            var filtro = Builders<Notificacao>.Filter.And(
                Builders<Notificacao>.Filter.Eq(n => n.RefUsa, refUsa),
                Builders<Notificacao>.Filter.Regex(n => n.Mensagem, new BsonRegularExpression(trechoMensagem, "i"))
            );

            await _colecao.DeleteManyAsync(filtro);
        }
        public async Task ExcluirNotificacoesAntigasAsync(DateTime dataLimite)
        {
            var filtro = Builders<Notificacao>.Filter.Lt(n => n.DataCriacao, dataLimite);
            await _colecao.DeleteManyAsync(filtro);
        }

        public async Task<int> ContarNaoVisualizadasAsync()
        {
            var filtro = Builders<Notificacao>.Filter.Eq(n => n.Visualizado, false);
            return (int)await _colecao.CountDocumentsAsync(filtro);
        }

        public async Task<List<Notificacao>> ObterNotificacoesNaoVisualizadasAsync(int limite = 20, int skip = 0)
        {
            var filtro = Builders<Notificacao>.Filter.Eq(n => n.Visualizado, false);
            var sort = Builders<Notificacao>.Sort.Descending(n => n.DataCriacao);

            return await _colecao.Find(filtro)
                .Sort(sort)
                .Skip(skip)
                .Limit(limite)
                .ToListAsync();
        }

        public async Task MarcarComoVisualizadoAsync(string refUsa, string mensagem)
        {
            var filtro = Builders<Notificacao>.Filter.And(
                Builders<Notificacao>.Filter.Eq(n => n.RefUsa, refUsa),
                Builders<Notificacao>.Filter.Eq(n => n.Mensagem, mensagem)
            );
            var update = Builders<Notificacao>.Update.Set(n => n.Visualizado, true);
            await _colecao.UpdateManyAsync(filtro, update);
        }
    }

    public class GerenciadorNotificacao
    {
        private readonly RepositorioNotificacao _notificacaoRepo;

        public GerenciadorNotificacao(IMongoDatabase database)
        {
            _notificacaoRepo = new RepositorioNotificacao(database);
        }

        public async Task CriarNotificacaoSeNecessarioAsync(string refUsa, string mensagem)
        {
            if (string.IsNullOrWhiteSpace(refUsa) || string.IsNullOrWhiteSpace(mensagem)) return;

            bool existe = await _notificacaoRepo.ExisteNotificacaoAsync(refUsa, mensagem);

            if (!existe)
            {
                var novaNotificacao = new Notificacao
                {
                    RefUsa = refUsa,
                    Mensagem = mensagem,
                    DataCriacao = DateTime.Now,
                    Visualizado = false
                };
                await _notificacaoRepo.InsertManyAsync(new List<Notificacao> { novaNotificacao });
            }
        }

        public async Task ExcluirNotificacoesAntigasAsync(DateTime dataLimite)
        {
            await _notificacaoRepo.ExcluirNotificacoesAntigasAsync(dataLimite);
        }

        public async Task SincronizarNotificacoesDoProcessoAsync(Processo processo)
        {
            if (processo == null || string.IsNullOrWhiteSpace(processo.Ref_USA)) return;

            await _notificacaoRepo.ExcluirPorRefUsaAsync(processo.Ref_USA);

            if (processo.DataDeAtracacao.HasValue)
            {
                int dias = (processo.DataDeAtracacao.Value - DateTime.Today).Days;

                if (dias is >= 0 and <= 5 && (processo.Redestinacao == null || processo.Redestinacao == false))
                {
                    await CriarNotificacaoSeNecessarioAsync(
                        processo.Ref_USA,
                        $"Processo {processo.Ref_USA}: Redestinar container ao terminal"
                    );
                }
            }

            if (processo.DataRegistroDI == null)
            {
                VerificarEVincularVencimento(processo, processo.VencimentoFreeTime, "FreeTime");

                VerificarEVincularVencimento(processo, processo.VencimentoFMA, "FMA");

                VerificarEVincularVencimento(processo, processo.VencimentoLI_LPCO, "LI/LPCO");
            }
        }

        private async void VerificarEVincularVencimento(Processo doc, DateTime? vencimento, string nomeExibicao)
        {
            if (!vencimento.HasValue) return;
            int dias = (vencimento.Value - DateTime.Today).Days;

            if (dias is >= 0 and <= 5)
            {
                string msg = $"Processo {doc.Ref_USA}: Vencimento {nomeExibicao} em {dias} dia(s)";
                await CriarNotificacaoSeNecessarioAsync(doc.Ref_USA, msg);
            }
        }
    }
    #endregion
   #region "Repositorio NotifUrgente"
    public class RepositorioNotifUrgente
    {
        private readonly IMongoCollection<NotifUrgente> _colecao;

        public RepositorioNotifUrgente(IMongoDatabase database)
        {
            _colecao = database.GetCollection<NotifUrgente>("NotifUrgente");
        }

        public async Task InsertAsync(NotifUrgente notif)
        {
            await _colecao.InsertOneAsync(notif);
        }

        public async Task UpdateAsync(NotifUrgente notif)
        {
            var filter = Builders<NotifUrgente>.Filter.Eq(x => x.Id, notif.Id);
            await _colecao.ReplaceOneAsync(filter, notif);
        }

        public async Task DeleteAsync(ObjectId id)
        {
            var filter = Builders<NotifUrgente>.Filter.Eq(x => x.Id, id);
            await _colecao.DeleteOneAsync(filter);
        }

        public async Task<List<NotifUrgente>> GetByUsuarioOrigemAsync(ObjectId usuarioOrigemId)
        {
            var filter = Builders<NotifUrgente>.Filter.Eq(x => x.UsuarioOrigemId, usuarioOrigemId);
            return await _colecao.Find(filter).ToListAsync();
        }

        public async Task<List<NotifUrgente>> GetByUsuarioDestinoAsync(ObjectId usuarioDestinoId)
        {
            var filter = Builders<NotifUrgente>.Filter.Eq(x => x.UsuarioDestinoId, usuarioDestinoId);
            return await _colecao.Find(filter).ToListAsync();
        }

        public async Task<List<NotifUrgente>> GetPendentesPorUsuarioAsync(ObjectId userId)
        {
            var filter = Builders<NotifUrgente>.Filter.And(
                Builders<NotifUrgente>.Filter.Or(
                    Builders<NotifUrgente>.Filter.Eq(x => x.UsuarioOrigemId, userId),
                    Builders<NotifUrgente>.Filter.Eq(x => x.UsuarioDestinoId, userId)
                ),
                Builders<NotifUrgente>.Filter.Eq(x => x.Done, false)
            );
            return await _colecao.Find(filter).ToListAsync();
        }
    }
    #endregion

   #region "Models Auxiliares"
    public class Agencia
    {
        public string Numero { get; set; } = string.Empty;
        public decimal Custo { get; set; }
    }
    #endregion

}