using CLUSA.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLUSA.Repositories
{
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
}
