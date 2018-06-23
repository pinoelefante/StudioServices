using Acr.UserDialogs;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using pinoelefante.ViewModels;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using StudioServices.Data.Sqlite.Registry;
using StudioServicesApp.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace StudioServicesApp.ViewModels
{
    public class AddIdentificationDocumentViewModel : MyAuthViewModel
    {
        private int _docIndex = 0;
        private string _docNumber;
        private DateTime _docIssue = DateTime.Now, _docExpire = DateTime.Now.AddYears(10);
        private RelayCommand _imageFromFile, _imageFromCamera, _addDocument;
        private bool _hasCamera = true;

        public int DocumentIndex { get => _docIndex; set => SetMT(ref _docIndex, value); }
        public string DocumentNumber { get => _docNumber; set => SetMT(ref _docNumber, value); }
        public DateTime DocumentIssue { get => _docIssue; set => SetMT(ref _docIssue, value); }
        public DateTime DocumentExpiry { get => _docExpire; set => SetMT(ref _docExpire, value); }

        public bool HasCamera { get => _hasCamera; set => SetMT(ref _hasCamera, value); }

        public MyObservableCollection<KeyValuePair<String, byte[]>> FileLoaded { get; } = new MyObservableCollection<KeyValuePair<string, byte[]>>();

        public AddIdentificationDocumentViewModel(INavigationService n, StudioServicesApi a, AlertService al, KeyValueService k) : base(n, a, al, k) { }
        public override async Task NavigatedToAsync(object parameter = null)
        {
            Reset();
            VerifyPersonEnabled = parameter == null ? false : (bool)parameter;
            InitAsync();
            await base.NavigatedToAsync();
        }
        private void Reset()
        {
            FileLoaded.Clear();
            DocumentIndex = 0;
            DocumentNumber = string.Empty;
            DocumentIssue = DateTime.Now;
            DocumentExpiry = DocumentIssue.AddYears(10);
            HasCamera = false;
        }
        private async Task InitAsync()
        {
            try
            {
                if(await CrossMedia.Current.Initialize())
                    HasCamera = CrossMedia.Current.IsCameraAvailable;
            }
            catch
            {
                HasCamera = false;
            }
        }

        public RelayCommand ImageGalleryCommand =>
            _imageFromFile ??
            (_imageFromFile = new RelayCommand(async () =>
            {
                var storageStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
                if (storageStatus != PermissionStatus.Granted)
                {
                    ShowMessage("Per poter continuare è necessario fornire i permessi di storage", "Storage");
                    return;
                }

                var mediaInit = await CrossMedia.Current.Initialize();
                if (!mediaInit)
                {
                    ShowMessage("Impossibile inizializzare la galleria");
                    return;
                }
                var file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions()
                {
                    SaveMetaData = true,
                    CompressionQuality = 90,
                    PhotoSize = PhotoSize.Full
                });

                if (file != null)
                {
                    using (var stream = new BinaryReader(file.GetStream()))
                    {
                        var _imageData = stream.ReadBytes((int)stream.BaseStream.Length);
                        FileLoaded.Add(new KeyValuePair<string, byte[]>(file.Path, _imageData));
                    }
                }
                else
                {
                    Debug.WriteLine("Immagine non selezionata");
                }
            }));

        public RelayCommand ImageCameraCommand =>
            _imageFromCamera ??
            (_imageFromCamera = new RelayCommand(async () =>
            {
                var cameraStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
                if(cameraStatus != PermissionStatus.Granted)
                {
                    ShowMessage("Per poter continuare è necessario fornire i permessi alla fotocamera", "Fotocamera");
                    return;
                }
                var storageStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
                if(storageStatus != PermissionStatus.Granted)
                {
                    ShowMessage("Per poter continuare è necessario fornire i permessi di storage", "Storage");
                    return;
                }

                var mediaInit = await CrossMedia.Current.Initialize();
                if(!mediaInit)
                {
                    ShowMessage("Impossibile inizializzare la fotocamera");
                    return;
                }
                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    ShowMessage("No Camera", "No camera available.\n:(");
                    return;
                }

                var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    AllowCropping = true,
                    DefaultCamera = CameraDevice.Rear,
                    PhotoSize = PhotoSize.Full,
                    SaveToAlbum = true,
                    CompressionQuality = 90,
                    SaveMetaData = true
                });

                if (file != null)
                {
                    using (var stream = new BinaryReader(file.GetStream()))
                    {
                        var _imageData = stream.ReadBytes((int)stream.BaseStream.Length);
                        FileLoaded.Add(new KeyValuePair<string, byte[]>(file.Path, _imageData));
                    }
                }
                else
                {
                    Debug.WriteLine("Immagine non scattata");
                }
            }));

        public RelayCommand AddDocumentCommand =>
            _addDocument ??
            (_addDocument = new RelayCommand(async () =>
            {
                var res = await UserDialogs.Instance.ConfirmAsync($"Vuoi salvare il documento?\nNumero: {DocumentNumber}\nEmesso il {DocumentIssue.ToShortDateString()}", "Conferma inserimento", "Salva", "Annulla");
                if (!res)
                    return;
                var fileName = $"DOC_{Persona.Id}.pdf";
                var document = new IdentificationDocument()
                {
                    Expire = DocumentExpiry,
                    Filename = fileName,
                    FileContentBase64 = string.Empty,
                    FileUpload = GetFileUploadContent(),
                    Issue = DocumentIssue,
                    Number = DocumentNumber,
                    PersonId = Persona.Id,
                    Type = (DocumentType)Enum.ToObject(typeof(DocumentType), DocumentIndex)
                };
                if (!ValidateDocument(document))
                    return;
                var response = await SendRequestAsync(() => api.Person_AddDocumentAsync(document));
                if (response.IsOK && response.Data)
                {
                    MessengerInstance.Send(true, "ReloadPerson");
                    // Navigation.NavigateTo(ViewModelLocator.NEWS_PAGE);
                    Navigation.GoBack();
                }
            }));
        private bool ValidateDocument(IdentificationDocument document)
        {
            try
            {
                return document.IsValid();
            }
            catch(DocumentExpiredException)
            {
                if (!UserDialogs.Instance.ConfirmAsync("Il documento è scaduto. Vuoi inserirlo comunque?", "Documento scaduto").Result)
                    return false;
            }
            catch(DocumentIssueDateException)
            {
                if (!UserDialogs.Instance.ConfirmAsync("Non è possibile che il documento sia stato emesso nel giorno indicato. Se la data è corretta, controlla la data del tuo dispositivo e clicca 'Continua'.", "Documento emesso in futuro", "Continua").Result)
                    return false;
            }
            catch(DocumentInvalidNumberException)
            {
                ShowMessage("Il numero del documento non è valido.", "Numero non valido");
                return false;
            }
            return true;
        }
        private List<string> GetFileUploadContent()
        {
            List<string> upload = new List<string>(FileLoaded.Count);
            for (int i = 0; i < FileLoaded.Count; i++)
            {
                var b64 = Convert.ToBase64String(FileLoaded[i].Value);
                upload.Add(b64);
            }
            return upload;
        }
        public RelayCommand<string> RemoveImageCommand =>
            new RelayCommand<string>(async (imgPath) =>
            {
                var item = FileLoaded.FirstOrDefault(x => x.Key.Equals(imgPath));
                var index = FileLoaded.IndexOf(item);
                if(index >= 0)
                {
                    ConfirmConfig confirmConfig = new ConfirmConfig()
                    {
                        Message = "Vuoi rimuovere l'immagine?",
                        Title = "Immagine documento"
                    };
                    if(await UserDialogs.Instance.ConfirmAsync(confirmConfig))
                        FileLoaded.RemoveAt(index);
                }
            });
    }
}
