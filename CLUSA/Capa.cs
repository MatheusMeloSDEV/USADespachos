namespace CLUSA
{
    public class Capa
    {
        public string Master { get; set; }
        public string Container { get; set; }
        public string Sigvig { get; set; }
        public DateTime? SigvigData { get; set; }
        public string Incoterm { get; set; }
        public string Numerario { get; set; }
        public string DTA { get; set; }
        public string Marinha { get; set; }
        public string CE { get; set; }
        public string[] Imposto { get; set; }

        // Bools
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

        // Datas
        public DateTime? AverbarData { get; set; }
        public DateTime? LiberarBLData { get; set; }
        public DateTime? MarinhaMercante_IsencaoData { get; set; }
        public DateTime? ICMS_ExoneracaoData { get; set; }
        public DateTime? SISCargaLiberadoData { get; set; }
        public string PagoPor { get; set; }
        public DateTime? ENTTransporteData { get; set; }
        public string ENTTransporteN { get; set; }
        public DateTime? ENTAlfandegaData { get; set; }
        public string ENTAlfandegaDossie { get; set; }
        public DateTime? ConferenciaFisicaData { get; set; }

        public string Observacoes { get; set; }
    }
}