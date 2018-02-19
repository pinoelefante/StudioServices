using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using pinoelefante.ViewModels;
using StudioServicesApp.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudioServicesApp.ViewModels
{
    public class RegisterViewModel : MyViewModel
    {
        public RegisterViewModel(INavigationService n, StudioServicesApi a) : base(n, a)
        {
        }
        private bool _isCliente = true;
        private RelayCommand _registerCmd;

        public bool IsClient { get => _isCliente; set => Set(ref _isCliente, value); }
        public string Username { get; set; }
        public string Password { get; set; }
        public RelayCommand RegisterCommand =>
            _registerCmd ?? (_registerCmd = new RelayCommand(() =>
            {
                MessengerInstance.Send(new Tuple<string, string>(Username, Password), "RegistrationComplete");
                navigation.GoBack();
            }));

    }
}
