﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using pinoelefante.Droid.Services;
using pinoelefante.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(CloseAppDroid))]
namespace pinoelefante.Droid.Services
{
    public class CloseAppDroid : IClosingApp
    {
        public void CloseApp()
        {
            Process.KillProcess(Process.MyPid());
        }
    }
}