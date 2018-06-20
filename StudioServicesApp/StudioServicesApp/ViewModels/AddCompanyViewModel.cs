using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using pinoelefante.ViewModels;
using StudioServices.Data.Sqlite.Accounting;
using StudioServices.Data.Sqlite.Registry;
using StudioServicesApp.Services;
using StudioServicesApp.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace StudioServicesApp.ViewModels
{
    public class AddCompanyViewModel : MyAuthViewModel
    {
        private string companyName, vatNumber;
        private Address selectedAddress;
        public AddCompanyViewModel(INavigationService n, StudioServicesApi a, AlertService alert, KeyValueService k) : base(n, a, alert, k)
        {
            OnClosePopupMessengerToken = "AddCompanyStatus";
        }

        public override async Task NavigatedToAsync(object parameter = null)
        {
            MessengerInstance.Register<bool>(this, "AddAddressStatus", async (x) =>
            {
                await LoadPersonAsync(true);
                await NavigatedToAsync();
            });
            await base.NavigatedToAsync();
            Device.BeginInvokeOnMainThread(() =>
            {
                ListAddresses.Clear();
                ListAddresses.AddRange(Persona?.GetPersonAddresses());
            });
        }
        public override void NavigatedFrom()
        {
            MessengerInstance.Unregister<bool>(this, "AddAddressStatus");
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
                var company = new Company()
                {
                    // Address = SelectedAddress,
                    AddressId = SelectedAddress.Id,
                    Name = CompanyName,
                    PersonId = Persona.Id,
                    VATNumber = VatNumber
                };
                var res = await SendRequestAsync(async () => await api.Warehouse_SaveCompanyAsync(company));
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
                Navigation.PushPopupAsync(new AddAddressPopup());
            }));
    }
}
