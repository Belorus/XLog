using System.Collections.Generic;

namespace XLog.Categories
{
    public class LogCategoryRegistrar
    {
        private readonly Dictionary<long, string> _idToNameMap = new Dictionary<long, string>();

        public long Register(string name)
        {
            long id = 1 << _idToNameMap.Count;

            _idToNameMap[id] = name;

            return id;
        }

        public string Get(long id)
        {
            return _idToNameMap[id];
        }

        public ICollection<long> GetAll()
        {
            return _idToNameMap.Keys;
        }
    }
}