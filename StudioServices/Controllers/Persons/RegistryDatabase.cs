using StudioServices.Controllers.Utils;
using StudioServices.Data.Registry;
using StudioServices.Registry.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioServices.Controllers.Persons
{
    public class RegistryDatabase : Database
    {
        public bool PersonSave(Person persona, bool aggiorna = true)
        {
            using (var connection = GetConnection())
            {
                if (SaveItem(persona))
                {
                    persona.Contacts.ForEach((x) => x.PersonId = persona.Id);
                    persona.Identifications.ForEach((x) => x.PersonId = persona.Id);
                    persona.Emails.ForEach((x) => x.PersonId = persona.Id);
                    persona.Addresses.ForEach((x) => x.PersonId = persona.Id);
                    bool contatti = persona.Contacts.Count > 0 ? SaveAll(persona.Contacts, aggiorna) : true;
                    bool documenti = persona.Identifications.Count > 0 ? SaveAll(persona.Identifications, aggiorna) : true;
                    bool emails = persona.Emails.Count > 0 ? SaveAll(persona.Emails, aggiorna) : true;
                    bool indirizzi = persona.Addresses.Count > 0 ? SaveAll(persona.Addresses, aggiorna) : true;
                    return (contatti || documenti || emails || indirizzi);
                }
                else
                    return false;
            }
        }
        public Person PersonSelect(int id)
        {
            Person persona = null;
            using (var connection = GetConnection())
            {
                persona = connection.Get<Person>(id);
                if (persona != null)
                {
                    connection.Table<ContactMethod>().Where(x => x.PersonId == id && x.Enabled).ToList().ForEach(x => persona.Add(x));
                    connection.Table<IdentificationDocument>().Where(x => x.PersonId == id && x.Enabled).ToList().ForEach(x => persona.Add(x));
                    connection.Table<Email>().Where(x => x.PersonId == id && x.Enabled).ToList().ForEach(x => persona.Add(x));
                    connection.Table<Address>().Where(x => x.PersonId == id && x.Enabled).ToList().ForEach(x => persona.Add(x));
                }
            }
            return persona;
        }
        public List<Person> SelectNewPersons()
        {
            using (var con = GetConnection())
            {
                return con.Table<Person>().Where(x => !x.Enabled && x.DisabledTime.Ticks == 0).ToList();
            }
        }
        public bool IdentificationDocumentExists(int persona, string numero, DocumentType tipo)
        {
            using (var con = GetConnection())
            {
                var numero_lower = numero.ToLower();
                return con.Table<IdentificationDocument>().Where(x => x.PersonId == persona && x.Type == tipo && x.Number.ToLower().Equals(numero_lower)).Any();
            }
        }
        public int VerifyPersonCode(string fiscal_code, string verify_code)
        {
            // Ritorna 0 in caso di codice errato
            // Ritorna -1 in caso in cui il codice sia NULL (persona già associata)
            // Ritorna -2 in caso il codice fiscale non sia presente
            var fc = fiscal_code.ToUpper();
            using (var con = GetConnection())
            {
                Person person = con.Table<Person>().Where(x => x.FiscalCode.Equals(fc)).FirstOrDefault();
                if (person == null)
                    return -2;
                if (person.AuthCode == null)
                    return -1;
                if (person.AuthCode.CompareTo(verify_code) == 0)
                    return person.Id;
                else
                    return 0;
            }
        }
        public Account SelectAccountByUsername(string username)
        {
            using (var con = GetConnection())
            {
                return con.Table<Account>().Where(x => x.Username.Equals(username)).FirstOrDefault();
            }
        }
        public Account SelectAccount(int account_id)
        {
            using (var con = GetConnection())
            {
                return con.Get<Account>(account_id);
            }
        }
        public List<Account> SelectNewAccounts()
        {
            using (var con = GetConnection())
            {
                return con.Table<Account>().Where(x => !x.Enabled && x.DisabledTime.Ticks == 0).ToList();
            }
        }
        public IdentificationDocument IdentificationDocumentSelect(int doc_id, int person_id)
        {
            var doc = IdentificationDocumentSelect(doc_id);
            if (doc != null && doc.PersonId != person_id)
                return null;
            return doc;
        }
        public IdentificationDocument IdentificationDocumentSelect(int doc_id)
        {
            using (var con = GetConnection())
            {
                return con.Get<IdentificationDocument>(doc_id);
            }
        }
        public IdentificationDocument IdentificationDocumentSelect(string number, DocumentType type, int person_id)
        {
            using (var con = GetConnection())
            {
                return con.Table<IdentificationDocument>().Where(x => x.Number.Equals(number) && x.Type == type && x.PersonId == person_id).FirstOrDefault();
            }
        }
        public IEnumerable<IdentificationDocument> IdentificationDocumentList(int id_person, bool all = false)
        {
            using (var con = GetConnection())
            {
                return con.Table<IdentificationDocument>().Where(x => x.PersonId == id_person && (all ? true : x.Enabled)).AsEnumerable();
            }
        }
        public ContactMethod ContactMethodSelect(int contact_id)
        {
            using (var con = GetConnection())
            {
                return con.Get<ContactMethod>(contact_id);
            }
        }
        public ContactMethod ContactMethodSelect(int contact_id, int person_id)
        {
            ContactMethod contact = ContactMethodSelect(contact_id);
            if (contact != null && contact.PersonId != person_id)
                return null;
            return contact;
        }
        public ContactMethod ContactMethodSelect(string number, ContactType type, int person_id)
        {
            using (var con = GetConnection())
            {
                return con.Table<ContactMethod>().Where(x => x.Number.Equals(number) && x.Type == type && x.PersonId == person_id).FirstOrDefault();
            }
        }
        public IEnumerable<ContactMethod> ContactMethodList(int id_person, bool all = false)
        {
            using (var con = GetConnection())
            {
                return con.Table<ContactMethod>().Where(x => x.PersonId == id_person && (all ? true : x.Enabled)).AsEnumerable();
            }
        }
        public Address AddressSelect(int id_address)
        {
            using (var con = GetConnection())
            {
                return con.Get<Address>(id_address);
            }
        }
        public Address AddressSelect(int id_address, int person_id)
        {
            var address = AddressSelect(id_address);
            if (address != null && address.PersonId != person_id)
                return null;
            return address;
        }
        public Address AddressSelect(int person_id, string street, string street_no, string city, string province, string country)
        {
            var addresses = AddressList(person_id);
            if (addresses == null || !addresses.Any())
                return null;
            return addresses.FirstOrDefault(x => x.Street.Equals(street) && x.CivicNumber.Equals(street_no) && x.City.Equals(city) && x.Province.Equals(province) && x.Country.Equals(country));
        }
        public IEnumerable<Address> AddressList(int person_id, bool all = false)
        {
            using (var con = GetConnection())
            {
                return con.Table<Address>().Where(x => x.PersonId == person_id && (all ? true : x.Enabled)).AsEnumerable();
            }
        }
        public Email EmailSelect(int email_id)
        {
            using (var con = GetConnection())
            {
                return con.Get<Email>(email_id);
            }
        }
        public Email EmailSelect(int email_id, int person_id)
        {
            var email = EmailSelect(email_id);
            if (email != null && email.PersonId != person_id)
                return null;
            return email;
        }
        public Email EmailSelect(string address, int person_id)
        {
            var emails = EmailList(person_id);
            if (emails == null || !emails.Any())
                return null;
            return emails.FirstOrDefault(x => x.Address.Equals(address));
        }
        public IEnumerable<Email> EmailList(int person_id, bool all = false)
        {
            using (var con = GetConnection())
            {
                return con.Table<Email>().Where(x => x.PersonId == person_id && (all ? true : x.Enabled)).AsEnumerable();
            }
        }
    }
}
