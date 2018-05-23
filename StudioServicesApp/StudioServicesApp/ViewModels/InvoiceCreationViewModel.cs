﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using pinoelefante.ViewModels;
using StudioServices.Data.Accounting;
using StudioServicesApp.Services;
using StudioServicesApp.Views;
using Xamarin.Forms;

namespace StudioServicesApp.ViewModels
{
    public class InvoiceCreationViewModel : MyAuthViewModel
    {
        private RelayCommand<string> nextPageCommand;
        public InvoiceCreationViewModel(INavigationService n, StudioServicesApi a, AlertService al, KeyValueService k) : base(n, a, al, k) { }

        public override async Task NavigatedToAsync(object parameter = null)
        {
            MessengerInstance.Register<bool>(this, "AddCompanyStatus", async (status) =>
            {
                await LoadCompaniesAsync(true);
                if (status)
                    await NavigatedToAsync();
                else if (MyCompanies.Count == 0)
                    Navigation.NavigateTo(ViewModelLocator.NEWS_PAGE);
            });
            await base.NavigatedToAsync();
            Device.BeginInvokeOnMainThread(() =>
            {
                ListCompanies?.Clear();
                ListCompanies.AddRange(GetListCompanies());

                if (ListCompanies.Count == 0)
                {
                    ShowMessage("Non sono presenti aziende.", "Aggiungi azienda", () =>
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            Navigation.PushPopupAsync(new AddCompanyPopup());
                        });
                    });
                }

                if (SelectedIndexInvoiceType < 0)
                    SelectedIndexInvoiceType = 0;

                if (ListCompanies.Count > 0 && SelectedCompany == null)
                    SelectedMyCompanyIndex = 0;
            });
        }
        public override void NavigatedFrom()
        {
            MessengerInstance.Unregister<bool>(this, "AddCompanyStatus");
        }

        private DateTime invoiceDate = DateTime.Now;
        private int invoiceTypeIndex = -1, myCompanyIndex = -1;
        private Company selectedCompany = null, myCompany = null;
        private string invoiceNumberText = string.Empty, invoiceNumberTextExtra = string.Empty;
        public int SelectedMyCompanyIndex
        {
            get => myCompanyIndex;
            set
            {
                if (myCompanyIndex != value)
                {
                    SetMT(ref myCompanyIndex, value);
                    InitFields();
                }
            }
        }
        public int SelectedIndexInvoiceType
        {
            get => invoiceTypeIndex;
            set
            {
                if (invoiceTypeIndex != value)
                {
                    SetMT(ref invoiceTypeIndex, value);
                    SelectedCompany = null;
                    if (value >= 0)
                        PopulateCompanyList(value == 0 ? InvoiceType.SELL : InvoiceType.PURCHASE);
                    InitFields();
                }
            }
        }
        public DateTime InvoiceDate
        {
            get => invoiceDate;
            set
            {
                SetMT(ref invoiceDate, value);
                InitFields();
            }
        }
        public Company SelectedMyCompany
        {
            get => myCompany;
            set
            {
                SetMT(ref myCompany, value);
                InitFields();
            }
        }
        public Company SelectedCompany
        {
            get => selectedCompany;
            set => SetMT(ref selectedCompany, value);
        }
        public MyObservableCollection<Company> ListCompanies { get; } = new MyObservableCollection<Company>();
        public MyObservableCollection<Company> CompanyList { get; } = new MyObservableCollection<Company>();
        public string InvoiceNumberText { get => invoiceNumberText; set => SetMT(ref invoiceNumberText, IntValidation(invoiceNumberText, value)); }
        public string InvoiceNumberExtraText { get => invoiceNumberTextExtra; set => SetMT(ref invoiceNumberTextExtra, StringValidation(InvoiceNumberExtraText, value, 5)); }

        private List<Company> fullListSell, fullListPurchase;
        private void PopulateCompanyList(InvoiceType invoiceType)
        {
            CompanyList.Clear();
            switch (invoiceType)
            {
                case InvoiceType.PURCHASE:
                    fullListPurchase = new List<Company>()
                    {
                        new Company(){Name="Elesoft", VATNumber="012345678912345" },
                        new Company(){Name="Softele", VATNumber="012345678912345" }
                    };
                    CompanyList.AddRange(fullListPurchase);
                    break;
                case InvoiceType.SELL:
                    fullListSell = new List<Company>()
                    {
                        new Company(){Name="Elesell", VATNumber="012345678912345" },
                        new Company(){Name="Softell", VATNumber="012345678912345" }
                    };
                    CompanyList.AddRange(fullListSell);
                    break;
            }
        }
        private void InitFields()
        {
            var invoiceType = (InvoiceType)Enum.ToObject(typeof(InvoiceType), SelectedIndexInvoiceType);
            var invoiceYear = InvoiceDate.Year;
            switch(invoiceType)
            {
                case InvoiceType.PURCHASE:
                    break;
                case InvoiceType.SELL:
                    break;
            }
            /**
             - Selezionare lista clienti o fornitori
             - Inizializzare il campo prossima fattura
             
             **/
        }
        private int GetNextInvoiceNumber()
        {
            // TODO implement
            return 1;
        }
        public RelayCommand<string> NextInvoicePageCommand => nextPageCommand ??
            (nextPageCommand = new RelayCommand<string>((pageIndex) =>
            {
                switch (pageIndex)
                {
                    case "invoice_details":
                        // TODO: verifica che il numero della fattura non esista
                        Navigation.NavigateTo(ViewModelLocator.INVOICE_CREATION_DETAILS);
                        break;
                }
                Debug.WriteLine($"PageIndex: {pageIndex}");
            }));

        private RelayCommand createMyNewCompanyCmd;
        public RelayCommand CreateMyNewCompanyCommand =>
            createMyNewCompanyCmd ??
            (createMyNewCompanyCmd = new RelayCommand(() =>
            {
                Navigation.PushPopupAsync(new AddCompanyPopup());
            }));
        public MyObservableCollection<string> InvoiceDetails { get; } = new MyObservableCollection<string>();
    }
}
