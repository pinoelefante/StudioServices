using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using pinoelefante.ViewModels;
using StudioServicesApp.Services;
using StudioServicesApp.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace StudioServicesApp.ViewModels
{
    public class LoginViewModel : MyViewModel
    {
        public LoginViewModel(INavigationService n, StudioServicesApi a) : base(n, a)
        {
            
        }
        private RelayCommand _loginCmd, _registerPageCmd;
        private string _username, _password;

        public string Username { get => _username; set => Set(ref _username, value); }
        public string Password { get => _password; set => Set(ref _password, value); }
        
        public RelayCommand LoginCommand =>
            _loginCmd ?? (_loginCmd = new RelayCommand(async () =>
            {
                IsBusyActive = true;
                var res = await SendRequestAsync(async () => await api.Authentication_LoginAsync(Username, Password));
                if (res.Code == ResponseCode.OK && res.Data)
                {
                    Debug.WriteLine("Login OK");
                    App.Current.MainPage = new MyMasterPage();
                }
                else
                    Debug.WriteLine("Login fail: "+res.Message);
                IsBusyActive = false;
            }));

        public RelayCommand OpenRegisterPageCommand =>
            _registerPageCmd ?? (_registerPageCmd = new RelayCommand(() =>
            {
                MessengerInstance.Register<Tuple<string, string>>(this, "RegistrationComplete", (x) =>
                {
                    Debug.WriteLine($"{x.Item1} - {x.Item2}");
                    Username = x.Item1;
                    Password = x.Item2;
                    // TODO mostrare alert tempo registrazione: "potrebbero essere necessari fino a 2 giorni lavorativi per completare l'attivazione"
                    MessengerInstance.Unregister<Tuple<string, string>>(this, "RegistrationComplete");
                });

                Debug.WriteLine("Opening Register Page");
                navigation.NavigateTo(ViewModelLocator.REGISTER_PAGE);
            }));
    }
}
