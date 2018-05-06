﻿using pinoelefante.Views;
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
	public partial class InvoiceCreationHome : MyContentPage
	{
		public InvoiceCreationHome ()
		{
			InitializeComponent ();
		}

        private void Entry_Completed(object sender, EventArgs e)
        {
            Debug.WriteLine("Entry completed");
        }
    }
}