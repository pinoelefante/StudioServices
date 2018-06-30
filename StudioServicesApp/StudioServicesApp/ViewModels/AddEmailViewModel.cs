using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Views;
using StudioServices.Data.Sqlite.Registry;
using StudioServicesApp.Services;

namespace StudioServicesApp.ViewModels
{
    public class AddEmailViewModel : MyAuthViewModel
    {
        private Email email;
        public Email Email { get => email; set => SetMT(ref email, value); }
        public AddEmailViewModel(INavigationService n, StudioServicesApi a, AlertService al, KeyValueService k) : base(n, a, al, k) {
            Email = new Email();
        }
        public override async Task NavigatedToAsync(object parameter = null)
        {
            await base.NavigatedToAsync(parameter);
            // Email = parameter!=null && parameter is Email ? parameter as Email : new Email();
            Email = new Email();
        }
    }
}
