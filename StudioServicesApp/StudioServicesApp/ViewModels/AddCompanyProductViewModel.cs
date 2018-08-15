using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using StudioServices.Data.Sqlite.Accounting;
using StudioServicesApp.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace StudioServicesApp.ViewModels
{
    public class AddCompanyProductViewModel : MyAuthViewModel
    {
        private Company company;
        private CompanyProduct product;
        private string unitType = "PZ", companyName;
        private bool editMode = false;

        public AddCompanyProductViewModel(INavigationService n, StudioServicesApi a, AlertService al, KeyValueService k) : base(n, a, al, k)
        {
        }

        public Company CurrentCompany { get => company; set => SetMT(ref company, value); }
        public CompanyProduct Product { get => product; set => SetMT(ref product, value); }
        public string SelectedUnitType { get => unitType; set => SetMT(ref unitType, value); }
        public string CompanyName { get => companyName; set => SetMT(ref companyName, value); }
        public bool EditMode { get => editMode; set => SetMT(ref editMode, value); }

        public override async Task NavigatedToAsync(object parameter = null)
        {
            await base.NavigatedToAsync(parameter);

            var x = parameter as Tuple<Company, CompanyProduct>;
            if (x.Item1 == null)
                Navigation.GoBack();
            
            CurrentCompany = x.Item1;
            Product = x.Item2 ?? new CompanyProduct();
            CompanyName = CurrentCompany.Name;
            SelectedUnitType = x.Item2 == null ? "PZ" : Enum.GetName(typeof(InvoiceQuantity), x.Item2.UnitMeasure);
            EditMode = x.Item2 == null;
        }
        private RelayCommand saveCmd;
        public RelayCommand SaveCommand =>
            saveCmd ??
            (saveCmd = new RelayCommand(async () =>
            {
                Debug.WriteLine("UnitType: "+SelectedUnitType);
                Product.Company = CurrentCompany;
                Product.CompanyId = CurrentCompany.Id;
                Product.UnitMeasure = (InvoiceQuantity)Enum.Parse(typeof(InvoiceQuantity), SelectedUnitType);

                var response = await SendRequestAsync(async () => await api.Warehouse_SaveProduct(Product));
                //TODO
            }));
    }
}
