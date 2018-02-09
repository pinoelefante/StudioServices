using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using StudioServices.Controllers.Persons;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StudioServicesWeb.Controllers
{
    [Route("api/authentication")]
    public class AuthenticationController : Controller
    {
        private static AuthenticationManager auth = new AuthenticationManager();
        //PersonController persons;
        public AuthenticationController()
        {
        }

        [Route("login")]
        [HttpGet("{username}&{password}")]
        public Response<bool> Login([FromQuery]string username, [FromQuery]string password)
        {
            int id = auth.Login(username, password, out string message);
            bool response = id > 0;
            if (response)
                HttpContext.Session.SetInt32("AccountId", id);
            return ResponseCreator.CreateBoolean(response, message);
        }
        
        [Route("register")]
        [HttpGet("{username}&{password}&{email}&{fiscal_code}&{verify_code}")]
        public Response<bool> CreateAccount([FromQuery]string username, string password, string email, string fiscal_code, string verify_code)
        {
            bool res = auth.AccountRegister(username, password, email, fiscal_code, verify_code, out string message);
            return ResponseCreator.CreateBoolean(res, message);
        }

        [HttpGet]
        public string Get()
        {
            int? sessionId = HttpContext.Session.GetInt32("AccountId");
            if (sessionId != null && sessionId > 0)
            {
                return "AccountId : " + sessionId;
            }
            else
            {
                return "Welcome!";
            }
        }
    }
}
