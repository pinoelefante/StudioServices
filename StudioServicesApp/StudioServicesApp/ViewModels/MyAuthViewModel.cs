using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using pinoelefante.ViewModels;
using StudioServices.Data.Sqlite.Accounting;
using StudioServices.Data.Sqlite.Registry;
using StudioServicesApp.Services;
using StudioServicesApp.Views;
using StudioServicesApp.Views.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Reflection;
using StudioServices.Data.Sqlite;

namespace StudioServicesApp.ViewModels
{
    public class MyAuthViewModel : MyViewModel
    {
        public static readonly string CACHE_MY_COMPANIES = "MyCompaniesCache",
            CACHE_IS_ADMIN = "IsAdminCache",
            CACHE_PERSON = "Person",
            CACHE_CLIENTS_SUPPLIERS = "ClientsSuppliers";

        private Person _person;
        private bool _isAdmin;

        public Person Persona { get => _person; set => SetMT(ref _person, value); }
        public MyObservableCollection<Company> MyCompanies { get; } = new MyObservableCollection<Company>();
        public Dictionary<int, List<CompanyProduct>> ProductList { get; } = new Dictionary<int, List<CompanyProduct>>();
        public MyObservableCollection<Company> ClientsSuppliers { get; } = new MyObservableCollection<Company>();

        public bool IsAdmin { get => _isAdmin; set => SetMT(ref _isAdmin, value); }
        public bool VerifyPersonEnabled { get; set; } = true;

        public MyAuthViewModel(INavigationService n, StudioServicesApi a, AlertService al, KeyValueService k) : base(n, a, al, k) { }

