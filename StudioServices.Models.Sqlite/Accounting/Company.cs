using SQLite;
using SQLiteNetExtensions.Attributes;
using StudioServices.Data.Sqlite.Registry;

namespace StudioServices.Data.Sqlite.Accounting
{
    public class Company : PersonReference
    {
        [Indexed(Name = "CompanyIndex", Order = 1, Unique = true), ForeignKey(typeof(Person))]
        public override int PersonId { get; set; }

        public string Name { get; set; }

        [Indexed(Name = "CompanyIndex", Order = 2, Unique = true)]
        public string VATNumber { get; set; }

        [ForeignKey(typeof(Address))]
        public int AddressId { get; set; }

        [OneToOne]
        public Address Address { get; set; }
    }
}
