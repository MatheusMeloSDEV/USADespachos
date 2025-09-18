using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CLUSA
{
    public class Recibo : IEntidadeBase
    {
        public Recibo() { }
        public Recibo(Processo processo)
        {
            Ref_USA = processo.Ref_USA;
            SR = processo.SR;
            Importador = processo.Importador;
            Exportador = processo.Exportador;
            Veiculo = processo.Veiculo;
            Mercadoria = processo.Produto;
        }
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        public string Ref_USA { get; set; } = string.Empty;
        public string SR { get; set; } = string.Empty;
        public string Importador { get; set; } = string.Empty;
        public string Exportador { get; set; } = string.Empty;
        public string Endereco_Importador { get; set; } = string.Empty;
        public string Veiculo { get; set; } = string.Empty;
        public string Mercadoria { get; set; } = string.Empty;
        public decimal EmissaoLicenca { get; set; } = 0;
        public decimal Expediente { get; set; } = 0;
        public decimal HonorariosDespachante { get; set; } = 0;
        public decimal Total { get; set; } = 0;
        public string Datahoje { get; set; } = DateTime.Now.ToString("dd 'de' MMMM yyyy", new System.Globalization.CultureInfo("pt-BR"));
    }
}
