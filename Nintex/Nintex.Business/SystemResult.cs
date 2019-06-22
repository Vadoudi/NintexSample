using System;
using System.Collections.Generic;
using System.Text;

namespace Nintex.Business
{
    public class BaseSystemResult
    {
        public bool HasError { get; set; }

        public string ErrorCode { get; set; }

        public string ErrorMessage { get; set; }

    }

    /// <summary>
    /// For return result and errors
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SystemResult<T> : BaseSystemResult
    {
        public T ResultObject { get; set; }
    }
}
