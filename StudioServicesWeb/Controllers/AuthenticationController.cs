using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        [HttpGet("{username}&{password}")]
        public Response<bool> Login([FromQuery]string username, [FromQuery]string password)
        {
            int account_id = auth.Login(username, password, out int person_id, out bool admin, out string message);
            bool response = account_id > 0;
            if (response)
                _setUserSession(account_id, person_id, admin);
            return CreateBoolean(response, ResponseCode.OK, message);
        }
        
        [Route("register")]
        [HttpGet("{username}&{password}&{email}&{fiscal_code}&{verify_code}")]
        public Response<bool> CreateAccount([FromQuery]string username, string password, string email, string fiscal_code, string verify_code)
        {
            bool res = auth.AccountRegister(username, password, email, fiscal_code, verify_code, out string message);
            return CreateBoolean(res, ResponseCode.OK, message);
        }

        [Route("is_logged")]
        [HttpGet]
        public Response<bool> IsLogged()
        {
            bool res = _isLogged();
            return CreateBoolean(res);
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
