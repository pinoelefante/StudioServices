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
            INVOICE_CREATION_DETAILS = "InvoiceCreationDetailsPage";

        public const string SERVER_SETTINGS = "ServerSettingsPage";

        static ViewModelLocator()
        {
            SimpleIoc.Default.Register<INavigationService>(() => new NavigationService());
            SimpleIoc.Default.Register<WebService>();
            SimpleIoc.Default.Register<StudioServicesApi>();
            SimpleIoc.Default.Register<CacheManager>();
            var sqlite = DependencyService.Get<ISQLite>();
            SimpleIoc.Default.Register<ISQLite>(() => sqlite);
            SimpleIoc.Default.Register<DatabaseService>();
            SimpleIoc.Default.Register<ConnectionStatus>();
            SimpleIoc.Default.Register<AlertService>();

            SimpleIoc.Default.Register<MyMasterDetailViewModel>();
            SimpleIoc.Default.Register<LoginViewModel>();
            SimpleIoc.Default.Register<RegisterViewModel>();
            SimpleIoc.Default.Register<NewsPageViewModel>();
            SimpleIoc.Default.Register<ViewMessageViewModel>();
            SimpleIoc.Default.Register<AddIdentificationDocumentViewModel>();
            SimpleIoc.Default.Register<InvoiceCreationViewModel>();

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

            NavigationService.Configure(SERVER_SETTINGS, typeof(ServerSettings));
        }

        public static NavigationService NavigationService => (NavigationService)GetService<INavigationService>();
        public static T GetService<T>() => SimpleIoc.Default.GetInstance<T>();

        public MyMasterDetailViewModel MyMasterDetailViewModel => GetService<MyMasterDetailViewModel>();
        public LoginViewModel LoginViewModel => GetService<LoginViewModel>();
        public RegisterViewModel RegisterViewModel => GetService<RegisterViewModel>();
        public NewsPageViewModel NewsPageViewModel => GetService<NewsPageViewModel>();
        public ViewMessageViewModel ViewMessageViewModel => GetService<ViewMessageViewModel>();
        public AddIdentificationDocumentViewModel AddIdentificationDocumentViewModel => GetService<AddIdentificationDocumentViewModel>();
        public InvoiceCreationViewModel InvoiceCreationViewModel => GetService<InvoiceCreationViewModel>();

        public ServerSettingsViewModel ServerSettingsViewModel => GetService<ServerSettingsViewModel>();
    }
}
