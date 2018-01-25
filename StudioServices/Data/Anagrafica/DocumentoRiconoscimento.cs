using System;
namespace StudioServices.Data.Anagrafica
{
    public class DocumentoRiconoscimento : DataFile
    {
        /*
            0 - carta d'identità
            1 - passaporto
            2 - patente
         */
        public int TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public string Filename { get; set; }
        public DateTime DataRilascio { get; set; }
        public DateTime DataScadenza { get; set; }
    }
}
