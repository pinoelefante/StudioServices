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
        [HttpGet]
        public Response<List<Message>> GetAllNews([FromQuery]long time = 0)
        {
            if (!_isLogged())
                return CreateLoginRequired<List<Message>>();
            var list = news.GetPublicMessages(_getPersonId(), new DateTime(time));
            return CreateResponse(list);
        }

        [HttpPost("{id}")]
        public Response<bool> SetRead([FromRoute]int id, [FromForm]int mode)
        {
            if(!_isLogged())
                return CreateLoginRequired<bool>();
            ReadMode mode_enum = (ReadMode)Enum.ToObject(typeof(ReadMode), mode);
            var message = news.GetMessage(id);
            if (message == null)
                return CreateBoolean(false, ResponseCode.FAIL, "Message is not available");
            if (_isAdmin() && message.IsPrivate)
                news.SetRead(0, id, mode_enum);
            var res = news.SetRead(_getPersonId(), id, mode_enum);
            return CreateResponse(res);
        }

        [HttpPost]
        public Response<bool> PostNews(string text, string title, bool is_private, int person_id, bool is_marked, bool expire, int e_year, int e_month, int e_day)
        {
            if(!_isAdmin())
            {
                // TODO log request
                return CreateOnlyAdmin<bool>();
            }
            var res = news.SendPublicMessage(text, _getPersonId(), title);
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
                return CreateResponse(messages);
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
