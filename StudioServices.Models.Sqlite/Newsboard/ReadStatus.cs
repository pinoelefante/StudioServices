using Newtonsoft.Json;
using SQLite;
using SQLiteNetExtensions.Attributes;
using StudioServices.Data.Sqlite.Registry;

namespace StudioServices.Data.Sqlite.Newsboard
{
    public class ReadStatus
    {
        [Indexed(Name = "ReadStatusId", Order = 1, Unique = true), ForeignKey(typeof(Person))]
        public int PersonId { get; set; }

        [Indexed(Name = "ReadStatusId", Order = 2, Unique = true), ForeignKey(typeof(Message))]
        public int MessageId { get; set; }

        public bool Whatsapp { get; set; }
        public bool Telegram { get; set; }
        public bool Email { get; set; }
        public bool App { get; set; }
        public bool Web { get; set; }
        [Ignore, JsonIgnore]
        public bool IsRead
        {
            get
            {
                return Whatsapp || Telegram || Email || App || Web;
            }
        }
    }
    public enum ReadMode
    {
        WHATSAPP = 0,
        TELEGRAM = 1,
        EMAIL = 2,
        APP = 3,
        WEB = 4
    }
}
