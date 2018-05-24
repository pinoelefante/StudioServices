using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace StudioServicesApp.Services
{
    public class ConnectionStatus
    {
        public IConnectivity Connectivity { get; }
        public ConnectionStatus()
        {
            Connectivity = CrossConnectivity.Current;
        }
        public bool IsConnected(string url)
        {
            if (IsLoopback(url))
                return true;
            if (!CrossConnectivity.IsSupported)
                return true;
            return Connectivity.IsConnected;
        }
        private bool IsLoopback(string url)
        {
            if(Uri.TryCreate(url, UriKind.Absolute, out Uri result))
            {
                switch(result.Host.ToLower())
                {
                    case "127.0.0.1":
                    case "localhost":
                    case "::1":
                        return true;
                }
            }
            return false;
        }
    }
}
