using pinoelefante.Views;
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
	public partial class AddCompanyProductPopup : MyPopupPage
	{
		public AddCompanyProductPopup (object parameters = null) : base(parameters)
		{
			InitializeComponent ();
		}
	}
}