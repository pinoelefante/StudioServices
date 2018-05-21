using System;
using System.Collections.Generic;
using System.Text;
using GalaSoft.MvvmLight.Views;
using StudioServicesApp.Services;

namespace StudioServicesApp.ViewModels
{
    public class AddAddressViewModel : MyAuthViewModel
    {
        public AddAddressViewModel(INavigationService n, StudioServicesApi a, AlertService al) : base(n, a, al)
        {
        }
    }
}
