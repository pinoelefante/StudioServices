using System;
namespace StudioServices.Data.Registry
{
    public class Address : PersonReference
    {
        public string Country { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Street { get; set; }
        public string CivicNumber { get; set; }
        public string ZipCode { get; set; }
        public AddressType AddressType { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return $"{Street},{CivicNumber}, {City} ({Province}) {Country}";
        }
    }
    public enum AddressType
    {
        HOME = 0,
        WORK = 1
    }
}
