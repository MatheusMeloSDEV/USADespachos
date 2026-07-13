using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace CLUSA.Models
{
    public class VistoriaDUIMP
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

        public string DUIMP { get; set; } = string.Empty;

        // Contagem de órgãos anuentes dentro dos catálogos do processo
        public int ContagemOrgaosAnuentes { get; set; } = 0;

        // Lista legível de órgãos anuentes, ex: "MAPA, ANVISA, DECEX"
        public string OrgaosAnuentesString { get; set; } = string.Empty;

        // Data do registro da DUIMP/DI no processo
        public DateTime? DataRegistro { get; set; }

        public string Ref_USA { get; set; } = string.Empty;
        public string Importador { get; set; } = string.Empty;
        public string Container { get; set; } = string.Empty;
        public string Produto { get; set; } = string.Empty;
        public string Terminal { get; set; } = string.Empty;
        public DateTime? DataDeAtracacao { get; set; }
        // Data em que a DUIMP foi deferida (pode ser nula)
        public DateTime? Deferido { get; set; }
        public string Notas { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}