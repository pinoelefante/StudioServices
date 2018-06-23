using System;
using System.Collections.Generic;
using System.Text;
using GalaSoft.MvvmLight.Views;
using StudioServicesApp.Services;

namespace StudioServicesApp.ViewModels
{
    public class AddEmailViewModel : MyAuthViewModel
    {
        public AddEmailViewModel(INavigationService n, StudioServicesApi a, AlertService al, KeyValueService k) : base(n, a, al, k)
        {
        }
    }
}
