using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudioServices.Data.EntityFramework;
using System.Runtime.CompilerServices;

namespace StudioServicesWeb.Controllers
{
    public class MyController : Controller
    {
        protected bool _isAdmin(bool logEnable = true, [CallerMemberName]string methodName = "")
        {
            var is_admin = HttpContext.Session.GetInt32("IsAdmin");
            var res = is_admin != null && is_admin > 0;
            if(logEnable && !res)
            {
                // TODO: log admin fail
            }
            return res;
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
        protected async Task _setUserSessionAsync(int account_id, int person_id, bool admin)
        {
            HttpContext.Session.SetInt32("AccountId", account_id);
            HttpContext.Session.SetInt32("PersonId", person_id);
            HttpContext.Session.SetInt32("IsAdmin", admin ? 1 : 0);
            await HttpContext.Session.CommitAsync();
        }
        protected bool CheckPerson(PersonReference data)
        {
            var personId = _getPersonId();
            return data.PersonId == personId;
        }
        protected ResponseCode CheckLoginAndPerson(PersonReference data)
        {
            if (!_isLogged())
                return ResponseCode.REQUIRE_LOGIN;
            if (!CheckPerson(data))
                return ResponseCode.INVALID_PERSON;
            return ResponseCode.OK;
        }
        public Response<Boolean> CreateBoolean(bool resp, ResponseCode code = ResponseCode.OK, string message = "")
        {
            return new Response<bool>(resp)
            {
                Code = code,
                Message = message
            };
        }
        public Response<T> CreateResponse<T>(T data, ResponseCode code = ResponseCode.OK, string message = "")
        {
            return new Response<T>(data)
            {
                Code = code,
                Message = message
            };
        }
        public Response<T> CreateLoginRequired<T>()
        {
            return CreateResponse(default(T), ResponseCode.REQUIRE_LOGIN);
        }
        public Response<T> CreateOnlyAdmin<T>()
        {
            return CreateResponse(default(T), ResponseCode.ADMIN_FUNCTION);
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
        MAINTENANCE = 4,
        INVALID_PERSON = 5
    }
}
