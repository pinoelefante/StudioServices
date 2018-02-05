using System;
using System.Collections.Generic;
using SQLite;
using StudioServices.Data;
using StudioServices.Data.Registry;

namespace StudioServices.Registry.Data
{
    public class Person : DataFile
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        [Unique]
        public string FiscalCode { get; set; }
        public string AuthCode { get; set; }
        public DateTime Birth { get; set; }
        public string BirthPlace { get; set; }
        public List<Email> Emails { get; } = new List<Email>(1);
        public List<Address> Addresses { get; } = new List<Address>(1);
        public List<ContactMethod> Contacts { get; } = new List<ContactMethod>(2);
        public List<IdentificationDocument> Identifications { get; } = new List<IdentificationDocument>(1);

        public void Add(DataFile newItem)
        {
            if (newItem is Email)
                Emails.Add(newItem as Email);
            else if (newItem is Address)
                Addresses.Add(newItem as Address);
            else if (newItem is ContactMethod)
                Contacts.Add(newItem as ContactMethod);
            else if (newItem is IdentificationDocument)
                Identifications.Add(newItem as IdentificationDocument);
            else
                throw new ArgumentException("Tipo di oggetto non supportato");
        }
    }
}
