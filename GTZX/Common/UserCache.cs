using System.Collections.Generic;

namespace Common
{
    public class UserCache
    {
        private readonly Dictionary<string, object> cacheDictionary = new Dictionary<string, object>();
        private readonly object lockObj = new object();

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>缓存对象</returns>
        public object this[string key]
        {
            get
            {
                lock (lockObj)
                {
                    return cacheDictionary.ContainsKey(key) ? cacheDictionary[key] : null;
                }
            }
            set
            {
                lock(lockObj)
                {
                    if (cacheDictionary.ContainsKey(key))
                    {
                        cacheDictionary[key] = value;
                    }
                    else
                    {
                        cacheDictionary.Add(key, value);
                    }
                }
            }
        }

        public void Remove(string key)
        {
            lock (lockObj)
            {
                if(cacheDictionary.ContainsKey(key))
                {
                    cacheDictionary.Remove(key);
                }
            }
        }

        public void Clear()
        {
            lock(lockObj)
            {
                cacheDictionary.Clear();
            }
        }
    }
}
