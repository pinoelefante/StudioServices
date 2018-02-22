using System;
using System.Collections.Generic;
using System.Text;

namespace StudioServicesApp.Services
{
    public class CacheManager
    {
        private Dictionary<string, object> dictionary;
        public CacheManager()
        {
            dictionary = new Dictionary<string, object>();
        }
        public T GetValue<T>(string key, T  def = default(T))
        {
            if (!dictionary.ContainsKey(key))
                return def;
            return (T)dictionary[key];
        }
        public void SetValue(string key, object value)
        {
            if (!dictionary.ContainsKey(key))
                dictionary.Add(key, value);
            else
                dictionary[key] = value;
        }
    }
}
