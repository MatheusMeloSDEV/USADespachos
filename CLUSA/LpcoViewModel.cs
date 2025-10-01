using MongoDB.Bson;
using System;

namespace CLUSA
{
    // Esta classe representa uma linha na sua grade de dados.
    public class LpcoViewModel
    {
        public ObjectId OrgaoAnuenteId { get; set; } // Id da LI pai

        // Dados do Processo / LI
        public string Ref_USA { get; set; } = string.Empty;
        public string Importador { get; set; } = string.Empty;
        public string NumeroLI { get; set; } = string.Empty; 
        public string LPCO { get; set; } = string.Empty;
        public string NomeOrgao { get; set; } = string.Empty;
        public DateTime? DataRegistroLPCO { get; set; }
        public string ParametrizacaoLPCO { get; set; } = string.Empty;
        public string Container { get; set; } = string.Empty;
        public string Origem { get; set; } = string.Empty;
        public string Conhecimento { get; set; } = string.Empty;
        public string Produto { get; set; } = string.Empty;
        public DateTime? Inspecao { get; set; }
        public DateTime? DataChegada { get; set; }
        public string StatusLI { get; set; } = string.Empty;
        public string MotivoExigencia { get; set; } = string.Empty;
        public string Pendencia { get; set; } = string.Empty;
        public string HistoricoDoProcesso { get; set; } = string.Empty;
    }
}