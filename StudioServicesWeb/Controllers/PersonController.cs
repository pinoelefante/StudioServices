using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudioServices.Controllers.Persons;
using StudioServices.Data.EntityFramework.Registry;

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
        public Response<bool> CreatePerson([FromBody]Person person)
        {
            if (!_isAdmin())
                return CreateBoolean(false,ResponseCode.ADMIN_FUNCTION, "Funzione non abilitata all'utente");
            bool res = persons.AddPerson(person, out string verify_code);
            return CreateBoolean(res);   
        }

        [Route("get")]
        [HttpGet]
        public Response<Person> GetPerson()
        {
            if (!_isLogged())
                return CreateResponse<Person>(null, ResponseCode.REQUIRE_LOGIN, "Login required");
            Person person = persons.GetPerson(_getPersonId());
            return CreateResponse(person);
        }

        [Route("status")]
        [HttpGet]
        public Response<bool> SetStatus(int person_id, bool status)
        {
            if (!_isAdmin())
                return CreateBoolean(false, ResponseCode.ADMIN_FUNCTION,"Funzione non abilitata");
            bool res = persons.ChangeActiveStatus(person_id, status);
            return CreateBoolean(res);
        }

        [Route("document")]
        [HttpPost]
        public Response<int> AddDocument([FromBody]IdentificationDocument document)
        {
            var checkCode = CheckLoginAndPerson(document);
            if (checkCode != ResponseCode.OK)
                return CreateResponse<int>(-1, checkCode);
            bool res = persons.AddIdentificationDocument(document);
            return CreateResponse(res ? document.Id : -1, res ? ResponseCode.OK : ResponseCode.FAIL);
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
        public Response<int> AddContact([FromBody]ContactMethod contact)
        {
            var checkCode = CheckLoginAndPerson(contact);
            if (checkCode != ResponseCode.OK)
                return CreateResponse<int>(-1, checkCode);
            var res = persons.AddContactNumber(contact);
            return CreateResponse(res ? contact.Id : -1, res ? ResponseCode.OK : ResponseCode.FAIL);
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
        public Response<int> AddAddress([FromBody]Address address)
        {
            var checkCode = CheckLoginAndPerson(address);
            if (checkCode != ResponseCode.OK)
                return CreateResponse(-1, checkCode);
            var res = persons.AddAddress(address);
            return CreateResponse(res ? address.Id : -1, res ? ResponseCode.OK : ResponseCode.FAIL);
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
        public Response<int> AddEmail([FromBody]Email email)
        {
            var user_ok = CheckLoginAndPerson(email);
            if (user_ok != ResponseCode.OK)
                return CreateResponse<int>(-1, user_ok);
            var res = persons.AddEmail(email);
            return CreateResponse(res ? email.Id : -1, res ? ResponseCode.OK : ResponseCode.FAIL);
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