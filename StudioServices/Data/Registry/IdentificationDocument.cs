using SQLite;
using System;
namespace StudioServices.Data.Registry
{
    public class IdentificationDocument : PersonReference
    {
        [Indexed(Name = "DocumentID", Order = 1, Unique = true)]
        public string Number { get; set; }
        /*
            0 - carta d'identità
            1 - passaporto
            2 - patente
        */
        [Indexed(Name = "DocumentID", Order = 2, Unique = true)]
        public int Type { get; set; }
        public string Filename { get; set; }
        public DateTime Issue { get; set; }
        public DateTime Expire { get; set; }
    }
}
