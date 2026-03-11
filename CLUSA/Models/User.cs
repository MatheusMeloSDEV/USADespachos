using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLUSA.Models
{
    public class Users
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool Admin { get; set; } = false;
        public bool UsarEstiloNovo { get; set; } = false; 
        public bool ModoEscuro { get; set; } = false;
        public int ItensPorPagina { get; set; } = 50; // 50 é o padrão caso seja a primeira vez
        public Dictionary<string, List<string>> PreferenciasGrids { get; set; } = new();
    }
    public class Logado
    {
        public ObjectId Id { get; set; }
        public bool admin = false;
        public bool log = false;
        public string Usuario = string.Empty;
    }
}
