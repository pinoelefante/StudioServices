using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using pinoelefante.ViewModels;
using Plugin.SecureStorage;
using StudioServicesApp.Services;
using StudioServicesApp.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace StudioServicesApp.ViewModels
{
    public class LoginViewModel : MyViewModel
    {
        public LoginViewModel(INavigationService n, StudioServicesApi a) : base(n, a)
        {

        }
        public override Task NavigatedToAsync(object parameter = null)
        {
            Username = CrossSecureStorage.Current.GetValue("username", "");
            Password = CrossSecureStorage.Current.GetValue("password", "");

            return Task.Factory.StartNew(async () =>
            {
                RequireLogin = false;
                //var busy = SetBusy(true, "Verifico accesso in corso");
                var message = await SendRequestAsync(() => api.Authentication_IsLoggedAsync());
                bool login_ok = message.Code == ResponseCode.OK && message.Data;

                if (!login_ok && !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password))
                {
                    await Task.Run(async () =>
                    {
                        var res = await SendRequestAsync(async () => await api.Authentication_LoginAsync(Username, Password));
                        login_ok = res.Code == ResponseCode.OK && res.Data;
                    });
                }
                //SetBusy(false, "", busy);
                RequireLogin = !login_ok;
                if(login_ok)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Debug.WriteLine("Login not required");
                        App.Current.MainPage = new MyMasterPage();
                    });
                }
            });
        }
        private RelayCommand _loginCmd, _registerPageCmd, _serverCmd;
        private string _username, _password;
        private bool _requireLogin;

        public string Username { get => _username; set => Set(ref _username, value); }
        public string Password { get => _password; set => Set(ref _password, value); }
        public bool RequireLogin { get => _requireLogin; set => SetMT(ref _requireLogin, value); }
        
        public RelayCommand LoginCommand =>
            _loginCmd ?? (_loginCmd = new RelayCommand(async () =>
            {
                if (string.IsNullOrEmpty(Username))
                {
                    ShowMessage("Inserire l'username", "Login");
                    return;
                }
                if (string.IsNullOrEmpty(Password))
                {
                    ShowMessage("Inserire la password", "Login");
                    return;
                }
                var progress = SetBusy(true, "Accesso in corso...");
                var res = await SendRequestAsync(async () => await api.Authentication_LoginAsync(Username, Password));
                if (res.Code == ResponseCode.OK && res.Data)
                    App.Current.MainPage = new MyMasterPage();
                else
                    ShowMessage(res.Message, "Login fallito");
                SetBusy(false, "", progress);
            }));
        public RelayCommand OpenRegisterPageCommand =>
            _registerPageCmd ?? (_registerPageCmd = new RelayCommand(() =>
            {
                MessengerInstance.Register<Tuple<string, string>>(this, "RegistrationComplete", (x) =>
                {
                    CrossSecureStorage.Current.SetValue("username", x.Item1);
                    CrossSecureStorage.Current.SetValue("password", x.Item1);
                    Username = x.Item1;
                    Password = x.Item2;
                    MessengerInstance.Unregister<Tuple<string, string>>(this, "RegistrationComplete");
                    ShowMessage("Registrazione completata con successo.\nPotrebbero essere necessari fino a 2 giorni lavorativi affinché l'account sia attivo", "Registrazione completata");
                });
                navigation.NavigateTo(ViewModelLocator.REGISTER_PAGE);
            }));
        public RelayCommand OpenServerSettings =>
            _serverCmd ?? (_serverCmd = new RelayCommand(() =>
            {
                navigation.NavigateTo(ViewModelLocator.SERVER_SETTINGS);
            }));
    }
}
