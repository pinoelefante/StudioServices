using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Xamarin.Forms;
using System.IO;
using pinoelefante.Services;
using pinoelefante.UWP.Service;
using System.Reflection;

[assembly: Dependency(typeof(SQLiteService_WPF))]
namespace pinoelefante.UWP.Service
{
    public class SQLiteService_WPF : ISQLite
    {
        public SQLiteService_WPF()
        {
            var baseFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            DATABASE = Path.Combine(baseFolder, "db.sqlite");
        }
        private static string DATABASE;
        public SQLiteConnection GetConnection()
        {
            var conn = new SQLiteConnection(DATABASE);
            return conn;
        }
    }
}
