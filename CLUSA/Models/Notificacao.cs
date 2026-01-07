using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLUSA.Models
{
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
}
