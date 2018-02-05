using SQLite;
using System;
namespace StudioServices.Data
{
    public class DataFile
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public bool Attivo { get; set; } = true;
        public DateTime Creazione { get; set; }
        public DateTime Disattivazione { get; set; }

        public DataFile()
        {
            Creazione = DateTime.Now;
        }

        public bool SetAttivo(bool status)
        {
            if (status == Attivo)
                return false;
            Attivo = status;
            if (!status)
                Disattivazione = DateTime.Now;
            return true;
        }
    }
}
