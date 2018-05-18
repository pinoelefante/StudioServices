using Acr.UserDialogs;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using pinoelefante.Services;
using Plugin.SecureStorage;
using StudioServicesApp;
using StudioServicesApp.Services;
using StudioServicesApp.ViewModels;
using StudioServicesApp.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace pinoelefante.ViewModels
{
    public class MyViewModel : ViewModelBase
    {
        private static readonly int LIMIT_TRY = 3;
        protected NavigationService navigation;
        protected CacheManager cache = ViewModelLocator.GetService<CacheManager>();
        protected StudioServicesApi api;
        protected AlertService messageService;
        public MyViewModel(INavigationService n, StudioServicesApi a, AlertService alert)
        {
            navigation = n as NavigationService;
            api = a;
            messageService = alert;
        }
        private bool busyActive;
        public bool IsBusyActive { get => busyActive; set => SetMT(ref busyActive, value); }
        
        public void SetBusy(bool status, string text)
        {
            messageService.SetBusy(text, status);
        }
        
        public void ShowMessage(string message, string title = "")
        {
            messageService.ShowMessage(message, title);
            // UserDialogs.Instance.Alert(message, title);
        }
        public void ShowToast(string message, int seconds = 3)
        {
            UserDialogs.Instance.Toast(new ToastConfig(message)
            {
                Position = ToastPosition.Bottom,
                Duration = TimeSpan.FromSeconds(seconds),
            });
        }

        public virtual Task NavigatedToAsync(object parameter = null)
        {
            return Task.CompletedTask;   
        }
        public virtual void NavigatedFrom() { }
        /*
         * OnBackPressed() must return true when override 
         */
        public virtual bool OnBackPressed()
        {
            navigation.GoBack();
            return true;
        }
        public void ShowLoginPrompt()
        {
            UserDialogs.Instance.Login(new LoginConfig()
            {
                Title = "Login StudioServices",
                Message = "Per poter completare l'operazione, è necessario effettuare il login",
                LoginPlaceholder = "Username",
                LoginValue = CrossSecureStorage.Current.GetValue("username", string.Empty),
                PasswordPlaceholder = "Password",
                OkText = "Accedi",
                OnAction = async (login) => 
                {
                    if (!string.IsNullOrEmpty(login.Value.UserName) && !string.IsNullOrEmpty(login.Value.Password))
                    {
                        var res = await SendRequestAsync(async () => await api.Authentication_LoginAsync(login.Value.UserName, login.Value.Password));
                        if (res.Code == ResponseCode.OK && res.Data)
                            ShowToast("Login effettuato");
                        else
                            ShowLoginPrompt();
                    }
                    else
                        ShowLoginPrompt();
                }
            });
        }
        
        public async Task<bool> ManageCommonServerResponseAsync<T>(ResponseMessage<T> m, int count = 1)
        {
            // ritorna un bool che fa capire se continuare o meno
            switch(m.Code)
            {
                case ResponseCode.ADMIN_FUNCTION:
                    ShowToast("Ahhh, volevi fare il furbetto!");
                    return false;
                case ResponseCode.MAINTENANCE:
                    ShowMessage("L'applicazione è attualmente in manutenzione. Riprovare più tardi");
                    return false;
                case ResponseCode.REQUIRE_LOGIN:
                    var res = await LoginAsync();
                    if (!res)
                        ShowLoginPrompt();
                    return res;
                case ResponseCode.COMMUNICATION_ERROR:
                case ResponseCode.FAIL:
                case ResponseCode.SERIALIZER_ERROR:
                    if (count == LIMIT_TRY)
                    {
                        Debug.WriteLine(m.Code + " - " + m.Message);
                        return false;
                    }
                    return true;
            }
            return false;
        }
        private async Task<bool> LoginAsync()
        {
            if (!CrossSecureStorage.Current.HasKey("username") || !CrossSecureStorage.Current.HasKey("password"))
                return false;
            
            string username = CrossSecureStorage.Current.GetValue("username");
            string password = CrossSecureStorage.Current.GetValue("password");
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return false;
            var res = await api.Authentication_LoginAsync(username, password);
            return (res != null && res.Code == ResponseCode.OK && res.Data);
        }
        public async Task<ResponseMessage<T>> SendRequestAsync<T>(Func<Task<ResponseMessage<T>>> action)
        {
            //TODO Verifica connessione
            int count = 0;
            ResponseMessage<T> res;
            do
            {
                res = await action.Invoke();
                count++;
                if (res.Code == ResponseCode.OK)
                    break;
                else
                {
                    if (!await ManageCommonServerResponseAsync(res, count))
                        break;
                }
            }
            while (count < LIMIT_TRY);
            return res;
        }
        public void SetMT<T>(ref T field, T value, [CallerMemberName]string fieldName="")
        {
            var old_value = field;
            field = value;
            Device.BeginInvokeOnMainThread(() =>
            {
                RaisePropertyChanged(fieldName, old_value, value);
            });
        }
        public string IntValidation(string oldValue, string newValue, bool allowEmpty=true, int? ifEmptyValue=null)
        {
            if (string.IsNullOrEmpty(newValue?.Trim()))
            {
                if (allowEmpty || ifEmptyValue == null)
                    return string.Empty;
                else
                    return ifEmptyValue.ToString();
            }
            if (Int32.TryParse(newValue, out int intValue))
                return newValue;
            return oldValue;
        }
        public string StringValidation(string oldValue, string newValue, int maxLen, Func<string, bool> validationFunc = null)
        {
            if (string.IsNullOrEmpty(newValue))
                return string.Empty;
            if (maxLen > 0 && newValue.Length > maxLen)
                return oldValue;
            if (validationFunc != null && !validationFunc(newValue))
                return oldValue;
            return newValue;
        }
        private RelayCommand<FocusEventArgs> entryIntUnfocused;
        public RelayCommand<FocusEventArgs> EntryIntUnfocused =>
            entryIntUnfocused ?? 
            (entryIntUnfocused = new RelayCommand<FocusEventArgs>((eventArgs) =>
            {
                var entry = eventArgs.VisualElement as Entry;
                if (entry != null && string.IsNullOrEmpty(entry.Text?.Trim()))
                    entry.Text = "0";
            }));
    }
}
