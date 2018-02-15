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
        public Response<Boolean> CreateBoolean(bool resp, ResponseCode code = ResponseCode.OK, string message = "")
        {
            return new Response<bool>(resp)
            {
                Code = code,
                Message = message
            };
        }
        public Response<T> Create<T>(T data, ResponseCode code = ResponseCode.OK, string message = "")
        {
            return new Response<T>(data)
            {
                Code = code,
                Message = message
            };
        }
        public Response<T> CreateLoginRequired<T>()
        {
            return Create(default(T), ResponseCode.REQUIRE_LOGIN);
        }
        public Response<T> CreateOnlyAdmin<T>()
        {
            return Create(default(T), ResponseCode.ADMIN_FUNCTION);
        }
    }
    public class Response<T>
    {
        public long Ticks { get; }
        public ResponseCode Code { get; set; }
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
    public enum ResponseCode
    {
        OK = 0,
        REQUIRE_LOGIN = 1,
        FAIL = 2,
        ADMIN_FUNCTION = 3,
        MAINTENANCE = 4
    }
}
