using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StudioServices.Controllers.Items;
using StudioServices.Data.Items;

namespace StudioServicesWeb.Controllers
{
    [Route("api/items")]
    public class ItemsController : MyController
    {
        private ItemsManager items;
        public ItemsController(ItemsManager m)
        {
            items = m;
        }

        [Route("items")]
        [HttpGet]
        public Response<List<PayableItem>> GetItemsList()
        {
            if (!_isLogged())
                return Create<List<PayableItem>>(null, ResponseCode.REQUIRE_LOGIN);
            var list = items.ListItems();
            return Create(list);
        }

        [Route("items")]
        [HttpGet("{id}")]
        public string GetItem(int id)
        {
            return $"Get {id}";
        }

        [Route("items")]
        [HttpDelete("{id}")]
        public Response<bool> DeleteItem(int id)
        {
            if (!_isAdmin())
            {
                // TODO log request
                return CreateBoolean(false, ResponseCode.ADMIN_FUNCTION);
            }
            var res = items.DeleteItem(id);
            return CreateBoolean(res);
        }

        [Route("request")]
        [HttpGet]
        public Response<List<ItemRequest>> GetRequestList()
        {
            if (!_isLogged())
                return CreateLoginRequired<List<ItemRequest>>();
            var list = items.ListRequests(_getPersonId());
            return Create(list);
        }

        [Route("request")]
        [HttpGet("{id}")]
        public string GetRequest(int id)
        {
            return "";
        }

        [Route("request")]
        [HttpPost]
        public Response<bool> RequestItem([FromForm]int item_id, bool print, string note, int item_quantity, int print_quantity)
        {
            if (!_isLogged())
                return CreateLoginRequired<bool>();
            var res = items.RequestModel(_getPersonId(), item_id, print, note, out string message, item_quantity, print_quantity);
            return CreateBoolean(res, ResponseCode.OK, message);
        }

        [Route("request")]
        [HttpDelete("{id}")]
        public Response<bool> DeleteRequest(int id)
        {
            if (!_isLogged())
                CreateLoginRequired<bool>();
            var res = items.DeleteRequest(id, _getPersonId());
            return CreateBoolean(res);
        }

        [Route("request")]
        [HttpPost("{id}&{note}")]
        public Response<bool> SetNote(int id, string note)
        {
            if (!_isLogged())
                return CreateLoginRequired<bool>();
            var res = items.ModifyNote(id, _getPersonId(), note);
            return CreateBoolean(res);
        }
    }
}