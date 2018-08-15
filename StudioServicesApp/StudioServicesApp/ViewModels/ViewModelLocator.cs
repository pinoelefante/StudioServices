using AC.Components.Util;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using pinoelefante.Services;
using StudioServicesApp.Services;
using StudioServicesApp.ViewModels.HiddenPages;
using StudioServicesApp.Views;
using StudioServicesApp.Views.HiddenPages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace StudioServicesApp.ViewModels
{
    public class ViewModelLocator
    {
        public const string LOGIN_PAGE = "LoginPage",
            NEWS_PAGE = "NewsPage",
            REGISTER_PAGE = "RegisterPage",
            VIEW_MESSAGE_PAGE = "ViewMessagePage",
            ADD_IDENTIFICATION_DOC_PAGE = "AddIdentificationDocPage",
            INVOICE_CREATION_HOME = "InvoiceCreationHomePage",
            INVOICE_CREATION_DETAILS = "InvoiceCreationDetailsPage",
            WAREHOUSE_HOME = "WarehouseHomePage",
            USERPROFILE_PAGE = "UserProfilePage";

        public const string SERVER_SETTINGS = "ServerSettingsPage";

        static ViewModelLocator()
        {
            SimpleIoc.Default.Register<INavigationService>(() => new NavigationService());
            SimpleIoc.Default.Register<WebService>();
            SimpleIoc.Default.Register<StudioServicesApi>();
            SimpleIoc.Default.Register<CacheManager>();
            var sqlite = DependencyService.Get<ISQLite>();
            SimpleIoc.Default.Register<ISQLite>(() => sqlite);
            SimpleIoc.Default.Register(() => DependencyService.Get<IClosingApp>());
            SimpleIoc.Default.Register<DatabaseService>();
            SimpleIoc.Default.Register<ConnectionStatus>();
            SimpleIoc.Default.Register<AlertService>();
            SimpleIoc.Default.Register<KeyValueService>();
            SimpleIoc.Default.Register<AssemblyFileReader>();

            SimpleIoc.Default.Register<MyMasterDetailViewModel>();
            SimpleIoc.Default.Register<LoginViewModel>();
            SimpleIoc.Default.Register<RegisterViewModel>();
            SimpleIoc.Default.Register<NewsPageViewModel>();
            SimpleIoc.Default.Register<ViewMessageViewModel>();
            SimpleIoc.Default.Register<AddIdentificationDocumentViewModel>();
            SimpleIoc.Default.Register<InvoiceCreationHomeViewModel>();
            SimpleIoc.Default.Register<InvoiceCreationDetailsViewModel>();
            SimpleIoc.Default.Register<AddCompanyViewModel>();
            SimpleIoc.Default.Register<AddAddressViewModel>();
            SimpleIoc.Default.Register<WarehouseProductsManagerViewModel>();
            SimpleIoc.Default.Register<WarehouseInvoiceListViewModel>();
            SimpleIoc.Default.Register<WarehouseClientsSuppliersListViewModel>();
            SimpleIoc.Default.Register<ViewUserProfileMainPageViewModel>();
            SimpleIoc.Default.Register<ViewUserProfileContactsViewModel>();
            SimpleIoc.Default.Register<ViewUserProfileDocumentsViewModel>();
            SimpleIoc.Default.Register<AddContactMethodViewModel>();
            SimpleIoc.Default.Register<AddEmailViewModel>();
            SimpleIoc.Default.Register<AddCompanyProductViewModel>();

            SimpleIoc.Default.Register<ServerSettingsViewModel>();

            RegisterPages();
        }
        public static void RegisterPages()
        {
            NavigationService.Configure(LOGIN_PAGE, typeof(LoginPage));
            NavigationService.Configure(NEWS_PAGE, typeof(NewsPage));
            NavigationService.Configure(REGISTER_PAGE, typeof(RegisterPage));
            NavigationService.Configure(VIEW_MESSAGE_PAGE, typeof(ViewMessagePage));
            NavigationService.Configure(ADD_IDENTIFICATION_DOC_PAGE, typeof(AddIdentificationDocumentPage));
            NavigationService.Configure(INVOICE_CREATION_HOME, typeof(InvoiceCreationHome));
            NavigationService.Configure(INVOICE_CREATION_DETAILS, typeof(InvoiceCreationDetails));
            NavigationService.Configure(WAREHOUSE_HOME, typeof(WarehouseHome));
            NavigationService.Configure(SERVER_SETTINGS, typeof(ServerSettings));
            NavigationService.Configure(USERPROFILE_PAGE, typeof(ViewUserProfilePage));
        }

        public static NavigationService NavigationService => (NavigationService)GetService<INavigationService>();
        public static T GetService<T>() => SimpleIoc.Default.GetInstance<T>();

        public MyMasterDetailViewModel MyMasterDetailViewModel => GetService<MyMasterDetailViewModel>();
        public LoginViewModel LoginViewModel => GetService<LoginViewModel>();
        public RegisterViewModel RegisterViewModel => GetService<RegisterViewModel>();
        public NewsPageViewModel NewsPageViewModel => GetService<NewsPageViewModel>();
        public ViewMessageViewModel ViewMessageViewModel => GetService<ViewMessageViewModel>();
        public AddIdentificationDocumentViewModel AddIdentificationDocumentViewModel => GetService<AddIdentificationDocumentViewModel>();
        public InvoiceCreationHomeViewModel InvoiceCreationHomeViewModel => GetService<InvoiceCreationHomeViewModel>();
        public InvoiceCreationDetailsViewModel InvoiceCreationDetailsViewModel => GetService<InvoiceCreationDetailsViewModel>();
        public AddCompanyViewModel AddCompanyViewModel => GetService<AddCompanyViewModel>();
        public AddAddressViewModel AddAddressViewModel => GetService<AddAddressViewModel>();
        public WarehouseProductsManagerViewModel WarehouseProductsManagerViewModel => GetService<WarehouseProductsManagerViewModel>();
        public WarehouseClientsSuppliersListViewModel WarehouseClientsSuppliersListViewModel => GetService<WarehouseClientsSuppliersListViewModel>();
        public WarehouseInvoiceListViewModel WarehouseInvoiceListViewModel => GetService<WarehouseInvoiceListViewModel>();
        public ViewUserProfileMainPageViewModel ViewUserProfileMainPageViewModel => GetService<ViewUserProfileMainPageViewModel>();
        public ViewUserProfileContactsViewModel ViewUserProfileContactsViewModel => GetService<ViewUserProfileContactsViewModel>();
        public ViewUserProfileDocumentsViewModel ViewUserProfileDocumentsViewModel => GetService<ViewUserProfileDocumentsViewModel>();
        public AddContactMethodViewModel AddContactMethodViewModel => GetService<AddContactMethodViewModel>();
        public ServerSettingsViewModel ServerSettingsViewModel => GetService<ServerSettingsViewModel>();
        public AddEmailViewModel AddEmailViewModel => GetService<AddEmailViewModel>();
        public AddCompanyProductViewModel AddCompanyProductViewModel => GetService<AddCompanyProductViewModel>();
    }
}
