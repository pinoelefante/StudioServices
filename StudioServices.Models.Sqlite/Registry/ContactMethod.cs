﻿using SQLite;

namespace StudioServices.Data.Sqlite.Registry
{
    public class ContactMethod : PersonReference
    {
        [Indexed(Name = "ContactMethodId", Order = 1, Unique = true)]
        public string Number { get; set; }
        [Indexed(Name = "ContactMethodId", Order = 2, Unique = true)]
        public ContactType Type { get; set; }
        /* Mobile */
        public bool IsWhatsApp { get; set; }
        public bool IsTelegram { get; set; }

        public int Priority { get; set; }

        public override string ToString()
        {
            return $"{Number} ({Type})";
        }
        protected override void CorrectFields()
        {
            if(Type != ContactType.MOBILE)
            {
                IsWhatsApp = false;
                if (Type != ContactType.TELEGRAM)
                    IsTelegram = false;
                else
                    IsTelegram = true;
            }
        }
    }
    public enum ContactType
    {
        PHONE = 0,
        MOBILE = 1,
        FAX = 2,
        TELEGRAM = 3,
        SKYPE = 4
    }
}
