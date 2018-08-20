using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using StudioServices.Data.EntityFramework.Registry;

namespace StudioServices.Data.EntityFramework
{
    public class PersonReference : DataFile
    {
        [ForeignKey(nameof(Person))]
        public virtual int PersonId { get; set; }
        
        public virtual Person Person { get; set; }
    }
}