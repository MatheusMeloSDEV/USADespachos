using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace CLUSA.Models
{
    // Nova classe para representar cada data dinâmica
    public class EventoVencimento
    {
        [BsonElement("tag")]
        public string Tag { get; set; } // Ex: "Procuração", "Radar", etc.

        [BsonElement("data")]
        public DateTime Data { get; set; }
    }

    public class Vencimento
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("importador")]
        public string Importador { get; set; }

        [BsonElement("cnpjs")]
        public List<string> Cnpjs { get; set; }

        // Nova propriedade que substitui todas as datas soltas
        [BsonElement("eventos")]
        public List<EventoVencimento> Eventos { get; set; } = new List<EventoVencimento>();

        [BsonElement("ultima_notificacao")]
        [BsonIgnoreIfNull]
        public DateTime? DataUltimaNotificacao { get; set; }
    }
}