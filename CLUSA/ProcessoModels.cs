using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CLUSA
{
    public class Processo
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        // Propriedades Calculadas (Não Persistidas)
        [BsonIgnore]
        public string OrgaosAnuentesString
        {
            get
            {
                var orgaos = LI?.SelectMany(li => li.LPCO)
                             .Select(lpco => lpco.NomeOrgao)
                             .Distinct()
                             .ToList();
                return orgaos != null && orgaos.Any() ? string.Join(", ", orgaos) : "-";
            }
        }

        // Vencimentos e Bools
        public bool PossuiEmbarque { get; set; } = false;
        public DateTime? VencimentoFreeTime { get; set; } = (DateTime?)null;
        public DateTime? VencimentoFMA { get; set; } = (DateTime?)null;
        public DateTime? VencimentoLI_LPCO { get; set; } = (DateTime?)null;

        // Dados Principais
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
        public string CE { get; set; } = string.Empty;

        // Dados do Container e Status
        public string Container { get; set; } = string.Empty;
        public bool PresencaDeCarga { get; set; } = false;
        public bool CapaOK { get; set; } = false;
        public bool SIGVIGLiberado { get; set; } = false;
        public bool SIGVIGSelecionado { get; set; } = false;
        public bool ResultadoLab { get; set; } = false;

        // Hierarquia de Licenças e Capa
        public List<LicencaImportacao> LI { get; set; } = new List<LicencaImportacao>();
        public Capa Capa { get; set; } = new Capa(); // Assumindo que Capa é um objeto separado/embeddado

        // Dados de Desembaraço
        public string LocalDeDesembaraco { get; set; } = string.Empty;
        public string DI { get; set; } = string.Empty;
        public string RascunhoDI { get; set; } = string.Empty;
        public DateTime? DataRegistroDI { get; set; } = (DateTime?)null;
        public DateTime? DataDesembaracoDI { get; set; } = (DateTime?)null;
        public DateTime? DataCarregamentoDI { get; set; } = (DateTime?)null;
        public DateTime? DataMinutaDI { get; set; } = (DateTime?)null;
        public string ParametrizacaoDI { get; set; } = string.Empty;

        // Datas de Movimentação
        public DateTime? DataDeAtracacao { get; set; } = (DateTime?)null;
        public DateTime? Inspecao { get; set; } = (DateTime?)null;
        public DateTime? DataEmbarque { get; set; } = (DateTime?)null;

        // Documentação e Origem
        public DateTime? DataRecebOriginais { get; set; } = (DateTime?)null;
        public string FormaRecOriginais { get; set; } = string.Empty;
        public string[] DocRecebidos { get; set; } = Array.Empty<string>();
        public string Origem { get; set; } = string.Empty;

        // Bools de Status
        public bool Amostra { get; set; } = false;
        public bool Desovado { get; set; } = false;
        public bool Redestinacao { get; set; } = false;
        public bool Numerario { get; set; } = false;
        public bool SigVig { get; set; } = false;

        // Histórico e Status
        public string HistoricoDoProcesso { get; set; } = string.Empty;
        public string Pendencia { get; set; } = string.Empty;
        public string Status { get; set; } = "Aguardando embarque";
        public string CondicaoProcesso { get; set; } = "AguardandoCE";
    }
    
    public class LicencaImportacao
    {
        public string Numero { get; set; } = string.Empty;
        public string NCM { get; set; } = string.Empty;
        public DateTime? DataRegistro { get; set; }
        public bool Amostra { get; set; } = false;
        public List<LpcoInfo> LPCO { get; set; } = new();
    }

    public class LpcoInfo
    {
        public string NomeOrgao { get; set; } = string.Empty;
        public string LPCO { get; set; } = string.Empty;
        public DateTime? DataRegistroLPCO { get; set; }
        public DateTime? DataDeferimentoLPCO { get; set; }
        public string ParametrizacaoLPCO { get; set; } = string.Empty;
        public bool EmExigencia { get; set; } = false;
        public string MotivoExigencia { get; set; } = string.Empty;
        public string StatusLPCO { get; set; } = string.Empty;
    }
    public class Capa
    {
        public string Master { get; set; } = string.Empty;
        public string Container { get; set; } = string.Empty;
        public bool SigvigSelecionado { get; set; }
        public bool SigvigLiberado { get; set; }
        public DateTime? SigvigData { get; set; }
        public string Incoterm { get; set; } = string.Empty;
        public string[] Numerario { get; set; } = Array.Empty<string>();
        public string DTA { get; set; } = string.Empty;
        public string Marinha { get; set; } = string.Empty;
        public string CE { get; set; } = string.Empty;
        public string[] Imposto { get; set; } = Array.Empty<string>();


        public bool TelaDoCanal { get; set; }
        public bool Averbar { get; set; }
        public bool LiberarBL { get; set; }
        public bool MarinhaMercante_Isencao { get; set; }
        public bool ICMS_Exoneracao { get; set; }
        public bool Lancado { get; set; }
        public bool ConsultaSEFAZ { get; set; }
        public bool DAT_IIDeferida { get; set; }
        public bool SISCargaLiberado { get; set; }
        public bool DANFE { get; set; }
        public bool Armazenagem { get; set; }
        public bool Faturado { get; set; }
        public bool Pago { get; set; }
        public bool ENTTransporte { get; set; }
        public bool ENTAlfandega { get; set; }
        public bool ConferenciaFisica { get; set; }

        public DateTime? AverbarData { get; set; }
        public DateTime? LiberarBLData { get; set; }
        public DateTime? MarinhaMercante_IsencaoData { get; set; }
        public DateTime? ICMS_ExoneracaoData { get; set; }
        public DateTime? SISCargaLiberadoData { get; set; }
        public string PagoPor { get; set; } = string.Empty;
        public DateTime? ENTTransporteData { get; set; }
        public string ENTTransporteN { get; set; } = string.Empty;
        public DateTime? ENTAlfandegaData { get; set; }
        public string ENTAlfandegaDossie { get; set; } = string.Empty;
        public DateTime? ConferenciaFisicaData { get; set; }

        public string Observacoes { get; set; } = string.Empty;
    }
    
}