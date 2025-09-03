using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System.Text.RegularExpressions;

namespace CLUSA
{
    public class RepositorioProcesso
    {
        private readonly IMongoCollection<Processo> _Processo;
        private readonly IMongoCollection<ANVISA> _Anvisa;
        private readonly IMongoCollection<DECEX> _Decex;
        private readonly IMongoCollection<IBAMA> _Ibama;
        private readonly IMongoCollection<INMETRO> _Inmetro;
        private readonly IMongoCollection<MAPA> _MAPA;
        private readonly IMongoCollection<Fatura> _Fatura;
        private readonly IMongoCollection<Recibo> _Recibo;

        private readonly RepositorioNotificacao _repositorioNotificacao;

        public RepositorioProcesso()
        {
            var mongoClient = new MongoClient("mongodb+srv://dev:dev@cluster0.cn10nzt.mongodb.net/");
            var mongoDatabase = mongoClient.GetDatabase("Trabalho");
            _Processo = mongoDatabase.GetCollection<Processo>("PROCESSO");
            _Anvisa = mongoDatabase.GetCollection<ANVISA>("ANVISA");
            _Decex = mongoDatabase.GetCollection<DECEX>("DECEX");
            _Ibama = mongoDatabase.GetCollection<IBAMA>("IBAMA");
            _Inmetro = mongoDatabase.GetCollection<INMETRO>("INMETRO");
            _MAPA = mongoDatabase.GetCollection<MAPA>("MAPA");
            _Fatura = mongoDatabase.GetCollection<Fatura>("Fatura");
            _Recibo = mongoDatabase.GetCollection<Recibo>("Recibo");
            _repositorioNotificacao = new(mongoDatabase);
        }

        public List<Processo> ListaProcesso => _Processo.Find(FilterDefinition<Processo>.Empty).ToList();

        public List<string> ObterValoresUnicos(string campo)
        {
            return _Processo.Distinct<string>(campo, FilterDefinition<Processo>.Empty).ToList();
        }

        public List<string> ObterImportadoresUnicos()
        {
            return _Processo.Distinct<string>("Importador", FilterDefinition<Processo>.Empty).ToList();
        }

        public async Task Create(Processo processo, string colecao)
        {
            await Task.Run(() =>
            {
                switch (colecao)
                {
                    case "MAPA":
                        _MAPA.InsertOne(new MAPA(processo));
                        break;
                    case "Anvisa":
                        _Anvisa.InsertOne(new ANVISA(processo));
                        break;
                    case "Decex":
                        _Decex.InsertOne(new DECEX(processo));
                        break;
                    case "IBAMA":
                        _Ibama.InsertOne(new IBAMA(processo));
                        break;
                    case "IMETRO":
                        _Inmetro.InsertOne(new INMETRO(processo));
                        break;
                    default:
                        _Processo.InsertOne(processo);
                        var fatura = new Fatura(processo);
                        _Fatura.InsertOne(fatura);
                        var recibo = new Recibo(processo);
                        _Recibo.InsertOne(recibo);
                        break;
                }
            });
        }

        public async Task Delete(Processo processo)
        {
            await Task.Run(() =>
            {
                var filterP = Builders<Processo>.Filter.Eq(p => p.Id, processo.Id);
                _Processo.DeleteOne(filterP);

                ExcluirRelacionado(_MAPA, processo.Ref_USA);
                ExcluirRelacionado(_Anvisa, processo.Ref_USA);
                ExcluirRelacionado(_Decex, processo.Ref_USA);
                ExcluirRelacionado(_Ibama, processo.Ref_USA);
                ExcluirRelacionado(_Inmetro, processo.Ref_USA);
                ExcluirRelacionado(_Fatura, processo.Ref_USA);
                ExcluirRelacionado(_Recibo, processo.Ref_USA);

                _repositorioNotificacao.ExcluirPorRefUsa(processo.Ref_USA);
            });
        }

        public async Task Update(Processo processo)
        {
            await AtualizarDocumento(_Processo, processo.Id, processo);

            var faturaAtual = new Fatura(processo) { Id = GetFaturaId(processo.Ref_USA) };
            await AtualizarDocumento(_Fatura, faturaAtual.Id, faturaAtual);

            var reciboAtual = new Recibo(processo) { Id = GetReciboId(processo.Ref_USA) };
            await AtualizarDocumento(_Recibo, reciboAtual.Id, reciboAtual);

            await GerenciarRelacoes(processo);
        }

        private async Task GerenciarRelacoes(Processo processo)
        {
            if (!processo.TMapa) await RemoverRelacionado(_MAPA, processo.Ref_USA);
            else await AtualizarRelacionado(_MAPA, processo.Ref_USA, new MAPA(processo));

            if (!processo.TAnvisa) await RemoverRelacionado(_Anvisa, processo.Ref_USA);
            else await AtualizarRelacionado(_Anvisa, processo.Ref_USA, new ANVISA(processo));

            if (!processo.TDecex) await RemoverRelacionado(_Decex, processo.Ref_USA);
            else await AtualizarRelacionado(_Decex, processo.Ref_USA, new DECEX(processo));

            if (!processo.TIbama) await RemoverRelacionado(_Ibama, processo.Ref_USA);
            else await AtualizarRelacionado(_Ibama, processo.Ref_USA, new IBAMA(processo));

            if (!processo.TImetro) await RemoverRelacionado(_Inmetro, processo.Ref_USA);
            else await AtualizarRelacionado(_Inmetro, processo.Ref_USA, new INMETRO(processo));
        }

