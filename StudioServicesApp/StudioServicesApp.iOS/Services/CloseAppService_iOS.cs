using pinoelefante.iOS.Services;
using pinoelefante.Services;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

[assembly: Xamarin.Forms.Dependency(typeof(CloseAppService_iOS))]
namespace pinoelefante.iOS.Services
{
    public class CloseAppService_iOS : IClosingApp
    {
        public void CloseApp()
        {
            Thread.CurrentThread.Abort();
        }
    }
}
