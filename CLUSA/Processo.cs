using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CLUSA
{
    public class Processo
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        //Não Aparecem
        public bool TDecex { get; set; } = false;
        public bool TAnvisa { get; set; } = false;
        public bool TMapa { get; set; } = false;
        public bool TImetro { get; set; } = false;
        public bool TIbama { get; set; } = false;
        public bool PossuiEmbarque { get; set; } = false;
        public DateTime? VencimentoFreeTime { get; set; } = (DateTime?)null;
        public DateTime? VencimentoFMA { get; set; } = (DateTime?)null;
        public DateTime? VencimentoLI_LPCO { get; set; } = (DateTime?)null;
        //

        public string Ref_USA { get; set; } = string.Empty;
        public string Importador { get; set; } = string.Empty;
        public string SR { get; set; } = string.Empty;
        public string Exportador { get; set; } = string.Empty;
        public string Produto { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string Veiculo { get; set; } = string.Empty; //Implementar
        public string PortoDestino { get; set; } = string.Empty;
        public string FLO { get; set; } = string.Empty;
        public int FreeTime { get; set; } = 0;
        public string Terminal { get; set; } = string.Empty;
        public string Conhecimento { get; set; } = string.Empty;
        public string Armador { get; set; } = string.Empty;

        //public string LI_LPCO { get; set; } = string.Empty;
        public List<LiInfo> LI { get; set; } = new List<LiInfo>();
        public List<Capa> Capa { get; set; } = new List<Capa>();

        public string DI { get; set; } = string.Empty;
        public DateTime? DataRegistroDI { get; set; } = (DateTime?)null;
        public bool CheckDataRegistroDI { get; set; } = false;
        public DateTime? DataDesembaracoDI { get; set; } = (DateTime?)null;
        public bool CheckDataDesembaracoDI { get; set; } = false;
        public DateTime? DataCarregamentoDI { get; set; } = (DateTime?)null;
        public bool CheckDataCarregamentoDI { get; set; } = false;
        public DateTime? DataMinutaDI { get; set; } = (DateTime?)null;
        public bool CheckDataMinutaDI { get; set; } = false;
        public string ParametrizacaoDI { get; set; } = string.Empty;


        public DateTime? DataDeAtracacao { get; set; } = (DateTime?)null;
        public bool CheckDataDeAtracacao { get; set; } = false;
        public DateTime? Inspecao { get; set; } = (DateTime?)null;
        public bool CheckInspecao { get; set; } = false;
        public DateTime? DataEmbarque { get; set; } = (DateTime?)null;
        public bool CheckDataEmbarque { get; set; } = false;

        public DateTime? DataRecebOriginais { get; set; } = (DateTime?)null;
        public bool CheckDataRecebOriginais { get; set; } = false;
        public string FormaRecOriginais { get; set; } = string.Empty; // Seletor - DHL, UPS,Correio, Fedex, Daytona
        public string[] DocRecebidos { get; set; } = Array.Empty<string>(); // Seletor Varios -  BL, Fatura, Packing List, CO, Fito, CSI, CA, CF (String Concat)

        public bool Amostra { get; set; } = false;
        public bool Desovado { get; set; } = false;
        public string Pendencia { get; set; } = string.Empty;
        public string StatusDoProcesso { get; set; } = string.Empty;

        public string Origem { get; set; } = string.Empty;


        //Implementar - Calculo Vencimento FreeTime = Data de Atracação + FreeTime
        //Implementar - Calculo Vencimento FMA = Data de Atracação + 85 dias
        //Implementar - Status / Conclusão - Verde, Atracado - Laranja, Esperado - Amarelo, Aguardando Embarque - Branco

    }
}
