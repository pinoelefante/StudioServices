using GalaSoft.MvvmLight.Command;
using pinoelefante.ViewModels;
using pinoelefante.Views;
using StudioServicesApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace StudioServicesApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterPage : MyContentPage
    {
        public ListView ListView { get { return listView; } }
        public MasterDetailPage MasterDetails { get; set; }
        private MyMasterDetailViewModel VM => this.BindingContext as MyMasterDetailViewModel;
        public MasterPage() : base()
        {
            InitializeComponent();
            ListView.ItemsSource = new List<MasterPageItem>()
            {
                new MasterPageItem()
                {
                    Title = "News & Messaggi",
                    PageKey = ViewModelLocator.NEWS_PAGE,
                    //IconSource = "home.png"
                },
                new MasterPageItem()
                {
                    Title = "Contabilità",
                    PageKey = ViewModelLocator.WAREHOUSE_HOME,
                    IconSource = "add_icon.png"
                }
            };
            VM.PropertyChanged += VM_PropertyChanged;
        }

        private void VM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                switch (e.PropertyName)
                {
                    case nameof(VM.MyCompanies):
                        switch (VM.MyCompanies.Count)
                        {
                            case 0:
                                Grid.SetColumn(addCompanyButton, 0);
                                addCompanyButton.Text = "Aggiungi azienda";
                                myCompaniesPicker.IsVisible = false;
                                myCompanyLabel.IsVisible = false;
                                break;
                            case 1:
                                Grid.SetColumn(addCompanyButton, 1);
                                addCompanyButton.Text = "+";
                                myCompaniesPicker.IsVisible = false;
                                myCompanyLabel.IsVisible = true;
                                break;
                            default:
                                Grid.SetColumn(addCompanyButton, 1);
                                addCompanyButton.Text = "+";
                                myCompaniesPicker.IsVisible = true;
                                myCompanyLabel.IsVisible = false;
                                break;
                        }
                        break;
                    case nameof(VM.SelectedCompany):
                        myCompanyLabel.Text = VM.SelectedCompany?.Name;
                        break;
                }
            });
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            if(Device.Idiom != TargetIdiom.Desktop)
                this.MasterDetails.IsPresented = false;
        }
    }
}
