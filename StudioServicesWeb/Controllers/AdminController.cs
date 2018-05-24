using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StudioServicesWeb.Controllers
{
    [Route("api/admin")]
    public class AdminController : MyController
    {
        [Route("act_as")]
        [HttpGet("{person_id}")]
        public async Task<Response<bool>> ActAsAsync(int person_id)
        {
            if (!_isAdmin())
                return CreateOnlyAdmin<bool>();
            await _setUserSessionAsync(_getAccountId(), person_id, true);
            return CreateBoolean(true);
        }
        [Route("is_admin")]
        [HttpGet]
        public Response<bool> IsAdmin()
        {
            if (!_isLogged())
                return CreateLoginRequired<bool>();
            return CreateBoolean(_isAdmin());
        }
    }
}
