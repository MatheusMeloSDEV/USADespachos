using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace CLUSA
{
    public enum TipoOrgaoAnuente { MAPA, ANVISA, DECEX, IBAMA, INMETRO }

    public class OrgaoAnuente
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

        [BsonRepresentation(BsonType.String)]
        public TipoOrgaoAnuente Tipo { get; set; }
        public string Ref_USA { get; set; } = string.Empty; 
        public DateTime? Inspecao { get; set; }
        public string Pendencia { get; set; } = string.Empty;
        public string StatusDoProcesso { get; set; } = string.Empty;
        public LicencaImportacao Licenca { get; set; } = new();

        public OrgaoAnuente() { }

        public OrgaoAnuente(string refUsa, TipoOrgaoAnuente tipo)
        {
            Ref_USA = refUsa;
            Tipo = tipo;
        }
    }
}