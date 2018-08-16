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
        private Company company;
        public Company CurrentCompany { get => company; set => SetMT(ref company, value); }
        public MyObservableCollection<CompanyProduct> ProductsList { get; } = new MyObservableCollection<CompanyProduct>();
        public WarehouseProductsManagerViewModel(INavigationService n, StudioServicesApi a, AlertService al, KeyValueService k) : base(n, a, al, k)
        {
        }
        public override void RegisterMessenger()
        {
            MessengerInstance.Register<Company>(this, MSG_SET_CURRENT_COMPANY, async (company) =>
            {
                await SetCompanyAsync(company);
            });
            MessengerInstance.Register<Company>(this, MSG_RESPONSE_CURRENT_COMPANY, async (company) =>
            {
                await SetCompanyAsync(company);
            });
            MessengerInstance.Register<CompanyProduct>(this, MSG_MY_COMPANY_PRODUCT_ADD, (product) =>
            {
                ProductsList.Add(product);
            });
        }
        public override void UnregisterMessenger()
        {
            MessengerInstance.Unregister<Company>(this, MSG_SET_CURRENT_COMPANY);
            MessengerInstance.Unregister<Company>(this, MSG_RESPONSE_CURRENT_COMPANY);
            MessengerInstance.Unregister<CompanyProduct>(this, MSG_MY_COMPANY_PRODUCT_ADD);
        }
        private async Task SetCompanyAsync(Company company)
        {
            if (CurrentCompany != company)
                CurrentCompany = company;
            if (company == null)
                return;
            var products = await GetProductsAsync(company.Id);
            ProductsList.AddRange(products, true);
        }
        public override async Task NavigatedToAsync(object parameter = null)
        {
            await base.NavigatedToAsync(parameter);
            MessengerInstance.Send<bool>(true, MSG_REQUEST_CURRENT_COMPANY);
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
