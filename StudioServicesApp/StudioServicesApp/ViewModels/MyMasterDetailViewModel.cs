using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using pinoelefante.ViewModels;
using StudioServices.Data.Sqlite.Registry;
using StudioServicesApp.Services;
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
        public Person Persona { get => person; set => SetMT(ref person, value); }
        public MyMasterDetailViewModel(INavigationService n, StudioServicesApi a, AlertService al, KeyValueService k) : base(n, a, al, k)
        {
            Task.Factory.StartNew(async () =>
            {
                Person p = null;
                do
                {
                    await Task.Delay(500);
                    p = cache.GetValue<Person>("person");
                } while (p == null);
                Persona = p;
            });
        }
        private RelayCommand openProfilePage;
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
    }
}
