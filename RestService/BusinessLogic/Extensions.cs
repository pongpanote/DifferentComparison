using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace Assignment.RestService.BusinessLogic
{
    public static class Extensions
    {
        public static bool IsBase64Encoded(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }

            str = str.Trim();
            return (str.Length % 4 == 0) && Regex.IsMatch(str, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
        }

        public static string ConvertToString(this Stream stream)
        {
            var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        public static Stream ConvertToStream(this string str)
        {
            var byteArray = Encoding.ASCII.GetBytes(str);
            return new MemoryStream(byteArray);
        }

        public static bool IsValidJson(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }

            str = str.Trim();
            if (!(str.StartsWith("{") && str.EndsWith("}")) && !(str.StartsWith("[") && str.EndsWith("]")))
            {
                return false;
            }

            try
            {
                JToken.Parse(str);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
