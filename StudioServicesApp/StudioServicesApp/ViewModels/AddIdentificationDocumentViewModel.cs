using GalaSoft.MvvmLight.Views;
using pinoelefante.ViewModels;
using StudioServicesApp.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StudioServicesApp.ViewModels
{
    public class AddIdentificationDocumentViewModel : MyAuthViewModel
    {
        public AddIdentificationDocumentViewModel(INavigationService n, StudioServicesApi a) : base(n, a)
        {
        }
        public override Task NavigatedToAsync(object parameter = null)
        {
            base.VerifyPersonEnabled = false;
            return base.NavigatedToAsync(parameter);
        }
    }
}
