using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using pinoelefante.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace StudioServicesApp.ViewModels
{
    public class ViewModelLocator
    {
        public const string HOME_PAGE = "HomePage";

        static ViewModelLocator()
        {
            // ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            var nav = new NavigationService();
            SimpleIoc.Default.Register<INavigationService>(() => nav);
            /*
            SimpleIoc.Default.Register<PRServer>();
            SimpleIoc.Default.Register<PRCache>();
            var dbService = DependencyService.Get<ISQLite>();
            SimpleIoc.Default.Register<ISQLite>(() => dbService);
            var closeApp = DependencyService.Get<IClosingApp>();
            SimpleIoc.Default.Register<IClosingApp>(() => closeApp);

            SimpleIoc.Default.Register<LoginPageViewModel>();
            SimpleIoc.Default.Register<RegisterPageViewModel>();
            SimpleIoc.Default.Register<ForgotPasswordViewModel>();
            SimpleIoc.Default.Register<MyMasterDetailViewModel>();
            SimpleIoc.Default.Register<ActivitiesListViewModel>();
            SimpleIoc.Default.Register<CreateActivityViewModel>();
            SimpleIoc.Default.Register<HomePageViewModel>();
            SimpleIoc.Default.Register<ActivityDetailsViewModel>();
            SimpleIoc.Default.Register<SearchActivityViewModel>();
            SimpleIoc.Default.Register<LoadingViewModel>();
            SimpleIoc.Default.Register<AddLocationViewModel>();
            SimpleIoc.Default.Register<ViewUserProfileViewModel>();
            SimpleIoc.Default.Register<FriendsViewModel>();
            SimpleIoc.Default.Register<SearchFriendsViewModel>();
            */
        }
        public static void RegisterPages()
        {
            /*
            nav.Configure(ForgotPassword, typeof(Views.ForgotPasswordPage));
            nav.Configure(LoginPage, typeof(Views.LoginPage));
            nav.Configure(RegisterPage, typeof(Views.RegisterPage));
            nav.Configure(HomePage, typeof(Views.HomePage));

            nav.Configure(Activities, typeof(Views.Activities));
            nav.Configure(CreateActivity, typeof(Views.CreateActivityPage));
            nav.Configure(AddLocation, typeof(Views.AddLocationPage));
            nav.Configure(CreateActivityConfirm, typeof(Views.CreateActivityConfirmPage));

            nav.Configure(ActivityDetails, typeof(Views.ActivityDetails));
            nav.Configure(ActivitySearch, typeof(Views.SearchActivity));
            nav.Configure(ActivitySearchResults, typeof(Views.SearchActivityResults));

            nav.Configure(ViewUserProfile, typeof(Views.ViewUserProfilePage));
            nav.Configure(FriendsPage, typeof(Views.FriendsPage));
            nav.Configure(SearchFriendsPage, typeof(Views.SearchFriendsPage));
            nav.Configure(SettingsPage, typeof(Views.SettingsPage));
            */
        }
        public static NavigationService NavigationService => (NavigationService)GetService<INavigationService>();
        public static T GetService<T>() => SimpleIoc.Default.GetInstance<T>();

        public MyMasterDetailViewModel MyMasterDetailViewModel => GetService<MyMasterDetailViewModel>();
        /*
        public LoginPageViewModel LoginPageViewModel => GetService<LoginPageViewModel>();
        public RegisterPageViewModel RegisterPageViewModel => GetService<RegisterPageViewModel>();
        public ForgotPasswordViewModel ForgotPasswordViewModel => GetService<ForgotPasswordViewModel>();
        public MyMasterDetailViewModel MyMasterDetailViewModel => GetService<MyMasterDetailViewModel>();
        public CreateActivityViewModel CreateActivityViewModel => GetService<CreateActivityViewModel>();
        public HomePageViewModel HomePageViewModel => GetService<HomePageViewModel>();
        public ActivitiesListViewModel ActivitiesListViewModel => GetService<ActivitiesListViewModel>();
        public ActivityDetailsViewModel ActivityDetailsViewModel => GetService<ActivityDetailsViewModel>();
        public SearchActivityViewModel SearchActivityViewModel => GetService<SearchActivityViewModel>();
        public LoadingViewModel LoadingViewModel => GetService<LoadingViewModel>();
        public AddLocationViewModel AddLocationViewModel => GetService<AddLocationViewModel>();
        public ViewUserProfileViewModel ViewUserProfileViewModel => GetService<ViewUserProfileViewModel>();
        public FriendsViewModel FriendsViewModel => GetService<FriendsViewModel>();
        public SearchFriendsViewModel SearchFriendsViewModel => GetService<SearchFriendsViewModel>();
        */
    }
}
