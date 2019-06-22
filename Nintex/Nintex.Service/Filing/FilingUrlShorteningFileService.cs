using Nintex.Business.Service;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Nintex.Service.Filing
{
    public class FilingUrlShorteningFileService : IUrlShorteningFileService
    {
        public void AppendToFile(string path, string content)
        {
            using (var stream = new StreamWriter(path, append: true))
            {
                stream.WriteLine(content);
            }
        }

        public void CreateDirectory(string folderPath)
        {
            Directory.CreateDirectory(folderPath);
        }

        public void CreateEmptyFile(string path)
        {
            using (var stream = new StreamWriter(path)) { }
        }

        public string CreateFile(string fileName, string fileTypeExtentionName, string folderName, bool createNewFileIfNotExists)
        {
            var folderPath = PathJoin(AppDomain.CurrentDomain.BaseDirectory, folderName);

            if (!DirectoryExists(folderPath))
                CreateDirectory(folderPath);

            var filePath = PathJoin(folderPath, $"{fileName}.{fileTypeExtentionName}");

            if (createNewFileIfNotExists && !FileExists(filePath))
                CreateEmptyFile(filePath);

            return filePath;
        }

        public bool DirectoryExists(string folderPath)
        {
            return Directory.Exists(folderPath);
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public string PathJoin(string firstPart, string secondPart)
        {
            return Path.Join(firstPart, secondPart);
        }

        public string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }

        public void WriteAllText(string path, string content)
        {
            File.WriteAllText(path, content);
        }
    }
}
