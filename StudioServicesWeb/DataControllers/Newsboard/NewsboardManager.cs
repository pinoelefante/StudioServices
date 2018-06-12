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
        public bool SendPublicMessage(string content, int sender_id, string title = "", bool isMarked = false,  bool isExpire = false, DateTime expireDate = default(DateTime))
        {
            Message msg = new Message()
            {
                Title = title,
                Content = content,
                ExpireDate = expireDate,
                IsExpireEnabled = isExpire,
                IsMarked = isMarked,
                IsPrivate = false,
                SenderId = sender_id
            };

            if(db.SaveItem(msg))
            {
                // TODO Inviare notifiche push
                return true;
            }
            return false;
        }
        public Message GetMessage(int message_id)
        {
            return db.GetMessage(message_id);
        }
        public bool SendConversationMessage(string content, int sender_id, int receiver_id)
        {
            Message msg = new Message()
            {
                Content = content,
                IsExpireEnabled = false,
                IsMarked = false,
                IsPrivate = true,
                SenderId = sender_id,
                PersonId = receiver_id
            };
            if(db.SaveItem(msg))
            {
                // TODO send push
                return true;
            }
            return false;
        }
        public bool SendMessageToAdmin(string content, int sender_id)
        {
            // TODO implement

            // Search Admin IDs
            
            return false;
        }
        public List<Message> GetPublicMessages(int person_id, DateTime last_message_date = default(DateTime))
        {
            return db.GetPublicMessages(person_id, last_message_date);
        }
        public bool DeleteMessage(int message_id)
        {
            Message msg = db.GetMessage(message_id);
            if(msg == null)
            {
                // Impossibile cancellare un messaggio che non esiste
                return true;
            }
            msg.SetAttivo(false);
            return db.SaveItem(msg);
        }
        public bool SetRead(int person_id, int message_id, ReadMode mode)
        {
            var read_status = db.GetReadStatus(message_id, person_id) ?? new ReadStatus() { PersonId = person_id, MessageId = message_id };
            switch(mode)
            {
                case ReadMode.APP:
                    read_status.App = true;
                    break;
                case ReadMode.EMAIL:
                    read_status.Email = true;
                    break;
                case ReadMode.TELEGRAM:
                    read_status.Telegram = true;
                    break;
                case ReadMode.WEB:
                    read_status.Web = true;
                    break;
                case ReadMode.WHATSAPP:
                    read_status.Whatsapp = true;
                    break;
                default:
                    return false;
            }
            return db.SaveObject(read_status);
        }
        public bool IsRead(int person_id, int message_id)
        {
            return db.GetReadStatus(message_id, person_id) != null;
        }
    }
}
