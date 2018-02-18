using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using pinoelefante.Services;
using StudioServicesApp.Services;
using StudioServicesApp.Views;
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
        public const string LOGIN_PAGE = "LoginPage";

        static ViewModelLocator()
        {
            SimpleIoc.Default.Register<INavigationService>(() => new NavigationService());
            var web = new WebService();
            SimpleIoc.Default.Register<WebService>();
            SimpleIoc.Default.Register<StudioServicesApi>();

            SimpleIoc.Default.Register<MyMasterDetailViewModel>();
            SimpleIoc.Default.Register<LoginViewModel>();
        }
        public static void RegisterPages()
        {
            NavigationService.Configure(LOGIN_PAGE, typeof(LoginPage));
        }

        public static NavigationService NavigationService => (NavigationService)GetService<INavigationService>();
        public static T GetService<T>() => SimpleIoc.Default.GetInstance<T>();

        public MyMasterDetailViewModel MyMasterDetailViewModel => GetService<MyMasterDetailViewModel>();
        public LoginViewModel LoginViewModel => GetService<LoginViewModel>();
    }
}
