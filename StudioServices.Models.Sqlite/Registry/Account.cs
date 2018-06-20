using Newtonsoft.Json;
using SQLite;

namespace StudioServices.Data.Sqlite.Registry
{
    public class Account : PersonReference
    {
        [Unique]
        public string Username { get; set; }
        [JsonIgnore]
        public string Password { get; set; }
        [JsonIgnore]
        public bool IsAdmin { get; private set; } = false;
    }
}
