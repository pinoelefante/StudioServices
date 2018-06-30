using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace StudioServicesApp.Services
{
    public class AssemblyFileReader
    {
        public T ReadLocalJson<T>(string filename)
        {
            var assembly = typeof(App).GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream($"StudioServicesApp.{filename.Replace(Path.PathSeparator, '.')}");
            string text = string.Empty;
            using (var reader = new StreamReader(stream))
            {
                text = reader.ReadToEnd();
            }
            return JsonConvert.DeserializeObject<T>(text);
        }
    }
}
