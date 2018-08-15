using GalaSoft.MvvmLight;
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace StudioServicesApp.ViewModels
{
    public class MyMasterDetailViewModel : MyViewModel
    {
        private Person person;
        private Company selectedCompany;
        public Person Persona { get => person; set => SetMT(ref person, value); }
        public MyObservableCollection<Company> MyCompanies { get; } = new MyObservableCollection<Company>();
        public Company SelectedCompany
        {
            get => selectedCompany;
            set
            {
                if (selectedCompany != value)
                {
                    SetMT(ref selectedCompany, value);
                    MessengerInstance.Send<Company>(value, MSG_SET_CURRENT_COMPANY);
                }
            }
        }

        public MyMasterDetailViewModel(INavigationService n, StudioServicesApi a, AlertService al, KeyValueService k) : base(n, a, al, k)
        {
            
        }
        public override void RegisterMessenger()
        {
            MessengerInstance.Register<Person>(this, MSG_PERSON_LOADED, (person) =>
            {
                if (person != Persona)
                    Persona = person;
            });
            MessengerInstance.Register<List<Company>>(this, MSG_MY_COMPANY_LOADED, (companies) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (companies != null && companies.Count != MyCompanies.Count)
                    {
                        MyCompanies.AddRange(companies, true);
                        if (SelectedCompany == null)
                            SelectedCompany = MyCompanies.FirstOrDefault();
                    }
                    RaisePropertyChanged(() => MyCompanies);
                });
            });
            MessengerInstance.Register<Company>(this, MSG_MY_COMPANY_ADD, (company) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (company.IsClient)
                        return;
                    MyCompanies.Add(company);
                    if (SelectedCompany == null)
                        SelectedCompany = company;
                    RaisePropertyChanged(() => MyCompanies);
                });
            });

            MessengerInstance.Register<Company>(this, MSG_MY_COMPANY_DEL, (company) =>
            {
                if (company.IsClient)
                    return;
                var del_company = MyCompanies.First(x => x.Id == company.Id);
                MyCompanies.Remove(del_company);
                if (SelectedCompany != null && del_company == SelectedCompany)
                    SelectedCompany = MyCompanies.FirstOrDefault();
                RaisePropertyChanged(() => MyCompanies);
            });
            MessengerInstance.Register<bool>(this, MSG_REQUEST_CURRENT_COMPANY, (x) =>
            {
                var curCompany = SelectedCompany;
                MessengerInstance.Send<Company>(curCompany, MSG_RESPONSE_CURRENT_COMPANY);
            });
        }
        private RelayCommand openProfilePage, addCompany, logoutCmd;
        public void Navigate(string pageKey)
        {
            Navigation.NavigateTo(pageKey);
        }
        public RelayCommand OpenProfilePage =>
            openProfilePage ??
            (openProfilePage = new RelayCommand(() =>
            {
                Navigate(ViewModelLocator.USERPROFILE_PAGE);
            }));
        public RelayCommand AddCompanyCommand =>
            addCompany ??
            (addCompany = new RelayCommand(() =>
            {
                Navigation.PushPopupAsync(new AddCompanyPopup(false));
            }));
        public RelayCommand LogoutCommand =>
            logoutCmd ??
            (logoutCmd = new RelayCommand(async () =>
            {
                var logout = await SendRequestAsync(async () => await api.Authentication_LogoutAsync());
                App.Current.MainPage = new NavigationPage(new LoginPage());
            }));
    }
}
