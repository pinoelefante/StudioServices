using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using pinoelefante.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pinoelefante.ViewModels
{
    public class MyViewModel : ViewModelBase
    {
        protected NavigationService navigation;
        public MyViewModel(INavigationService n)
        {
            navigation = n as NavigationService;
        }
        private bool busyActive;
        public bool IsBusyActive { get => busyActive; set => Set(ref busyActive, value); }

        public virtual void NavigatedToAsync(object parameter = null)
        {

        }
        public virtual void NavigatedFrom()
        {

        }
        /*
         * OnBackPressed() must return true when override 
         */
        public virtual bool OnBackPressed()
        {
            navigation.GoBack();
            return true;
        }
        /*
        public bool ManageCommonServerResponse(StatusCodes code)
        {
            return false;
        }
        */
    }
}
