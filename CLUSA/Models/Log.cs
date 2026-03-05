using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace CLUSA.Models
{
    public class Log
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("data_hora")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)] 
        public DateTime DataHora { get; set; } = DateTime.Now;

        [BsonElement("tipo_acao")]
        public string TipoAcao { get; set; }

        [BsonElement("mensagem")]
        public string Mensagem { get; set; }

        // --- NOVO CAMPO ADICIONADO AQUI ---
        [BsonElement("autor")]
        [BsonIgnoreIfNull]
        public string Autor { get; set; }

        [BsonElement("detalhes_tecnicos")]
        [BsonIgnoreIfNull]
        public string Detalhes { get; set; }
    }
}