using SQLite;
using System;
namespace StudioServices.Data.Registry
{
    public class ContactMethod : PersonReference
    {
        [Indexed(Name = "ContactMethodId", Order = 1, Unique = true)]
        public string Number { get; set; }
        /*
           0 - telefono
           1 - cellulare
           2 - fax
           3 - telegram account
           4 - skype account
         */
        [Indexed(Name = "ContactMethodId", Order = 2, Unique = true)]
        public int Type { get; set; }

        public bool IsWhatsApp { get; set; }
        public bool IsTelegram { get; set; }

        public int Priority { get; set; }
    }
}
