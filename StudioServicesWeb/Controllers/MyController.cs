using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudioServicesWeb.Controllers
{
    public class MyController : Controller
    {
        protected bool _isAdmin()
        {
            var is_admin = HttpContext.Session.GetInt32("IsAdmin");
            return is_admin != null && is_admin > 0;
        }
        protected bool _isLogged()
        {
            var account_id = HttpContext.Session.GetInt32("AccountId");
            return account_id != null && account_id > 0;
        }
        protected int _getAccountId()
        {
            var account_id = HttpContext.Session.GetInt32("AccountId");
            return account_id != null ? account_id.Value : -1;
        }
        protected int _getPersonId()
        {
            var account_id = HttpContext.Session.GetInt32("PersonId");
            return account_id != null ? account_id.Value : -1;
        }
        protected void _setUserSession(int account_id, int person_id, bool admin)
        {
            HttpContext.Session.SetInt32("AccountId", account_id);
            HttpContext.Session.SetInt32("PersonId", person_id);
            HttpContext.Session.SetInt32("IsAdmin", admin ? 1 : 0);
        }
        public Response<Boolean> CreateBoolean(bool resp, string message = "")
        {
            return new Response<bool>(resp)
            {
                Result = resp,
                Message = message
            };
        }
        public Response<T> Create<T>(T data, string message = "")
        {
            return new Response<T>(data)
            {
                Result = data != null ? true : false,
                Message = message
            };
        }
    }
    public class Response<T>
    {
        public long Ticks { get; }
        public bool Result { get; set; } = true;
        public string Message { get; set; }
        public T Data { get; set; }

        public Response()
        {
            Ticks = DateTime.Now.Ticks;
        }
        public Response(T data) : this()
        {
            Data = data;
        }
    }
}
