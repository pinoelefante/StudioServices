using StudioServices.Data.Registry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioServices.Data.Accounting
{
    public class Company : PersonReference
    {
        public string Name { get; set; }
        public string VATNumber { get; set; }
        public int AddressId { get; set; }
    }
}
