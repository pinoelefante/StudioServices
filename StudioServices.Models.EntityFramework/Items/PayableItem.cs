using StudioServices.Data.EntityFramework;
using System;

namespace StudioServices.Data.EntityFramework.Items
{
    public class PayableItem : DataFile
    {
        public string Name { get; set; }
        // [Indexed(Name = "ModelId", Order = 1, Unique = true)]
        public string Code { get; set; }
        // [Indexed(Name = "ModelId", Order = 2, Unique = true)]
        public int Year { get; set; }
        public double RequestCost { get; set; }
        public double RequestPrintCost { get; set; }
        public bool IsUnique { get; set; } // indica che può essere richiesto solo una volta
        public bool IsPrintable { get; set; }
        public bool IsRequestable { get; set; } // Per servizi aggiungibili solo dagli amministratori
        public string Description { get; set; }
        public bool IsOther { get; set; }
    }
}