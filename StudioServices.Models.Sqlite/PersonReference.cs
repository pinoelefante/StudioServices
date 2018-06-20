using SQLiteNetExtensions.Attributes;
using StudioServices.Data.Sqlite.Registry;

namespace StudioServices.Data.Sqlite
{
    public class PersonReference : DataFile
    {
        [ForeignKey(typeof(Person))]
        public virtual int PersonId { get; set; }

        public virtual Person Person { get; set; }
    }
}
