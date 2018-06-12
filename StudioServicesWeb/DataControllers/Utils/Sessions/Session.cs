using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioServices.Controllers.Utils.Sessions
{
    public class Session
    {
        public string SessionId { get; }

        public Session()
        {
            SessionId = "SSID_"+Guid.NewGuid().ToString();
        }
    }
}
