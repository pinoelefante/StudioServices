using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using StudioServices.Data.Sqlite.Accounting;
using StudioServicesApp.Services;
using Xamarin.Forms;
using System.Linq;

namespace StudioServicesApp.ViewModels
{
    public class WarehouseInvoiceListViewModel : MyAuthViewModel
    {
        public WarehouseInvoiceListViewModel(INavigationService n, StudioServicesApi a, AlertService al, KeyValueService k) : base(n, a, al, k)
        {

        }

        public override async Task NavigatedToAsync(object parameter = null)
        {
            await base.NavigatedToAsync(parameter);
            Device.BeginInvokeOnMainThread(() =>
            {
                CompanyList.Clear();
                CompanyList.AddRange(GetMyCompaniesList());

                if (CompanyList.Count == 0)
                {
                }
                else
                {
                    if (CompanyIndex < 0)
                        CompanyIndex = 0;
                }
            });
        }
        private int _companyIndex=-1, _yearIndex = -1;
        private RelayCommand openInvoiceCreatorPurchase, openInvoiceCreatorSell;
        private RelayCommand<Invoice> openInvoiceCreatorWithInvoice;

        public MyObservableCollection<Company> CompanyList { get; } = new MyObservableCollection<Company>();
        public MyObservableCollection<int> YearsList { get; } = new MyObservableCollection<int>();
        public MyObservableCollection<Invoice> InvoiceByYear { get; } = new MyObservableCollection<Invoice>();
        public MyObservableCollection<Invoice> InvoiceSell { get; } = new MyObservableCollection<Invoice>();
        public MyObservableCollection<Invoice> InvoicePurchase { get; } = new MyObservableCollection<Invoice>();
        public int CompanyIndex
        {
            get => _companyIndex;
            set
            {
                if (value == _companyIndex) return;
                SetMT(ref _companyIndex, value);
                if(value >= 0)
                    LoadInvoiceByCompany(CompanyList[value]);
            }
        }
        public int YearIndex
        {
            get => _yearIndex;
            set
            {
                if (value == _yearIndex) return;
                SetMT(ref _yearIndex, value);
                if(value >= 0)
                    LoadInvoiceByYear(CompanyList[CompanyIndex], YearsList[value]);
            }
        }

        public RelayCommand OpenInvoicePurchaseCommand =>
            openInvoiceCreatorPurchase ??
            (openInvoiceCreatorPurchase = new RelayCommand(() =>
            {
                var invoice = new Invoice()
                {
                    Type = InvoiceType.PURCHASE,
                    SenderId = CompanyIndex >= 0 ? CompanyList[CompanyIndex].Id : 0,
                };
                Navigation.NavigateTo(ViewModelLocator.INVOICE_CREATION_HOME, invoice);
            }));
        public RelayCommand OpenInvoiceCreatorSellCommand =>
            openInvoiceCreatorSell ??
            (openInvoiceCreatorSell = new RelayCommand(() =>
            {
                var invoice = new Invoice()
                {
                    Type = InvoiceType.SELL,
                    SenderId = CompanyIndex >= 0 ? CompanyList[CompanyIndex].Id : 0,
                };
                Navigation.NavigateTo(ViewModelLocator.INVOICE_CREATION_HOME, invoice);
            }));
        public RelayCommand<Invoice> OpenInvoiceCreatorWithInvoiceCommand =>
            openInvoiceCreatorWithInvoice ??
            (openInvoiceCreatorWithInvoice = new RelayCommand<Invoice>((invoice) =>
            {
                Navigation.NavigateTo(ViewModelLocator.INVOICE_CREATION_HOME, invoice);
            }));
        private void LoadInvoiceByCompany(Company company)
        {
            var invoices = testInvoice;
            var years = invoices.Select(x => x.Emission.Year).Distinct().OrderByDescending(x => x);
            YearsList?.Clear();
            YearsList.AddRange(years);

            if (YearIndex < 0 && YearsList.Count > 0)
                YearIndex = 0;
        }
        private void LoadInvoiceByYear(Company company, int year)
        {
            var invoices = testInvoice;
            var invoices_year = invoices.Where(x => x.Emission.Year == year).OrderByDescending(x => x.Number).ThenByDescending(x => x.NumberExtra).ToList();

            var sell = invoices_year.Where(x => x.Type == InvoiceType.SELL);
            var purchase = invoices_year.Where(x => x.Type == InvoiceType.PURCHASE);

            InvoiceByYear.Clear();
            InvoiceByYear.AddRange(invoices_year);

            InvoicePurchase?.Clear();
            InvoicePurchase.AddRange(purchase);

            InvoiceSell?.Clear();
            InvoiceSell.AddRange(sell);
        }
        private List<Invoice> testInvoice = new List<Invoice>()
        {
            new Invoice(){ Emission = new DateTime(2018, 3, 28), Number = 3, Type = InvoiceType.SELL},
            new Invoice(){ Emission = new DateTime(2018, 2, 28), Number = 2, Type = InvoiceType.SELL},
            new Invoice(){ Emission = new DateTime(2018, 1, 28), Number = 1, Type = InvoiceType.PURCHASE},
            new Invoice(){ Emission = new DateTime(2017, 3, 28), Number = 3, Type = InvoiceType.SELL},
            new Invoice(){ Emission = new DateTime(2017, 2, 28), Number = 2, Type = InvoiceType.PURCHASE},
            new Invoice(){ Emission = new DateTime(2017, 1, 28), Number = 1, Type = InvoiceType.SELL},
            new Invoice(){ Emission = new DateTime(2015, 3, 28), Number = 3, Type = InvoiceType.PURCHASE},
            new Invoice(){ Emission = new DateTime(2015, 2, 28), Number = 2, Type = InvoiceType.SELL},
            new Invoice(){ Emission = new DateTime(2015, 1, 28), Number = 1, Type = InvoiceType.SELL},
        };
    }
}
