using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudioServices.Data.EntityFramework.Registry
{
    public class IdentificationDocument : PersonReference
    {
        // [Indexed(Name = "DocumentID", Order = 1, Unique = true)]
        public string Number { get; set; }
        // [Indexed(Name = "DocumentID", Order = 2, Unique = true)]
        public DocumentType Type { get; set; }
        public string Filename { get; set; }
        public DateTime Issue { get; set; }
        public DateTime Expire { get; set; }

        [NotMapped]
        public string FileContentBase64 { get; set; }

        public string GetFileExtension()
        {
            if (string.IsNullOrEmpty(Filename) || !Filename.Contains("."))
                return string.Empty;
            var dotIndex = Filename.LastIndexOf('.');
            return Filename.Substring(dotIndex);
        }
    }
    public enum DocumentType
    {
        IDENTIFICATION_DOCUMENT = 0,
        PASSPORT = 1,
        DRIVING_LICENSE = 2
    }
}
