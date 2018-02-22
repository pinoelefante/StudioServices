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
        
        public string Username { get; set; }
        public string Password { get; set; }
        public string Password2 { get; set; }
        public string Email { get; set; }
        public bool IsClient { get => _isCliente; set => Set(ref _isCliente, value); }
        public string FiscalCode { get; set; }
        public string VerifyCode { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime Birthday { get; set; } = new DateTime(DateTime.Now.Subtract(TimeSpan.FromDays(365.25 * 18)).Ticks);
        public string BirthPlace { get; set; }

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
