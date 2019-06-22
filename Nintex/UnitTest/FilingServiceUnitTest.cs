using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nintex.Business.Service;
using Nintex.Service.Caching;
using Nintex.Service.Filing;
using System.Collections.Generic;

namespace UnitTest
{
    [TestClass]
    public class FilingServiceUnitTest
    {
        //IUrlShorteningFileService _urlShorteningFileService = new CachingUrlShorteningFileService();
        IUrlShorteningFileService _urlShorteningFileService = new FilingUrlShorteningFileService();

        [TestMethod]
        public void TestAdd()
        {
            var service = new FilingUrlShorteningService(_urlShorteningFileService);

            var key = service.Add("http://translate.google.com/a");

            var key1 = service.Add("http://translate.google.com/b", "0123456");

            var key2 = service.Add("http://translate.google.com/", "01234");

            var key3 = service.Add("http://translate.google.com/c", "abv01234");

            var key4 = service.Add("http://translate.google.com/d", "a0123456");
        }

        [TestMethod]
        public void TestGet()
        {
            var service = new FilingUrlShorteningService(_urlShorteningFileService);

            var key = service.Get("a0001AVAo");

            var key1 = service.Get("0123456");

            var key2 = service.Get("01234");

            var key3 = service.Get("abv01234");

            var key4 = service.Get("a0123456");

            var key5 = service.Get("a0123456");

            var key6 = service.Get("abcd0123456");
        }

        [TestMethod]
        public void TestExists()
        {
            var service = new FilingUrlShorteningService(_urlShorteningFileService);

            var key = service.Exists("a0001AVAo");

            var key1 = service.Exists("0123456");

            var key2 = service.Exists("01234");

            var key3 = service.Exists("abv01234");

            var key4 = service.Exists("a0123456");
        }

        [TestMethod]
        public void TestBulkAdd()
        {
            var dic = new Dictionary<string, string>();

            for (var i = 0; i < 10000; i++)
            {
                var service = new FilingUrlShorteningService(_urlShorteningFileService);

                var key = service.Add($"http://translate.google.com/{i}");

                dic.Add(key.ResultObject, "");
            }

            var t = dic;
        }

        [TestMethod]
        public void TestBulkAddThread()
        {
            var dic = new Dictionary<string, string>();

            for (var i = 0; i < 10000; i++)
            {
                var thr = new System.Threading.Thread(() => Add(dic, i));
                thr.Start();
            }

            var t = dic;
        }

        private void Add(Dictionary<string, string> dic, int counter)
        {
            var service = new FilingUrlShorteningService(_urlShorteningFileService);

            var key = service.Add($"http://translate.google.com/{counter}");

            dic.Add(key.ResultObject, "");
        }
    }
}