        private static void ExcluirRelacionado<T>(IMongoCollection<T> colecao, string refUsa) where T : class
        {
            var filtro = Builders<T>.Filter.Eq("Ref_USA", refUsa);
            colecao.DeleteMany(filtro);
        }

        private static async Task RemoverRelacionado<T>(IMongoCollection<T> colecao, string refUsa) where T : class
        {
            var filtro = Builders<T>.Filter.Eq("Ref_USA", refUsa);
            await colecao.DeleteManyAsync(filtro);
        }

        private static async Task AtualizarDocumento<T>(IMongoCollection<T> colecao, ObjectId id, T documento) where T : class
        {
            var filtro = Builders<T>.Filter.Eq("_id", id);
            var updateDef = CriarAtualizacaoSemId(documento);
            await colecao.UpdateOneAsync(filtro, updateDef, new UpdateOptions { IsUpsert = true });
        }

        private static async Task AtualizarRelacionado<T>(IMongoCollection<T> colecao, string refUsa, T documento)
            where T : class, new()
        {
            var filtro = Builders<T>.Filter.Eq("Ref_USA", refUsa);
            var existente = await colecao.Find(filtro).FirstOrDefaultAsync();
            if (existente != null)
            {
                var idProp = typeof(T)
                    .GetProperties()
                    .FirstOrDefault(p =>
                        Attribute.IsDefined(p, typeof(BsonIdAttribute)) ||
                        p.Name == "Id" ||
                        p.Name == "_id");

                if (idProp != null)
                {
                    var existingId = idProp.GetValue(existente);
                    idProp.SetValue(documento, existingId);
                }

                await colecao.ReplaceOneAsync(filtro, documento);
            }
            else
            {
                await colecao.InsertOneAsync(documento);
            }
        }

        private static UpdateDefinition<T> CriarAtualizacaoSemId<T>(T documento)
        {
            var bson = documento.ToBsonDocument();
            bson.Remove("_id");
            bson.Remove("Id");
            bson.Remove("Inspecao");

            var setDoc = new BsonDocument();
            foreach (var elem in bson)
            {
                if (elem.Value != BsonNull.Value)
                    setDoc.Add(elem.Name, elem.Value);
            }

            return new BsonDocumentUpdateDefinition<T>(
                new BsonDocument("$set", setDoc)
            );
        }

        private ObjectId GetFaturaId(string refUsa)
        {
            var f = _Fatura.Find(Builders<Fatura>.Filter.Eq("Ref_USA", refUsa)).FirstOrDefault();
            return f != null ? f.Id : ObjectId.GenerateNewId();
        }
        private ObjectId GetReciboId(string refUsa)
        {
            var f = _Recibo.Find(Builders<Recibo>.Filter.Eq("Ref_USA", refUsa)).FirstOrDefault();
            return f != null ? f.Id : ObjectId.GenerateNewId();
        }

        public bool VerificarExistencia(Processo processo)
        {
            bool existeMapa = ExisteNaColecao(_MAPA, "TMapa", processo.TMapa);
            bool existeAnvisa = ExisteNaColecao(_Anvisa, "TAnvisa", processo.TAnvisa);
            bool existeDecex = ExisteNaColecao(_Decex, "TDecex", processo.TDecex);
            bool existeImetro = ExisteNaColecao(_Inmetro, "TImetro", processo.TImetro);
            bool existeIbama = ExisteNaColecao(_Ibama, "TIbama", processo.TIbama);

            return existeMapa || existeAnvisa || existeDecex || existeImetro || existeIbama;
        }

        public static bool ExisteNaColecao<T>(IMongoCollection<T> colecao, string campo, bool valor) where T : class
        {
            var filtro = Builders<T>.Filter.Eq(campo, valor);
            return colecao.Find(filtro).Any();
        }

        public List<Processo> FindAll()
        {
            return _Processo.Find(FilterDefinition<Processo>.Empty).ToList();
        }
        public async Task<List<Processo>> FindAllAsync()
        {
            try
            {
                var processos = await _Processo.Find(FilterDefinition<Processo>.Empty).ToListAsync();
                Console.WriteLine($"Total de processos encontrados: {processos.Count}");
                return processos;
            }
            catch (MongoException ex)
            {
                Console.WriteLine($"Erro no MongoDB ao buscar todos os processos: {ex.Message}");
                return new List<Processo>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro inesperado ao buscar todos os processos: {ex.Message}");
                return new List<Processo>();
            }
        }
        public List<Processo> Find(string filtro, string pesquisa)
        {
            try
            {
                var property = typeof(Processo).GetProperty(filtro) ?? throw new KeyNotFoundException($"O campo '{filtro}' não existe no modelo Processo.");
                var filter = Builders<Processo>.Filter.Regex(filtro, new BsonRegularExpression(new Regex(pesquisa, RegexOptions.IgnoreCase)));

                var resultados = _Processo.Find(filter).ToList();

                Console.WriteLine($"Filtro aplicado no MongoDB: {filtro} com pesquisa '{pesquisa}'. Itens encontrados: {resultados.Count}");
                return resultados;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar dados no MongoDB: {ex.Message}");
                return new List<Processo>();
            }
        }
    }
}
