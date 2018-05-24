using System;
using StudioServices.Registry.Data;
using StudioServices.Controllers.Utils;
using StudioServices.Data.Registry;
using System.IO;

namespace StudioServices.Controllers.Persons
{
    public class PersonsManager
    {
        private RegistryDatabase db;
        public PersonsManager()
        {
            db = new RegistryDatabase();
        }
        public bool AddPerson(string name, string surname, string fiscal_code, DateTime birthday, string birth_place, out string verify_code, bool enabled = false)
        {
            // Usabile solo dall'amministratore

            // Verifica che la persona non esista nel database (Verificato con l'utilizzo della clausola Unique di Persona su codice fiscale)
            verify_code = StringUtils.RandomString(8);
            Person persona = new Person()
            {
                Name = name,
                Surname = surname,
                FiscalCode = fiscal_code,
                Birth = birthday,
                BirthPlace = birth_place,
                AuthCode = verify_code,
                Enabled = enabled
            };
            return db.PersonSave(persona);
        }
        public Person GetPerson(int person_id)
        {
            return db.PersonSelect(person_id);
        }
        public bool ChangeActiveStatus(int person_id, bool state)
        {
            Person p = GetPerson(person_id);
            p.SetAttivo(state);
            return db.SaveItem(p);
        }
        // Aggiunge o aggiorna un documento
        public bool AddIdentificationDocument(int person_id, int document_type, string number, DateTime issue_date, DateTime expire_date, byte[] file = null, string file_ext = "jpg")
        {
            // Crea oggetto documento
            var timestamp = DateTime.Now;
            IdentificationDocument document = db.IdentificationDocumentSelect(number, (DocumentType)Enum.ToObject(typeof(DocumentType), document_type), person_id) ?? new IdentificationDocument();
            document.Issue = issue_date;
            document.Expire = expire_date;
            document.Number = number;
            document.Type = (DocumentType)Enum.ToObject(typeof(DocumentType), document_type);
            document.PersonId = person_id;
            document.SetAttivo(true);
            bool isUpdate = document.CreationTime.CompareTo(timestamp) < 0;

            // Salvataggio file e generazione nome
            string filename = document.Filename ?? String.Format("{0:D4}_{1:D2}_{2}", person_id, document_type, StringUtils.RandomString(), file_ext);
            document.Filename = filename;
            if (FileUtils.WriteFile("", filename, file))
            {
                if (db.SaveItem(document))
                    return true;
                else
                {
                    if (!isUpdate) // cancella il file solo se è un nuovo inserimento
                        FileUtils.Delete("", filename);
                    return false;
                }
            }
            else
            {
                // Impossibile salvare il file
                return false;
            }
        }
        public bool RemoveIdentificationDocument(int id_persona, int id_documento)
        {
            IdentificationDocument doc = db.IdentificationDocumentSelect(id_documento, id_persona);
            var list = db.IdentificationDocumentList(id_persona);
            if(doc == null || list.Count <=1)
            {
                return false;
            }

            doc.SetAttivo(false);
            return db.SaveItem(doc);
        }
        public bool AddContactNumber(int id_persona, int tipo_contatto, string numero, bool whatsapp = false, bool telegram = false, int priorita = 0)
        {
            ContactMethod contact = db.ContactMethodSelect(numero, (ContactType)Enum.ToObject(typeof(ContactType),tipo_contatto), id_persona) ?? new ContactMethod();
            contact.IsTelegram = telegram;
            contact.IsWhatsApp = whatsapp;
            contact.Number = numero;
            contact.Type = (ContactType)Enum.ToObject(typeof(ContactType), tipo_contatto);
            contact.Priority = priorita;
            contact.PersonId = id_persona;
            contact.SetAttivo(true);

            return db.SaveItem(contact, true);
        }
        public bool RemoveContactNumber(int id_persona, int id_contatto)
        {
            var contact = db.ContactMethodSelect(id_contatto, id_persona);
            if (contact != null)
                return false;
            contact.SetAttivo(false);
            return db.SaveItem(contact);
        }
        public bool AddAddress(int id_persona, int tipo_indirizzo, string nazione, string citta, string provincia, string indirizzo, string numero, string cap, string descrizione = null)
        {
            var address = db.AddressSelect(id_persona, indirizzo, numero, citta, provincia, nazione) ?? new Address();
            address.AddressType = (AddressType)Enum.ToObject(typeof(AddressType), tipo_indirizzo);
            address.City = citta;
            address.CivicNumber = numero;
            address.Country = nazione;
            address.Description = descrizione;
            address.PersonId = id_persona;
            address.Province = provincia;
            address.Street = indirizzo;
            address.ZipCode = cap;
            address.SetAttivo(true);

            return db.SaveItem(address);
        }
        public bool RemoveAddress(int id_persona, int id_indirizzo)
        {
            var address = db.AddressSelect(id_indirizzo, id_persona);
            if (address == null)
                return false;
            address.SetAttivo(false);
            return db.SaveItem(address);
        }

        public bool AddEmail(int id_persona, string casella, bool is_pec = false, bool is_gestita = false, string password = null, string nome_visualizzato = null, string imap_address = null, int imap_port = 0, string imap_username = null, string smtp_address = null, int smtp_port = 0, string smtp_username = null, string service_username = null, string service_password = null, DateTime scadenza = default(DateTime), bool rinnovo_automatico = false, string rinnovo_paypal = null)
        {
            var email = db.EmailSelect(casella, id_persona) ?? new Email();
            email.Address = casella;
            email.IsPec = is_pec;
            email.IsManaged = is_gestita;
            if (is_gestita)
            {
                email.Password = password;
                email.FullName = nome_visualizzato;
                email.IMAPAddress = imap_address;
                email.IMAPUsername = imap_username;
                email.IMAPPort = imap_port;
                email.SMTPAddress = smtp_address;
                email.SMTPUsername = smtp_username;
                email.SMTPPort = smtp_port;
                email.ServiceUsername = service_username;
                email.ServicePassword = service_password;
                email.AutoRenewEnabled = rinnovo_automatico;
                email.AutoRenewPaypalAddress = rinnovo_paypal;
                email.Expire = scadenza;
            }
            email.SetAttivo(true);
            return db.SaveItem(email);
        }
        public bool RemoveEmail(int id_persona, int id_email)
        {
            var email = db.EmailSelect(id_email, id_persona);
            if (email == null)
                return false;
            email.SetAttivo(false);
            return db.SaveItem(email);
        }
    }
}
