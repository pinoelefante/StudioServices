using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using StudioServices.Data.Sqlite.Accounting;
using StudioServicesApp.Services;

namespace StudioServicesApp.ViewModels
{
    public class AddInvoiceProductPopupViewModel : MyAuthViewModel
    {
        private string unitPrice = "1", taxValue = "0", quantity = "1";
        private RelayCommand<CompanyProduct> selectedProductCmd;
        private RelayCommand addProductCmd;

        public AddInvoiceProductPopupViewModel(INavigationService n, StudioServicesApi a, AlertService al, KeyValueService k) : base(n, a, al, k) { }

        public MyObservableCollection<CompanyProduct> CompanyProducts { get; } = new MyObservableCollection<CompanyProduct>();
        public string UnitPrice
        {
            get => unitPrice;
            set
            {
                SetMT(ref unitPrice, value);
                Calculate();
            }
        }
        public string Quantity
        {
            get => quantity;
            set
            {
                SetMT(ref quantity, value);
                Calculate();
            }
        }
        public string VAT
        {
            get => taxValue;
            set
            {
                SetMT(ref taxValue, value);
                Calculate();
            }
        }
        private double withoutTax, withTax;
        public double WithoutTax
        {
            get => withoutTax;
            set => SetMT(ref withoutTax, value);
        }
        public double WithTax
        {
            get => withTax;
            set => SetMT(ref withTax, value);
        }

        private Company CurrentCompany { get; set; }
        private CompanyProduct SelectedProduct { get; set; }

        public override async Task NavigatedToAsync(object parameter = null)
        {
            await base.NavigatedToAsync(parameter);
            SelectedProduct = null;
            CurrentCompany = parameter as Company;
            var products = await GetProductsAsync(CurrentCompany.Id);
            CompanyProducts.AddRange(products, true);
        }
        
        public RelayCommand<CompanyProduct> OnCompanyProductSelected =>
            selectedProductCmd ??
            (selectedProductCmd = new RelayCommand<CompanyProduct>((product) =>
            {
                SelectedProduct = product;
                // popolare campi
                UnitPrice = product.UnitPrice.ToString();
                VAT = product.Tax.ToString();

                Calculate();
            }));
        public RelayCommand AddProductCommand =>
            addProductCmd ??
            (addProductCmd = new RelayCommand(async () =>
            {
                float quantity = string.IsNullOrEmpty(Quantity) ? 1 : float.Parse(Quantity);
                float unitPrice = string.IsNullOrEmpty(UnitPrice) ? 1 : float.Parse(UnitPrice);
                float vat = string.IsNullOrEmpty(VAT) ? 0 : float.Parse(VAT);

                var detail = new InvoiceDetail()
                {
                    Product = SelectedProduct,
                    ProductId = SelectedProduct.Id,
                    Quantity = quantity,
                    Tax = vat,
                    UnitPrice = unitPrice,
                    UnitPriceDiscount = 0
                };
                MessengerInstance.Send<InvoiceDetail>(detail, MSG_INVOICE_DETAIL_ADD);
                await Navigation.PopPopupAsync();
            }));

        private void Calculate()
        {
            //if (string.IsNullOrEmpty(Quantity))
            //    Quantity = "1";
            double quantity = string.IsNullOrEmpty(Quantity) ? 1 : double.Parse(Quantity);

            //if (string.IsNullOrEmpty(UnitPrice))
            //    UnitPrice = "1";
            double unitPrice = string.IsNullOrEmpty(UnitPrice) ? 1 : double.Parse(UnitPrice);

            //if (string.IsNullOrEmpty(VAT))
            //    VAT = "0";
            double vat = string.IsNullOrEmpty(VAT) ? 0 : double.Parse(VAT);

            double withoutTax = quantity * unitPrice;
            double withTax = withoutTax * (1 + (vat / 100));

            WithoutTax = withoutTax;
            WithTax = withTax;
        }
    }
}
