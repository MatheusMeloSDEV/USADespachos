using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using CLUSA.Interfaces;

namespace CLUSA.Models
{
    public enum TipoOrgaoAnuente { MAPA, ANVISA, DECEX, IBAMA, INMETRO }

    [BsonIgnoreExtraElements]
    public class OrgaoAnuente : IEntidadeBase
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
        public string Pendencia { get; set; } = string.Empty;
        public string HistoricoDoProcesso { get; set; } = string.Empty;

        public OrgaoAnuente() { }
    }
    public class LpcoViewModel
    {
        // ID para identificar o registro original no banco ao clicar em Editar
        public object OrgaoAnuenteId { get; set; }

        // Dados Gerais (vindos da LI/Processo)
        public string Ref_USA { get; set; }
        public string Importador { get; set; }
        public string NumeroLI { get; set; }
        public string Produto { get; set; }
        public string Container { get; set; }
        public string Terminal { get; set; }
        public string Conhecimento { get; set; }
        public string Origem { get; set; }

        // Datas
        public DateTime? DataChegada { get; set; }
        public DateTime? Inspecao { get; set; }

        // Status e Controle
        public string HistoricoDoProcesso { get; set; }
        public string Pendencia { get; set; }

        // Dados Específicos do LPCO (da sublista de LPCOs)
        public string LPCO { get; set; } // Número do LPCO
        public string NomeOrgao { get; set; }
        public string StatusLPCO { get; set; }
        public string MotivoExigencia { get; set; }

        // Datas específicas do LPCO
        public DateTime? DataRegistroLPCO { get; set; }
        public string ParametrizacaoLPCO { get; set; }
    }
}
