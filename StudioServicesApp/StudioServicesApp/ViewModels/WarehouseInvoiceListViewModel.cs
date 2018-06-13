using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using StudioServices.Data.Accounting;
using StudioServicesApp.Services;

namespace StudioServicesApp.ViewModels
{
    public class WarehouseInvoiceListViewModel : MyAuthViewModel
    {
        public WarehouseInvoiceListViewModel(INavigationService n, StudioServicesApi a, AlertService al, KeyValueService k) : base(n, a, al, k)
        {

        }

        public override Task NavigatedToAsync(object parameter = null)
        {
            var companies = GetMyCompaniesList();
            if(companies.Count == 0)
            {

            }
            else
            {

            }
            return base.NavigatedToAsync(parameter);
        }

        private RelayCommand openInvoiceCreatorPurchase, openInvoiceCreatorSell;
        private RelayCommand<Invoice> openInvoiceCreatorWithInvoice;

        public RelayCommand OpenInvoicePurchaseCommand =>
            openInvoiceCreatorPurchase ??
            (openInvoiceCreatorPurchase = new RelayCommand(() =>
            {
                var invoice = new Invoice()
                {
                    Type = InvoiceType.PURCHASE
                };
                Navigation.NavigateTo(ViewModelLocator.INVOICE_CREATION_HOME, invoice);
            }));
        public RelayCommand OpenInvoiceCreatorSellCommand =>
            openInvoiceCreatorSell ??
            (openInvoiceCreatorSell = new RelayCommand(() =>
            {
                var invoice = new Invoice()
                {
                    Type = InvoiceType.SELL
                };
                Navigation.NavigateTo(ViewModelLocator.INVOICE_CREATION_HOME, invoice);
            }));
        public RelayCommand<Invoice> OpenInvoiceCreatorWithInvoiceCommand =>
            openInvoiceCreatorWithInvoice ??
            (openInvoiceCreatorWithInvoice = new RelayCommand<Invoice>((invoice) =>
            {
                Navigation.NavigateTo(ViewModelLocator.INVOICE_CREATION_HOME, invoice);
            }));
    }
}
