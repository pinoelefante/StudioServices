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
        private int imapPort = 993, smtpPort = 465;
        private DateTime? expire = DateTime.Now.AddYears(1);

        /* Settaggi */
        public bool IsPec { get => pec; set => Set(ref pec, value); }
        public bool IsManaged { get => managed; set => Set(ref managed, value); } // se gestito dallo studio

        /* Email */
        [Unique]
        public string Address { get => address; set => Set(ref address, value); }
        
        public string Password { get => password; set => Set(ref password, value); }

        /* Tipo account aruba */
        public string ServiceUsername { get => serviceName; set => Set(ref serviceName, value); }
        
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
        public override void Reset()
        {
            base.Reset();
            this.Address = null;
            this.AutoRenewEnabled = false;
            this.AutoRenewPaypalAddress = null;
            this.Expire = DateTime.Now.AddYears(1);
            this.FullName = null;
            this.IMAPAddress = null;
            this.IMAPPort = 993;
            this.IMAPUsername = null;
            this.IsManaged = false;
            this.IsPec = false;
            this.Password = null;
            this.ServicePassword = null;
            this.ServiceUsername = null;
            this.SMTPAddress = null;
            this.SMTPPort = 465;
            this.SMTPUsername = null;
        }
        public override void InitFrom(DataFile f)
        {
            base.InitFrom(f);
            var e = f as Email;
            if (e == null)
                return;
            this.Address = e.Address;
            this.AutoRenewEnabled = e.AutoRenewEnabled;
            this.AutoRenewPaypalAddress = e.AutoRenewPaypalAddress;
            this.Expire = e.Expire;
            this.FullName = e.FullName;
            this.IMAPAddress = e.IMAPAddress;
            this.IMAPPort = e.IMAPPort;
            this.IMAPUsername = e.IMAPUsername;
            this.IsManaged = e.IsManaged;
            this.IsPec = e.IsPec;
            this.Password = e.Password;
            this.ServicePassword = e.ServicePassword;
            this.ServiceUsername = e.ServiceUsername;
            this.SMTPAddress = e.SMTPAddress;
            this.SMTPPort = e.SMTPPort;
            this.SMTPUsername = e.SMTPUsername;
        }
    }
}
