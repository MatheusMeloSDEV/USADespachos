using CLUSA.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLUSA.Models
{
    public class Catalogo : IEntidadeBase
    {
        public Catalogo()
        {
            Mercadoria = string.Empty;
            NCM = string.Empty;
            cClassTrib = string.Empty;
            Orgaos = new List<Orgao>(); // <-- Alterado aqui
        }

        public Catalogo(string mercadoria,string ncm, string _cClassTrib, List<Orgao> orgao)
        {
            Mercadoria = mercadoria ?? string.Empty;
            NCM = ncm ?? string.Empty;
            cClassTrib = _cClassTrib ?? string.Empty;
            Orgaos = orgao ?? new List<Orgao>(); // <-- Alterado aqui
        }
        
        public ObjectId Id { get; set; }
        public string Mercadoria { get; set; } 
        public string NCM { get; set; }
        public string cClassTrib { get; set; }
        public List<Orgao> Orgaos { get; set; }
    }
    public class Orgao
    {
        public Orgao(string orgaoId, string parametrizacao, DateTime inspecao, DateTime coleta, string comunicado)
        {
            OrgaoId = orgaoId;
            Parametrizacao = parametrizacao;
            Inspecao = inspecao;
            Coleta = coleta;
            Comunicado = comunicado;
        }

        public string OrgaoId { get; set; }
        public string Parametrizacao { get; set; }
        public DateTime? Inspecao { get; set; }
        public DateTime? Coleta { get; set; }
        public string Comunicado { get; set; }
    }
}
