using System;
using StudioServicesApp.ViewModels;
using StudioServicesApp.Views;
using Xamarin.Forms;

namespace StudioServicesApp
{
	public partial class App : Application
	{

		public App ()
		{
			InitializeComponent();

            var nav = new NavigationPage(new InvoiceCreationHome());
            //var nav = new NavigationPage(new LoginPage());
            MainPage = nav;
            //ConfigureNavigation(nav, ViewModelLocator.LOGIN_PAGE);
        }
        public static void ConfigureNavigation(NavigationPage nav, string homePageKey, bool askToClose = false)
        {
            /*
            IClosingApp closer = ViewModelLocator.GetService<IClosingApp>();
            Action act = async () =>
            {
                if (askToClose)
                {
                    if (!await UserDialogs.Instance.ConfirmAsync("Would you like to close the app?", "Closing app"))
                        return;
                }
                closer.CloseApp();
            };
            ViewModelLocator.NavigationService.Initialize(nav, homePageKey, act);
            */
            ViewModelLocator.NavigationService.Initialize(nav, homePageKey);
        }

        protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
