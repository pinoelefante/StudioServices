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
    public class WarehouseProductsManagerViewModel : MyAuthViewModel
    {
        public Company CurrentCompany { get; set; }
        public MyObservableCollection<CompanyProduct> ProductsList { get; } = new MyObservableCollection<CompanyProduct>();
        public WarehouseProductsManagerViewModel(INavigationService n, StudioServicesApi a, AlertService al, KeyValueService k) : base(n, a, al, k)
        {
        }
        public override void RegisterMessenger()
        {
            MessengerInstance.Register<Company>(this, MSG_SET_CURRENT_COMPANY, (company) =>
            {
                if(company != CurrentCompany)
                {
                    CurrentCompany = company;
                    RaisePropertyChanged(() => CurrentCompany);
                }
            });
            MessengerInstance.Register<Company>(this, MSG_RESPONSE_CURRENT_COMPANY, (company) =>
            {
                if (company != CurrentCompany)
                {
                    CurrentCompany = company;
                    RaisePropertyChanged(() => CurrentCompany);
                }
            });
        }
        public override Task NavigatedToAsync(object parameter = null)
        {
            MessengerInstance.Send<bool>(true, MSG_REQUEST_CURRENT_COMPANY);
            return base.NavigatedToAsync(parameter);
        }

        private RelayCommand<CompanyProduct> openProductCmd;
        public RelayCommand<CompanyProduct> OpenProductCommand =>
            openProductCmd ??
            (openProductCmd = new RelayCommand<CompanyProduct>((product) =>
            {
                var parameters = new Tuple<Company, CompanyProduct>(CurrentCompany, product);
                Navigation.PushPopupAsync(new AddCompanyProductPopup(parameters));
            }));
    }
}
