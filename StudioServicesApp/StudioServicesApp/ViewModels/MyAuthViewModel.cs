using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using pinoelefante.ViewModels;
using StudioServices.Data.Sqlite.Accounting;
using StudioServices.Data.Sqlite.Registry;
using StudioServicesApp.Services;
using StudioServicesApp.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace StudioServicesApp.ViewModels
{
    public class MyAuthViewModel : MyViewModel
    {
        private Person _person;
        public Person Persona { get => _person; set => SetMT(ref _person, value); }
        public Dictionary<Company, List<CompanyProduct>> MyCompanies { get; private set; } = new Dictionary<Company, List<CompanyProduct>>();
        public List<Company> ClientsSuppliers { get; private set; } = new List<Company>();

        private bool _isAdmin;
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
        public override void NavigatedFrom()
        {
            base.NavigatedFrom();
        }
        protected async Task LoadPersonAsync(bool force = false)
        {
            if (force || cache.GetValue<Person>("person", null) == null)
            {
                var busy_message = "Recupero profilo in corso";
                SetBusy(true, busy_message);
                var res = await SendRequestAsync(async () => await api.Person_GetAsync());
                SetBusy(false, busy_message);
                if (res.Code == ResponseCode.OK && res.Data != null)
                    cache.SetValue("person", res.Data);
                else
                    ShowMessage("Non è stato possibile recuperare le informazioni del profilo", "Informazioni profilo");
            }
            Persona = cache.GetValue<Person>("person");

            if (cache.GetValue<bool?>("is_admin", null) == null)
            {
                var res = await SendRequestAsync(async () => await api.Admin_IsAdminAsync());
                if (res.Code == ResponseCode.OK)
                    cache.SetValue("is_admin", res.Data);
            }
            var admin = cache.GetValue("is_admin", false);
            IsAdmin = admin;
        }
        protected async Task LoadCompaniesAsync(bool force = false)
        {
            if(force || cache.GetValue<Dictionary<Company,List<CompanyProduct>>>("my_companies",null) == null)
            {
                var res = await SendRequestAsync(async () => await api.Warehouse_GetMyCompaniesAsync());
                if (res.Code == ResponseCode.OK && res.Data != null)
                {
                    MyCompanies.Clear();
                    foreach(var c in res.Data)
                        MyCompanies.Add(c, new List<CompanyProduct>());
                    cache.SetValue("my_companies", MyCompanies);
                }
                else
                    ShowMessage("Non è stato possibile recuperare le informazioni delle aziende", "Aziende");
            }
            else
                MyCompanies = cache.GetValue<Dictionary<Company, List<CompanyProduct>>>("my_companies", new Dictionary<Company, List<CompanyProduct>>());
        }
        protected async Task LoadClientsSuppliersAsync(bool force = false)
        {
            if (force || cache.GetValue<List<Company>>("clients_suppliers", null) == null)
            {
                var res = await SendRequestAsync(async () => await api.Warehouse_ClientsSuppliersList());
                if (res.IsOK && res.Data != null)
                {
                    cache.SetValue("clients_suppliers", res.Data);
                    ClientsSuppliers.Clear();
                    ClientsSuppliers.AddRange(res.Data);
                }
            }
            else
                ClientsSuppliers = cache.GetValue<List<Company>>("clients_suppliers");
        }
        public List<Company> GetMyCompaniesList()
        {
            List<Company> list = new List<Company>();
            if(MyCompanies!=null)
            {
                foreach (var company in MyCompanies.Keys)
                    list.Add(company);
            }
            list.TrimExcess();
            return list;
        }
        protected void CheckPerson()
        {
            if (Persona == null)
                throw new Exception("Person is null, but can't be null");
            if(Persona.Identifications.Count == 0)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Navigation.NavigateTo(ViewModelLocator.ADD_IDENTIFICATION_DOC_PAGE);
                });
            }
        }
        public Company GetMyCompany(int id)
        {
            foreach (var k in MyCompanies.Keys)
                if (k.Id == id)
                    return k;
            return null;
        }
        public Company GetClientSupplier(int id) => ClientsSuppliers.Find(x => x.Id == id);

        private RelayCommand openAddAddressPopup;

        public RelayCommand OpenAddAddressPopupCommand =>
            openAddAddressPopup ??
            (openAddAddressPopup = new RelayCommand(() =>
            {
                MessengerInstance.Register<bool>(this, "AddAddressStatus", async (x) =>
                {
                    Debug.WriteLine("AddAddressStatusMessenger");
                    await LoadPersonAsync(true);

                    MessengerInstance.Unregister<bool>(this, "AddAddressStatus");
                });
                Navigation.PushPopupAsync(new AddAddressPopup());
            }));
    }
}
