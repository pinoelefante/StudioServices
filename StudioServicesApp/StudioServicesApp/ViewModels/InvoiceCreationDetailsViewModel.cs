using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using StudioServices.Data.Sqlite.Accounting;
using StudioServicesApp.Services;
using StudioServicesApp.Views;
using System.Diagnostics;

namespace StudioServicesApp.ViewModels
{
    public class InvoiceCreationDetailsViewModel : MyAuthViewModel
    {
        public InvoiceCreationDetailsViewModel(INavigationService n, StudioServicesApi a, AlertService al, KeyValueService k) : base(n, a, al, k)
        {
        }
        private Invoice _currInvoice;
        private Company recipientCompany;

        private RelayCommand _openAddInvoiceProductCmd, saveInvoiceCmd;

        public Invoice Invoice { get => _currInvoice; set => SetMT(ref _currInvoice, value); }
        public Company RecipientCompany { get => recipientCompany; set => SetMT(ref recipientCompany, value); }
        public MyObservableCollection<InvoiceDetail> InvoiceDetails { get; } = new MyObservableCollection<InvoiceDetail>();

        public override async Task NavigatedToAsync(object parameter = null)
        {
            await base.NavigatedToAsync();
            Invoice = parameter as Invoice;
            RecipientCompany = ClientsSuppliers.FirstOrDefault(x => x.Id == Invoice.Recipient);
        }

        public override void RegisterMessenger()
        {
            MessengerInstance.Register<InvoiceDetail>(this, MSG_INVOICE_DETAIL_ADD, (detail) =>
            {
                // controllare non ci sia un altro dettaglio con lo stesso prodotto, prezzo, tasse
                InvoiceDetails.Add(detail);
            });
        }
        public override void UnregisterMessenger()
        {
            MessengerInstance.Unregister<InvoiceDetail>(this, MSG_INVOICE_DETAIL_ADD);
        }

        public RelayCommand OpenAddInvoiceDetailCommand =>
            _openAddInvoiceProductCmd ??
            (_openAddInvoiceProductCmd = new RelayCommand(() =>
            {
                Navigation.PushPopupAsync(new AddInvoiceProductPopup(Invoice.Sender));
            }));
        public RelayCommand SaveInvoiceCommand =>
            saveInvoiceCmd ??
            (saveInvoiceCmd = new RelayCommand(async () =>
            {
                Invoice.InvoiceDetails.AddRange(InvoiceDetails);

                var response = await SendRequestAsync(async () => await api.Warehouse_SaveInvoice(Invoice));
                Debug.WriteLine(response.Code);
            }));
    }
}
