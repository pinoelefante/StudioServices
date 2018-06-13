using System;
using System.Collections.Generic;
using System.Text;
using GalaSoft.MvvmLight.Views;
using StudioServicesApp.Services;

namespace StudioServicesApp.ViewModels
{
    public class WarehouseClientsSuppliersListViewModel : MyAuthViewModel
    {
        public WarehouseClientsSuppliersListViewModel(INavigationService n, StudioServicesApi a, AlertService al, KeyValueService k) : base(n, a, al, k)
        {
        }
    }
}
