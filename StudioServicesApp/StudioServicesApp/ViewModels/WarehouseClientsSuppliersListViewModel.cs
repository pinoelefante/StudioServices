using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using StudioServices.Data.Sqlite.Accounting;
using StudioServicesApp.Services;
using StudioServicesApp.Views;

namespace StudioServicesApp.ViewModels
{
    public class WarehouseClientsSuppliersListViewModel : MyAuthViewModel
    {
        public WarehouseClientsSuppliersListViewModel(INavigationService n, StudioServicesApi a, AlertService al, KeyValueService k) : base(n, a, al, k)
        {
        }
        public override async Task NavigatedToAsync(object parameter = null)
        {
            await base.NavigatedToAsync(parameter);
        }
        private RelayCommand addClSuplCmd;
        public RelayCommand AddClientSupplierCommand =>
            addClSuplCmd ??
            (addClSuplCmd = new RelayCommand(() =>
            {
                Navigation.PushPopupAsync(new AddCompanyPopup(true));
            }));
    }
}
