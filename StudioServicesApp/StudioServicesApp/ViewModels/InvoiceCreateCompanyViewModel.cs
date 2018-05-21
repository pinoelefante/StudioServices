using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using pinoelefante.ViewModels;
using StudioServices.Data.Registry;
using StudioServicesApp.Services;
using StudioServicesApp.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace StudioServicesApp.ViewModels
{
    public class InvoiceCreateCompanyViewModel : MyAuthViewModel
    {
        private string companyName, vatNumber;
        private Address selectedAddress;
        public InvoiceCreateCompanyViewModel(INavigationService n, StudioServicesApi a, AlertService alert) : base(n, a, alert) { }

        public override async Task NavigatedToAsync(object parameter = null)
        {
            await base.NavigatedToAsync();
            Device.BeginInvokeOnMainThread(() =>
            {
                ListAddresses.Clear();
                ListAddresses.AddRange(Persona?.Addresses);
            });
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
        public MyObservableCollection<Address> ListAddresses { get; } = new MyObservableCollection<Address>();
        private RelayCommand addCompanyCmd, openAddAddressCmd;
        public RelayCommand AddCompanyCommand =>
            addCompanyCmd ??
            (addCompanyCmd = new RelayCommand(async () =>
            {
                // TODO controllare che non esista già
                var res = await SendRequestAsync(async () => await api.Warehouse_SaveCompanyAsync(CompanyName, VatNumber, SelectedAddress.Id));
                if (res.IsOK)
                {
                    MessengerInstance.Send<bool>(res.Data, "AddCompanyStatus");
                    await Navigation.PopPopupAsync();
                }
                else
                    ShowMessage("Azienda non aggiunta");
            }));
        public RelayCommand OpenAddAddressPopup =>
            openAddAddressCmd ??
            (openAddAddressCmd = new RelayCommand(() =>
            {
                MessengerInstance.Register<bool>(this, "AddAddressStatus", async (x) => 
                {
                    MessengerInstance.Unregister<bool>(this, "AddAddressStatus");
                    await LoadPersonAsync(true);
                    await NavigatedToAsync();
                });
                Navigation.PushPopupAsync(new AddAddressPopup());
            }));
    }
}
