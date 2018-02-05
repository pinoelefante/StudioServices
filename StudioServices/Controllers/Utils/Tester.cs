using StudioServices.Controllers.Persons;
using StudioServices.Services.Persons;
using System;
using System.IO;

namespace StudioServices.Controllers.Utils
{
    public class Tester
    {
        static void Main(string[] args)
        {

            PersonController pc = new PersonController();
            //string verify_code = null;
            bool creation = pc.AddPerson("Giuseppe", "Elefante", "LFNGPP90C28I483C", new DateTime(1990, 3, 28), "Scafati (SA)", out string verify_code);
            Console.WriteLine("Person created: " + creation + " - Code: " + verify_code);

            AuthenticationController auth = new AuthenticationController();
            bool account = auth.AccountRegister("pinoelefante", "elefante", "pinoelefante@hotmail.it", "LFNGPP90C28I483C", verify_code, out string account_message);
            Console.WriteLine("Account created: " + account + " " + account_message);

            Console.WriteLine("Premere un tasto per continuare...");
            Console.ReadKey();
        }
    }
}
