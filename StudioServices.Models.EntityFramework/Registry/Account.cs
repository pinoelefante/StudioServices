using Newtonsoft.Json;
using System;

namespace StudioServices.Data.EntityFramework.Registry
{
    public class Account : PersonReference
    {
        // [Unique]
        public string Username { get; set; }
        [JsonIgnore]
        public string Password { get; set; }
        [JsonIgnore]
        public bool IsAdmin { get; private set; } = false;
    }
}
