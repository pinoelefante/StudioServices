using System;
namespace StudioServices.Data.Anagrafica
{
    public class Indirizzo : DataFile
    {
        public string Nazione { get; set; }
        public string Citta { get; set; }
        public string Provincia { get; set; }
        public string Indirizzo1 { get; set; }
        public string NumeroCivico { get; set; }

        /* 
            0 - casa
            1 - lavoro
        */
        public int TipoIndirizzo { get; set; }
        public string Descrizione { get; set; }
    }
}
