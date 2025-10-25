using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CLUSA
{
    public enum StatusVistoria
    {
        AguardandoChegadaParaAgendar,
        SolicitarDataVistoria,
        VistoriaAgendada,
        AguardandoDeferimento,
        AguardandoLaudo,
        ProcessoDadoEntrada
    }

    public class Vistoria
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

        // Chave única para evitar duplicatas
        public string LPCO { get; set; } = string.Empty;

        // Dados copiados para exibição
        public string LI { get; set; } = string.Empty;
        public string Importador { get; set; } = string.Empty;
        public string Container { get; set; } = string.Empty;
        public string Conhecimento { get; set; } = string.Empty;
        public string Ref_USA { get; set; } = string.Empty;
        public string Produto { get; set; } = string.Empty;
        public string ParametrizacaoLPCO { get; set; } = string.Empty;
        public string Terminal { get; set; } = string.Empty;
        public DateTime? Previsao { get; set; } = null;

        // Dados específicos da Vistoria (editáveis)
        public string Notas { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.String)]
        public StatusVistoria Status { get; set; } = StatusVistoria.AguardandoChegadaParaAgendar;
    }
}