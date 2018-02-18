using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using pinoelefante.ViewModels;
using StudioServicesApp.Services;
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

        public string Username { get; set; }
        public string Password { get; set; }

        private RelayCommand _loginCmd;
        public RelayCommand LoginCommand =>
            _loginCmd ?? (_loginCmd = new RelayCommand(async () =>
            {
                IsBusyActive = true;
                var res = await SendRequestAsync(async () => await api.Authentication_LoginAsync(Username, Password));
                if (res.Code == ResponseCode.OK && res.Data)
                {
                    Debug.WriteLine("Login OK");
                }
                else
                    Debug.WriteLine("Login fail: "+res.Message);
                IsBusyActive = false;
            }));


    }
}
