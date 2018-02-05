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

        /* 
            0 - casa
            1 - lavoro
        */
        public int AddressType { get; set; }
        public string Description { get; set; }
    }
}
