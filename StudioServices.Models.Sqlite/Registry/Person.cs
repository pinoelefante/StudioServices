using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;

namespace StudioServices.Data.Sqlite.Registry
{
    public abstract class Person : DataFile
    {
        public virtual string Name { get; set; }
        public virtual string Surname { get; set; }
        public virtual string FiscalCode { get; set; }
        [JsonIgnore]
        public virtual string AuthCode { get; set; }
        public virtual DateTime Birth { get; set; }
        public virtual string BirthPlace { get; set; }
        public virtual List<Email> Emails { get; set; } = new List<Email>(1);
        public virtual List<Address> Addresses { get; set; } = new List<Address>(1);
        public virtual List<ContactMethod> Contacts { get; set; } = new List<ContactMethod>(2);
        public virtual List<IdentificationDocument> Identifications { get; set; } = new List<IdentificationDocument>(1);

        public List<Address> GetPersonAddresses()
        {
            return Addresses.FindAll(x => x.PersonId == Id);
        }
        public List<Address> GetInvoiceAddresses()
        {
            return Addresses.FindAll(x => x.PersonId == -Id);
        }
        public Address GetAddress(int id)
        {
            return Addresses.Find(x => x.Id == id);
        }
        protected override void Validate()
        {
            Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(string.IsNullOrEmpty(Name) ? string.Empty : Name);
            Surname = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(string.IsNullOrEmpty(Surname) ? string.Empty : Surname);
        }
    }
}