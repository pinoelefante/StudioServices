using StudioServices.Controllers.Utils;
using StudioServices.Data.Newsboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioServices.Controllers.Newsboard
{
    public class NewsboardDatabase : Database
    {
        public IEnumerable<Message> MessageList(int person_id, DateTime last_message = default(DateTime), bool all = false)
        {
            using (var con = GetConnection())
            {
                return con.Table<Message>().Where(x => (x.PersonId == person_id || !x.IsPrivate) && x.Creazione.CompareTo(last_message) > 0 && (all ? true : x.Attivo)).OrderBy(x => x.Creazione).AsEnumerable();
            }
        }
        public Message SelectMessage(int message_id)
        {
            using (var con = GetConnection())
            {
                return con.Get<Message>(message_id);
            }
        }
    }
}
