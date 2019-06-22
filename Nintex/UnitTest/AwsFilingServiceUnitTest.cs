using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Nintex.Business;
using Nintex.Business.Service;
using Nintex.Service.Caching;
using Nintex.Service.Filing;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UnitTest
{
    [TestClass]
    public class AwsFilingServiceUnitTest
    {
        const string _awsUrl = "http://localhost:58184/api/shortening";

        [TestMethod]
        public void TestAdd()
        {
            var key = Add("http://translate.google.com/a");

            var key1 = Add("http://translate.google.com/b", "0123456");

            var key2 = Add("http://translate.google.com/", "01234");

            var key3 = Add("http://translate.google.com/c", "abv01234");

            var key4 = Add("http://translate.google.com/d", "a0123456");
        }

        [TestMethod]
        public void TestGet()
        {
            var key = Get("a0001AVAo");

            var key1 = Get("0123456");

            var key2 = Get("01234");

            var key3 = Get("abv01234");

            var key4 = Get("a0123456");

            var key5 = Get("a0123456");

            var key6 = Get("abcd0123456");
        }


        private SystemResult<string> Add(string userUrl, string userAlias = "")
        {
            var dic = new Dictionary<string, string>();

            var model = new AddModel(userUrl, userAlias);

            var modelString = JsonConvert.SerializeObject(model);
           
            var url = $"{_awsUrl}/Add";

            var response = Utility.Post(url, modelString);

            var result = JsonConvert.DeserializeObject<SystemResult<string>>(response);

            dic.Add(result.ResultObject, "");

            return result;
        }

        private SystemResult<string> Get(string key)
        {
            var dic = new Dictionary<string, string>();

            var url = $"{_awsUrl}/Get/{key}";

            var response = Utility.Get(url);

            var result = JsonConvert.DeserializeObject<SystemResult<string>>(response);

            dic.Add(result.ResultObject, "");

            return result;
        }

        [DataContract(Name = "data")]
        class AddModel
        {
            public AddModel(string userUrl,string userAlias)
            {
                UserUrl = userUrl;

                UserAlias = userAlias ?? "";
            }

            [JsonProperty("UserUrl")]
            public string UserUrl { get; set; }

            [JsonProperty("UserAlias")]
            public string UserAlias { get; set; }
        }
    }
}
