using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nintex.Business
{
    /// <summary>
    /// This is an object to keep system url and user url 
    /// </summary>
    public class NintexObject
    {
        /// <summary>
        /// For dividing Key and OriginalUrl in file
        /// </summary>
        public const string DividerChar = "\t";

        public NintexObject(string row)
        {
            if (row.IsNullOrEmpty())
            {
                Key = "";

                OriginalUrl = "";
                return;
            }

            var temp = row.Split(DividerChar);

            Key = temp[0];

            OriginalUrl = temp[1];
        }

        public NintexObject(string key, string originalUrl)
        {
            Key = key ?? "";

            OriginalUrl = originalUrl ?? "";
        }

        /// <summary>
        /// key is a string that system generated it
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// This is the user url
        /// </summary>
        public string OriginalUrl { get; set; }

        /// <summary>
        /// This method convert content of NintexObject that save in a file to list of NintexObject
        /// </summary>
        /// <param name="stringValue"></param>
        /// <returns></returns>
        public static List<NintexObject> ConvertToNintexObject(string stringValue)
        {
            var result = new List<NintexObject>();

            var rows = stringValue.Split(Environment.NewLine);

            for (var i = 0; i < rows.Length; i++)
            {
                if (!rows[i].IsNullOrEmpty())
                    result.Add(new NintexObject(rows[i]));
            }

            return result;
        }

        /// <summary>
        /// This method return a value for saving to this object to a file.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Key}{DividerChar}{OriginalUrl}";
        }
    }
}
