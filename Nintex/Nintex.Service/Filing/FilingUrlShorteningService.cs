using Nintex.Business;
using Nintex.Business.Service;
using System;
using System.Collections.Generic;
//using System.IO;
using System.Linq;
using System.Text;

namespace Nintex.Service.Filing
{
    /// <summary>
    /// This class keep static data that FilingUrlShorteningService class needs
    /// </summary>
    static class FilingUrlShorteningServiceStaticData
    {
        public static Locker SaveToFileLocker = new Locker();

        /// <summary>
        /// Keep genereated or loaded NintexObject in this dictionary
        /// </summary>
        public static Dictionary<string, string> NintexObjectCache = new Dictionary<string, string>();

        /// <summary>
        /// Application add a path of file in this dictionary when it load the file
        /// </summary>
        public static Dictionary<string, string> LoadedFile = new Dictionary<string, string>();

        /// <summary>
        /// this is the last number that application uses it in GenerateStaticNumberPart method
        /// </summary>
        public static int? TheLastNumber = null;

        /// <summary>
        /// this is the current characters that used in the last generated key that application uses it in GenerateStaticStringPart method
        /// </summary>
        public static string TheLastStringStaticName = "";

        /// <summary>
        /// this is the MaxNumber that application uses it in GenerateStaticNumberPart method
        /// </summary>
        public const int MaxNumber = 8999;

        /// <summary>
        /// this is the lendth of number(add zero to the left of number) that application uses it in GenerateStaticNumberPart method
        /// </summary>
        public const int StaticNumberLength = 4;

        /// <summary>
        /// this is the first string that application uses it in GenerateStaticStringPart method
        /// </summary>
        public const string FirstString = "a";

        /// <summary>
        /// this is the last string that application uses it in GenerateStaticStringPart method
        /// </summary>
        public const string LastString = "z";
    }


    /// <summary>
    /// In this class application save all things in files
    /// 
    /// application uses multiple file and keep them in "data" folder 
    /// all generater keys and related urls keep in files that the name of them start with first letter of their keys(for example key am1245pAlo keeps in file am.ntfl)
    /// if user alias key start with letter they keep as the same of system generated key if not all of them keep in useralias.ntfl file( for example key 123aappll keeps in  useralias.ntfl file)
    /// </summary>
    public class FilingUrlShorteningService : BaseUrlShorteningService, IUrlShorteningService
    {
        const string FolderName = "data";

        const string FileTypeExtentionName = "ntfl";

        /// <summary>
        /// keep the last number in this file
        /// </summary>
        const string StaticNumberFileName = "staticnumber";

        /// <summary>
        /// keep the last static characters in this file
        /// </summary>
        const string StaticStringFileName = "staticstring";

        /// <summary>
        /// for keeping some user alias in this file
        /// </summary>
        const string UserAliasFolderName = "useralias";

        IUrlShorteningFileService _urlShorteningFileService;
        public FilingUrlShorteningService(IUrlShorteningFileService urlShorteningFileService)
        {
            _urlShorteningFileService = urlShorteningFileService;
        }

        /// <summary>
        /// save generated key in related file name
        /// application uses multiple file and keep them in "data" folder 
        /// all generater keys and related urls keep in files that the name of them start with first letter of their keys(for example key am1245pAlo keeps in file am.ntfl)
        /// if user alias key start with letter they keep as the same of system generated key if not all of them keep in useralias.ntfl file( for example key 123aappll keeps in  useralias.ntfl file)
        /// </summary>
        /// <param name="staticStringPart"></param>
        /// <param name="key">generated key or user alias</param>
        /// <param name="userUrl">this is user url </param>
        /// <param name="isUserAlias">shows the key is parameter user alias or not</param>
        /// <returns></returns>
        protected override SystemResult<string> Save(string staticStringPart, string key, string userUrl, bool isUserAlias = false)
        {
            var path = "";
            if (!isUserAlias)
                path = CheckForExistingFile(staticStringPart, true);
            else
                path = GetPathFromKey(key, true);

            lock (FilingUrlShorteningServiceStaticData.SaveToFileLocker[path])
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

                var result = SaveToFile(path, new NintexObject(key, userUrl));

                FilingUrlShorteningServiceStaticData.NintexObjectCache.Add(key, userUrl);

                return result;
            }
        }

