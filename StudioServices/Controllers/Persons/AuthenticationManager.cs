using StudioServices.Data.Registry;
using StudioServices.Registry.Data;
using System;
namespace StudioServices.Controllers.Persons
{
    public class AuthenticationManager
    {
        private RegistryDatabase db;
        public AuthenticationManager()
        {
            db = new RegistryDatabase();
        }
        public bool AccountRegister(string username, string password, string email, string codice_fiscale, string codice_persona, out string message)
        {
            message = "";
            // Verifica codice fiscale & codice persona
            // Verifica restituisce l'id della persona
            int id_person = db.VerifyPersonCode(codice_fiscale, codice_persona);

            if(id_person <= 0)
            {
                switch(id_person)
                {
                    case 0:
                        message = "Codice verifica errato";
                        break;
                    case -1:
                        message = "Codice fiscale non presente";
                        break;
                    case -2:
                        message = "Persona associata ad un altro account";
                        break;
                }
                message = "Verifica codice fallita";
                return false;
            }
            Account account = new Account()
            {
                Username = username,
                Password = PasswordSecurity.PasswordStorage.CreateHash(password),
                Attivo = false,
                PersonId = id_person
            };

            if(db.SaveItem(account))
            {
                // Imposta l'authcode a Null perché già utilizzato
                Person person = db.PersonSelect(id_person);
                person.AuthCode = null;
                // Aggiunge l'email all'account
                person.Add(new Email()
                {
                    Address = email,
                    FullName = person.Name + " " + person.Surname
                });
                return db.PersonSave(person);
            }
            message = "Errore durante il salvataggio";
            return false;
        }
        public bool AccountRegisterWithPerson(string username, string password, string name, string surname, DateTime birth, string fiscal_code, string birth_place)
        {
            // TODO Registrazione account con creazione Persona
            return false;
        }
        public int Login(string username, string password, out string message)
        {
            message = "";
            Account acc = db.SelectAccountByUsername(username);
            if (acc == null)
            {
                message = "L'account non esiste";
                return -1;
            }
            if(!acc.Attivo)
            {
                message = "Account non abilitato";
                return -1;
            }
            if (PasswordSecurity.PasswordStorage.VerifyPassword(password, acc.Password))
            {
                return acc.Id;
            }
            else
            {
                message = "Password errata";
                return -1;
            }
        }
        public bool ChangePassword(int account_id, string vecchia_password, string nuova_password)
        {
            Account acc = db.SelectAccount(account_id);
            if(acc == null)
            {
                // Account assente
                return false;
            }
            if(PasswordSecurity.PasswordStorage.VerifyPassword(vecchia_password, acc.Password))
            {
                acc.Password = PasswordSecurity.PasswordStorage.CreateHash(nuova_password);
                return db.SaveItem(acc);
            }
            else
            {
                // Vecchia password errata
                return false;
            }
        }
        public bool ChangePassword(int id_account, string nuova_password)
        {
            // usabile solo dall'amministratore
            Account acc = db.SelectAccount(id_account);
            if (acc == null)
                return false;
            acc.Password = PasswordSecurity.PasswordStorage.CreateHash(nuova_password);
            return db.SaveItem(acc);
        }
    }
}
