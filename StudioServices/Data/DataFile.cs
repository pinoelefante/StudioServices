using SQLite;
using System;
namespace StudioServices.Data
{
    public class DataFile
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public bool Enabled { get; set; } = true;
        public DateTime CreationTime { get; set; }
        public DateTime DisabledTime { get; set; }

        public DataFile()
        {
            CreationTime = DateTime.Now;
        }

        public bool SetAttivo(bool status)
        {
            if (status == Enabled)
                return false;
            Enabled = status;
            if (!status)
                DisabledTime = DateTime.Now;
            return true;
        }
    }
}
