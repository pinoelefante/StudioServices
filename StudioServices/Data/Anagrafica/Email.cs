using System;
namespace StudioServices.Data.Anagrafica
{
    public class Email : DataFile
    {
        /* Settaggi */
        public bool IsPec { get; set; }
        public bool IsGestita { get; set; } // se gestito dallo studio

        /* Email */
        public string Indirizzo { get; set; }
        public string Password { get; set; }

        /* Tipo account aruba */
        public string ServiceUsername { get; set; }
        public string ServicePassword { get; set; }

        /* Servizi per caselle email a pagamento */
        public DateTime Scadenza { get; set; }
        public bool RinnovoAutomaticoAttivo { get; set; }
        public string RinnovoAutomaticoPaypal { get; set; }

        /* Settaggi server posta */
        public string IMAPAddress { get; set; }
        public int IMAPPort { get; set; }
        public string IMAPUsername { get; set; }

        public string SMTPAddress { get; set; }
        public int SMTPPort { get; set; }
        public string SMTPUsername { get; set; }

        public string NomeVisualizzato { get; set; }
    }
}
