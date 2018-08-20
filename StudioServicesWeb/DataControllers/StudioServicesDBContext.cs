using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using StudioServices.Data.EntityFramework;
using StudioServices.Data.EntityFramework.Accounting;
using StudioServices.Data.EntityFramework.Items;
using StudioServices.Data.EntityFramework.Newsboard;
using StudioServices.Data.EntityFramework.Payment;
using StudioServices.Data.EntityFramework.Registry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudioServicesWeb.DataControllers
{
    public class StudioServicesDBContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<ContactMethod> ContactMethods { get; set; }
        public DbSet<Email> Emails { get; set; }
        public DbSet<IdentificationDocument> IdentificationDocuments { get; set; }
        public DbSet<Person> Persons { get; set; }

        public DbSet<Company> Companies { get; set; }
        public DbSet<CompanyProduct> CompanyProducts { get; set; }

        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceDetail> InvoiceDetails { get; set; }

        public DbSet<ItemRequest> ItemRequests { get; set; }
        public DbSet<PayableItem> PayableItems { get; set; }

        public DbSet<Message> Messages { get; set; }
        public DbSet<ReadStatus> ReadStatuses { get; set; }

        public DbSet<PaymentHistory> PaymentHistories { get; set; }

        public static DbContextOptions CtxOptions;

        public StudioServicesDBContext() : base(CtxOptions)
        {
            this.Database.EnsureCreated();
            InitDbDictionary();
        }
        /*
        public StudioServicesDBContext(DbContextOptions options) : base(options)
        {
            this.Database.EnsureCreated();
            InitDbDictionary();
        }
        */
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ReadStatus>().HasKey(x => new { x.PersonId, x.MessageId } );

            // modelBuilder.Entity<InvoiceDetail>().HasOne<Invoice>(x => x.Invoice).WithOne().HasForeignKey<InvoiceDetail>(x => x.InvoiceId);
            // modelBuilder.Entity<InvoiceDetail>().HasOne<CompanyProduct>(x => x.Product).WithOne().HasForeignKey<InvoiceDetail>(x => x.ProductId);
            // modelBuilder.Entity<InvoiceDetail>().Ignore(x => x.Invoice).Ignore(x => x.Product);

            modelBuilder.Entity<CompanyProduct>().HasIndex((x) => new { x.CompanyId, x.ProductCode }).IsUnique(true);
            // modelBuilder.Entity<CompanyProduct>().HasOne<Company>(x => x.Company).WithOne().HasForeignKey<CompanyProduct>(x => x.CompanyId);
        }

        private Dictionary<Type, object> @switch = new Dictionary<Type, object>();
        public DbSet<T> GetDbSet<T>() where T : DataFile
        {
            if (@switch.ContainsKey(typeof(T)))
                return (DbSet<T>)@switch[typeof(T)];
            return null;
        }
        private void InitDbDictionary()
        {
            @switch.Add(typeof(Account), Accounts);
            @switch.Add(typeof(Address), Addresses);
            @switch.Add(typeof(ContactMethod), ContactMethods);
            @switch.Add(typeof(Email), Emails);
            @switch.Add(typeof(IdentificationDocument), IdentificationDocuments);
            @switch.Add(typeof(Person), Persons);
            @switch.Add(typeof(Company), Companies);
            @switch.Add(typeof(CompanyProduct), CompanyProducts);
            @switch.Add(typeof(Invoice), Invoices);
            @switch.Add(typeof(InvoiceDetail), InvoiceDetails);
            @switch.Add(typeof(ItemRequest), ItemRequests);
            @switch.Add(typeof(PayableItem), PayableItems);
            @switch.Add(typeof(Message), Messages);
            @switch.Add(typeof(ReadStatus), ReadStatuses);
            @switch.Add(typeof(PaymentHistory), PaymentHistories);
        }
        public bool InsertDataFileItem<T>(T item) where T : DataFile
        {
            var db = GetDbSet<T>();
            if (db == null)
                return false;
            db.Add(item);
            return SaveChanges() > 0;
        }
        public bool UpdateDataFileItem<T>(T item) where T : DataFile
        {
            var db = GetDbSet<T>();
            if (db == null)
                return false;
            db.Update(item);
            return SaveChanges() > 0;
        }
        public bool SaveDataFileItems<T>(IEnumerable<T> items) where T : DataFile
        {
            var db = GetDbSet<T>();
            if (db == null)
                return false;
            db.AddRange(items);
            return SaveChanges() > 0;
        }
        public T GetByPK<T>(int pk, bool onlyEnabled = true) where T : DataFile
        {
            var db = GetDbSet<T>();
            if (db == null)
                return null;
            var query = from item in db
                        where item.Id == pk && (onlyEnabled ? item.Enabled == true : true)
                        select item;
            return query.FirstOrDefault();
        }
        public List<T> GetListWithPersonId<T>(int person_id, bool onlyEnabled = true) where T : PersonReference
        {
            var db = GetDbSet<T>();
            if (db == null)
                return null;
            var doc_q = from id in db
                        where id.PersonId == person_id && (onlyEnabled ? id.Enabled == true : true)
                        select id;
            return doc_q.ToList();
        }
        public List<T> GetAll<T>(bool onlyEnabled = true) where T : DataFile
        {
            var db = GetDbSet<T>();
            if (db == null)
                return null;
            var query = from items in db
                        where (onlyEnabled ? items.Enabled == true : true)
                        select items;
            return query.ToList();
        }
        public bool DeleteByPK<T>(int pk) where T : DataFile
        {
            var db = GetDbSet<T>();
            if (db == null)
                return false;
            var item = GetByPK<T>(pk);
            if (item == null)
                return true; // it means already deleted
            db.Remove(item);
            return SaveChanges() > 0;
        }
    }
}
