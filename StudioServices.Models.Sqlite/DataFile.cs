using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace StudioServices.Data.Sqlite
{
    public abstract class DataFile : INotifyPropertyChanged
    {
        public virtual int Id { get; set; }
        public virtual bool Enabled { get; set; } = true;
        public virtual DateTime CreationTime { get; set; }
        public virtual DateTime DisabledTime { get; set; }

        [SQLite.Ignore]
        public List<string> FileUpload { get; set; } = new List<string>();

        public DataFile()
        {
            CreationTime = DateTime.Now;
            CorrectFields();
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
        protected virtual void CorrectFields() { }

        public virtual void Reset()
        {
            Id = 0;
            Enabled = true;
            CreationTime = DateTime.Now;
            DisabledTime = DateTime.FromBinary(0);
        }
        public virtual void InitFrom(DataFile f)
        {
            Id = f.Id;
            Enabled = f.Enabled;
            CreationTime = f.CreationTime;
            DisabledTime = f.DisabledTime;
        }
        public virtual bool IsValid() => true;
    }
}