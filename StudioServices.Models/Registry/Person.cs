using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SQLite;
using SQLiteNetExtensions.Attributes;
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
        [JsonIgnore]
        public string AuthCode { get; set; }
        public DateTime Birth { get; set; }
        public string BirthPlace { get; set; }

        [OneToMany]
        public List<Email> Emails { get; } = new List<Email>(1);
        [OneToMany]
        public List<Address> Addresses { get; } = new List<Address>(1);
        [OneToMany]
        public List<ContactMethod> Contacts { get; } = new List<ContactMethod>(2);
        [OneToMany]
        public List<IdentificationDocument> Identifications { get; } = new List<IdentificationDocument>(1);

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
    }
}
