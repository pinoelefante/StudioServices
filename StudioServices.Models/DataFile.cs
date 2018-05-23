using SQLite;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace StudioServices.Data
{
    public class DataFile : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public virtual int Id { get; set; }
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
        public event PropertyChangedEventHandler PropertyChanged;
        protected void Set<T>(ref T item, T value, [CallerMemberName]string name = "")
        {
            item = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
