using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using pinoelefante.Services;
using StudioServicesApp.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pinoelefante.ViewModels
{
    public class MyViewModel : ViewModelBase
    {
        protected NavigationService navigation;
        protected StudioServicesApi api;
        public MyViewModel(INavigationService n, StudioServicesApi a)
        {
            navigation = n as NavigationService;
            api = a;
        }
        private bool busyActive;
        public bool IsBusyActive { get => busyActive; set => Set(ref busyActive, value); }

        public virtual void NavigatedToAsync(object parameter = null)
        {

        }
        public virtual void NavigatedFrom()
        {

        }
        /*
         * OnBackPressed() must return true when override 
         */
        public virtual bool OnBackPressed()
        {
            navigation.GoBack();
            return true;
        }
        
        public async Task<bool> ManageCommonServerResponseAsync<T>(ResponseMessage<T> m)
        {
            // ritorna un bool che fa capire se continuare o meno
            switch(m.Code)
            {
                case ResponseCode.ADMIN_FUNCTION:
                case ResponseCode.MAINTENANCE:
                    return false;
                case ResponseCode.REQUIRE_LOGIN:
                    return await LoginAsync();
                case ResponseCode.COMMUNICATION_ERROR:
                case ResponseCode.FAIL:
                case ResponseCode.SERIALIZER_ERROR:
                    return true;
            }
            return false;
        }
        private async Task<bool> LoginAsync()
        {
            /*
            string username = "";
            string password = "";
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return false;
            var res = await api.Authentication_LoginAsync(username, password);
            return (res != null && res.Code == ResponseCode.OK && res.Data);
            */
            return false;
        }
        public async Task<ResponseMessage<T>> SendRequestAsync<T>(Func<Task<ResponseMessage<T>>> action)
        {
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
                    if (!await ManageCommonServerResponseAsync(res))
                        break;
                }
            }
            while (count < 3);
            return res;
        }
    }
}
