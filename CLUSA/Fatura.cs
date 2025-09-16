using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CLUSA
{
    public class Fatura : IEntidadeBase
    {
        public Fatura() { }
        public Fatura(Processo processo)
        {
            Ref_USA = processo.Ref_USA;
            SR = processo.SR;
            Importador = processo.Importador;
            Veiculo = processo.Veiculo;
            FLO = processo.FLO;
            Mercadoria = processo.Produto;
            Marca = processo.Marca;
            DataAtracacao = processo.DataDeAtracacao;
            DI = processo.DI;
            DataDesembaracoDI = processo.DataDesembaracoDI;
            DAtaDI = processo.DataRegistroDI;
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        public string Ref_USA { get; set; } = string.Empty;
        public string SR { get; set; } = string.Empty;
        public string Importador { get; set; } = string.Empty;
        public string Endereco_Importador { get; set; } = string.Empty;
        public string FLO { get; set; } = string.Empty;
        public DateTime? DataAtracacao { get; set; } = (DateTime?)null;
        public string Veiculo { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public float Quantidade { get; set; } = 0;
        public string Mercadoria { get; set; } = string.Empty;
        public decimal ValRecebidos { get; set; } = 0;
        public DateTime? DataRecebimento { get; set; } = (DateTime?)null;
        public string DI { get; set; } = string.Empty;
        public DateTime? DataDesembaracoDI { get; set; } = (DateTime?)null;
        public DateTime? DAtaDI { get; set; } = (DateTime?)null;
        public decimal ImpostoImportacao { get; set; } = 0;
        public decimal IPI { get; set; } = 0;
        public decimal DI_ADICAO { get; set; } = 0;
        public decimal PIS_PASEP { get; set; } = 0;
        public decimal COFINS { get; set; } = 0;
        public decimal MULTA_LI { get; set; } = 0;
        public decimal ICMS { get; set; } = 0;
        public List<Agencia> Agencias { get; set; } = new();
        public string ArmazenagemN { get; set; } = string.Empty;
        public decimal ArmazenagemP { get; set; } = 0;
        public string FreteMaritimoN { get; set; } = string.Empty;
        public decimal FreteMaritimoP { get; set; } = 0;
        public string Marinha_MercanteN { get; set; } = string.Empty;
        public decimal Marinha_MercanteP { get; set; } = 0;
        public string GRUANVISAN { get; set; } = string.Empty;
        public decimal GRUANVISAP { get; set; } = 0;
        public string LiCancelada_IndeferidaN { get; set; } = string.Empty;
        public decimal LiCancelada_IndeferidaP { get; set; } = 0;
        public string ExpedienteLiCanceladaN { get; set; } = string.Empty;
        public decimal ExpedienteLiCanceladaP { get; set; } = 0;
        public string EncaminhamentoAmostrasN { get; set; } = string.Empty;
        public decimal EncaminhamentoAmostrasP { get; set; } = 0;
        public string DarfAnvisaN { get; set; } = string.Empty;
        public decimal DarfAnvisaP { get; set; } = 0;
        public string MotoboyN { get; set; } = string.Empty;
        public decimal MotoboyP { get; set; } = 0;
        public decimal LiP { get; set; } = 0;
        public decimal Expediente { get; set; } = 0;
        public string DespesasDesembaracoN { get; set; } = string.Empty;
        public decimal DespesasDesembaracoP { get; set; } = 0;
        public decimal HD { get; set; } = 0;
        public decimal Cartorio { get; set; } = 0;
        public string[] NomesDocumentosAnexos { get; set; } = Array.Empty<string>();
        public string[] NumeroDocumentosAnexos { get; set; } = Array.Empty<string>();
        public decimal TotalDespesas { get; set; } = 0;
        public decimal NComissao { get; set; } = 0;
        public decimal SubTotal { get; set; } = 0;
        public decimal Adiantamento { get; set; } = 0;
        public decimal Saldo { get; set; } = 0;
        public string TipoFinalizacao { get; set; } = string.Empty;
    }
}
