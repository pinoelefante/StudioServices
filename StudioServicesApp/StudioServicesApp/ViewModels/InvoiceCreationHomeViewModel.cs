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
        public InvoiceCreationHomeViewModel(INavigationService n, StudioServicesApi a, AlertService al, KeyValueService k) : base(n, a, al, k) { }

        private Invoice invoice;
        private DateTime invoiceDate = DateTime.Now;
        private Company selectedCompany = null;
        private string invoiceNumberText = string.Empty, invoiceNumberTextExtra = string.Empty;

        private RelayCommand nextPageCommand;
        private RelayCommand<string> searchClientSupplierCmd;

        public MyObservableCollection<Company> CompanyList { get; } = new MyObservableCollection<Company>();
        public string InvoiceNumberText { get => invoiceNumberText; set => SetMT(ref invoiceNumberText, IntValidation(invoiceNumberText, value)); }
        public string InvoiceNumberExtraText { get => invoiceNumberTextExtra; set => SetMT(ref invoiceNumberTextExtra, StringValidation(InvoiceNumberExtraText, value, 5)); }

        public Invoice Invoice { get => invoice; set => SetMT(ref invoice, value); }

        public override async Task NavigatedToAsync(object parameter = null)
        {
            await base.NavigatedToAsync();
            Invoice = parameter as Invoice;

            Device.BeginInvokeOnMainThread(() =>
            {
                PopulateFields(Invoice);
                SearchClientSupplierCommand.Execute(null);
            });
        }
        public override void RegisterMessenger()
        {
            MessengerInstance.Register<Company>(this, MSG_MY_COMPANY_CLIENT_ADD, (company) =>
            {
                SearchClientSupplierCommand.Execute(null);
            });
        }
        public override void UnregisterMessenger()
        {
            MessengerInstance.Unregister<Company>(this, MSG_MY_COMPANY_CLIENT_DEL);
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
        public Company SelectedCompany
        {
            get => selectedCompany;
            set => SetMT(ref selectedCompany, value);
        }
        
        private void InitInvoiceNumber()
        {
            if (Invoice == null)
                return;
            var invoiceYear = InvoiceDate.Year;
            InvoiceNumberExtraText = string.Empty;
            switch (Invoice.Type)
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
            Cleanup();

            InvoiceDate = invoice.Emission;
            InvoiceNumberText = invoice.Number.ToString();
            InvoiceNumberExtraText = invoice.NumberExtra;
            SelectedCompany = ClientsSuppliers.FirstOrDefault(x => x.Id == invoice.Recipient);
        }
        private int GetNextInvoiceNumber(int year)
        {
            // TODO implement
            return 1;
        }
        public RelayCommand NextInvoicePageCommand => nextPageCommand ??
            (nextPageCommand = new RelayCommand(async () =>
            {
                if (await VerifyInvoiceHomeAsync())
                {
                    Invoice.Emission = InvoiceDate;
                    Invoice.Number = Int32.Parse(InvoiceNumberText);
                    Invoice.NumberExtra = InvoiceNumberExtraText;
                    Invoice.Recipient = SelectedCompany.Id;

                    Navigation.NavigateTo(ViewModelLocator.INVOICE_CREATION_DETAILS, Invoice);
                }
            }));
        private async Task<bool> VerifyInvoiceHomeAsync()
        {
            //DataFattura
            
            // TODO: verifica che il numero della fattura non esista (anno - numero fattura - numero extra)
            if (string.IsNullOrEmpty(IntValidation(string.Empty, InvoiceNumberText, false)))
            {
                ShowMessage("Inserisci un numero di fattura valido");
                return false;
            }
            if(InvoiceDate.Year < DateTime.Now.Year)
            {
                var confirm = await UserDialogs.Instance.ConfirmAsync($"Stai inserendo una fattura del {InvoiceDate.Year} ma siamo nel {DateTime.Now.Year}. Vuoi continuare?", "Verifica anno della fattura", "Si", "No");
                if (!confirm)
                    return false;
            }

            // NumeroExtra
            if (SelectedCompany == null)
            {
                ShowMessage("Seleziona il cliente/fornitore");
                return false;
            }
            return true;
        }
        public RelayCommand<string> SearchClientSupplierCommand =>
            searchClientSupplierCmd ??
            (searchClientSupplierCmd = new RelayCommand<string>(search =>
            {
                if(string.IsNullOrEmpty(search))
                    CompanyList.AddRange(base.ClientsSuppliers, true);
                else
                {
                    var lowersearch = search.ToLower();
                    var res = ClientsSuppliers.Where(x => x.Name.ToLower().Contains(lowersearch) || x.VATNumber.Contains(lowersearch));
                    CompanyList.AddRange(res, true);
                }
            }));

        public override void Cleanup()
        {
            CompanyList.Clear();
            SelectedCompany = null;
            InvoiceNumberText = string.Empty;
            InvoiceNumberExtraText = string.Empty;
            base.Cleanup();
        }
    }
}
