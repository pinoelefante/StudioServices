using System;
using System.Collections.Generic;
using System.Text;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using StudioServices.Data.Sqlite.Registry;
using StudioServicesApp.Services;

namespace StudioServicesApp.ViewModels
{
    public class AddAddressViewModel : MyAuthViewModel
    {
        public AddAddressViewModel(INavigationService n, StudioServicesApi a, AlertService al, KeyValueService k) : base(n, a, al, k) { }

        private int typeIndex = 0;
        private string address, civicNum, city, province, zipcode, country = "IT", description;
        public int TypeIndex { get => typeIndex; set => SetMT(ref typeIndex, value); }
        public string Address { get => address; set => SetMT(ref address, value); }
        public string CivicNumber { get => civicNum; set => SetMT(ref civicNum, value); }
        public string City { get => city; set => SetMT(ref city, value); }
        public string Province { get => province; set => SetMT(ref province, StringValidation(province, value, 2)); }
        public string ZIPCode { get => zipcode; set => SetMT(ref zipcode, StringValidation(zipcode, value, 5)); }
        public string Country { get => country; set => SetMT(ref country, value); }
        public string Description { get => description; set => SetMT(ref description, value); }

        private RelayCommand addAddressCmd;
        public RelayCommand AddAddressCommand =>
            addAddressCmd ??
            (addAddressCmd = new RelayCommand(async () =>
            {
                var addr = new Address()
                {
                    AddressType = AddressType.HOME,
                    City = City,
                    CivicNumber = CivicNumber,
                    Country = Country,
                    Description = Description,
                    PersonId = Persona.Id,
                    Province = Province,
                    Street = Address,
                    ZipCode = ZIPCode
                };
                var res = await SendRequestAsync(() => api.Person_AddAddressAsync(addr));
                if (res.IsOK)
                {
                    addr.Id = res.Data;
                    MessengerInstance.Send<Address>(addr, MSG_PERSON_ADD_ADDRESS);
                    await Navigation.PopPopupAsync();
                }
                else
                    ShowMessage("Indirizzo non aggiunto. Riprova");
            }));
    }
}
