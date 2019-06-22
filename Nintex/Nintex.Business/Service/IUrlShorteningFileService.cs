using System;
using System.Collections.Generic;
using System.Text;

namespace Nintex.Business.Service
{
    public interface IUrlShorteningFileService
    {
        /// <summary>
        /// Check is file exists
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        bool FileExists(string path);

        /// <summary>
        /// appends the content to file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="content"></param>
        void AppendToFile(string path, string content);

        /// <summary>
        /// This method read all file as text
        /// </summary>
        /// <param name="path"></param>
        /// <returns>returns content of file</returns>
        string ReadAllText(string path);

        /// <summary>
        /// This method write content in a file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="content"></param>
        void WriteAllText(string path, string content);

        /// <summary>
        /// This method checks existing of folder and file and if it is'nt exists, method create it.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileTypeExtentionName">this is the extention of file.</param>
        /// <param name="folderName"></param>
        /// <param name="createNewFileIfNotExists">if true: this methode create file, if false: system do nothing.</param>
        /// <returns>return path of the file</returns>
        string CreateFile(string fileName, string fileTypeExtentionName, string folderName, bool createNewFileIfNotExists);

        /// <summary>
        /// Check is folder exists
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        bool DirectoryExists(string folderPath);

        /// <summary>
        /// Create the folder 
        /// </summary>
        /// <param name="folderPath"></param>
        void CreateDirectory(string folderPath);

        /// <summary>
        /// Just create a file with no content
        /// </summary>
        /// <param name="path"></param>
        void CreateEmptyFile(string path);

        /// <summary>
        /// This method concatenate two part of string with path seperator "\"
        /// </summary>
        /// <param name="firstPart"></param>
        /// <param name="secondPart"></param>
        /// <returns></returns>
        string PathJoin(string firstPart, string secondPart);
    }
}
