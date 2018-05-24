using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudioServices.Controllers.Persons;

namespace StudioServicesWeb.Controllers
{
    [Route("api/authentication")]
    public class AuthenticationController : MyController
    {
        private AuthenticationManager auth;
        public AuthenticationController(AuthenticationManager manager)
        {
            auth = manager;
        }

        [Route("login")]
        [HttpPost]
        public async Task<Response<bool>> LoginAsync([FromForm]string username, [FromForm]string password)
        {
            int account_id = auth.Login(username, password, out int person_id, out bool admin, out string message);
            bool response = account_id > 0;
            if (response)
                await _setUserSessionAsync(account_id, person_id, admin);
            return CreateBoolean(response, ResponseCode.OK, message);
        }
        
        [Route("register")]
        [HttpPost]
        public Response<bool> CreateAccount([FromForm]string username, [FromForm]string password, [FromForm]string email, [FromForm]string fiscal_code, [FromForm]string verify_code)
        {
            bool res = auth.AccountRegister(username, password, email.ToLower(), fiscal_code.ToUpper(), verify_code, out string message);
            return CreateBoolean(res, ResponseCode.OK, message);
        }

        [Route("is_logged")]
        [HttpGet]
        public Response<bool> IsLogged()
        {
            bool res = _isLogged();
            return CreateBoolean(res);
        }

        [Route("logout")]
        [HttpPost]
        public Response<bool> Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.Response.Cookies.Delete("SSWSESSID");
            return CreateBoolean(true);
        }

        [HttpGet]
        public string Get()
        {
            int accountId = _getAccountId();
            if (accountId > 0)
                return $"AccountId {accountId} ({_isAdmin()})";
            else
                return "Welcome!";
        }
    }
}
