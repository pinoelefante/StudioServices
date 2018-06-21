using Newtonsoft.Json;
using SQLite;
using System;

namespace StudioServices.Data.Sqlite.Registry
{
    public class Email : PersonReference
    {
        /* Settaggi */
        public bool IsPec { get; set; } = false;
        public bool IsManaged { get; set; } = false; // se gestito dallo studio

        /* Email */
        [Unique]
        public string Address { get; set; }
        [JsonIgnore]
        public string Password { get; set; }

        /* Tipo account aruba */
        public string ServiceUsername { get; set; }
        [JsonIgnore]
        public string ServicePassword { get; set; }

        /* Servizi per caselle email a pagamento */
        public DateTime? Expire { get; set; }
        public bool AutoRenewEnabled { get; set; }
        public string AutoRenewPaypalAddress { get; set; }

        /* Settaggi server posta */
        public string IMAPAddress { get; set; }
        public int IMAPPort { get; set; }
        public string IMAPUsername { get; set; }

        public string SMTPAddress { get; set; }
        public int SMTPPort { get; set; }
        public string SMTPUsername { get; set; }

        public string FullName { get; set; }

        public override string ToString()
        {
            return Address;
        }
    }
}
