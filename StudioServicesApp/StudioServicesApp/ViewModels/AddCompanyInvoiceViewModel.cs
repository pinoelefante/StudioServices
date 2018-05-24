using System;
using System.Collections.Generic;
using System.Text;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using StudioServices.Data.Accounting;
using StudioServicesApp.Services;

namespace StudioServicesApp.ViewModels
{
    public class AddCompanyInvoiceViewModel : MyAuthViewModel
    {
        public AddCompanyInvoiceViewModel(INavigationService n, StudioServicesApi a, AlertService al, KeyValueService k) : base(n, a, al, k)
        {
        }
        private string companyName, vatNumber, street, civicNumber, city, province, zipcode, country;
        public string CompanyName { get => companyName; set => SetMT(ref companyName, value); }
        public string VatNumber { get => vatNumber; set => SetMT(ref vatNumber, value); }
        public string Street { get => street; set => SetMT(ref street, value); }
        public string CivicNumber { get => civicNumber; set => SetMT(ref civicNumber, value); }
        public string City { get => city; set => SetMT(ref city, value); }
        public string Province { get => province; set => SetMT(ref province, value); }
        public string ZipCode { get => zipcode; set => SetMT(ref zipcode, value); }
        public string Country { get => country; set => SetMT(ref country, value); }

        private RelayCommand addCompanyCmd;
        public RelayCommand AddCompanyCommand =>
            addCompanyCmd ??
            (addCompanyCmd = new RelayCommand(async () =>
            {
                var res = await SendRequestAsync(async () => await api.Warehouse_SaveClientSupplier(CompanyName, VatNumber, Country, City, Province, Street, CivicNumber, ZipCode, 0, 0));
                if (res.IsOK && res.Data != null)
                {
                    MessengerInstance.Send(res.Data, "AddCompanyInvoiceStatus");
                    await Navigation.PopPopupAsync();
                }
                else
                    ShowMessage("Non è stato possibile aggiungere l'azienda","Errore");
            }));
    }
}
