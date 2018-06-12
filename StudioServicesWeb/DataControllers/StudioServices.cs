using StudioServices.Controllers.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioServices
{
    public class StudioServicesConfig
    {
        public static string IDENTIFICATIONS_FOLDER = "id_folder";
        public static string INVOICE_FOLDER = "invoices";
        public static void Init()
        {
            if(File.Exists("studio_config.json"))
            {
                var options = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText("studio_config.json"));
                IDENTIFICATIONS_FOLDER = options.ContainsKey("identifications_folder") ? options["identifications_folder"] : IDENTIFICATIONS_FOLDER;
                INVOICE_FOLDER = options.ContainsKey("invoices_folder") ? options["invoices_folder"] : INVOICE_FOLDER;
            }
        }
    }
}
