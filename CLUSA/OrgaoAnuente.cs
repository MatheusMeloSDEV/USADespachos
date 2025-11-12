using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CLUSA
{
    public enum TipoOrgaoAnuente { MAPA, ANVISA, DECEX, IBAMA, INMETRO }

    public class OrgaoAnuente
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

        // Propriedades da Licença de Importação (LI)
        public string Numero { get; set; } = string.Empty;
        public string NCM { get; set; } = string.Empty;
        public DateTime? DataRegistro { get; set; }
        

        // Lista de LPCOs DENTRO desta LI
        public List<LpcoInfo> LPCO { get; set; } = new();

        // Dados de status específicos desta LI/Órgão

        // Dados de contexto (copiados do Processo)
        //public TipoOrgaoAnuente Tipo { get; set; } // O órgão principal desta LI
        public string Ref_USA { get; set; } = string.Empty;
        public string Importador { get; set; } = string.Empty;
        public string Container { get; set; } = string.Empty;
        public string Origem { get; set; } = string.Empty;
        public string Conhecimento { get; set; } = string.Empty;
        public string Terminal { get; set; } = string.Empty;
        public string Produto { get; set; } = string.Empty;
        public DateTime? Inspecao { get; set; }
        public DateTime? DataChegada { get; set; }
        public string StatusLI { get; set; } = string.Empty;
        public string Pendencia { get; set; } = string.Empty;
        public string HistoricoDoProcesso { get; set; } = string.Empty;

        public OrgaoAnuente() { }
    }
}