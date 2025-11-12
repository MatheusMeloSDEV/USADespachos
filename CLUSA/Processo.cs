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
        [BsonIgnore] // Não salva esta propriedade no banco
        public string OrgaosAnuentesString
        {
            get
            {
                // Pega os nomes dos órgãos da sua lista de LIs/LPCOs
                var orgaos = LI?.SelectMany(li => li.LPCO)
                                 .Select(lpco => lpco.NomeOrgao)
                                 .Distinct()
                                 .ToList();

                // Se não houver órgãos, retorna um traço. Se houver, junta os nomes com ", ".
                return orgaos != null && orgaos.Any() ? string.Join(", ", orgaos) : "-";
            }
        }
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
        public bool PresencaDeCarga { get; set; } = false;
        public bool CapaOK { get; set; } = false;
        public bool SIGVIGLiberado { get; set; } = false;
        public bool SIGVIGSelecionado { get; set; } = false;
        public bool ResultadoLab { get; set; } = false;
        //

        public List<LicencaImportacao> LI { get; set; } = new List<LicencaImportacao>();

        public Capa Capa { get; set; } = new Capa();

        public string DI { get; set; } = string.Empty;
        public string RascunhoDI { get; set; } = string.Empty;
        public DateTime? DataRegistroDI { get; set; } = (DateTime?)null;
        public DateTime? DataDesembaracoDI { get; set; } = (DateTime?)null;
        public DateTime? DataCarregamentoDI { get; set; } = (DateTime?)null;
        public DateTime? DataMinutaDI { get; set; } = (DateTime?)null;
        public string ParametrizacaoDI { get; set; } = string.Empty;


        public DateTime? DataDeAtracacao { get; set; } = (DateTime?)null;
        public DateTime? Inspecao { get; set; } = (DateTime?)null;
        public DateTime? DataEmbarque { get; set; } = (DateTime?)null;

        public DateTime? DataRecebOriginais { get; set; } = (DateTime?)null;
        public string FormaRecOriginais { get; set; } = string.Empty;
        public string[] DocRecebidos { get; set; } = Array.Empty<string>();
        public string Origem { get; set; } = string.Empty;

        public bool Amostra { get; set; } = false;
        public bool Desovado { get; set; } = false;
        public bool Redestinacao { get; set; } = false;
        public bool Numerario { get; set; } = false;
        public bool SigVig { get; set; } = false;

        public string HistoricoDoProcesso { get; set; } = string.Empty;
        public string Pendencia { get; set; } = string.Empty;
        
        public string Status { get; set; } = "Aguardando embarque";
        public string CondicaoProcesso { get; set; } = "AguardandoCE";
    }
}
