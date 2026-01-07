using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLUSA.Models
{
    public class Vencimento
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("importador")]
        public string Importador { get; set; } // Ex: "FREEWAY"

        [BsonElement("cnpjs")]
        public List<string> Cnpjs { get; set; } // Ex: ["04.600.../0001", "04.600.../0002"]

        [BsonElement("data_radar")]
        [BsonIgnoreIfNull]
        public DateTime? DataVencimentoRadar { get; set; }

        [BsonElement("data_procuracao")]
        [BsonIgnoreIfNull]
        public DateTime? DataVencimentoProcuracao { get; set; }

        [BsonElement("data_ecac")]
        [BsonIgnoreIfNull]
        public DateTime? DataVencimentoEcac { get; set; }

        [BsonElement("data_sigvig")]
        [BsonIgnoreIfNull]
        public DateTime? DataVencimentoSigvig { get; set; }

        [BsonElement("data_lecom")]
        [BsonIgnoreIfNull]
        public DateTime? DataVencimentoLecom { get; set; }

        [BsonElement("ultima_notificacao")]
        [BsonIgnoreIfNull]
        public DateTime? DataUltimaNotificacao { get; set; }
    }
}
