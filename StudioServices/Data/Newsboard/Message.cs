using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioServices.Data.Newsboard
{
    public class Message : PersonReference
    {
        public string Content { get; set; }
        public bool IsPrivate { get; set; }
        public bool IsExpireEnabled { get; set; }
        public DateTime ExpireDate { get; set; }
        public bool IsMarked { get; set; }
    }
}
