using SQLite;
using SQLiteNetExtensions.Attributes;
using StudioServices.Registry.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioServices.Data.Newsboard
{
    public class Message : PersonReference
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public bool IsPrivate { get; set; }
        public bool IsExpireEnabled { get; set; }
        public DateTime ExpireDate { get; set; }
        public bool IsMarked { get; set; }

        [ForeignKey(typeof(Person))]
        public int SenderId { get; set; }

        [Ignore]
        public string ShortContent
        {
            get
            {
                Content.Substring(0, (Content.Length > 120 ? 120 : Content.Length));
                return Content;
            }
        }
        private bool _readStatus;
        [Ignore]
        public bool IsRead { get => _readStatus; set { Set(ref _readStatus, value); } }
    }
}
