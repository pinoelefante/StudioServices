using System;
namespace StudioServices.Data.Anagrafica
{
    public class Account : DataFile
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public int PersonaId { get; set; }
    }
}
