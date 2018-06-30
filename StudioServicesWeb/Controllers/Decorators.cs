using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace StudioServicesWeb.Controllers
{
    public class IsLoggedAttribute : ActionFilterAttribute
    {
        
    }
    
}
