using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioServices.Data.Newsboard
{
    public class ReadStatus
    {
        [Indexed(Name = "ReadStatusId", Order = 1, Unique = true)]
        public int PersonId { get; set; }
        [Indexed(Name = "ReadStatusId", Order = 2, Unique = true)]
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
