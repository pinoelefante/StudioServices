using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudioServicesApp.Services
{
    public class KeyValueService
    {
        private DatabaseService db;
        public KeyValueService(DatabaseService service)
        {
            db = service;
        }
        public bool Add(string key, string value)
        {
            try
            {
                db.SaveItem(new KVSetting(key, value));
                return true;
            }
            catch
            {
                return false;
            }
        }
        public string Get(string key, string defValue = null)
        {
            var k = db.GetByPk<KVSetting>(key);
            if (k == null)
                return defValue;
            return k.Value;
        }
        public bool Remove(string key)
        {
            return db.DeleteByPk<KVSetting>(key);
        }
    }
    public class KVSetting
    {
        [PrimaryKey]
        public string Key { get; set; }
        public string Value { get; set; }

        public KVSetting() { }
        public KVSetting(string k, string v)
        {
            Key = k;
            Value = v;
        }
    }
}
