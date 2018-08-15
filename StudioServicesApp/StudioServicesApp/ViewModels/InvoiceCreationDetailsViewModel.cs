using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using StudioServices.Data.Sqlite.Accounting;
using StudioServicesApp.Services;

namespace StudioServicesApp.ViewModels
{
    public class InvoiceCreationDetailsViewModel : MyAuthViewModel
    {
        public InvoiceCreationDetailsViewModel(INavigationService n, StudioServicesApi a, AlertService al, KeyValueService k) : base(n, a, al, k)
        {
        }
        private Invoice _currInvoice;
        private Company myCompany, recipientCompany;
        public Invoice CurrentInvoice { get => _currInvoice; set => SetMT(ref _currInvoice, value); }
        public Company MyCompany { get => myCompany; set => SetMT(ref myCompany, value); }
        public Company RecipientCompany { get => recipientCompany; set => SetMT(ref recipientCompany, value); }
        public MyObservableCollection<InvoiceDetail> InvoiceDetails { get; } = new MyObservableCollection<InvoiceDetail>();

        public override async Task NavigatedToAsync(object parameter = null)
        {
            await base.NavigatedToAsync();
            CurrentInvoice = parameter as Invoice;
            MyCompany = MyCompanies.FirstOrDefault(x => x.Id == CurrentInvoice.SenderId);
            RecipientCompany = ClientsSuppliers.FirstOrDefault(x => x.Id == CurrentInvoice.Recipient);
        }
        private RelayCommand _openAddInvoiceProductCmd;

        public RelayCommand OpenAddInvoiceDetailCommand =>
            _openAddInvoiceProductCmd ??
            (_openAddInvoiceProductCmd = new RelayCommand(() =>
            {

            }));
    }
}
