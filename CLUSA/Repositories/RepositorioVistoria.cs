using CLUSA.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CLUSA.Repositories
{
    public class RepositorioVistorias
    {
        private readonly IMongoCollection<Vistoria> _colecao;

        public RepositorioVistorias(IMongoDatabase? database = null)
        {
            var db = database ?? ConfigDatabase.GetDatabase();
            _colecao = db.GetCollection<Vistoria>("Vistorias");
        }

        // --- MÉTODOS DE LEITURA ---

        public async Task<List<Vistoria>> GetAllAsync()
        {
            // Traz tudo (Usado para popular o Grid)
            return await _colecao.Find(FilterDefinition<Vistoria>.Empty).ToListAsync();
        }

        public async Task<List<Vistoria>> GetTodasAsVistoriasDoBancoAsync()
        {
            // Traz tudo (Usado pelo Robô para achar Zumbis)
            return await _colecao.Find(_ => true).ToListAsync();
        }

        public async Task<Vistoria?> GetByLPCOAsync(string lpco)
        {
            if (string.IsNullOrWhiteSpace(lpco)) return null;
            return await _colecao.Find(v => v.LPCO == lpco).FirstOrDefaultAsync();
        }

        public async Task<List<Vistoria>> GetByListaRefUsaAsync(IEnumerable<string> refsUsa)
        {
            var filter = Builders<Vistoria>.Filter.In(v => v.Ref_USA, refsUsa);
            return await _colecao.Find(filter).ToListAsync();
        }

        // --- MÉTODOS DE ESCRITA ---

        public async Task UpsertAsync(Vistoria vistoria)
        {
            if (string.IsNullOrWhiteSpace(vistoria.LPCO)) return;

            // 1. Tenta achar pelo LPCO primeiro (Blindagem contra duplicidade)
            var filtroLpco = Builders<Vistoria>.Filter.Eq(v => v.LPCO, vistoria.LPCO);
            var existente = await _colecao.Find(filtroLpco).Project(v => v.Id).FirstOrDefaultAsync();

            if (existente != ObjectId.Empty)
            {
                vistoria.Id = existente; // Usa o ID que já existe
            }
            else if (vistoria.Id == ObjectId.Empty)
            {
                vistoria.Id = ObjectId.GenerateNewId(); // Gera novo se não existe
            }

            // 2. Faz o Upsert pelo ID
            var filtroId = Builders<Vistoria>.Filter.Eq(v => v.Id, vistoria.Id);
            await _colecao.ReplaceOneAsync(filtroId, vistoria, new ReplaceOptions { IsUpsert = true });
        }

        public async Task ExecutarBulkAsync(IEnumerable<WriteModel<Vistoria>> operations)
        {
            if (operations != null && operations.Any())
            {
                await _colecao.BulkWriteAsync(operations);
            }
        }

        // --- MÉTODOS DE EXCLUSÃO ---

        public async Task DeleteByLpcoAsync(string numeroLpco)
        {
            if (string.IsNullOrEmpty(numeroLpco)) return;
            await _colecao.DeleteOneAsync(v => v.LPCO == numeroLpco);
        }

        public async Task DeleteByListaLpcosAsync(IEnumerable<string> listaLpcos)
        {
            if (listaLpcos == null || !listaLpcos.Any()) return;
            var filter = Builders<Vistoria>.Filter.In(v => v.LPCO, listaLpcos);
            await _colecao.DeleteManyAsync(filter);
        }
    }
}