        public override Task NavigatedToAsync(object parameter = null)
        {
            return Task.Factory.StartNew(async () =>
            {
                await base.NavigatedToAsync();
                await LoadPersonAsync();
                await LoadCompaniesAsync();
                await LoadClientsSuppliersAsync();
                if(VerifyPersonEnabled)
                    CheckPerson();
            });
        }
        public override void RegisterMessenger()
        {
            MessengerInstance.Register<Address>(this, MSG_PERSON_ADD_ADDRESS, (address) =>
            {
                lock (Persona)
                {
                    if (Persona.Addresses.FirstOrDefault(x => x.Id == address.Id) == null)
                        Persona.Addresses.Add(address);
                }
                RaisePropertyChangedMT(() => Persona);
                RaisePropertyChangedMT(() => Persona.Addresses);
            });
            MessengerInstance.Register<ContactMethod>(this, MSG_PERSON_ADD_CONTACT, (contact) =>
            {
                lock (Persona)
                {
                    if (Persona.Contacts.FirstOrDefault(x => x.Id == contact.Id) == null)
                        Persona.Contacts.Add(contact);
                }
                RaisePropertyChangedMT(() => Persona);
                RaisePropertyChangedMT(() => Persona.Contacts);
            });
            MessengerInstance.Register<Email>(this, MSG_PERSON_ADD_EMAIL, (email) =>
            {
                lock (Persona)
                {
                    if (Persona.Emails.FirstOrDefault(x => x.Id == email.Id) == null)
                        Persona.Emails.Add(email);
                }
                Persona.Emails.Add(email);
                RaisePropertyChangedMT(() => Persona);
                RaisePropertyChangedMT(() => Persona.Emails);
            });
            MessengerInstance.Register<IdentificationDocument>(this, MSG_PERSON_ADD_DOCUMENT, (doc) =>
            {
                lock (Persona)
                {
                    if(Persona.Identifications.FirstOrDefault(x => x.Id == doc.Id) == null)
                        Persona.Identifications.Add(doc);
                }
            });
            MessengerInstance.Register<Company>(this, MSG_MY_COMPANY_ADD, (company) =>
            {
                if (company == null)
                    return;

                if (company.IsClient)
                {
                    var clientsList = ClientsSuppliers.ToList();
                    clientsList.Add(company);
                    clientsList.Sort(companyComparerByName);
                    var index = clientsList.IndexOf(company);
                    ClientsSuppliers.Insert(index, company);
                    RaisePropertyChanged(() => ClientsSuppliers);
                }
                else
                {
                    MyCompanies.Add(company);
                    RaisePropertyChanged(() => MyCompanies);
                }
            });
            MessengerInstance.Register<Address>(this, MSG_PERSON_DEL_ADDRESS, (address) =>
            {
                Persona.Addresses.Remove(address);
            });
            MessengerInstance.Register<ContactMethod>(this, MSG_PERSON_DEL_CONTACT, (contact) =>
            {
                Persona.Contacts.Remove(contact);
            });
            MessengerInstance.Register<Email>(this, MSG_PERSON_DEL_EMAIL, (email) =>
            {
                Persona.Emails.Remove(email);
            });
            MessengerInstance.Register<IdentificationDocument>(this, MSG_PERSON_DEL_DOCUMENT, (doc) =>
            {
                Persona.Identifications.Remove(doc);
            });
        }
        public override void UnregisterMessenger()
        {
            MessengerInstance.Unregister<Address>(this, MSG_PERSON_ADD_ADDRESS);
            MessengerInstance.Unregister<Address>(this, MSG_PERSON_DEL_ADDRESS);
            MessengerInstance.Unregister<ContactMethod>(this, MSG_PERSON_ADD_CONTACT);
            MessengerInstance.Unregister<ContactMethod>(this, MSG_PERSON_DEL_CONTACT);
            MessengerInstance.Unregister<Email>(this, MSG_PERSON_ADD_EMAIL);
            MessengerInstance.Unregister<Email>(this, MSG_PERSON_DEL_EMAIL);
            MessengerInstance.Unregister<IdentificationDocument>(this, MSG_PERSON_ADD_DOCUMENT);
            MessengerInstance.Unregister<IdentificationDocument>(this, MSG_PERSON_DEL_DOCUMENT);
        }
        public override void NavigatedFrom()
        {
            base.NavigatedFrom();
            SaveCache();
        }
        protected void SaveCache()
        {
            cache.SetValue(CACHE_PERSON, Persona);
            // TODO - complete saving
        }
        protected async Task LoadPersonAsync(bool force = false)
        {
            if (force || cache.GetValue<Person>(CACHE_PERSON, null) == null)
            {
                var res = await SendRequestAsync(async () => await api.Person_GetAsync());
                if (res.IsOK && res.Data != null)
                    cache.SetValue(CACHE_PERSON, res.Data);
                else
                {
                    ShowMessage("Non è stato possibile recuperare le informazioni del profilo", "Informazioni profilo");
                    return;
                }
            }
            Persona = cache.GetValue<Person>(CACHE_PERSON);
            if(Persona == null)
            {
                // TODO : Navigate to loginPage
                Debug.WriteLine("Catastrophic error: Persona = null");
            }
            MessengerInstance.Send<Person>(Persona, MSG_PERSON_LOADED);

            if (cache.GetValue<bool?>(CACHE_IS_ADMIN, null) == null)
            {
                var res = await SendRequestAsync(async () => await api.Admin_IsAdminAsync());
                if (res.IsOK)
                    cache.SetValue(CACHE_IS_ADMIN, res.Data);
            }
            var admin = cache.GetValue(CACHE_IS_ADMIN, false);
            IsAdmin = admin;
        }
        protected async Task LoadCompaniesAsync(bool force = false)
        {
            if (force || cache.GetValue<List<Company>>(CACHE_MY_COMPANIES, null) == null)
            {
                var res = await SendRequestAsync(async () => await api.Warehouse_GetMyCompaniesAsync());
                if (res.Code == ResponseCode.OK && res.Data != null && res.Data.Any())
                {
                    cache.SetValue(CACHE_MY_COMPANIES, res.Data);
                    MessengerInstance.Send<List<Company>>(res.Data, MSG_MY_COMPANY_LOADED);
                }
                else
                {
                    ShowMessage("Non è stato possibile recuperare le informazioni delle aziende", "Aziende");
                    return;
                }
            }
            var companies = cache.GetValue<List<Company>>(CACHE_MY_COMPANIES, null);
            MyCompanies.AddRange(companies, true);
            RaisePropertyChanged(() => MyCompanies);
        }
        protected async Task LoadClientsSuppliersAsync(bool force = false)
        {
            if (force || cache.GetValue<List<Company>>(CACHE_CLIENTS_SUPPLIERS, null) == null)
            {
                var res = await SendRequestAsync(async () => await api.Warehouse_ClientsSuppliersList());
                if (res.IsOK && res.Data != null)
                    cache.SetValue(CACHE_CLIENTS_SUPPLIERS, res.Data);
                else
                {
                    // not retrieved
                    return;
                }
            }
            var clients = cache.GetValue<List<Company>>(CACHE_CLIENTS_SUPPLIERS, null);
            ClientsSuppliers.AddRange(clients, true);
            RaisePropertyChanged(() => ClientsSuppliers);
        }
        protected void CheckPerson()
        {
            if (Persona == null)
            {
                // var logout = SendRequestAsync(async () => await api.Authentication_LogoutAsync()).Result;
                // App.Current.MainPage = new LoginPage();
                Debug.WriteLine("Person is null, but can't be null");
                throw new Exception("Person is null, but can't be null");
            }
            if(Persona.Identifications.Count == 0)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Navigation.NavigateTo(ViewModelLocator.ADD_IDENTIFICATION_DOC_PAGE);
                });
            }
        }

        private RelayCommand openAddAddressPopup, openAddContactPopup, openAddEmailPopup;
        private RelayCommand<string> openAddCompanyPopup;

        public RelayCommand OpenAddAddressPopupCommand =>
            openAddAddressPopup ??
            (openAddAddressPopup = new RelayCommand(() =>
            {
                Navigation.PushPopupAsync(new AddAddressPopup());
            }));
        public RelayCommand OpenAddContactPopupCommand =>
            openAddContactPopup ??
            (openAddContactPopup = new RelayCommand(() =>
            {
                Navigation.PushPopupAsync(new AddContactMethodPopup());
            }));
        public RelayCommand OpenAddEmailPopupCommand =>
            openAddEmailPopup ??
            (openAddEmailPopup = new RelayCommand(() =>
            {
                Navigation.PushPopupAsync(new AddEmailPopup());
            }));
        private static CompanyComparerByName companyComparerByName = new CompanyComparerByName();
        public RelayCommand<string> OpenAddCompanyPopup =>
            openAddCompanyPopup ??
            (openAddCompanyPopup = new RelayCommand<string>((isClientS) =>
            {
                bool isClient = bool.Parse(isClientS);
                Navigation.PushPopupAsync(new AddCompanyPopup(isClient));
            }));
    }
}
