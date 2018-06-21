using Microsoft.EntityFrameworkCore;
using StudioServices.Data.EntityFramework;
using StudioServices.Data.EntityFramework.Accounting;
using StudioServices.Data.EntityFramework.Items;
using StudioServices.Data.EntityFramework.Newsboard;
using StudioServices.Data.EntityFramework.Registry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudioServicesWeb.DataControllers
{
    public class DatabaseEF
    {
        private StudioServicesDBContext db;

        public DatabaseEF(StudioServicesDBContext ctx)
        {
            db = ctx;
        }
        #region Generic Operations
        public T Get<T>(int id, bool onlyEnabled = true) where T : DataFile
        {
            return db.GetByPK<T>(id, onlyEnabled);
        }
        public List<T> GetList<T>(int person_id, bool onlyEnabled = true) where T : PersonReference
        {
            return db.GetListWithPersonId<T>(person_id, onlyEnabled);
        }
        public List<T> GetAll<T>(bool only_enable = true) where T : DataFile
        {
            return db.GetAll<T>(only_enable);
        }
        public bool Delete<T>(int id) where T : DataFile
        {
            return db.DeleteByPK<T>(id);
        }
        public bool Insert<T>(T item) where T : DataFile
        {
            return db.InsertDataFileItem<T>(item);
        }
        public bool InsertAll<T>(IEnumerable<T> items) where T : DataFile
        {
            return db.SaveDataFileItems(items);
        }

        public bool UpdateDataFileItem<T>(T item) where T : DataFile
        {
            return db.UpdateDataFileItem(item);
        }
        public bool Save<T>(T item) where T : DataFile
        {
            if (item.Id > 0)
                return db.UpdateDataFileItem(item);
            else
                return db.InsertDataFileItem(item);
        }
        public bool SaveObj(object obj)
        {
            db.Add(obj);
            return db.SaveChanges() > 0;
        }
        #endregion

        #region Authentication
        public Tuple<int, Person> Auth_VerifyPersonCode(string fiscal_code, string auth_code)
        {
            var person = db.Persons.Where(x => x.FiscalCode.Equals(fiscal_code)).FirstOrDefault();

            if (person == null)
                return new Tuple<int, Person>(-2, null);
            if (person.AuthCode == null || string.IsNullOrEmpty(auth_code))
                return new Tuple<int, Person>(-1, null);
            if (person.AuthCode.CompareTo(auth_code) == 0)
                return new Tuple<int, Person>(1, person);
            return new Tuple<int, Person>(0, null);
        }
        public Account Auth_GetAccountByUsername(string username)
        {
            var account_q = from acc in db.Accounts
                            where acc.Username.Equals(username, StringComparison.InvariantCultureIgnoreCase)
                            select acc;

            return account_q.FirstOrDefault();
        }
        #endregion

        #region Registry
        public Person Registry_GetPerson(int id)
        {
            var person = db.Persons.Include((p) => p.Emails)
                .Include((p) => p.Addresses)
                .Include((p) => p.Contacts)
                .Include((p) => p.Identifications)
                .Where((p) => p.Id == id)
                .FirstOrDefault();
            if (person != null)
                person.Addresses = person.Addresses.Where(x => x.AddressType == AddressType.HOME).ToList();
            return person;
        }
        public IdentificationDocument Registry_GetIdentificationDocument(int id_documento, int id_persona)
        {
            var doc_q = from id in db.IdentificationDocuments
                        where id.Id == id_documento && id.PersonId == id_persona
                        select id;
            return doc_q.FirstOrDefault();
        }
        #endregion

        #region Warehouse
        public bool Warehouse_IsProductCodeExists(string code, int companyId)
        {
            var query = from pc in db.CompanyProducts
                        where pc.CompanyId == companyId && pc.ProductCode.Equals(code, StringComparison.InvariantCultureIgnoreCase)
                        select pc;
            return query.FirstOrDefault() != null;
        }

        public List<Invoice> Warehouse_GetInvoices(int company, int? year)
        {
            var query = from invoices in db.Invoices
                        where invoices.SenderId == company && (year != null ? invoices.Emission.Year == year : true)
                        orderby invoices.Number descending, invoices.NumberExtra descending
                        select invoices;
            return query.ToList();
        }
        public List<Company> Warehouse_GetClientSupplierList(int person_id)
        {
            var query = from company in db.Companies
                        where company.PersonId == person_id && company.IsClient
                        orderby company.Name
                        select company;
            return query.ToList();
        }
        #endregion

        #region Message
        public List<Message> Message_GetPublicMessages(int person_id, DateTime last_message_date)
        {
            var query = from msg in db.Messages
            where !msg.IsPrivate && msg.CreationTime.CompareTo(last_message_date) > 0
            orderby msg.CreationTime descending
            select msg;

            return query.ToList();
        }

        public ReadStatus Message_GetReadStatus(int message_id, int person_id)
        {
            var query = from msg_s in db.ReadStatuses
                        where msg_s.MessageId == message_id && msg_s.PersonId == person_id
                        select msg_s;
            return query.FirstOrDefault();
        }
        #endregion
    }
}
