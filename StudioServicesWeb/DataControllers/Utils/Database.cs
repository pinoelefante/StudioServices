using SQLite;
using StudioServices.Registry.Data;
using StudioServices.Data;
using StudioServices.Data.Registry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudioServices.Data.Newsboard;
using StudioServices.Data.Items;
using StudioServices.Data.Payment;
using StudioServices.Data.Accounting;
using SQLiteNetExtensions.Extensions;

namespace StudioServices.Controllers.Utils
{
    public class Database
    {
        public Database()
        {
            CreateDatabase();
        }
        private void CreateDatabase()
        {
            using (var connection = GetConnection())
            {
                // connection.Execute("PRAGMA foreign_keys = ON");

                /* Accounting */
                connection.CreateTable<Company>();
                connection.CreateTable<CompanyProduct>();
                connection.CreateTable<Invoice>();
                connection.CreateTable<InvoiceDetail>();
                
                /* Items */
                connection.CreateTable<PayableItem>();
                connection.CreateTable<ItemRequest>();

                /* Newsboard */
                connection.CreateTable<Message>();
                connection.CreateTable<ReadStatus>();

                /* Payment */
                connection.CreateTable<PaymentHistory>();

                /* Registry */
                connection.CreateTable(typeof(IdentificationDocument));
                connection.CreateTable(typeof(Email));
                connection.CreateTable(typeof(Address));
                connection.CreateTable(typeof(ContactMethod));
                connection.CreateTable(typeof(Account));
                connection.CreateTable(typeof(Person));
            } 
        }
        public SQLiteConnection GetConnection()
        {
            return new SQLiteConnection("database.sqlite");
        }
        public SQLiteAsyncConnection GetConnectionAsync()
        {
            return new SQLiteAsyncConnection("database.sqlite");
        }
        public bool SaveObject<T>(T item, bool update = true)
        {
            using (var con = GetConnection())
            {
                try
                {
                    con.InsertOrReplaceWithChildren(item);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
        public bool SaveItem(DataFile item, bool update = true)
        {
            using (var connection = GetConnection())
            {
                try
                {
                    if (item.Id > 0 && update)
                    {
                        connection.UpdateWithChildren(item);
                    }
                    else if (item.Id == 0)
                    {
                        connection.InsertWithChildren(item);
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
        public bool SaveAll(IEnumerable<DataFile> items, bool update = true)
        {
            using (var connection = GetConnection())
            {
                try
                {
                    connection.RunInTransaction(() =>
                    {
                        connection.InsertOrReplaceAllWithChildren(items);
                    });
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
        public void Delete<T>(DataFile data)
        {
            using (var con = GetConnection())
            {
                con.Delete<T>(data.Id);
            }
        }
    }
}
