using SQLite;
using System;
namespace StudioServices.Data.Registry
{
    public class Account : PersonReference
    {
        [Unique]
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
