using System;
using System.Collections.Generic;
using StudioServices.Data;
using StudioServices.Data.Anagrafica;

namespace StudioServices.Anagrafica.Data
{
    public class Persona : DataFile
    {
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public string CodiceFiscale { get; set; }
        public string CodiceVerifica { get; set; }
        public DateTime DataNascita { get; set; }
        public string LuogoNascita { get; set; }
        public List<Email> Emails { get; } = new List<Email>(1);
        public List<Indirizzo> Indirizzi { get; } = new List<Indirizzo>(1);
        public List<Telefono> Contatti { get; } = new List<Telefono>(2);
        public List<DocumentoRiconoscimento> Documenti { get; } = new List<DocumentoRiconoscimento>(1);

        public void Aggiungi(DataFile nuovo)
        {
            if(nuovo.Id > 0)
                throw new ArgumentException("Non è possibile aggiungere un oggetto con Id maggiore di zero");

            if (nuovo is Email)
                Emails.Add(nuovo as Email);
            else if (nuovo is Indirizzo)
                Indirizzi.Add(nuovo as Indirizzo);
            else if (nuovo is Telefono)
                Contatti.Add(nuovo as Telefono);
            else if (nuovo is DocumentoRiconoscimento)
                Documenti.Add(nuovo as DocumentoRiconoscimento);
            else
                throw new ArgumentException("Tipo di oggetto non supportato");
        }
    }
}
