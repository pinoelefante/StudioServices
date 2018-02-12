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
        public string ActAs(int person_id)
        {
            if(!_isAdmin())
            {
                // TODO : log request
                // return CreateBoolean(false, ResponseCode.ADMIN_FUNCTION);
                return "not logged";
            }
            return "";
        }
    }
}
