using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using pinoelefante.ViewModels;
using StudioServices.Data.Accounting;
using StudioServicesApp.Services;
using Xamarin.Forms;

namespace StudioServicesApp.ViewModels
{
    public class InvoiceCreationViewModel : MyViewModel
    {
        private RelayCommand<string> nextPageCommand;
        public InvoiceCreationViewModel(INavigationService n, StudioServicesApi a, AlertService al) : base(n, a, al) { }

        public override Task NavigatedToAsync(object parameter = null)
        {
            if (SelectedIndexInvoiceType < 0)
                SelectedIndexInvoiceType = 0;
            return base.NavigatedToAsync(parameter);
        }

        private DateTime invoiceDate = DateTime.Now;
        private int invoiceTypeIndex = -1;
        private Company selectedCompany = null, myCompany = null;
        private string invoiceNumberText = string.Empty, invoiceNumberTextExtra = string.Empty;
        
        public int SelectedIndexInvoiceType
        {
            get => invoiceTypeIndex;
            set
            {
                if (invoiceTypeIndex != value)
                {
                    SetMT(ref invoiceTypeIndex, value);
                    SelectedCompany = null;
                    if (value >= 0)
                        PopulateCompanyList(value == 0 ? InvoiceType.SELL : InvoiceType.PURCHASE);
                    InitFields();
                }
            }
        }
        public DateTime InvoiceDate { get => invoiceDate; set => SetMT(ref invoiceDate, value); }
        public Company SelectedMyCompany { get => myCompany; set => SetMT(ref myCompany, value); }
        public Company SelectedCompany { get => selectedCompany; set => SetMT(ref selectedCompany, value); }
        public MyObservableCollection<Company> MyCompanies { get; } = new MyObservableCollection<Company>() { new Company() { Name = "Azienda di prova", VATNumber = "0123456789" } };
        public MyObservableCollection<Company> CompanyList { get; } = new MyObservableCollection<Company>();
        public string InvoiceNumberText { get => invoiceNumberText; set => SetMT(ref invoiceNumberText, value); }
        public string InvoiceNumberExtraText { get => invoiceNumberTextExtra; set => SetMT(ref invoiceNumberTextExtra, value); }

        private List<Company> fullListSell, fullListPurchase;
        private void PopulateCompanyList(InvoiceType invoiceType)
        {
            CompanyList.Clear();
            switch (invoiceType)
            {
                case InvoiceType.PURCHASE:
                    fullListPurchase = new List<Company>()
                    {
                        new Company(){Name="Elesoft", VATNumber="012345678912345" },
                        new Company(){Name="Softele", VATNumber="012345678912345" }
                    };
                    CompanyList.AddRange(fullListPurchase);
                    break;
                case InvoiceType.SELL:
                    fullListSell = new List<Company>()
                    {
                        new Company(){Name="Elesell", VATNumber="012345678912345" },
                        new Company(){Name="Softell", VATNumber="012345678912345" }
                    };
                    CompanyList.AddRange(fullListSell);
                    break;
            }
        }
        private void InitFields()
        {

        }
        public RelayCommand<string> NextInvoicePageCommand => nextPageCommand ??
            (nextPageCommand = new RelayCommand<string>((pageIndex) =>
            {
                switch (pageIndex)
                {
                    case "invoice_details":
                        navigation.NavigateTo(ViewModelLocator.INVOICE_CREATION_DETAILS);
                        break;
                }
                Debug.WriteLine($"PageIndex: {pageIndex}");
            }));
        public RelayCommand ReloadMyCompaniesCommand => null;

        public MyObservableCollection<string> InvoiceDetails { get; } = new MyObservableCollection<string>() { "Mele", "Pere", "Banane" };
    }
}
