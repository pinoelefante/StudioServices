using StudioServices.Data.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioServices.Data.EntityFramework.Newsboard
{
    public class Message : PersonReference
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public bool IsPrivate { get; set; }
        public bool IsExpireEnabled { get; set; }
        public DateTime ExpireDate { get; set; }
        public bool IsMarked { get; set; }

        // [ForeignKey(typeof(Person))]
        public int SenderId { get; set; }

        [NotMapped]
        public string ShortContent
        {
            get
            {
                Content.Substring(0, (Content.Length > 120 ? 120 : Content.Length));
                return Content;
            }
        }
        private bool _readStatus;

        [NotMapped]
        public bool IsRead { get => _readStatus; set { Set(ref _readStatus, value); } }
    }
}
