using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nintex.Business
{
    /// <summary>
    /// This for locking object with a string key
    /// </summary>
    public class Locker
    {
        private Dictionary<string, object> locks;

        private object myLock;

        public Locker()
        {
            locks = new Dictionary<string, object>();

            myLock = new object();
        }

        public object this[string index]
        {
            get
            {
                lock(myLock)
                {
                    object result;

                    if (locks.TryGetValue(index, out result))
                        return result;

                    result = new object();

                    locks[index] = result;

                    return result;
                }
            }
        }
    }
}
