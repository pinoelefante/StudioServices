using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pinoelefante.ViewModels;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;

namespace pinoelefante.Views
{
    public class MyContentPage : ContentPage
    {
        private MyPopupMessage busyPopup;
        public MyContentPage(object parameter = null)
        {
            navigationParameter = parameter;
            busyPopup = new MyPopupMessage();
        }
        protected MyViewModel ViewModel => this.BindingContext as MyViewModel;
        protected object navigationParameter = null;
        protected override void OnAppearing()
        {
            if (ViewModel != null)
            {
                ViewModel.PropertyChanged += ViewModel_PropertyChanged;
                ViewModel.NavigatedToAsync(navigationParameter);
            }
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case nameof(ViewModel.IsBusyActive):
                    if(ViewModel.IsBusyActive)
                    {
                        busyPopup.SetMessage(ViewModel.BusyMessage);
                        if (!ViewModel.Navigation.IsShowingPopup(busyPopup))
                            ViewModel.Navigation.PushPopupAsync(busyPopup);
                    }
                    else
                        ViewModel.Navigation.PopPopupAsync();
                    break;
            }
        }

        protected override void OnDisappearing()
        {
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
        
        public class MyPopupMessage : PopupPage
        {
            private Label lbl;
            public MyPopupMessage() : base()
            {
                lbl = new Label()
                {
                    FontSize = 14,
                    TextColor = Color.Black
                };
                StackLayout layout = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    BackgroundColor = Color.FromHex("#43A047"),
                    VerticalOptions = LayoutOptions.Start,
                    HeightRequest = 50,
                    Padding = new Thickness(20, 0),
                };
                layout.Children.Add(lbl);
            }
            public void SetMessage(string msg)
            {
                lbl.Text = msg;
            }
        }
    }
}
