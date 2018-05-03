using GalaSoft.MvvmLight.Views;
using pinoelefante.ViewModels;
using StudioServices.Registry.Data;
using StudioServicesApp.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace StudioServicesApp.ViewModels
{
    public class MyAuthViewModel : MyViewModel
    {
        private Person _person;
        public Person Persona { get => _person; set => SetMT(ref _person, value); }
        private bool _isAdmin;
        public bool IsAdmin { get => _isAdmin; set => SetMT(ref _isAdmin, value); }
        public bool VerifyPersonEnabled { get; set; } = true;

        public MyAuthViewModel(INavigationService n, StudioServicesApi a, AlertService al) : base(n, a, al) { }

        public override Task NavigatedToAsync(object parameter = null)
        {
            return Task.Factory.StartNew(async () =>
            {
                await LoadPersonAsync();
                if(VerifyPersonEnabled)
                    CheckPerson();
            });
        }
        private async Task LoadPersonAsync()
        {
            if (cache.GetValue<Person>("person", null) == null)
            {
                var busy_message = "Recupero profilo in corso";
                SetBusy(true, busy_message);
                var res = await SendRequestAsync(async () => await api.Person_GetAsync());
                SetBusy(false, busy_message);
                if (res.Code == ResponseCode.OK && res.Data != null)
                    cache.SetValue("person", res.Data);
                else
                    ShowMessage("Non è stato possibile recuperare le informazioni del profilo", "Informazioni profilo");
            }
            Persona = cache.GetValue<Person>("person");

            if (cache.GetValue<bool?>("is_admin", null) == null)
            {
                var res = await SendRequestAsync(async () => await api.Admin_IsAdminAsync());
                if (res.Code == ResponseCode.OK)
                    cache.SetValue("is_admin", res.Data);
            }
            var admin = cache.GetValue("is_admin", false);
            IsAdmin = admin;
        }
        private void CheckPerson()
        {
            var person = cache.GetValue<Person>("person");
            if (person == null)
                throw new Exception("Person is null, but can't be null");

            if (person.Identifications.Count == 0)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    navigation.NavigateTo(ViewModelLocator.ADD_IDENTIFICATION_DOC_PAGE);
                });
                
            }
        }
    }
}
