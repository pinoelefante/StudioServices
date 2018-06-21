using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using StudioServicesApp.Services;

namespace StudioServicesApp.ViewModels
{
    public class ViewUserProfileMainPageViewModel : MyAuthViewModel
    {
        public ViewUserProfileMainPageViewModel(INavigationService n, StudioServicesApi a, AlertService al, KeyValueService k) : base(n, a, al, k)
        {
        }
        public override async Task NavigatedToAsync(object parameter = null)
        {
            await base.NavigatedToAsync(parameter);
            Init();
        }
        private void Init()
        {
            Name = Persona.Name;
            Surname = Persona.Surname;
            BirthPlace = Persona.BirthPlace;
            FiscalCode = Persona.FiscalCode;
            BirthDay = Persona.Birth;
        }
        private string name, surname, birthplace, fiscalcode;
        private DateTime birthday;
        private RelayCommand saveProfileCmd;

        public string Name { get => name; set => SetMT(ref name, value); }
        public string Surname { get => surname; set => SetMT(ref surname, value); }
        public string BirthPlace { get => birthplace; set => SetMT(ref birthplace, value); }
        public string FiscalCode { get => fiscalcode; set => SetMT(ref fiscalcode, value); }
        public DateTime BirthDay { get => birthday; set => SetMT(ref birthday, value); }
        public RelayCommand SaveProfileCommand =>
            saveProfileCmd ??
            (saveProfileCmd = new RelayCommand(() =>
            {

            }));
    }
}
