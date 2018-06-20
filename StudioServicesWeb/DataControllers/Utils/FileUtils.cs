﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioServices.Controllers.Utils
{
    public class FileUtils
    {
        public static bool WriteFile(string base_folder, string folder, string filename, byte[] content)
        {
            string destination = Path.Combine(base_folder, folder, filename);
            CreateDirectoryFromFilePath(destination);
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
        public static bool CreateDirectoryFromFilePath(string file_path)
        {
            var dir_path = file_path.Substring(0, file_path.LastIndexOf(Path.DirectorySeparatorChar));
            var dir = Directory.CreateDirectory(dir_path);
            return (dir != null && dir.Exists);
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
