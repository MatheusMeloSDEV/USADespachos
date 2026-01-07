using Newtonsoft.Json;

namespace CLUSA.Services
{
    public class AppConfig
    {
        [JsonProperty("mongodb_uri")]
        public string MongoUri { get; set; } = string.Empty;

        [JsonProperty("banco_dados")]
        public string BancoDados { get; set; } = string.Empty;

        [JsonProperty("colecao")]
        public string Colecao { get; set; } = string.Empty;

        [JsonProperty("caminho_logo")]
        public string CaminhoLogo { get; set; } = string.Empty;

        [JsonProperty("caminho_pasta_followup")]
        public string CaminhoPastaFollowUp { get; set; } = string.Empty;

        [JsonProperty("caminho_libreoffice")]
        public string CaminhoLibreOffice { get; set; } = string.Empty;

        [JsonProperty("caminho_perfil_libreoffice")]
        public string CaminhoPerfilLibreOffice { get; set; } = string.Empty;
    }
}