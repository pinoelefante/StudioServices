using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudioServices.Controllers.Persons;

namespace StudioServicesWeb.Controllers
{
    //[Produces("application/json")]
    [Route("api/persons")]
    public class PersonsController : Controller
    {
        private static PersonsManager persons = new PersonsManager();

        [Route("create")]
        [HttpGet]
        public Response<bool> CreatePerson(string name, string surname, string fiscal_code, int b_year, int b_month, int b_day, string b_place)
        {
            // TODO Verifica admin
            //bool res = persons.AddPerson(name, surname, fiscal_code, new DateTime(b_year, b_month, b_day), b_place, out string verify_code);
            return ResponseCreator.CreateBoolean(false);
            
        }

        [HttpGet]
        public string Get()
        {
            return "PersonsController";
        }
    }
}