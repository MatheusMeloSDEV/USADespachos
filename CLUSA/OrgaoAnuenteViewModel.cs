using MongoDB.Bson;
using System;

namespace CLUSA
{
    // Esta classe representa uma linha na sua grade de dados.
    public class OrgaoAnuenteViewModel
    {
        // Propriedades para identificar o documento original
        public ObjectId OrgaoAnuenteId { get; set; }

        // Propriedades do OrgaoAnuente
        public TipoOrgaoAnuente Tipo { get; set; }
        public string Ref_USA { get; set; } = string.Empty;
        public string Pendencia { get; set; } = string.Empty;
        public string StatusDoProcesso { get; set; } = string.Empty;
        public DateTime? Inspecao { get; set; }

        // Propriedade que vem do Processo relacionado
        public string Importador { get; set; } = string.Empty;
    }
}