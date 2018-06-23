using System;
using StudioServices.Controllers.Utils;
using StudioServices.Data.EntityFramework.Registry;
using System.IO;
using StudioServicesWeb;
using StudioServicesWeb.DataControllers;
using System.Linq;

namespace StudioServices.Controllers.Persons
{
    public class PersonsManager
    {
        // private RegistryDatabaseEF db;
        private DatabaseEF db;
        public PersonsManager(DatabaseEF d)
        {
            db = d;  
        }
        public bool AddPerson(Person persona, out string verify_code, bool enabled = false)
        {
            //TODO: Usabile solo dall'amministratore

            verify_code = StringUtils.RandomString(8);
            persona.AuthCode = verify_code;
            persona.Enabled = enabled;
            var res = db.Save(persona);
            verify_code = res ? verify_code : string.Empty;
            return res;
        }
        public Person GetPerson(int person_id)
        {
            var person = db.Registry_GetPerson(person_id);
            return person;
        }
        public bool ChangeActiveStatus(int person_id, bool state)
        {
            Person p = db.Get<Person>(person_id);
            p.SetAttivo(state);
            return db.UpdateDataFileItem(p);
        }
        // Aggiunge o aggiorna un documento
        public bool AddIdentificationDocument(IdentificationDocument document)
        {
            var file_ext = document.GetFileExtension();
            byte[] file = file_ext.Equals(".pdf", StringComparison.InvariantCultureIgnoreCase) ?
                Convert.FromBase64String(document.FileUpload.FirstOrDefault()) :
                FileUtils.ConvertB64ImagesToPDF(document.FileUpload.ToArray());

            // Crea oggetto documento
            bool isUpdate = document.Id > 0;

            // Salvataggio file e generazione nome
            string filename = string.Format("{0:D6}_{1:D2}_{2}_{3}", document.PersonId, (int)document.Type, StringUtils.RandomString(), document.Filename);
            document.Filename = filename;
            if (FileUtils.WriteFile(StudioServicesConfig.IDENTIFICATIONS_FOLDER, document.PersonId.ToString("D6"), filename, file))
            {
                document.SetAttivo(true);
                if (db.Save(document))
                    return true;
                else
                {
                    if (!isUpdate) // cancella il file solo se è un nuovo inserimento
                        FileUtils.Delete(StudioServicesConfig.IDENTIFICATIONS_FOLDER, filename);
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
            IdentificationDocument doc = db.Registry_GetIdentificationDocument(id_documento, id_persona);
            var list = db.GetList<IdentificationDocument>(id_persona);
            if(doc == null || list.Count <=1)
                return false;
            doc.SetAttivo(false);
            return db.UpdateDataFileItem(doc);
        }
        public bool AddContactNumber(ContactMethod contact)
        {
            contact.SetAttivo(true);
            return db.Save(contact);
        }
        public bool RemoveContactNumber(int id_persona, int id_contatto)
        {
            var contact = db.Get<ContactMethod>(id_contatto);
            if (contact == null || contact.PersonId != id_persona)
                return false;
            contact.SetAttivo(false);
            return db.Save(contact);
        }
        public bool AddAddress(Address address)
        {
            address.SetAttivo(true);
            return db.Save(address);
        }
        public bool RemoveAddress(int id_persona, int id_indirizzo)
        {
            var address = db.Get<Address>(id_indirizzo);
            if (address == null || address.PersonId != id_persona)
                return false;
            address.SetAttivo(false);
            return db.Save(address);
        }

        public bool AddEmail(Email email)
        {
            email.SetAttivo(true);
            return db.Save(email);
        }
        public bool RemoveEmail(int id_persona, int id_email)
        {
            var email = db.Get<Email>(id_email);
            if (email == null || email.PersonId != id_persona)
                return false;
            email.SetAttivo(false);
            return db.Save(email);
        }
    }
}
