using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Nintex.Business
{
    public static class Utility
    {
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static string AddTextToLeftInt(this int value, int totalWidth)
        {
            return value.ToString().PadLeft(totalWidth, '0');
        }

        public static string AddTextToLeftInt(this string value, int totalWidth)
        {
            return value.PadLeft(totalWidth, '0');
        }


        public static string Post(string url, Dictionary<string, string> data)
        {
            HttpClient client = new HttpClient();

            var content = new FormUrlEncodedContent(data);
            
            var response = client.PostAsync(url, content).Result;

            var responseString = response.Content.ReadAsStringAsync().Result;

            return responseString;
            
        }

        public static string Post(string url, string contentString)
        {
            HttpClient client = new HttpClient();

            var content = new StringContent(contentString, Encoding.UTF8, "application/json");

            var response = client.PostAsync(url, content).Result;

            var responseString = response.Content.ReadAsStringAsync().Result;

            return responseString;

        }

        public static string Get(string url)
        {
            HttpClient client = new HttpClient();

            var responseString = client.GetStringAsync(url).Result;

            return responseString;
        }
    }
}
