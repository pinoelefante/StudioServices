using AC.Components.Util;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using pinoelefante.ViewModels;
using StudioServicesApp.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace StudioServicesApp.ViewModels
{
    public class RegisterViewModel : MyViewModel
    {
        public RegisterViewModel(INavigationService n, StudioServicesApi a) : base(n, a) { }
        private bool _isCliente = true;
        private RelayCommand _registerCmd;

        private string _username, _pass1, _pass2, _email, _fiscalcode, _verifyCode, _name, _surname, _birthplace;
        private DateTime _birthday;
        public string Username { get => _username; set => Set(ref _username, value); }
        public string Password { get => _pass1; set => Set(ref _pass1, value); }
        public string Password2 { get => _pass2; set => Set(ref _pass2, value); }
        public string Email { get => _email; set => Set(ref _email, value); }
        public bool IsClient { get => _isCliente; set => Set(ref _isCliente, value); }
        public string FiscalCode { get => _fiscalcode; set => Set(ref _fiscalcode, value); }
        public string VerifyCode { get => _verifyCode; set => Set(ref _verifyCode, value); }
        public string Name { get => _name; set => Set(ref _name, value); }
        public string Surname { get => _surname; set => Set(ref _surname, value); }
        public DateTime Birthday { get => _birthday; set => Set(ref _birthday, value); }
        public string BirthPlace { get => _birthplace; set => Set(ref _birthplace, value); }

        private void ResetForm()
        {
            Username = string.Empty;
            Password = string.Empty;
            Password2 = string.Empty;
            Email = string.Empty;
            FiscalCode = string.Empty;
            VerifyCode = string.Empty;
            Name = string.Empty;
            Surname = string.Empty;
            Birthday = new DateTime(DateTime.Now.Subtract(TimeSpan.FromDays(365.25 * 18)).Ticks);
            BirthPlace = string.Empty;
            IsClient = true;
        }
        public override Task NavigatedToAsync(object parameter = null)
        {
            ResetForm();
            return base.NavigatedToAsync(parameter);
        }
        public RelayCommand RegisterCommand =>
            _registerCmd ?? (_registerCmd = new RelayCommand(async () =>
            {
                if(!ValidateFields(out string message))
                {
                    ShowMessage(message, "Registrazione");
                    return;
                }
                ResponseMessage<bool> res = null;
                if(IsClient)
                    res = await SendRequestAsync(async () => await api.Authentication_RegisterAsync(Username, Password, Email, FiscalCode, VerifyCode));
                else
                {
                    ShowMessage("La registrazione per gli utenti non ancora clienti non è ancora supportata", "Registrazione");
                    return;
                }
                if (res.Code == ResponseCode.OK && res.Data)
                {
                    MessengerInstance.Send(new Tuple<string, string>(Username, Password), "RegistrationComplete");
                    navigation.GoBack();
                }
                else
                    ShowMessage($"{res.Code}: {res.Message}", "Registrazione");
            }));
        private bool ValidateFields(out string message)
        {
            message = null;
            /* Password validation */
            if (Password.CompareTo(Password2) != 0)
            {
                message = "Password mismatch";
                return false;
            }
            else if(Password.Length < 8)
            {
                message = "Password must have at least 8 characters";
                return false;
            }

            /* Mail validation */
            try
            {
                var mail = new MailAddress(Email);
            }
            catch
            {
                message = "Email is not valid";
                return false;
            }

            /* Fiscal code validation */
            if (!FiscalCodeCalculator.ControlloFormaleOK(FiscalCode))
            {
                message = "Fiscal code formally invalid";
                return false;
            }
            return true;
        }
    }
}
