using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioServices.Services.NullaostaLavoro
{
    public class NullaostaLavoroAPI
    {
        public void Login(string username, string password)
        {
            // URL: https://nullaostalavoro.dlci.interno.it/Ministero/Login
            // Method: POST
            // Parameters: passwordLogin userLogin
            // response: html
            // verifica login: presenza  tasto logout / email in class welIntro
        }
        public void Logout()
        {
            // URL: https://nullaostalavoro.dlci.interno.it/Ministero/Logout
        }
        public void ChangePassword()
        {
            // URL: https://nullaostalavoro.dlci.interno.it/Ministero/ModificaPwd
            // Method: POST
            // Parameters: captcha email newPass newPass2 oldPass
            // response: html
        }
    }
}
