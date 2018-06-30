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
        private RelayCommand openProfilePage;
        public void Navigate(string pageKey)
        {
            Navigation.NavigateTo(pageKey);
        }
        public RelayCommand OpenProfilePage =>
            openProfilePage ??
            (openProfilePage = new RelayCommand(() =>
            {
                Navigation.NavigateTo(ViewModelLocator.USERPROFILE_PAGE);
            }));
    }
}
