using SQLite;
using System;
namespace StudioServices.Data.Registry
{
    public class IdentificationDocument : PersonReference
    {
        [Indexed(Name = "DocumentID", Order = 1, Unique = true)]
        public string Number { get; set; }
        [Indexed(Name = "DocumentID", Order = 2, Unique = true)]
        public DocumentType Type { get; set; }
        public string Filename { get; set; }
        public DateTime Issue { get; set; }
        public DateTime Expire { get; set; }
    }
    public enum DocumentType
    {
        IDENTIFICATION_DOCUMENT = 0,
        PASSPORT = 1,
        DRIVING_LICENSE = 2
    }
}
