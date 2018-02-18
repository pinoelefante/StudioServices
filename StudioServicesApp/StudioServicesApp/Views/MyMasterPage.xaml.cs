using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pinoelefante.ViewModels;
using pinoelefante.Views;
using StudioServicesApp.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace StudioServicesApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyMasterPage : MasterDetailPage
    {
        public MyMasterPage()
        {
            InitializeComponent();
            masterPage.ListView.ItemTapped += ListView_ItemTapped;
            masterPage.MasterDetails = this;
            App.ConfigureNavigation(navigationPage, ViewModelLocator.NEWS_PAGE, true);
        }
        private MyMasterDetailViewModel VM => this.BindingContext as MyMasterDetailViewModel;
        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var item = e.Item as MasterPageItem;
            VM.Navigate(item.PageKey);
            if(Device.Idiom != TargetIdiom.Desktop)
                IsPresented = false;
        }
    }
}
