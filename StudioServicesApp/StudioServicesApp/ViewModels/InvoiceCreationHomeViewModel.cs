using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using pinoelefante.ViewModels;
using StudioServices.Data.Sqlite.Accounting;
using StudioServicesApp.Services;
using StudioServicesApp.Views;
using StudioServicesApp.Views.Framework;
using Xamarin.Forms;

namespace StudioServicesApp.ViewModels
{
    public class InvoiceCreationHomeViewModel : MyAuthViewModel
    {
        
        private RelayCommand nextPageCommand;

        public InvoiceCreationHomeViewModel(INavigationService n, StudioServicesApi a, AlertService al, KeyValueService k) : base(n, a, al, k) { }

        public override async Task NavigatedToAsync(object parameter = null)
        {
            base.PropertyChanged += InvoiceCreationHomeViewModel_PropertyChangedAsync;
            await base.NavigatedToAsync();
            Device.BeginInvokeOnMainThread(() =>
            {
                PopulateFields(parameter as Invoice);

                if (SelectedIndexInvoiceType < 0)
                    SelectedIndexInvoiceType = 0;
            });
        }
        private void InvoiceCreationHomeViewModel_PropertyChangedAsync(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                Debug.WriteLine("PropertyChanged: " + e.PropertyName);
                switch (e.PropertyName)
                {
                    case nameof(MyCompanies):
                        if (MyCompanies.Count == 0)
                        {
                            using (UserDialogs.Instance.Confirm(
                                new ConfirmConfig()
                                {
                                    Message = "Non sono presenti aziende.",
                                    Title = "Aggiungi azienda",
                                    OkText = "Aggiungi",
                                    CancelText = "Indietro",
                                    OnAction = (res) =>
                                    {
                                        if (res)
                                            OpenAddCompanyPopup.Execute(false);
                                        else
                                            Navigation.NavigateTo(ViewModelLocator.NEWS_PAGE);
                                    }
                                })) { }
                        }
                        else if (MyCompanies.Count != ListCompanies.Count)
                        {
                            ListCompanies.AddRange(MyCompanies, true);
                            SelectedMyCompanyIndex = SelectedMyCompany != null ? ListCompanies.IndexOf(SelectedMyCompany) : (ListCompanies.Count > 0 ? 0 : -1);
                        }
                        break;
                    case nameof(ClientsSuppliers):
                        SearchClientSupplierCommand.Execute(null);
                        break;
                }
            });
        }
        public override void NavigatedFrom()
        {
            base.PropertyChanged -= InvoiceCreationHomeViewModel_PropertyChangedAsync;
            Cleanup();
        }

        private DateTime invoiceDate = DateTime.Now;
        private int invoiceTypeIndex = -1, myCompanyIndex = -1;
        private Company selectedCompany = null, myCompany = null;
        private string invoiceNumberText = string.Empty, invoiceNumberTextExtra = string.Empty;
        public int SelectedMyCompanyIndex
        {
            get => myCompanyIndex;
            set
            {
                if (myCompanyIndex != value)
                {
                    SetMT(ref myCompanyIndex, value);
                    InitInvoiceNumber();
                }
            }
        }
        public int SelectedIndexInvoiceType
        {
            get => invoiceTypeIndex;
            set
            {
                if (invoiceTypeIndex != value)
                {
                    SetMT(ref invoiceTypeIndex, value);
                    InitInvoiceNumber();
                }
            }
        }
        public DateTime InvoiceDate
        {
            get => invoiceDate;
            set
            {
                SetMT(ref invoiceDate, value);
                InitInvoiceNumber();
            }
        }
        public Company SelectedMyCompany
        {
            get => myCompany;
            set
            {
                SetMT(ref myCompany, value);
                InitInvoiceNumber();
            }
        }
        public Company SelectedCompany
        {
            get => selectedCompany;
            set => SetMT(ref selectedCompany, value);
        }
        public MyObservableCollection<Company> ListCompanies { get; } = new MyObservableCollection<Company>();
        public MyObservableCollection<Company> CompanyList { get; } = new MyObservableCollection<Company>();
        public string InvoiceNumberText { get => invoiceNumberText; set => SetMT(ref invoiceNumberText, IntValidation(invoiceNumberText, value)); }
        public string InvoiceNumberExtraText { get => invoiceNumberTextExtra; set => SetMT(ref invoiceNumberTextExtra, StringValidation(InvoiceNumberExtraText, value, 5)); }

        private void InitInvoiceNumber()
        {
            var invoiceType = (InvoiceType)Enum.ToObject(typeof(InvoiceType), SelectedIndexInvoiceType);
            var invoiceYear = InvoiceDate.Year;
            InvoiceNumberExtraText = string.Empty;
            switch (invoiceType)
            {
                case InvoiceType.PURCHASE:
                    InvoiceNumberText = string.Empty;
                    break;
                case InvoiceType.SELL:
                    var number = GetNextInvoiceNumber(invoiceYear);
                    InvoiceNumberText = $"{number}";
                    break;
            }
        }
        private void PopulateFields(Invoice invoice)
        {
            if (invoice == null)
                return;
            InvoiceDate = invoice.Emission;
            InvoiceNumberText = invoice.Number.ToString();
            InvoiceNumberExtraText = invoice.NumberExtra;
            SelectedIndexInvoiceType = (int)invoice.Type;
            SelectedMyCompanyIndex = ListCompanies.IndexOf(MyCompanies.First(x => x.Id == invoice.SenderId));
            SelectedCompany = ClientsSuppliers.FirstOrDefault(x => x.Id == invoice.Recipient);
        }
        private int GetNextInvoiceNumber(int year)
        {
            // TODO implement
            return 1;
        }
        public RelayCommand NextInvoicePageCommand => nextPageCommand ??
            (nextPageCommand = new RelayCommand(() =>
            {
                if (VerifyInvoiceHome())
                {
                    Invoice invoice = new Invoice()
                    {
                        Emission = InvoiceDate,
                        Number = Int32.Parse(InvoiceNumberText),
                        NumberExtra = InvoiceNumberExtraText,
                        SenderId = SelectedMyCompany.Id,
                        Recipient = SelectedCompany.Id,
                        Type = (InvoiceType)Enum.ToObject(typeof(InvoiceType), SelectedIndexInvoiceType)
                    };
                    Navigation.NavigateTo(ViewModelLocator.INVOICE_CREATION_DETAILS, invoice);
                }
            }));
        private bool VerifyInvoiceHome()
        {
            if(SelectedMyCompany == null)
            {
                ShowMessage("Seleziona la tua azienda");
                return false;
            }
            if(SelectedIndexInvoiceType < 0 || SelectedIndexInvoiceType > 1)
            {
                ShowMessage("Seleziona il tipo di fattura");
                return false;
            }
            //DataFattura

            // TODO: verifica che il numero della fattura non esista
            if (string.IsNullOrEmpty(IntValidation(string.Empty, InvoiceNumberText, false)))
            {
                ShowMessage("Inserisci un numero di fattura valido");
                return false;
            }
            //NumeroExtra
            if(SelectedCompany == null)
            {
                ShowMessage("Seleziona il cliente/fornitore");
                return false;
            }
            return true;
        }
        private RelayCommand<string> searchCSCmd;
        public RelayCommand<string> SearchClientSupplierCommand =>
            searchCSCmd ??
            (searchCSCmd = new RelayCommand<string>(search =>
            {
                CompanyList.Clear();
                if(string.IsNullOrEmpty(search))
                    CompanyList.AddRange(base.ClientsSuppliers);
                else
                {
                    var lowersearch = search.ToLower();
                    var res = ClientsSuppliers.Where(x => x.Name.ToLower().Contains(lowersearch) || x.VATNumber.StartsWith(lowersearch));
                    CompanyList.AddRange(res);
                }
            }));
        public override void Cleanup()
        {
            ListCompanies.Clear();
            CompanyList.Clear();
            SelectedCompany = null;
            InvoiceNumberText = string.Empty;
            InvoiceNumberExtraText = string.Empty;
            base.Cleanup();
        }
    }
}
