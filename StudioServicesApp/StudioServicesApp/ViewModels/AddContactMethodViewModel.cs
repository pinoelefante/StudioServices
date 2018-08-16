using System;
using System.Collections.Generic;
using System.Text;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using StudioServices.Data.Sqlite.Registry;
using StudioServicesApp.Services;

namespace StudioServicesApp.ViewModels
{
    public class AddContactMethodViewModel : MyAuthViewModel
    {
        private bool whatsapp, telegram;
        private string number;
        private int typeIndex=0;
        private RelayCommand addCommand;
        public AddContactMethodViewModel(INavigationService n, StudioServicesApi a, AlertService al, KeyValueService k) : base(n, a, al, k) { }

        public int TypeIndex { get => typeIndex; set => SetMT(ref typeIndex, value); }
        public string Number { get => number; set => SetMT(ref number, value); }
        public bool IsWhatsapp { get => whatsapp; set => SetMT(ref whatsapp, value); }
        public bool IsTelegram { get => telegram; set => SetMT(ref telegram, value); }
        public RelayCommand AddContactCommand =>
            addCommand ??
            (addCommand = new RelayCommand(async () =>
            {
                var type = (ContactType)Enum.ToObject(typeof(ContactType), typeIndex);
                if (string.IsNullOrEmpty(Number))
                    return;
                var contact = new ContactMethod()
                {
                    IsTelegram = IsTelegram,
                    IsWhatsApp = IsWhatsapp,
                    Number = Number,
                    PersonId = Persona.Id,
                    Type = type
                };
                var response = await SendRequestAsync(async () => await api.Person_AddContactAsync(contact));
                if (response.IsOK)
                {
                    contact.Id = response.Data;
                    MessengerInstance.Send<ContactMethod>(contact, MSG_PERSON_ADD_CONTACT);
                    await Navigation.PopPopupAsync();
                }
                else
                    ShowMessage("Contatto non inserito", "Inserimento contatto");
            }));
    }
}
