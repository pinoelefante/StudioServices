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
        public ViewUserProfileDocumentsViewModel(INavigationService n, StudioServicesApi a, AlertService al, KeyValueService k) : base(n, a, al, k)
        {
        }
        public override async Task NavigatedToAsync(object parameter = null)
        {
            base.PropertyChanged += ViewUserProfileDocumentsViewModel_PropertyChanged;
            await base.NavigatedToAsync(parameter);
        }
        public MyObservableCollection<IdentificationDocument> Documents { get; } = new MyObservableCollection<IdentificationDocument>();

        private void ViewUserProfileDocumentsViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(base.Persona)))
                Documents.AddRange(Persona.Identifications, true);
        }
        private RelayCommand<IdentificationDocument> deleteCmd;
        public RelayCommand AddDocument =>
            new RelayCommand(() =>
            {
                Navigation.NavigateTo(ViewModelLocator.ADD_IDENTIFICATION_DOC_PAGE);
            });
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
                {
                    Persona.Identifications.Remove(Persona.Identifications.FirstOrDefault(x => x.Id == doc.Id));
                    cache.SetValue("person", Persona);
                    MessengerInstance.Send(false, "ReloadPerson");
                }
                else
                    ShowMessage("Il documento non è stato eliminato", "Elimina documento");
            }));
    }
}
