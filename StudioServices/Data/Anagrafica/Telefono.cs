using System;
namespace StudioServices.Data.Anagrafica
{
    public class Telefono : DataFile
    {
        public string Numero { get; set; }
        /*
           0 - telefono
           1 - cellulare
           2 - fax
           3 - telegram account
           4 - skype account
         */
        public int TipoTelefono { get; set; }

        public bool IsWhatsApp { get; set; }
        public bool IsTelegram { get; set; }

        public int Priorita { get; set; }
    }
}
