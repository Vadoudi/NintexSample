using Nintex.Business.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nintex.Service.Caching
{
    /// <summary>
    /// In this class application don't save anything in file and all value keep in memory   
    /// </summary>
    public class CachingUrlShorteningFileService : IUrlShorteningFileService
    {
        public void AppendToFile(string path, string content)
        {

        }

        public void CreateDirectory(string folderPath)
        {
        }

        public void CreateEmptyFile(string path)
        {
        }

        public string CreateFile(string fileName, string fileTypeExtentionName, string folderName, bool createNewFileIfNotExists)
        {
            var folderPath = folderName;

            var filePath = $"{folderPath}.{fileName}.{fileTypeExtentionName}";

            return filePath;
        }

        public bool DirectoryExists(string folderPath)
        {
            return true;
        }

        public bool FileExists(string path)
        {
            return false;
        }

        public string PathJoin(string firstPart, string secondPart)
        {
            return "";
        }

        public string ReadAllText(string path)
        {
            return string.Empty;
        }

        public void WriteAllText(string path, string content)
        {
        }
    }
}
