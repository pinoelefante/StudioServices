using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioServices.Controllers.Utils
{
    public class StringUtils
    {
        private static Random random = new Random();
        const string chars = "abcdefghijklmnopqrstuvxywzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        public static string RandomString(int length)
        {
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static char[] RandomLetters(int length)
        {
            return Enumerable.Repeat(chars.Substring(0, chars.Length-10), length).Select(c => c[random.Next(c.Length)]).ToArray();
        }
        public static string RandomString()
        {
            return Path.GetRandomFileName().Replace(".", "");
        }
    }
}
