using CLUSA.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public async Task ExecutarBulkAsync(IEnumerable<WriteModel<Vistoria>> operations)
        {
            if (operations != null && operations.Any())
            {
                await _colecao.BulkWriteAsync(operations);
            }
        }
        public async Task<List<Vistoria>> GetAllAsync()
        {
            return await _colecao.Find(FilterDefinition<Vistoria>.Empty).ToListAsync();
        }
        public async Task<List<Vistoria>> GetByListaRefUsaAsync(IEnumerable<string> refsUsa)
        {
            var filter = Builders<Vistoria>.Filter.In(v => v.Ref_USA, refsUsa);
            return await _colecao.Find(filter).ToListAsync();
        }

        public async Task UpsertAsync(Vistoria vistoria)
        {
            if (string.IsNullOrWhiteSpace(vistoria.LPCO)) return;

            // 1. BLINDAGEM: Busca se já existe este LPCO no banco antes de qualquer coisa
            var filtroExistente = Builders<Vistoria>.Filter.Eq(v => v.LPCO, vistoria.LPCO);

            // Projeta apenas o ID para ser rápido (não traz o objeto todo)
            var vistoriaExistente = await _colecao.Find(filtroExistente)
                                                  .Project(v => new { v.Id })
                                                  .FirstOrDefaultAsync();

            if (vistoriaExistente != null)
            {
                // SE JÁ EXISTE: Forçamos o objeto novo a usar o ID antigo.
                // Isso garante que o ReplaceOne vai atualizar o registro correto e não criar duplicata.
                vistoria.Id = vistoriaExistente.Id;
            }
            else
            {
                // SE NÃO EXISTE: Só agora geramos um ID novo
                if (vistoria.Id == ObjectId.Empty)
                {
                    vistoria.Id = ObjectId.GenerateNewId();
                }
            }

            // 2. Agora fazemos o Replace/Upsert seguro pelo ID (que é único e imutável)
            var filtroId = Builders<Vistoria>.Filter.Eq(v => v.Id, vistoria.Id);

            await _colecao.ReplaceOneAsync(filtroId, vistoria, new ReplaceOptions { IsUpsert = true });
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
}
