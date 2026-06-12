using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CLUSA.Models
{
    [BsonIgnoreExtraElements]
    public class Processo
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        // Propriedades Calculadas
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

        public bool PossuiEmbarque { get; set; } = false;
        public DateTime? VencimentoFreeTime { get; set; }
        public DateTime? VencimentoFMA { get; set; }
        public DateTime? VencimentoLI_LPCO { get; set; }
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

        // Catálogos (lista embutida no documento Processo)
        [MongoDB.Bson.Serialization.Attributes.BsonElement("Catalogos")]
        [MongoDB.Bson.Serialization.Attributes.BsonIgnoreIfNull]
        public List<Catalogo> Catalogos { get; set; } = new List<Catalogo>();

        [BsonElement("catalogo")]
        [System.Obsolete("Use Catalogos (lista) em vez de catalogo singular.")]
        public Catalogo? CatalogoLegacy
        {
            get => (Catalogos != null && Catalogos.Count == 1) ? Catalogos[0] : null;
            set
            {
                if (value != null && (Catalogos == null || Catalogos.Count == 0))
                {
                    Catalogos = new List<Catalogo> { value };
                }
            }
        }
        public bool ShouldSerializeCatalogoLegacy()
        {
            return false;
        }

        public bool RegistroPendente { get; set; } = false;
        public bool RegistroRegistrado { get; set; } = false;
        public string Container { get; set; } = string.Empty;
        public bool PresencaDeCarga { get; set; } = false;
        public bool CapaOK { get; set; } = false;
        public bool SIGVIGLiberado { get; set; } = false;
        public bool SIGVIGSelecionado { get; set; } = false;
        public bool ResultadoLab { get; set; } = false;
        public List<LicencaImportacao> LI { get; set; } = new List<LicencaImportacao>();
        public Capa Capa { get; set; } = new Capa();
        public string LocalDeDesembaraco { get; set; } = string.Empty;
        public string DI { get; set; } = string.Empty;
        public string RascunhoDI { get; set; } = string.Empty;
        public DateTime? DataRegistroDI { get; set; }
        public DateTime? DataDesembaracoDI { get; set; }
        public DateTime? DataCarregamentoDI { get; set; }
        public DateTime? DataMinutaDI { get; set; }
        public string ParametrizacaoDI { get; set; } = string.Empty;
        public DateTime? DataDeAtracacao { get; set; }
        public DateTime? Inspecao { get; set; }
        public DateTime? DataEmbarque { get; set; }
        public DateTime? DataRecebOriginais { get; set; }
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