using SQLite;
using System;

namespace StudioServices.Data.Models
{
    public class Model : DataFile
    {
        public string Name { get; set; }
        [Indexed(Name = "ModelId", Order = 1, Unique = true)]
        public string Code { get; set; }
        [Indexed(Name = "ModelId", Order = 2, Unique = true)]
        public int Year { get; set; }
        public double RequestCost { get; set; }
        public double RequestPrintCost { get; set; }
        public bool IsUnique { get; set; } // indica che può essere richiesto solo una volta
        public bool IsPrintable { get; set; }
    }
}