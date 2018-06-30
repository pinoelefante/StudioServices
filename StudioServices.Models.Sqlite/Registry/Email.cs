using Newtonsoft.Json;
using SQLite;
using System;

namespace StudioServices.Data.Sqlite.Registry
{
    public class Email : PersonReference
    {
        private bool pec, managed, autorenew;
        private string address, password, serviceName, servicePassword,
            autorenewPaypal, imapAddr, imapUser, smtpAddr, smtpUser, fullName;
        private int imapPort, smtpPort;
        private DateTime? expire = new DateTime(1);

        /* Settaggi */
        public bool IsPec { get => pec; set => Set(ref pec, value); }
        public bool IsManaged { get => managed; set => Set(ref managed, value); } // se gestito dallo studio

        /* Email */
        [Unique]
        public string Address { get => address; set => Set(ref address, value); }
        [JsonIgnore]
        public string Password { get => password; set => Set(ref password, value); }

        /* Tipo account aruba */
        public string ServiceUsername { get => serviceName; set => Set(ref serviceName, value); }
        [JsonIgnore]
        public string ServicePassword { get => servicePassword; set => Set(ref servicePassword, value); }

        /* Servizi per caselle email a pagamento */
        public DateTime? Expire { get => expire; set => Set(ref expire, value); }
        public bool AutoRenewEnabled { get => autorenew; set => Set(ref autorenew, value); }
        public string AutoRenewPaypalAddress { get => autorenewPaypal; set => Set(ref autorenewPaypal, value); }

        /* Settaggi server posta */
        public string IMAPAddress { get => imapAddr; set => Set(ref imapAddr, value); }
        public int IMAPPort { get => imapPort; set => Set(ref imapPort, value); }
        public string IMAPUsername { get => imapUser; set => Set(ref imapUser, value); }

        public string SMTPAddress { get => smtpAddr; set => Set(ref smtpAddr, value); }
        public int SMTPPort { get => smtpPort; set => Set(ref smtpPort, value); }
        public string SMTPUsername { get => smtpUser; set => Set(ref smtpUser, value); }

        public string FullName { get => fullName; set => Set(ref fullName, value); }

        public override string ToString()
        {
            return Address;
        }
    }
}
