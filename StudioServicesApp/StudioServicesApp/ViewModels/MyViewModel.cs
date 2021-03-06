﻿using Acr.UserDialogs;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using pinoelefante.Services;
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
        public static readonly string MSG_PERSON_RELOAD = "PersonReload",
            MSG_PERSON_LOADED = "PersonLoaded",
            MSG_PERSON_ADD_EMAIL = "PersonAddEmail",
            MSG_PERSON_DEL_EMAIL = "PersonDelEmail",
            MSG_PERSON_UPD_EMAIL = "PersonUpdEmail",
            MSG_PERSON_ADD_ADDRESS = "PersonAddAddress",
            MSG_PERSON_DEL_ADDRESS = "PersonDelAddress",
            MSG_PERSON_UPD_ADDRESS = "PersonUpdAddress",
            MSG_PERSON_ADD_CONTACT = "PersonAddContact",
            MSG_PERSON_DEL_CONTACT = "PersonDelContact",
            MSG_PERSON_UPD_CONTACT = "PersonUpdContact",
            MSG_PERSON_ADD_DOCUMENT = "PersonAddDocument",
            MSG_PERSON_DEL_DOCUMENT = "PersonDelDocument",
            MSG_PERSON_UPD_DOCUMENT = "PersonUpdDocument",
            MSG_MY_COMPANY_LOADED = "MyCompaniesLoaded",
            MSG_MY_COMPANY_ADD = "MyCompanyAdd",
            MSG_MY_COMPANY_DEL = "MyCompanyDel",
            MSG_MY_COMPANY_UPD = "MyCompanyUpd",
            MSG_MY_COMPANY_PRODUCT_ADD = "MyCompanyProductAdd",
            MSG_MY_COMPANY_PRODUCT_DEL = "MyCompanyProductDel",
            MSG_MY_COMPANY_PRODUCT_UPD = "MyCompanyProductUpd",
            MSG_MY_COMPANY_CLIENT_ADD = "MyCompanyClientAdd",
            MSG_MY_COMPANY_CLIENT_DEL = "MyCompanyClientDel",
            MSG_MY_COMPANY_CLIENT_UPD = "MyCompanyClientUpd",
            MSG_REQUEST_CURRENT_COMPANY = "RequestCurrentCompany",
            MSG_SET_CURRENT_COMPANY = "SetCurrentCompany",
            MSG_RESPONSE_CURRENT_COMPANY = "ResponseCurrentCompany",
            MSG_INVOICE_DETAIL_ADD = "InvoiceDetailAdd";

        private static readonly int LIMIT_TRY = 3;
        public NavigationService Navigation { get; private set; }
        protected CacheManager cache = ViewModelLocator.GetService<CacheManager>();
        protected StudioServicesApi api;
        protected AlertService messageService;
        protected KeyValueService kvSettings;
        public MyViewModel(INavigationService n, StudioServicesApi a, AlertService alert, KeyValueService kv)
        {
            Navigation = n as NavigationService;
            api = a;
            messageService = alert;
            kvSettings = kv;
        }
        private bool busyActive;
        private string busyMsg;
        public bool IsBusyActive { get => busyActive; set => SetMT(ref busyActive, value); }
        public string BusyMessage { get => busyMsg; set => SetMT(ref busyMsg, value); }
        
        public void SetBusy(bool status, string text)
        {
            BusyMessage = text;
            IsBusyActive = status;
        }
        
        public void ShowMessage(string message, string title = "", Action okAction=null)
        {
            messageService.ShowMessage(message ?? string.Empty, title, okAction);
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
            this.RegisterMessenger();
            return Task.CompletedTask;   
        }
        public virtual void RegisterMessenger() {
            Debug.WriteLine("RegisterMessenger: MyViewModel");
        }
        public virtual void UnregisterMessenger() { }
        public virtual void NavigatedFrom()
        {
            UnregisterMessenger();
        }
        /*
         * OnBackPressed() must return true when override 
         */
        public virtual bool OnBackPressed()
        {
            Navigation.GoBack();
            return true;
        }
        public void ShowLoginPrompt()
        {
            UserDialogs.Instance.Login(new LoginConfig()
            {
                Title = "Login StudioServices",
                Message = "Per poter completare l'operazione, è necessario effettuare il login",
                LoginPlaceholder = "Username",
                LoginValue = kvSettings.Get("username", string.Empty),
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
                case ResponseCode.INTERNET_NOT_AVAILABLE:
                    ShowMessage("Connessione assente");
                    return false;
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
            if (string.IsNullOrEmpty(kvSettings.Get("username")) || string.IsNullOrEmpty(kvSettings.Get("password")))
                return false;
            
            string username = kvSettings.Get("username");
            string password = kvSettings.Get("password");
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
            RaisePropertyChangedMT(fieldName);
        }
        public void RaisePropertyChangedMT<T>(System.Linq.Expressions.Expression<Func<T>> expression)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                RaisePropertyChanged(expression);
            });
        }
        public void RaisePropertyChangedMT([CallerMemberName]string propertyName="")
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                RaisePropertyChanged(propertyName);
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

        private RelayCommand closePopupCmd;
        public RelayCommand ClosePopupCommand =>
            closePopupCmd ??
            (closePopupCmd = new RelayCommand(() =>
            {
                Navigation.PopPopupAsync();
            }));
    }
}
