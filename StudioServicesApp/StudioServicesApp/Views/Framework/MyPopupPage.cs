using pinoelefante.ViewModels;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Text;

namespace pinoelefante.Views
{
    public class MyPopupPage : PopupPage
    {
        public MyPopupPage() : base() { }

        protected MyViewModel ViewModel => this.BindingContext as MyViewModel;

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel?.NavigatedToAsync();
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
