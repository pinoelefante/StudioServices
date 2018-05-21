using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using pinoelefante.ViewModels;
using StudioServices.Data.Registry;
using StudioServicesApp.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudioServicesApp.ViewModels
{
    public class InvoiceCreateCompanyViewModel : MyViewModel
    {
        private string companyName, vatNumber;
        private Address selectedAddress;
        public InvoiceCreateCompanyViewModel(INavigationService n, StudioServicesApi a, AlertService alert) : base(n, a, alert)
        {
        }

        public string CompanyName
        {
            get => companyName;
            set => SetMT(ref companyName, StringValidation(companyName, value, -1, (x) => 
                {
                    return !string.IsNullOrEmpty(x.Trim());
                }
            ));
        }
        public string VatNumber { get => vatNumber; set => SetMT(ref vatNumber, StringValidation(vatNumber, value, 16)); }
        public Address SelectedAddress { get => selectedAddress; set => SetMT(ref selectedAddress, value); }
        public MyObservableCollection<Address> ListAddresses { get; } = new MyObservableCollection<Address>()
        {
            new Address()
            {
                AddressType = AddressType.HOME,
                City = "Santa Maria La Carità",
                CivicNumber = "290",
                Country = "IT",
                Province = "NA",
                Street = "Via Visitazione"
            }
        };
        private RelayCommand addCompanyCmd;
        public RelayCommand AddCompanyCommand =>
            addCompanyCmd ??
            (addCompanyCmd = new RelayCommand(() =>
            {
                // TODO controllare che non esista già

            }));
    }
}
