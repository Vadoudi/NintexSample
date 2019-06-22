using System;
using System.Collections.Generic;
using System.Text;

namespace Nintex.Business.Service
{
    public interface IUrlShorteningService
    {
        /// <summary>
        /// This method create a new record in the system
        /// </summary>
        /// <param name="userUrl">this is the original url</param>
        /// <param name="userAlias">this is the user aliad url</param>
        /// <returns></returns>
        SystemResult<string> Add(string userUrl, string userAlias = "");

        /// <summary>
        /// This method checks key is exists or not
        /// </summary>
        /// <param name="key">This is the generated key</param>
        /// <returns></returns>
        SystemResult<bool> Exists(string key);

        /// <summary>
        /// This method return the urlthe system that have saved in 
        /// </summary>
        /// <param name="key">This is the generated key</param>
        /// <returns></returns>
        SystemResult<string> Get(string key);

    }
}
