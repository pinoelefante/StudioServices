using System;

using StudioServicesApp.Views;
using Xamarin.Forms;

namespace StudioServicesApp
{
	public partial class App : Application
	{

		public App ()
		{
			InitializeComponent();


            MainPage = new MyMasterPage();
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
