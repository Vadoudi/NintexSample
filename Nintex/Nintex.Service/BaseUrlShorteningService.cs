using Nintex.Business;
using Nintex.Business.Service;
using Nintex.Service.Filing;
using System;
using System.Linq;

namespace Nintex.Service
{
    /// <summary>
    /// This class is a base class for do url Shortening and implements IUrlShorteningService.
    /// The key in this service generate in 3 part 
    /// first: GenerateStaticStringPart()
    /// second:GenerateStaticNumberPart()
    /// Third:GenerateRandomStringPart()
    /// </summary>
    public abstract class BaseUrlShorteningService : IUrlShorteningService
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
        const int MinimumUserAliasLength = 6;

        private static Random random = new Random();

        public SystemResult<string> Add(string userUrl, string userAlias = "")
        {
            var key = "";
            var staticStringPart = "";
            var isUserAlias = false;

            if (userAlias.IsNullOrEmpty())
            {
                staticStringPart = GenerateStaticStringPart();

                string staticNumberPart = GenerateStaticNumberPart();

                string randomStringPart = GenerateRandomStringPart();

                key = string.Join("", staticStringPart, staticNumberPart, randomStringPart);
            }
            else
            {
                if (userAlias.Length < MinimumUserAliasLength)
                {
                    return new SystemResult<string>
                    {
                        HasError = true,
                        ErrorCode = "1001",
                        ErrorMessage = "The custom alias you've chosen is not available. We've created a random one for you instead, but you can try assigning a different custom alias again below. Use 6 characters or more for the best chance of getting a unique unassigned alias.",
                        ResultObject = string.Empty
                    };
                }

                isUserAlias = true;

                key = userAlias;
            }

            return Save(staticStringPart, key, userUrl, isUserAlias);
        }

        protected abstract SystemResult<string> Save(string staticStringPart, string key, string userUrl, bool isUserAlias = false);

        /// <summary>
        /// Each child should implement this method by itself.
        /// </summary>
        /// <returns></returns>
        protected abstract string GenerateStaticStringPart();

        /// <summary>
        /// Each child should implement this method by itself.
        /// </summary>
        /// <returns></returns>
        protected abstract string GenerateStaticNumberPart();

        /// <summary>
        /// With using chars parameter this method generate 4 random char
        /// </summary>
        /// <returns></returns>
        protected string GenerateRandomStringPart()
        {
            int length = 4;

            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public abstract SystemResult<bool> Exists(string key);

        public abstract SystemResult<string> Get(string systemUrl);

    }
}
