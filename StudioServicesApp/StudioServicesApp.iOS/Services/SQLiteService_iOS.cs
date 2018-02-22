using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using Xamarin.Forms;
using System.IO;
using pinoelefante.iOS.Services;
using pinoelefante.Services;

[assembly : Dependency(typeof(SQLiteService_iOS))]
namespace pinoelefante.iOS.Services
{
    public class SQLiteService_iOS : ISQLite
    {
        public SQLiteService_iOS()
        {
            var sqliteFilename = "db.sqlite";
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
            string libraryPath = Path.Combine(documentsPath, "..", "Library"); // Library folder
            DATABASE_PATH = Path.Combine(libraryPath, sqliteFilename);
        }
        private static string DATABASE_PATH;
        public SQLiteConnection GetConnection()
        {
            // Create the connection
            var conn = new SQLite.SQLiteConnection(DATABASE_PATH);
            // Return the database connection
            return conn;
        }
    }
}
