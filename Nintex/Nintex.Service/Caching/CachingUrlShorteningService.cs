using Nintex.Business;
using Nintex.Business.Service;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nintex.Service.Caching
{
    static class CachingUrlShorteningServiceStaticData
    {
        public static Locker SaveToFileLocker = new Locker();

        public static Dictionary<string, string> NintexObjectCache = new Dictionary<string, string>();

        public static Dictionary<string, string> LoadedFile = new Dictionary<string, string>();

        public static int? TheLastNumber = null;

        public static string TheLastStringStaticName = "";

        public const int MaxNumber = 8999;

        public const int StaticNumberLength = 4;

        public const string FirstString = "a";

        public const string LastString = "z";
    }

    public class CachingUrlShorteningService : BaseUrlShorteningService, IUrlShorteningService
    {
        const string FolderName = "data";

        const string FileTypeExtentionName = "ntfl";

        const string StaticNumberFileName = "staticnumber";

        const string StaticStringFileName = "staticstring";

        const string UserAliasFolderName = "useralias";

        protected override SystemResult<string> Save(string staticStringPart, string key, string userUrl, bool isUserAlias = false)
        {
            var path = "";
            if (!isUserAlias)
                path = CheckForExistingFile(staticStringPart);
            else
                path = GetPathFromKey(key);

            lock (CachingUrlShorteningServiceStaticData.SaveToFileLocker[path])
            {
                var value = "";

                SystemResult<bool> checkKeyResult = IsKeyExists(path, key, true, out value);

                if (checkKeyResult.HasError)
                {
                    SystemResult<string> tempRes = new SystemResult<string>
                    {
                        ErrorCode = checkKeyResult.ErrorCode,
                        ErrorMessage = checkKeyResult.ErrorMessage,
                        HasError = checkKeyResult.HasError,
                        ResultObject = ""
                    };

                    return tempRes;
                }

                CachingUrlShorteningServiceStaticData.NintexObjectCache.Add(key, userUrl);

                return new SystemResult<string>
                {
                    ResultObject = key
                };
            }
        }

        private SystemResult<bool> IsKeyExists(string path, string key, bool setErrorIfExists, out string value)
        {
            value = string.Empty;

            if (CachingUrlShorteningServiceStaticData.NintexObjectCache.TryGetValue(key, out value))
            {
                if (setErrorIfExists)
                    return new SystemResult<bool>
                    {
                        HasError = true,
                        ErrorCode = "1001",
                        ErrorMessage = "The custom alias you've chosen is not available. We've created a random one for you instead, but you can try assigning a different custom alias again below. Use 6 characters or more for the best chance of getting a unique unassigned alias.",
                        ResultObject = true
                    };
                else
                    return new SystemResult<bool>
                    {
                        ResultObject = true
                    };
            }

            return new SystemResult<bool>
            {
                ResultObject = false
            };
        }

        private string CheckForExistingFile(string staticStringPart)
        {
            string filePath = CreateLockKey(staticStringPart);

            return filePath;
        }

        private static string CreateLockKey(string name)
        {
            var folderPath = FolderName;

            var filePath = $"{folderPath}.{name}.{FileTypeExtentionName}";

            return filePath;
        }

        public override SystemResult<bool> Exists(string key)
        {
            var value = "";

            string path = GetPathFromKey(key);

            return IsKeyExists(path, key, false, out value);
        }

        private string GetPathFromKey(string key)
        {
            string staticStringPart = new String(key.TakeWhile(Char.IsLetter).ToArray());

            if (staticStringPart.IsNullOrEmpty())
                staticStringPart = UserAliasFolderName;

            var path = CheckForExistingFile(staticStringPart);

            return path;
        }

        public override SystemResult<string> Get(string key)
        {
            string path = GetPathFromKey(key);

            var result = string.Empty;

            if (!IsKeyExists(path, key, true, out result).ResultObject)
            {
                return new SystemResult<string>
                {
                    HasError = true,
                    ErrorCode = "1002",
                    ErrorMessage = "Your url is not exists.",
                    ResultObject = result
                };
            }

            return new SystemResult<string>
            {
                ResultObject = result
            };

        }

        protected override string GenerateStaticNumberPart()
        {
            return GenerateStaticNumberPartMethod(false);
        }

        private string GenerateStaticNumberPartMethod(bool resetValue = false)
        {
            var result = "";

            lock (CachingUrlShorteningServiceStaticData.SaveToFileLocker["GenerateStaticNumberPart"])
            {
                var path = CreateLockKey(StaticNumberFileName);

                if (resetValue || !CachingUrlShorteningServiceStaticData.TheLastNumber.HasValue)
                    CachingUrlShorteningServiceStaticData.TheLastNumber = 0;

                CachingUrlShorteningServiceStaticData.TheLastNumber++;

                result = CachingUrlShorteningServiceStaticData.TheLastNumber.Value.AddTextToLeftInt(CachingUrlShorteningServiceStaticData.StaticNumberLength); ;
            }

            return result;
        }

        protected override string GenerateStaticStringPart()
        {
            var result = "";

            lock (CachingUrlShorteningServiceStaticData.SaveToFileLocker["GenerateStaticStringPart"])
            {
                var path = CreateLockKey(StaticStringFileName);

                if (CachingUrlShorteningServiceStaticData.TheLastStringStaticName.IsNullOrEmpty())
                {
                    CachingUrlShorteningServiceStaticData.TheLastStringStaticName = CachingUrlShorteningServiceStaticData.FirstString;
                }

                if (CachingUrlShorteningServiceStaticData.TheLastNumber.HasValue)
                    if (CachingUrlShorteningServiceStaticData.TheLastNumber.Value >= CachingUrlShorteningServiceStaticData.MaxNumber)
                    {
                        var lastTail = CachingUrlShorteningServiceStaticData.TheLastStringStaticName.Substring(CachingUrlShorteningServiceStaticData.TheLastStringStaticName.Length - 1);

                        lastTail = GetNextChar(lastTail);

                        CachingUrlShorteningServiceStaticData.TheLastStringStaticName = CachingUrlShorteningServiceStaticData.TheLastStringStaticName.Substring(0, CachingUrlShorteningServiceStaticData.TheLastStringStaticName.Length - 1) + lastTail;

                        GenerateStaticNumberPartMethod(true);
                    }

                result = CachingUrlShorteningServiceStaticData.TheLastStringStaticName;
            }

            return result;
        }

        private static string GetNextChar(string lastTail)
        {
            if (lastTail == CachingUrlShorteningServiceStaticData.LastString)
            {
                lastTail += CachingUrlShorteningServiceStaticData.FirstString;
            }
            else
            {
                lastTail = ((char)(((int)lastTail.Last()) + 1)).ToString();
            }

            return lastTail;
        }

    }
}
