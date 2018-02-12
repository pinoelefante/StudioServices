using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace StudioServicesWeb.Controllers
{
    [Route("api/items")]
    public class ItemsController : MyController
    {
        [Route("items")]
        [HttpGet]
        public string GetItemsList()
        {
            return "GetList";
        }

        [Route("items")]
        [HttpGet("{id}")]
        public string GetItem(int id)
        {
            return $"Get {id}";
        }

        [Route("items")]
        [HttpDelete("{id}")]
        public string DeleteItem(int id)
        {
            if (!_isAdmin())
            {
                // TODO log request
                return "Admin only";
                //return CreateBoolean(false, ResponseCode.ADMIN_FUNCTION);
            }
            return $"Delete {id}";
        }

        [Route("request")]
        [HttpGet]
        public string GetRequestList()
        {
            return "";
        }

        [Route("request")]
        [HttpGet("{id}")]
        public string GetRequest(int id)
        {
            return "";
        }

        [Route("request")]
        [HttpPost]
        public string RequestItem([FromForm]int item_id)
        {
            return $"Request {item_id}";
        }

        [Route("request")]
        [HttpDelete("{id}")]
        public string DeleteRequest(int id)
        {
            return "";
        }

        [Route("request")]
        [HttpPost("{id}&{note}")]
        public string SetNote(int id, string note)
        {
            return "";
        }
    }
}