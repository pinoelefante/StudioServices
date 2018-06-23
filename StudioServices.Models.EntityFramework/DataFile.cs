using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace StudioServices.Data.EntityFramework
{
    public abstract class DataFile : INotifyPropertyChanged
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id { get; set; }

        public virtual bool Enabled { get; set; } = true;
        public virtual DateTime CreationTime { get; set; }
        public virtual DateTime DisabledTime { get; set; }

        [NotMapped]
        public List<string> FileUpload { get; set; } = new List<string>();

        public DataFile()
        {
            CreationTime = DateTime.Now;
            Validate();
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
        protected virtual void Validate() { }
    }
}