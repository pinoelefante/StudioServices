using StudioServices.Data.Newsboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioServices.Controllers.Newsboard
{
    public class NewsboardManager
    {
        private NewsboardDatabase db;
        public NewsboardManager()
        {
            db = new NewsboardDatabase();
        }
        public bool SendMessage(string content, bool isPrivate = false, int person_id = 0, bool isMarked = false,  bool isExpire = false, DateTime expireDate = default(DateTime))
        {
            //TODO controllare che venga chiamato da un amministratore

            Message msg = new Message()
            {
                Content = content,
                ExpireDate = expireDate,
                IsExpireEnabled = isExpire,
                IsMarked = isExpire,
                IsPrivate = isPrivate,
                PersonId = person_id
            };

            if(db.SaveItem(msg))
            {
                // TODO Inviare notifiche push
                return true;
            }
            return false;
        }
        public IEnumerable<Message> ListMessages(int person_id, DateTime last_message_date = default(DateTime))
        {
            return db.MessageList(person_id, last_message_date);
        }
        public bool DeleteMessage(int message_id)
        {
            Message msg = db.SelectMessage(message_id);
            if(msg == null)
            {
                // Impossibile cancellare un messaggio che non esiste
                return true;
            }
            msg.SetAttivo(false);
            return db.SaveItem(msg);
        }
    }
}
