using SQLite;
using System;

namespace StudioServices.Data.Sqlite.Registry
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

        [Ignore]
        public string FileContentBase64 { get; set; }

        public string GetFileExtension()
        {
            if (string.IsNullOrEmpty(Filename) || !Filename.Contains("."))
                return string.Empty;
            var dotIndex = Filename.LastIndexOf('.');
            return Filename.Substring(dotIndex);
        }
        public override bool IsValid()
        {
            if (string.IsNullOrEmpty(Number))
                throw new DocumentInvalidNumberException();
            var now = DateTime.Now;
            if (Issue.CompareTo(now) > 0)
                throw new DocumentIssueDateException();
            if (Expire.CompareTo(now) < 0)
                throw new DocumentExpiredException();
            return true;
        }
        public override string ToString()
        {
            return $"{Type} - {Number}";
        }
    }
    public class DocumentExpiredException : Exception { }
    public class DocumentIssueDateException : Exception { }
    public class DocumentInvalidNumberException : Exception { }
    public enum DocumentType
    {
        IDENTIFICATION_DOCUMENT = 0,
        PASSPORT = 1,
        DRIVING_LICENSE = 2
    }
}
