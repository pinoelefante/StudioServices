using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pinoelefante.ViewModels;
using Xamarin.Forms;

namespace pinoelefante.Views
{
    public class MyContentPage : ContentPage
    {
        public MyContentPage(object parameter = null)
        {
            navigationParameter = parameter;
        }
        protected MyViewModel ViewModel => this.BindingContext as MyViewModel;
        protected object navigationParameter = null;
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
            if (ViewModel!=null)
                return ViewModel.OnBackPressed();
            return false;
        }
        protected void Entry_CheckInt(object sender, TextChangedEventArgs e)
        {
            var entry = sender as Entry;
            if (entry == null)
            {
                Debug.WriteLine("Entry_CheckInt - sender is not an entry");
                return;
            }
            if (string.IsNullOrEmpty(e.NewTextValue))
                return;
            if (!Int32.TryParse(e.NewTextValue, out int newValue))
                entry.Text = string.IsNullOrEmpty(e.OldTextValue) ? "" : e.OldTextValue;
        }
        protected void Entry_UnfocusedInt(object sender, FocusEventArgs e)
        {
            var entry = sender as Entry;
            if (entry != null && string.IsNullOrEmpty(entry.Text?.Trim()))
                entry.Text = "0";
        }
        protected void ListView_ItemSelectionDisable(object sender, SelectedItemChangedEventArgs e)
        {
            var listView = (sender as ListView);
            if (listView != null)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    listView.SelectedItem = null;
                });
            }
        }
    }
}
