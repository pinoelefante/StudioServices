using SQLite;
using SQLiteNetExtensions.Attributes;
using StudioServices.Data.Registry;
using StudioServices.Registry.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioServices.Data.Accounting
{
    public class Company : PersonReference
    {
        [ForeignKey(typeof(Person))]
        [Indexed(Name = "CompanyIndex", Order = 1, Unique = true)]
        public override int PersonId { get => base.PersonId; set => base.PersonId = value; }

        public string Name { get; set; }

        [Indexed(Name = "CompanyIndex", Order = 2, Unique = true)]
        public string VATNumber { get; set; }
        // public int AddressId { get; set; }

        [OneToOne]
        public Address Address { get; set; }
    }
}
