using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using pinoelefante.ViewModels;
using StudioServicesApp.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StudioServicesApp.ViewModels.HiddenPages
{
    public class ServerSettingsViewModel : MyViewModel
    {
        public ServerSettingsViewModel(INavigationService n, StudioServicesApi a) : base(n, a)
        {
        }
        public override Task NavigatedToAsync(object parameter = null)
        {
            string server_addr = api.GetServerAddress();
            Uri uri = new Uri(server_addr);
            ServerAddress = uri.Host;
            Port = uri.Port;
            Protocol = uri.Scheme;
            return base.NavigatedToAsync(parameter);
        }

        private string _prot, _address;
        private int _port;
        private RelayCommand _saveCmd;

        public string Protocol { get => _prot; set => Set(ref _prot, value); }
        public string ServerAddress { get => _address; set => Set(ref _address, value); }
        public int Port { get => _port; set => Set(ref _port, value); }
        public RelayCommand SaveCommand =>
            _saveCmd ?? (_saveCmd = new RelayCommand(() =>
            {
                api.SetNewAddress(ServerAddress, Port, Protocol);
                navigation.GoBack();
            }));
    }
}
