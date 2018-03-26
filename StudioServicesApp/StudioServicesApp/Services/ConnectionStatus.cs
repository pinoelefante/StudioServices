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
        public bool IsConnected()
        {
            if (!CrossConnectivity.IsSupported)
                return true;
            return Connectivity.IsConnected;
        }
    }
}
