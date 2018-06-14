using SQLiteNetExtensions.Attributes;
using StudioServices.Registry.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioServices.Data
{
    public class PersonReference : DataFile
    {
        [ForeignKey(typeof(Person))]
        public virtual int PersonId { get; set; }
    }
}
