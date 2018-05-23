using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioServices.Data
{
    public class PersonReference : DataFile
    {
        public virtual int PersonId { get; set; }
    }
}
