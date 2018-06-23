using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
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
        private RelayCommand<Address> deleteAddress;
        private RelayCommand<ContactMethod> deleteContact;
        private RelayCommand<Email> deleteEmail;
        public ViewUserProfileContactsViewModel(INavigationService n, StudioServicesApi a, AlertService al, KeyValueService k) : base(n, a, al, k)
        {
        }
        public override async Task NavigatedToAsync(object parameter = null)
        {
            base.PropertyChanged += ViewUserProfileContactsViewModel_PropertyChanged;
            await base.NavigatedToAsync(parameter);
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
        public MyObservableCollection<Address> Addresses { get; } = new MyObservableCollection<Address>();
        public MyObservableCollection<ContactMethod> Contacts { get; } = new MyObservableCollection<ContactMethod>();
        public MyObservableCollection<Email> Emails { get; } = new MyObservableCollection<Email>();

        public RelayCommand<Address> DeleteAddressCommand =>
            deleteAddress ??
            (deleteAddress = new RelayCommand<Address>(async (SelectedAddress) =>
            {
                if (SelectedAddress == null)
                    return;
                if (!await UserDialogs.Instance.ConfirmAsync("Vuoi eliminare l'indirizzo?", "Elimina indirizzo", "Si", "No"))
                    return;
                var res = await SendRequestAsync(async () => await api.Person_DeleteAddressAsync(SelectedAddress.Id));
                if (res.IsOK && res.Data)
                {
                    Persona.Addresses.Remove(Persona.Addresses.FirstOrDefault(x => x.Id == SelectedAddress.Id));
                    cache.SetValue("person", Persona);
                    MessengerInstance.Send(false, "ReloadPerson");
                }
                else
                    ShowMessage("L'indirizzo non è stato eliminato", "Elimina indirizzo");
            }));
        public RelayCommand<ContactMethod> DeleteContactCommand =>
            deleteContact ??
            (deleteContact = new RelayCommand<ContactMethod>(async (SelectedContact) =>
            {
                if (SelectedContact == null)
                    return;
                if (!await UserDialogs.Instance.ConfirmAsync("Vuoi eliminare il contatto?", "Elimina contatto", "Si", "No"))
                    return;
                var res = await SendRequestAsync(async () => await api.Person_DeleteContactAsync(SelectedContact.Id));
                if (res.IsOK && res.Data)
                {
                    Persona.Contacts.Remove(Persona.Contacts.FirstOrDefault(x => x.Id == SelectedContact.Id));
                    cache.SetValue("person", Persona);
                    MessengerInstance.Send(false, "ReloadPerson");
                }
                else
                    ShowMessage("Il contatto non è stato eliminato", "Elimina contatto");
            }));
        public RelayCommand<Email> DeleteEmailCommand =>
            deleteEmail ??
            (deleteEmail = new RelayCommand<Email>(async (SelectedEmail) =>
            {
                if (SelectedEmail == null)
                    return;
                if(Persona.Emails.Count == 1)
                {
                    ShowMessage("Per motivi di sicurezza, devi sempre avere un indirizzo email collegato al tuo account.", "Elimina email");
                    return;
                }
                if (!await UserDialogs.Instance.ConfirmAsync("Vuoi eliminare l'indirizzo email?", "Elimina email", "Si", "No"))
                    return;
                var res = await SendRequestAsync(async () => await api.Person_DeleteEmailAsync(SelectedEmail.Id));
                if (res.IsOK && res.Data)
                {
                    Persona.Emails.Remove(Persona.Emails.FirstOrDefault(x => x.Id == SelectedEmail.Id));
                    cache.SetValue("person", Persona);
                    MessengerInstance.Send(false, "ReloadPerson");
                }
                else
                    ShowMessage("L'indirizzo email non è stato eliminato", "Elimina email");
            }));
    }
}
