using System;
namespace StudioServices.Services.Persone
{
    public class AutenticazioneController
    {
        public bool RegistraAccount(string username, string password, string codice_fiscale, string codice_persona)
        {
            throw new NotImplementedException();
        }
        public bool Login(string username, string password)
        {
            throw new NotImplementedException();
        }
        public bool CambiaPassword(string vecchia_password, string nuova_password)
        {
            throw new NotImplementedException();
        }
        public bool CambiaPassword(int id_account, string nuova_password)
        {
            // usabile dall'amministratore
            throw new NotImplementedException();
        }
    }
}
