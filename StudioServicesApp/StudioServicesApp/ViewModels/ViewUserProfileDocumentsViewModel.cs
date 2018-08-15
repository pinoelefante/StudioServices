using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using StudioServices.Data.Sqlite.Registry;
using StudioServicesApp.Services;
using Acr.UserDialogs;

namespace StudioServicesApp.ViewModels
{
    public class ViewUserProfileDocumentsViewModel : MyAuthViewModel
    {
        public MyObservableCollection<IdentificationDocument> Documents { get; } = new MyObservableCollection<IdentificationDocument>();

        public ViewUserProfileDocumentsViewModel(INavigationService n, StudioServicesApi a, AlertService al, KeyValueService k) : base(n, a, al, k) { }

        public override async Task NavigatedToAsync(object parameter = null)
        {
            await base.NavigatedToAsync(parameter);
            Documents.AddRange(Persona.Identifications, true);
        }
        
        public override void RegisterMessenger()
        {
            MessengerInstance.Register<IdentificationDocument>(this, MSG_PERSON_ADD_DOCUMENT, (doc) =>
            {
                Documents.Add(doc);
            });
            MessengerInstance.Register<IdentificationDocument>(this, MSG_PERSON_DEL_DOCUMENT, (doc) =>
            {
                Documents.Remove(doc);
            });
        }
        public override void UnregisterMessenger()
        {
            MessengerInstance.Unregister<IdentificationDocument>(this, MSG_PERSON_ADD_DOCUMENT);
            MessengerInstance.Unregister<IdentificationDocument>(this, MSG_PERSON_DEL_DOCUMENT);
        }
        private RelayCommand<IdentificationDocument> deleteCmd;
        private RelayCommand addDocumentCmd;

        public RelayCommand AddDocument =>
            addDocumentCmd ??
            (addDocumentCmd = new RelayCommand(() =>
            {
                Navigation.NavigateTo(ViewModelLocator.ADD_IDENTIFICATION_DOC_PAGE);
            }));
        public RelayCommand<IdentificationDocument> DeleteDocumentCommand =>
            deleteCmd ??
            (deleteCmd = new RelayCommand<IdentificationDocument>(async (doc) =>
            {
                if (doc == null)
                    return;
                if(Persona.Identifications.Count == 1)
                {
                    ShowMessage("E' necessario avere sempre un documento valido inserito", "Elimina documento");
                    return;
                }
                if (!await UserDialogs.Instance.ConfirmAsync("Vuoi eliminare il documento?", "Elimina documento", "Si", "No"))
                    return;
                var res = await SendRequestAsync(async () => await api.Person_DeleteDocumentAsync(doc.Id));
                if (res.IsOK && res.Data)
                    MessengerInstance.Send<IdentificationDocument>(doc, MSG_PERSON_DEL_DOCUMENT);
                else
                    ShowMessage("Il documento non è stato eliminato", "Elimina documento");
            }));
    }
}
