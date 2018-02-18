using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudioServices.Controllers.Persons;
using StudioServices.Registry.Data;

namespace StudioServicesWeb.Controllers
{
    //[Produces("application/json")]
    [Route("api/person")]
    public class PersonController : MyController
    {
        private PersonsManager persons;
        public PersonController(PersonsManager manager)
        {
            persons = manager;
        }

        [Route("create")]
        [HttpPost]
        public Response<bool> CreatePerson([FromForm]string name, [FromForm]string surname, [FromForm]string fiscal_code, [FromForm]int b_year, [FromForm]int b_month, [FromForm]int b_day, [FromForm]string b_place)
        {
            if (!_isAdmin())
            {
                // TODO : loggare tentativo
                return CreateBoolean(false,ResponseCode.ADMIN_FUNCTION, "Funzione non abilitata all'utente");
            }
            name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name);
            surname = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(surname);
            bool res = persons.AddPerson(name, surname, fiscal_code.ToUpper(), new DateTime(b_year, b_month, b_day), b_place, out string verify_code);
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

        [Route("status")]
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

        [Route("document")]
        [HttpPost]
        public Response<bool> AddDocument(int type, string number, long i_ticks, long e_ticks, byte[] file, string file_ext)
        {
            if (!_isLogged())
                return CreateBoolean(false, ResponseCode.REQUIRE_LOGIN);
            number = number.ToUpper();
            DateTime issue_date = new DateTime(i_ticks);
            DateTime expire_date = new DateTime(e_ticks);
            bool res = persons.AddIdentificationDocument(_getPersonId(), type, number, issue_date, expire_date, file, file_ext);
            return CreateBoolean(res);
        }

        [Route("document")]
        [HttpDelete]
        public Response<bool> RemoveDocument(int id)
        {
            if (!_isLogged())
                return CreateBoolean(false, ResponseCode.REQUIRE_LOGIN);
            bool res = persons.RemoveIdentificationDocument(_getPersonId(), id);
            return CreateBoolean(res);
        }

        [Route("contact")]
        [HttpPost]
        public Response<bool> AddContact(int type, string number, bool whatsapp = false, bool telegram = false, int priorita = 0)
        {
            var res = persons.AddContactNumber(_getPersonId(), type, number, whatsapp, telegram, priorita);
            return CreateBoolean(res);
        }
        [Route("contact")]
        [HttpDelete]
        public Response<bool> RemoveContact(int id)
        {
            bool res = persons.RemoveContactNumber(_getPersonId(), id);
            return CreateBoolean(res);
        }

        [Route("address")]
        [HttpPost]
        public Response<bool> AddAddress(int type, string country, string city, string province, string address, string number, string description)
        {
            var res = persons.AddAddress(_getPersonId(), type, country, city, province, address, number, description);
            return CreateBoolean(res);
        }

        [Route("address")]
        [HttpDelete]
        public Response<bool> RemoveAddress(int id)
        {
            var res = persons.RemoveAddress(_getPersonId(), id);
            return CreateBoolean(res);
        }

        [Route("email")]
        [HttpPost]
        public Response<bool> AddEmail(string address, bool pec, bool managed, string password, string fullname, string imap_address, int imap_port, string imap_username, string smtp_address, int smtp_port, string smtp_username, string service_username, string service_password, long expire_day, bool renew_auto, string renew_paypal)
        {
            var res = persons.AddEmail(_getPersonId(), address, pec, managed, password, fullname, imap_address, imap_port, imap_username, smtp_address, smtp_port, smtp_username, service_username, service_password, new DateTime(expire_day), renew_auto, renew_paypal);
            return CreateBoolean(res);
        }

        [Route("email")]
        [HttpDelete]
        public Response<bool> RemoveEmail(int id)
        {
            var res = persons.RemoveEmail(_getPersonId(), id);
            return CreateBoolean(res);
        }

        [HttpGet]
        public string Get()
        {
            return "PersonsController";
        }
    }
}