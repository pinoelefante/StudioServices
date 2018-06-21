using pinoelefante.ViewModels;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Text;

namespace pinoelefante.Views
{
    public class MyPopupPage : PopupPage
    {
        public MyPopupPage(object parameter = null) : base()
        {
            navigationParameter = parameter;
        }
        private object navigationParameter;
        protected MyViewModel ViewModel => this.BindingContext as MyViewModel;

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel?.NavigatedToAsync(navigationParameter);
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            ViewModel?.NavigatedFrom();
        }
        protected override bool OnBackButtonPressed()
        {
            if (ViewModel != null)
                return ViewModel.OnBackPressed();
            return false;
        }
    }
}
