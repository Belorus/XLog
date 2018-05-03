using System;
using System.Collections.Generic;
using System.Linq;

namespace XLog.NET.Targets.LogRoom
{
    public class InMemoryStorage<T> : IStorage<T>
    {
        private readonly Dictionary<string, T> _map = new Dictionary<string, T>();

        public Dictionary<string, T> Load()
        {
            lock (_map)
            {
                return _map.ToDictionary(kv => kv.Key, kv => kv.Value);
            }
        }

        public void Remove(string key)
        {
            lock (_map)
            {
                _map.Remove(key);
            }
        }

        public void Store(T data)
        {
            lock (_map)
            {
                _map[Guid.NewGuid().ToString()] = data;
            }
        }

        public bool HasData
        {
            get
            {
                lock (_map)
                {
                    return _map.Count > 0;
                }
            }
        }
    }
}