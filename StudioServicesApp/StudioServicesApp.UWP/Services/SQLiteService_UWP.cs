using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Xamarin.Forms;
using System.IO;
using Windows.Storage;
using pinoelefante.Services;
using pinoelefante.UWP.Service;

[assembly: Dependency(typeof(SQLiteService_UWP))]
namespace pinoelefante.UWP.Service
{
    public class SQLiteService_UWP : ISQLite
    {
        public SQLiteService_UWP() { }
        private static readonly string DATABASE = Path.Combine(ApplicationData.Current.LocalFolder.Path, "db.sqlite");
        public SQLiteConnection GetConnection()
        {
            var conn = new SQLiteConnection(DATABASE);
            return conn;
        }
    }
}
