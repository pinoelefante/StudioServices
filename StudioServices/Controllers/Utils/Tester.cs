using System;
namespace StudioServices.Controllers.Utils
{
    public class Tester
    {
        static void Main(string[] args)
        {
            // Display the number of command line arguments:
            string hash1 = PasswordSecurity.PasswordStorage.CreateHash("elefante");
            System.Console.WriteLine("Hash di \"elefante\" = "+hash1);
            string hash2 = PasswordSecurity.PasswordStorage.CreateHash("elefante");
            System.Console.WriteLine("Hash di \"elefante\" = " + hash2);

            bool check1 = PasswordSecurity.PasswordStorage.VerifyPassword("elefante", hash1);
            bool check2 = PasswordSecurity.PasswordStorage.VerifyPassword("elefante", hash2);
            System.Console.WriteLine("Check1 = "+check1+ "\nCheck2 = "+check2);
        }
    }
}
