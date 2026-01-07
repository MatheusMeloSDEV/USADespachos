using CLUSA.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLUSA.Repositories
{
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
}
