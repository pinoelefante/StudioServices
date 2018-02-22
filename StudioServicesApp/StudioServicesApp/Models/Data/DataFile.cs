using SQLite;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace StudioServices.Data
{
    public class DataFile : INotifyPropertyChanged
    {
        [PrimaryKey]
        public int Id { get; set; }
        public bool Enabled { get; set; } = true;
        public DateTime CreationTime { get; set; }
        public DateTime DisabledTime { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void Set<T>(ref T item, T value, [CallerMemberName]string name="")
        {
            item = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
