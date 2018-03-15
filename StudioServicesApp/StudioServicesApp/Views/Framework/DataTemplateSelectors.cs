using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudioServices.Data.Newsboard;
using Xamarin.Forms;

namespace StudioServicesApp.Views
{
	public class NewsboardNewsTemplateSelector : DataTemplateSelector
	{
		public DataTemplate NewsReaded { get; set; }
		public DataTemplate NewsNotRead { get; set; }

		protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
		{
			var message = item as Message;
			if (message == null)
				return null;
			if (message.IsRead)
				return NewsReaded;
			return NewsNotRead;
		}
	}
}
