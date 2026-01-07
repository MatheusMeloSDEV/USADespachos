using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLUSA.Models
{
    public class Log
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("data_hora")]
        public DateTime DataHora { get; set; } = DateTime.Now; // Pega a hora atual automaticamente

        [BsonElement("tipo_acao")]
        public string TipoAcao { get; set; } // Ex: "Criação", "Edição", "Exclusão", "Email"

        [BsonElement("mensagem")]
        public string Mensagem { get; set; } // Ex: "Vencimento da FREEWAY editado."

        [BsonElement("detalhes_tecnicos")]
        [BsonIgnoreIfNull]
        public string Detalhes { get; set; } // Opcional: Para guardar erros ou IDs
    }
}
