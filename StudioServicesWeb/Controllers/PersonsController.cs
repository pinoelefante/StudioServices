using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudioServices.Controllers.Persons;
using StudioServices.Registry.Data;

namespace StudioServicesWeb.Controllers
{
    //[Produces("application/json")]
    [Route("api/persons")]
    public class PersonsController : MyController
    {
        private PersonsManager persons;
        public PersonsController(PersonsManager manager)
        {
            persons = manager;
        }

        [Route("create")]
        [HttpGet]
        public Response<bool> CreatePerson(string name, string surname, string fiscal_code, int b_year, int b_month, int b_day, string b_place)
        {
            if (!_isAdmin())
            {
                // TODO : loggare tentativo
                return CreateBoolean(false,ResponseCode.ADMIN_FUNCTION, "Funzione non abilitata all'utente");
            }
            bool res = persons.AddPerson(name, surname, fiscal_code, new DateTime(b_year, b_month, b_day), b_place, out string verify_code);
            return CreateBoolean(res);   
        }

        [Route("get")]
        [HttpGet]
        public Response<Person> GetPerson()
        {
            if (!_isLogged())
                return Create<Person>(null, ResponseCode.REQUIRE_LOGIN, "Login required");
            Person person = persons.GetPerson(_getPersonId());
            return Create(person);
        }

        [Route("set_status")]
        [HttpGet]
        public Response<bool> SetStatus(int person_id, bool status)
        {
            if (!_isAdmin())
            {
                // TODO : loggare tentativo
                return CreateBoolean(false, ResponseCode.ADMIN_FUNCTION,"Funzione non abilitata");
            }
            bool res = persons.ChangeActiveStatus(person_id, status);
            return CreateBoolean(res);
        }

        [HttpGet]
        public string Get()
        {
            return "PersonsController";
        }
    }
}