using System;
using StudioServices.Anagrafica.Data;
using StudioServices.Data.Anagrafica;

namespace StudioServices.Controllers.Persone
{
    public class PersoneController
    {
        public PersoneController()
        {
            /* 
                Dipendenze da aggiungere:
                    - Manager sessione
                    - Manager cache
                    - Database
            */    
        }
        public bool AggiungiPersona(string nome, string cognome, string codice_fiscale, DateTime data_nascita, string luogo_nascita)
        {
            // Usabile solo dall'amministratore

            // Verifica che la persona non esista nel database

            Persona persona = new Persona()
            {
                Nome = nome,
                Cognome = cognome,
                CodiceFiscale = codice_fiscale,
                DataNascita = data_nascita,
                LuogoNascita = luogo_nascita
            };

            throw new NotImplementedException();
        }
        public bool DisabilitaPersona(int id_persona)
        {
            throw new NotImplementedException();
        }
        public bool AbilitaPersona(int id_persona)
        {
            throw new NotImplementedException();
        }
        public bool AggiungiDocumento(int id_persona, int tipo_documento, string numero, DateTime rilascio, DateTime scadenza, byte[] file = null)
        {
            // Recupera persona dalla cache o dal database

            // Verifica l'esistenza di un documento uguale

            // Salvataggio file e generazione nome

            // Aggiunta documento a persona

            throw new NotImplementedException();
        }
        public bool RimuoviDocumento(int id_persona, int id_documento)
        {
            // Disabilita documento

            throw new NotImplementedException();
        }
        public bool AggiungiContatto(int id_persona, int tipo_contatto, string numero, bool whatsapp = false, bool telegram = false, int priorita = 0)
        {
            // Recupera oggetto Persona dalla cache

            // Verifica l'esistenza di un contatto con lo stesso numero e lo stesso tipo

            // Se non esiste, creare il contatto

            throw new NotImplementedException();
        }
        public bool RimuoviContatto(int id_persona, int id_contatto)
        {
            throw new NotImplementedException();
        }
        public bool AggiungiIndirizzo(int id_persona, int tipo_indirizzo, string nazione, string citta, string provincia, string indirizzo, int numero, string descrizione)
        {
            // Recuperare oggetto Persona

            // Verificare che l'indirizzo non sia presente

            // Aggiungere l'indirizzo

            throw new NotImplementedException();
        }
        public bool RimuoviIndirizzo(int id_persona, int id_indirizzo)
        {
            throw new NotImplementedException();
        }

        public bool AggiungiEmail(int id_persona, string casella, bool is_pec = false, bool is_gestita = false, string password = null, string nome_visualizzato = null, string imap_address = null, int imap_port = 0, string imap_username = null, string smtp_address = null, int smtp_port = 0, string smtp_username = null, string service_username = null, string service_password = null, DateTime? scadenza = null, bool rinnovo_automatico = false, string rinnovo_paypal = null)
        {
            throw new NotImplementedException();
        }
        public bool RimuoviEmail(int id_persona, int id_email)
        {
            throw new NotImplementedException();
        }
    }
}
