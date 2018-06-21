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
        private string currentPage = "0";
        private RelayCommand addCompanyCmd;
        private RelayCommand<string> navPage;
        private string address, civicNum, city, province, zipcode, nation, addressNote;
        private bool isClient;

        public AddCompanyViewModel(INavigationService n, StudioServicesApi a, AlertService alert, KeyValueService k) : base(n, a, alert, k)
        {
            OnClosePopupMessengerToken = "AddCompanyStatus";
        }
        public override async Task NavigatedToAsync(object parameter = null)
        {
            IsClient = parameter == null ? false : (bool)parameter;
            await base.NavigatedToAsync();
            CurrentPage = "0";
        }
        public bool IsClient { get => isClient; private set => SetMT(ref isClient, value); }
        public string CurrentPage { get => currentPage; set => SetMT(ref currentPage, value); }
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
        public string Address { get => address; set => SetMT(ref address, value); }
        public string CivicNumber { get => civicNum; set => SetMT(ref civicNum, value); }
        public string City { get => city; set => SetMT(ref city, value); }
        public string Province { get => province; set => SetMT(ref province, value); }
        public string ZIPCode { get => zipcode; set => SetMT(ref zipcode, value); }
        public string Country { get => nation; set => SetMT(ref nation, value); }
        public string Description { get => addressNote; set => SetMT(ref addressNote, value); }
        public RelayCommand AddCompanyCommand =>
            addCompanyCmd ??
            (addCompanyCmd = new RelayCommand(async () =>
            {
                var address = new Address()
                {
                    AddressType = AddressType.WORK,
                    City = City,
                    CivicNumber = CivicNumber,
                    Country = Country,
                    Description = Description,
                    PersonId = Persona.Id,
                    Province = Province,
                    Street = Address,
                    ZipCode = ZIPCode
                };
                var company = new Company()
                {
                    Address = address,
                    AddressId = 0,
                    Name = CompanyName,
                    PersonId = Persona.Id,
                    VATNumber = VatNumber,
                    IsClient = IsClient
                };

                if(IsClient)
                {
                    var res = await SendRequestAsync(async () => await api.Warehouse_SaveClientSupplier(company));
                    if (res.IsOK && res.Data != null)
                    {
                        MessengerInstance.Send(res.Data, "AddCompanyInvoiceStatus");
                        await Navigation.PopPopupAsync();
                    }
                    else
                        ShowMessage("Non è stato possibile aggiungere l'azienda", "Errore");
                }
                else
                {
                    var res = await SendRequestAsync(async () => await api.Warehouse_SaveCompanyAsync(company));
                    if (res.IsOK)
                    {
                        MessengerInstance.Send<bool>(res.Data, "AddCompanyStatus");
                        await Navigation.PopPopupAsync();
                    }
                    else
                        ShowMessage("Azienda non aggiunta");
                }
                
            }));
        public RelayCommand<string> GoPage =>
            navPage ??
            (navPage = new RelayCommand<string>((pageIndex) =>
            {
                CurrentPage = pageIndex;
            }));
    }
}
