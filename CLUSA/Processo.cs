using Microsoft.Win32;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel;

namespace CLUSA
{
    public class Processo
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        //Não Aparecem
        public List<TipoOrgaoAnuente> OrgaosAnuentesEnvolvidos { get; set; } = new();
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
        public string Veiculo { get; set; } = string.Empty;
        public string PortoDestino { get; set; } = string.Empty;
        public string FLO { get; set; } = string.Empty;
        public int FreeTime { get; set; } = 0;
        public string Terminal { get; set; } = string.Empty;
        public string Conhecimento { get; set; } = string.Empty;
        public string Armador { get; set; } = string.Empty;

        //adicionados
        public string Container { get; set; } = string.Empty;
        public bool PresençaDeCarga { get; set; } = false;
        public bool SIGVIG { get; set; } = false;
        public bool ResultadoLab { get; set; } = false;
        //

        public List<LicencaImportacao> LI { get; set; } = new List<LicencaImportacao>();

        public Capa Capa { get; set; } = new Capa();

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
        public string FormaRecOriginais { get; set; } = string.Empty; // Seletor Vários - DHL, UPS, Correio, Fedex, Daytona, 
        public string[] DocRecebidos { get; set; } = Array.Empty<string>(); // Seletor Varios -  BL, Fatura, Packing List, CO, Fito, CSI, CA, CF (String Concat)

        public bool Amostra { get; set; } = false;
        public bool Desovado { get; set; } = false;
        public string Pendencia { get; set; } = string.Empty;
        public string HistóricoDoProcesso { get; set; } = string.Empty;

        public string Origem { get; set; } = string.Empty;

        public string Status { get; set; } = "Aguardando embarque";

        //Aguardando atracação, Aguardando presença de carga, Aguardando SIGVIG
        //Aguardando LI/LPCO, Aguardando parametrização LI/LPCO, Aguardando inspeção/coleta LI/LPCO
        //Aguardando deferimento LI/LPCO, Aguardando registro DI/DUIMP, Aguardando parametrização DI/DUIMP
        //Aguardando inspeção DI/DUIMP, Aguardando minuta devolução container vazio, Aguardando resultado laboratório, Finalizado
    }
}
