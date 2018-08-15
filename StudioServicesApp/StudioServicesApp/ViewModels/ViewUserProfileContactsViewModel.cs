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
using System.Diagnostics;
using Xamarin.Forms;

namespace StudioServicesApp.ViewModels
{
    public class ViewUserProfileContactsViewModel : MyAuthViewModel
    {
        public ViewUserProfileContactsViewModel(INavigationService n, StudioServicesApi a, AlertService al, KeyValueService k) : base(n, a, al, k) { }
        private RelayCommand<Address> deleteAddress;
        private RelayCommand<ContactMethod> deleteContact;
        private RelayCommand<Email> deleteEmail;

        public MyObservableCollection<Address> Addresses { get; } = new MyObservableCollection<Address>();
        public MyObservableCollection<Email> Emails { get; } = new MyObservableCollection<Email>();
        public MyObservableCollection<ContactMethod> Contacts { get; } = new MyObservableCollection<ContactMethod>();

        public override async Task NavigatedToAsync(object parameter = null)
        {
            await base.NavigatedToAsync(parameter);

            Addresses.AddRange(Persona.Addresses, true);
            Contacts.AddRange(Persona.Contacts, true);
            Emails.AddRange(Persona.Emails, true);
        }

        public override void RegisterMessenger()
        {
            MessengerInstance.Register<Address>(this, MSG_PERSON_ADD_ADDRESS, (address) =>
            {
                Addresses.Add(address);
            });
            MessengerInstance.Register<ContactMethod>(this, MSG_PERSON_ADD_CONTACT, (contact) =>
            {
                Contacts.Add(contact);
            });
            MessengerInstance.Register<Email>(this, MSG_PERSON_ADD_EMAIL, (email) =>
            {
                Emails.Add(email);
            });
            MessengerInstance.Register<Address>(this, MSG_PERSON_DEL_ADDRESS, (address) =>
            {
                Addresses.Remove(address);
            });
            MessengerInstance.Register<ContactMethod>(this, MSG_PERSON_DEL_CONTACT, (contact) =>
            {
                Contacts.Remove(contact);
            });
            MessengerInstance.Register<Email>(this, MSG_PERSON_DEL_EMAIL, (email) =>
            {
                Emails.Remove(email);
            });
        }
        public override void UnregisterMessenger()
        {
            base.UnregisterMessenger();
            MessengerInstance.Unregister<Address>(this, MSG_PERSON_ADD_ADDRESS);
            MessengerInstance.Unregister<Address>(this, MSG_PERSON_DEL_ADDRESS);
            MessengerInstance.Unregister<ContactMethod>(this, MSG_PERSON_ADD_CONTACT);
            MessengerInstance.Unregister<ContactMethod>(this, MSG_PERSON_DEL_CONTACT);
            MessengerInstance.Unregister<Email>(this, MSG_PERSON_ADD_EMAIL);
            MessengerInstance.Unregister<Email>(this, MSG_PERSON_DEL_EMAIL);
        }

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
                    MessengerInstance.Send<Address>(SelectedAddress, MSG_PERSON_DEL_ADDRESS);
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
                    MessengerInstance.Send<ContactMethod>(SelectedContact, MSG_PERSON_DEL_CONTACT);
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
                    MessengerInstance.Send<Email>(SelectedEmail, MSG_PERSON_DEL_EMAIL);
                else
                    ShowMessage("L'indirizzo email non è stato eliminato", "Elimina email");
            }));
    }
}