        private SystemResult<bool> IsKeyExists(string path, string key, bool setErrorIfExists, out string value)
        {
            value = string.Empty;

            bool isFileExists = _urlShorteningFileService.FileExists(path);

            if (isFileExists)
                if (!FilingUrlShorteningServiceStaticData.LoadedFile.ContainsKey(path))
                {
                    var fileContent = _urlShorteningFileService.ReadAllText(path);

                    if (!fileContent.IsNullOrEmpty())
                    {
                        var data = NintexObject.ConvertToNintexObject(fileContent);

                        for (var i = 0; i < data.Count; i++)
                        {
                            FilingUrlShorteningServiceStaticData.NintexObjectCache.Add(data[i].Key, data[i].OriginalUrl);
                        }
                    }

                    FilingUrlShorteningServiceStaticData.LoadedFile.Add(path, "");
                }

            if (FilingUrlShorteningServiceStaticData.NintexObjectCache.TryGetValue(key, out value))
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

        private SystemResult<string> SaveToFile(string path, NintexObject nintexObject)
        {
            _urlShorteningFileService.AppendToFile(path, nintexObject.ToString());

            return new SystemResult<string>
            {
                ResultObject = nintexObject.Key
            };
        }

        private string CheckForExistingFile(string staticStringPart, bool createNewFileIfNotExists)
        {
            string filePath = CreateFile(staticStringPart, createNewFileIfNotExists);

            return filePath;
        }

        private string CreateFile(string fileName, bool createNewFileIfNotExists)
        {
            return _urlShorteningFileService.CreateFile(fileName, FileTypeExtentionName, FolderName, createNewFileIfNotExists);
        }

        public override SystemResult<bool> Exists(string key)
        {
            var value = "";

            string path = GetPathFromKey(key, false);

            return IsKeyExists(path, key, false, out value);
        }

        /// <summary>
        /// in this method get path from the key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="createNewFileIfNotExists"></param>
        /// <returns></returns>
        private string GetPathFromKey(string key, bool createNewFileIfNotExists)
        {
            //get firt characters that they are letter.
            string staticStringPart = new String(key.TakeWhile(Char.IsLetter).ToArray());

            //If staticStringPart be empty application use UserAliasFolderName parameter for staticStringPart(when staticStringPart be empty means key not exists or application didn't generate it on its rules)
            if (staticStringPart.IsNullOrEmpty())
                staticStringPart = UserAliasFolderName;

            var path = CheckForExistingFile(staticStringPart, createNewFileIfNotExists);

            return path;
        }

        public override SystemResult<string> Get(string key)
        {
            string path = GetPathFromKey(key, false);

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

        /// <summary>
        /// The same as GenerateStaticNumberPart but with reset  FilingUrlShorteningServiceStaticData.TheLastNumber parameter capability
        /// </summary>
        /// <param name="resetValue">let method to reset  FilingUrlShorteningServiceStaticData.TheLastNumber</param>
        /// <returns></returns>
        private string GenerateStaticNumberPartMethod(bool resetValue = false)
        {
            var result = "";
            //Lock the on key GenerateStaticNumberPart
            lock (FilingUrlShorteningServiceStaticData.SaveToFileLocker["GenerateStaticNumberPart"])
            {
                var path = CreateFile(StaticNumberFileName, true);

                //Check for reseting value or is first time to call GenerateStaticNumberPart method if not Add one unit to FilingUrlShorteningServiceStaticData.TheLastNumber
                if (resetValue)
                    FilingUrlShorteningServiceStaticData.TheLastNumber = 0;
                else if (!FilingUrlShorteningServiceStaticData.TheLastNumber.HasValue)
                {
                    var temp = _urlShorteningFileService.ReadAllText(path);

                    if (temp.IsNullOrEmpty())
                        FilingUrlShorteningServiceStaticData.TheLastNumber = 0;
                    else
                    {
                        int tempInt = 0;
                        int.TryParse(temp, out tempInt);
                        FilingUrlShorteningServiceStaticData.TheLastNumber = tempInt;
                    }
                }

                FilingUrlShorteningServiceStaticData.TheLastNumber++;

                //save FilingUrlShorteningServiceStaticData.TheLastNumber in staticnumber.ntfl
                result = SaveNumberToFile(path);
            }

            return result;
        }

        /// <summary>
        /// save FilingUrlShorteningServiceStaticData.TheLastNumber in staticnumber.ntfl
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string SaveNumberToFile(string path)
        {
            string result = FilingUrlShorteningServiceStaticData.TheLastNumber.Value.AddTextToLeftInt(FilingUrlShorteningServiceStaticData.StaticNumberLength);
            _urlShorteningFileService.WriteAllText(path, result);
            return result;
        }

        protected override string GenerateStaticStringPart()
        {
            var result = "";

            //Lock the on key GenerateStaticStringPart
            lock (FilingUrlShorteningServiceStaticData.SaveToFileLocker["GenerateStaticStringPart"])
            {
                var path = CreateFile(StaticStringFileName, true);

                //Check is the first time of application to GenerateStaticStringPart
                if (FilingUrlShorteningServiceStaticData.TheLastStringStaticName.IsNullOrEmpty())
                {
                    //try to load saved TheLastStringStaticName parameter from staticstring.ntfl file
                    var temp = _urlShorteningFileService.ReadAllText(path);
                    
                    //means this is first time of application start
                    if (temp.IsNullOrEmpty())
                        FilingUrlShorteningServiceStaticData.TheLastStringStaticName = FilingUrlShorteningServiceStaticData.FirstString;
                    else
                    {
                        //put value that save in file in TheLastStringStaticName parameter
                        FilingUrlShorteningServiceStaticData.TheLastStringStaticName = temp;
                    }
                }

                //Check is TheLastNumber bigger than MaxNumber or not
                //if yes reset the last number and change TheLastStringStaticName
                if (FilingUrlShorteningServiceStaticData.TheLastNumber.HasValue)
                    if (FilingUrlShorteningServiceStaticData.TheLastNumber.Value >= FilingUrlShorteningServiceStaticData.MaxNumber)
                    {
                        var lastTail = FilingUrlShorteningServiceStaticData.TheLastStringStaticName.Substring(FilingUrlShorteningServiceStaticData.TheLastStringStaticName.Length - 1);

                        lastTail = GetNextChar(lastTail);

                        FilingUrlShorteningServiceStaticData.TheLastStringStaticName = FilingUrlShorteningServiceStaticData.TheLastStringStaticName.Substring(0, FilingUrlShorteningServiceStaticData.TheLastStringStaticName.Length - 1) + lastTail;

                        GenerateStaticNumberPartMethod(true);
                    }

                result = FilingUrlShorteningServiceStaticData.TheLastStringStaticName;

                //save FilingUrlShorteningServiceStaticData.TheLastStringStaticName to file staticstring.ntfl
                _urlShorteningFileService.WriteAllText(path, result);
            }

            return result;
        }

        /// <summary>
        /// get next char for TheLastStringStaticName parameter
        /// </summary>
        /// <param name="lastTail"></param>
        /// <returns></returns>
        private static string GetNextChar(string lastTail)
        {
            //check if the last character of TheLastStringStaticName is equal LastString add first string to the last string(if last tail is z return za)
            //if not add one unit to asscii code of last tail(if last tail is a(65) it return b(66))
            if (lastTail == FilingUrlShorteningServiceStaticData.LastString)
            {
                lastTail += FilingUrlShorteningServiceStaticData.FirstString;
            }
            else
            {
                lastTail = ((char)(((int)lastTail.Last()) + 1)).ToString();
            }

            return lastTail;
        }

    }
}
