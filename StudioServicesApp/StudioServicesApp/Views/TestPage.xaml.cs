using StudioServicesApp.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace StudioServicesApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TestPage : ContentPage
	{
        private WebService web;
		public TestPage ()
		{
			InitializeComponent ();
            web = new WebService();
		}
        public async Task Button_Clicked(object sender, object parameters)
        {
            var res = await web.SendGetRequestAsync("https://www.google.com", new KeyValuePair<string, string>("test", "prova"), new KeyValuePair<string,string>("test2","prova"));
            // Debug.WriteLine(res);
        }
	}
}