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
        public List<Message> MessageList(int person_id, DateTime last_message = default(DateTime), bool all = false)
        {
            using (var con = GetConnection())
            {
                //return con.Table<Message>().Where(x => (x.PersonId == person_id || !x.IsPrivate) && x.CreationTime.CompareTo(last_message) > 0 && (all ? true : x.Enabled)).OrderByDescending(x => x.CreationTime).ToList();
                return con.Query<Message>($"SELECT * FROM Message WHERE CreationTime > ? ORDER BY CreationTime ASC", last_message.Ticks);
            }
        }
        public Message SelectMessage(int message_id)
        {
            using (var con = GetConnection())
            {
                return con.Get<Message>(message_id);
            }
        }
        public ReadStatus GetReadStatus(int message_id, int person_id)
        {
            using (var con = GetConnection())
            {
                return con.Table<ReadStatus>().Where(x => x.MessageId == message_id && x.PersonId == person_id).FirstOrDefault();
            }
        }
    }
}
