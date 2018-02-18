using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StudioServices.Controllers.Newsboard;
using StudioServices.Data.Newsboard;

namespace StudioServicesWeb.Controllers
{
    [Route("api/news")]
    public class NewsController : MyController
    {
        private NewsboardManager news;
        public NewsController(NewsboardManager m)
        {
            news = m;
        }

        [Route("all")]
        [HttpGet("{time}")]
        public Response<List<Message>> GetAllNews(long time = 0)
        {
            if (!_isLogged())
                return CreateLoginRequired<List<Message>>();
            var list = news.ListMessages(_getPersonId(), new DateTime(time));
            return Create(list);
        }

        [HttpGet("{id}")]
        public Response<bool> SetRead(int message_id, int mode)
        {
            if(!_isLogged())
                return CreateLoginRequired<bool>();
            ReadMode mode_enum = (ReadMode)Enum.ToObject(typeof(ReadMode), mode);
            var res = news.SetRead(!_isAdmin() ? _getPersonId() : 0, message_id, mode_enum);
            return Create(res);
        }

        [HttpPost]
        public Response<bool> PostNews(string text, bool is_private, int person_id, bool is_marked, bool expire, int e_year, int e_month, int e_day)
        {
            if(!_isAdmin())
            {
                // TODO log request
                return CreateOnlyAdmin<bool>();
            }
            var res = news.SendPublicMessage(text, _getPersonId());
            return CreateBoolean(res);
        }

        [Route("conversation")]
        [HttpPost]
        public Response<bool> SendConversationMessage(string text, int destination)
        {
            if (!_isLogged())
                return CreateLoginRequired<bool>();
            if (!_isAdmin() && destination != 0)
                return CreateOnlyAdmin<bool>();
            var sender = _isAdmin() ? 0 : _getPersonId();
            var res = news.SendConversationMessage(text, sender, destination);
            return CreateBoolean(res);
        }

        [Route("conversation")]
        [HttpGet]
        public Response<List<Message>> GetConversation(int person_id)
        {
            if (!_isLogged())
                return CreateLoginRequired<List<Message>>();
            if (!_isAdmin() && person_id == 0) // conversazione con admin
            {
                //TODO implement
                var messages = new List<Message>();
                return Create(messages);
            }
            if (!_isAdmin())
            {
                // TODO log request
                return CreateOnlyAdmin<List<Message>>();
            }
            
            // TODO implement
            throw new NotImplementedException();
        }


        [HttpDelete("{id}")]
        public Response<bool> DeleteNews(int id)
        {
            if(!_isAdmin())
            {
                // TODO log request
                return CreateOnlyAdmin<bool>();
            }
            var res = news.DeleteMessage(id);
            return CreateBoolean(res);
        }
    }
}
