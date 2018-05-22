using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using pinoelefante.ViewModels;
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
        public MyMasterDetailViewModel(INavigationService n, StudioServicesApi a, AlertService al, KeyValueService k) : base(n, a, al, k) { }
        public void Navigate(string pageKey)
        {
            Navigation.NavigateTo(pageKey);
        }
        public RelayCommand CloseMasterPage { get; set; }

        // private RelayCommand _openProfile;
        /*
        public RelayCommand OpenUserProfile =>
            _openProfile ??
            (_openProfile = new RelayCommand(() =>
            {
                navigation.NavigateTo(ViewModelLocator.ViewUserProfile, cache.CurrentUser.Id);
                CloseMasterPage?.Execute(null);
            }));
         */
    }
}
