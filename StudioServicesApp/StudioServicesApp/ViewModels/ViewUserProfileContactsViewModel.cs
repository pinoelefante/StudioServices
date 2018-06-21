using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using StudioServices.Data.Sqlite.Registry;
using StudioServicesApp.Services;
using StudioServicesApp.Views;
using Xamarin.Forms;

namespace StudioServicesApp.ViewModels
{
    public class ViewUserProfileContactsViewModel : MyAuthViewModel
    {
        private Address address;
        private ContactMethod contact;
        private Email email;
        public ViewUserProfileContactsViewModel(INavigationService n, StudioServicesApi a, AlertService al, KeyValueService k) : base(n, a, al, k)
        {
        }
        public override async Task NavigatedToAsync(object parameter = null)
        {
            base.PropertyChanged += ViewUserProfileContactsViewModel_PropertyChanged;
            await base.NavigatedToAsync(parameter);
            Init();
        }
        public override void NavigatedFrom()
        {
            base.PropertyChanged -= ViewUserProfileContactsViewModel_PropertyChanged;
            base.NavigatedFrom();
        }
        private void ViewUserProfileContactsViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName.Equals(nameof(Persona)))
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Addresses.Clear();
                    Addresses.AddRange(Persona.Addresses);

                    Contacts.Clear();
                    Contacts.AddRange(Persona.Contacts);

                    Emails.Clear();
                    Emails.AddRange(Persona.Emails);
                });
            }
        }

        private void Init()
        {
            SelectedAddress = null;
            SelectedContact = null;
            SelectedEmail = null;
        }
        public MyObservableCollection<Address> Addresses { get; } = new MyObservableCollection<Address>();
        public MyObservableCollection<ContactMethod> Contacts { get; } = new MyObservableCollection<ContactMethod>();
        public MyObservableCollection<Email> Emails { get; } = new MyObservableCollection<Email>();
        
        public Address SelectedAddress { get => address; set => SetMT(ref address, value); }
        public ContactMethod SelectedContact { get => contact; set => SetMT(ref contact, value); }
        public Email SelectedEmail { get => email; set => SetMT(ref email, value); }
    }
}
