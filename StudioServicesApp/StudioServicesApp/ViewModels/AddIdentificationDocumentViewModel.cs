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

namespace StudioServicesApp.ViewModels
{
    public class AddIdentificationDocumentViewModel : MyAuthViewModel
    {
        private int _docIndex = 0;
        private string _docNumber, _fileExt, _fileName;
        private DateTime _docIssue = DateTime.Now, _docExpire = DateTime.Now.AddYears(10);
        private RelayCommand _imageFromFile, _imageFromCamera, _addDocument;
        private bool _hasCamera = true, _imageLoaded = false;
        private byte[] _imageData;

        public int DocumentIndex { get => _docIndex; set => SetMT(ref _docIndex, value); }
        public string DocumentNumber { get => _docNumber; set => SetMT(ref _docNumber, value); }
        public DateTime DocumentIssue { get => _docIssue; set => SetMT(ref _docIssue, value); }
        public DateTime DocumentExpiry { get => _docExpire; set => SetMT(ref _docExpire, value); }

        public bool HasCamera { get => _hasCamera; set => SetMT(ref _hasCamera, value); }
        public bool IsImageLoaded { get => _imageLoaded; set => SetMT(ref _imageLoaded, value); }
        public string FileExtension { get => _fileExt; set => SetMT(ref _fileExt, value); }
        public string FileName { get => _fileName; set => SetMT(ref _fileName, value); }

        public AddIdentificationDocumentViewModel(INavigationService n, StudioServicesApi a, AlertService al, KeyValueService k) : base(n, a, al, k) { }
        public override async Task NavigatedToAsync(object parameter = null)
        {
            Reset();

            VerifyPersonEnabled = parameter == null ? false : (bool)parameter;
            // HasCamera = CrossMedia.Current.IsCameraAvailable;
            await base.NavigatedToAsync();
        }
        private void Reset()
        {
            DocumentIndex = 0;
            DocumentNumber = string.Empty;
            DocumentIssue = DateTime.Now;
            DocumentExpiry = DocumentIssue.AddYears(10);
            IsImageLoaded = false;
            FileExtension = string.Empty;
            HasCamera = false;
            _imageData = null;
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
                        _imageData = stream.ReadBytes((int)stream.BaseStream.Length);
                    }
                    IsImageLoaded = _imageData != null;
                    FileExtension = file.Path.Substring(file.Path.LastIndexOf("."));
                    FileName = Path.GetFileName(file.Path);
                }
                else
                {
                    Debug.WriteLine("Immagine non selezionata");
                    // await UserDialogs.Instance.AlertAsync("Immagine non selezionata");
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
                        _imageData = stream.ReadBytes((int)stream.BaseStream.Length);
                    }
                    IsImageLoaded = _imageData != null;
                    FileExtension = file.Path.Substring(file.Path.LastIndexOf("."));
                    FileName = Path.GetFileName(file.Path);
                }
                else
                {
                    Debug.WriteLine("Immagine non scattata");
                    // await UserDialogs.Instance.AlertAsync("Image error");
                }
            }));

        public RelayCommand AddDocumentCommand =>
            _addDocument ??
            (_addDocument = new RelayCommand(async () =>
            {
                var res = await UserDialogs.Instance.ConfirmAsync($"Vuoi salvare il documento?\nNumero: {DocumentNumber}\nEmesso il {DocumentIssue.ToShortDateString()}", "Conferma inserimento", "Salva", "Annulla");
                if (!res)
                    return;
                var document = new IdentificationDocument()
                {
                    Expire = DocumentExpiry,
                    Filename = FileName,
                    FileContentBase64 = _imageData!=null && _imageData.Length > 0 ? Convert.ToBase64String(_imageData) : string.Empty,
                    Issue = DocumentIssue,
                    Number = DocumentNumber,
                    PersonId = Persona.Id,
                    Type = (DocumentType)Enum.ToObject(typeof(DocumentType), DocumentIndex)
                };
                var response = await SendRequestAsync(() => api.Person_AddDocumentAsync(document));
                if (response.IsOK)
                {
                    await LoadPersonAsync(true);
                    Navigation.NavigateTo(ViewModelLocator.NEWS_PAGE);
                }
            }));
        
    }
}
