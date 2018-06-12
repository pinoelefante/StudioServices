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
        public List<Message> GetPublicMessages(int person_id, DateTime last_message = default(DateTime), bool all = false)
        {
            using (var con = GetConnection())
            {
                //return con.Table<Message>().Where(x => (x.PersonId == person_id || !x.IsPrivate) && x.CreationTime.CompareTo(last_message) > 0 && (all ? true : x.Enabled)).OrderByDescending(x => x.CreationTime).ToList();

                var messages = con.Query<Message>($"SELECT * FROM Message WHERE IsPrivate=0 AND CreationTime > ? ORDER BY CreationTime DESC", last_message.Ticks);
                messages.ForEach((x) =>
                {
                    var readStatus = GetReadStatus(x.Id, person_id);
                    x.IsRead = readStatus != null && readStatus.IsRead;
                });
                return messages;
            }
        }
        public Message GetMessage(int message_id)
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
