using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioServices.Controllers.Utils
{
    public class FileUtils
    {
        public static bool WriteFile(string folder_path, string filename, byte[] content)
        {
            string destination = Path.Combine(folder_path, filename);
            try
            {
                File.WriteAllBytes(destination, content);
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        public static bool Exists(string folder_path, string filename)
        {
            string path = Path.Combine(folder_path, filename);
            return (File.Exists(path) && new FileInfo(path).Length > 0);
        }
        public static bool Delete(string folder, string filename)
        {
            try
            {
                File.Delete(Path.Combine(folder, filename));
                return true;
            }
            catch
            {
                return false;
            }
            
        }
    }
}
