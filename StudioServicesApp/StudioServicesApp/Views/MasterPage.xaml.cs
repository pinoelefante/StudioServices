﻿using GalaSoft.MvvmLight.Command;
using pinoelefante.ViewModels;
using pinoelefante.Views;
using StudioServicesApp.ViewModels;
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
    public partial class MasterPage : MyContentPage
    {
        public ListView ListView { get { return listView; } }
        public MasterDetailPage MasterDetails { get; set; }
        public MasterPage() : base()
        {
            InitializeComponent();
            ListView.ItemsSource = new List<MasterPageItem>()
            {
                new MasterPageItem()
                {
                    Title = "News & Messaggi",
                    PageKey = ViewModelLocator.NEWS_PAGE,
                    //IconSource = "home.png"
                },
                new MasterPageItem()
                {
                    Title = "Contabilità",
                    PageKey = ViewModelLocator.WAREHOUSE_HOME,
                    IconSource = "add_icon.png"
                }
            };
        }
    }
}
