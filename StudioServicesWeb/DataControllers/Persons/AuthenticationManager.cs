﻿using StudioServices.Data.EntityFramework.Registry;
using StudioServicesWeb.DataControllers;
using System;

namespace StudioServices.Controllers.Persons
{
    public class AuthenticationManager
    {
        private DatabaseEF db;
        public AuthenticationManager(DatabaseEF d)
        {
            db = d;
            if (db.Get<Person>(1) == null)
                db.Save<Person>(new Person()
                {
                    Birth = new DateTime(1990, 3, 28),
                    BirthPlace = "Scafati",
                    FiscalCode = "LFNGPP90C28I483C",
                    Enabled = true,
                    Name = "Giuseppe",
                    Surname = "Elefante"
                });
        }
        public bool AccountRegister(string username, string password, string email, string codice_fiscale, string codice_persona, out string message)
        {
            message = "";
            // Verifica codice fiscale & codice persona
            // Verifica restituisce l'id della persona
            var personTuple = db.Auth_VerifyPersonCode(codice_fiscale, codice_persona);

            if(personTuple.ResponseCode <= 0)
            {
                switch(personTuple.ResponseCode)
                {
                    case 0:
                        message = "Codice verifica errato";
                        break;
                    case -1:
                        message = "Associazione già avvenuta in precedenza";

                        break;
                    case -2:
                        message = "Codice fiscale non presente";
                        break;
                }
                message = "Verifica codice fallita";
                return false;
            }
            Account account = new Account()
            {
                Username = username,
                Password = PasswordSecurity.PasswordStorage.CreateHash(password),
                Enabled = false,
                PersonId = personTuple.Person.Id,
            };
            var person = personTuple.Person;
            if(db.Save(account))
            {
                // Imposta l'authcode a Null perché già utilizzato
                person.AuthCode = null;
                // Aggiunge l'email all'account
                person.Emails.Add(new Email()
                {
                    PersonId = person.Id,
                    Address = email,
                    FullName = person.Name + " " + person.Surname
                });
                return db.Save(person);
            }
            message = "Errore durante il salvataggio";
            return false;
        }
        public bool AccountRegisterWithPerson(string username, string password, string email, string name, string surname, DateTime birth, string fiscal_code, string birth_place)
        {
            // TODO Registrazione account con creazione Persona
            return false;
        }
        public int Login(string username, string password, out int person_id, out bool admin, out string message)
        {
            message = "";
            person_id = -1;
            admin = false;
            Account acc = db.Auth_GetAccountByUsername(username);
            if (acc == null)
            {
                message = "L'account non esiste";
                return -1;
            }
            if(!acc.Enabled)
            {
                message = "Account non abilitato";
                return -1;
            }
            if (PasswordSecurity.PasswordStorage.VerifyPassword(password, acc.Password))
            {
                person_id = acc.PersonId;
                admin = acc.IsAdmin;
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
            Account acc = db.Get<Account>(account_id);
            if(acc == null)
            {
                // Account assente
                return false;
            }
            if(PasswordSecurity.PasswordStorage.VerifyPassword(vecchia_password, acc.Password))
            {
                acc.Password = PasswordSecurity.PasswordStorage.CreateHash(nuova_password);
                return db.UpdateDataFileItem(acc);
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
            Account acc = db.Get<Account>(id_account);
            if (acc == null)
                return false;
            acc.Password = PasswordSecurity.PasswordStorage.CreateHash(nuova_password);
            return db.UpdateDataFileItem(acc);
        }
    }
}
