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
                
                /* Registry */
                connection.CreateTable(typeof(IdentificationDocument));
                connection.CreateTable(typeof(Email));
                connection.CreateTable(typeof(Address));
                connection.CreateTable(typeof(ContactMethod));
                connection.CreateTable(typeof(Account));
                connection.CreateTable(typeof(Person));

                /* Newsboard */
                connection.CreateTable<Message>();
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
        public bool SaveItem(DataFile item, bool update = true)
        {
            using (var connection = GetConnection())
            {
                try
                {
                    if (item.Id > 0 && update)
                        return connection.Update(item) > 0;
                    else if (item.Id == 0)
                    {
                        int id = connection.Insert(item);
                        return id > 0;
                    }
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }
        public bool SaveAll(IEnumerable<DataFile> items, bool update = true)
        {
            using (var connection = GetConnection())
            {
                try
                {
                    int count = 0;
                    connection.RunInTransaction(() =>
                    {
                        foreach (var x in items)
                        {
                            if (x.Id > 0 && update)
                            {
                                if (connection.Update(x) > 0)
                                    count++;
                            }
                            else if (x.Id == 0)
                            {
                                if (connection.Insert(x) > 0)
                                    count++;
                            }
                        }
                    });
                    return count > 0;
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
