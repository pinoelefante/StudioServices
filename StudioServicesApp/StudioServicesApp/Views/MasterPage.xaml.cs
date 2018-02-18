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
                    PageKey = ViewModelLocator.LOGIN_PAGE,
                    //IconSource = "home.png"
                },
                /*
                new MasterPageItem()
                {
                    Title = "Activities",
                    PageKey = ViewModelLocator.Activities
                },
                new MasterPageItem()
                {
                    Title = "Friends",
                    PageKey = ViewModelLocator.FriendsPage
                }
                */
            };

            (ViewModel as MyMasterDetailViewModel).CloseMasterPage = new RelayCommand(() =>
            {
                if(Device.Idiom != TargetIdiom.Desktop)
                    MasterDetails.IsPresented = false;
            });
        }
    }
}
