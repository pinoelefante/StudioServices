using pinoelefante.Services;
using SQLite;
using StudioServices.Data.Newsboard;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudioServicesApp.Services
{
    public class DatabaseService
    {
        private ISQLite conMngr;
        public DatabaseService(ISQLite sqlite)
        {
            conMngr = sqlite;
            CreateDatabase();
        }
        private void CreateDatabase()
        {
            using (var con = GetConnection())
            {
                con.CreateTable<Message>();
            }
        }
        private SQLiteConnection GetConnection()
        {
            return conMngr.GetConnection();
        }
        public void DeleteDatabase()
        {
            using (var con = GetConnection())
            {
                con.DropTable<Message>();
            }
            CreateDatabase();
        }
        public void SaveItem<T>(T item)
        {
            using (var conn = GetConnection())
            {
                conn.InsertOrReplace(item);
            }
        }
        public void SaveItems<T>(IEnumerable<T> list)
        {
            using (var con = GetConnection())
            {
                con.RunInTransaction(() =>
                {
                    foreach (T item in list)
                        con.InsertOrReplace(item);
                });
            }
        }
        public void Delete<T>(T item)
        {
            using (var con = GetConnection())
            {
                con.Delete(item);
            }
        }
        public List<Message> GetMessages()
        {
            using (var con = GetConnection())
            {
                var news = con.Table<Message>().Where(x => !x.IsPrivate).OrderByDescending(x => x.CreationTime);
                return GetEnumerable(news);
            }
        }
        private List<T> GetEnumerable<T>(IEnumerable<T> content)
        {
            var list = new List<T>();
            foreach(var item in content)
                list.Add(item);
            return list;
        }
        public IEnumerable<Message> GetConversation(int person_id = 0)
        {
            using (var con = GetConnection())
            {
                var news = con.Table<Message>().Where(x => x.IsPrivate && (x.SenderId == person_id || x.PersonId == person_id)).OrderBy(x => x.CreationTime.Ticks);
                return news;
            }
        }
    }
}
