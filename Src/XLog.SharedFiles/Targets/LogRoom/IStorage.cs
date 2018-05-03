using System.Collections.Generic;

namespace XLog.NET.Targets.LogRoom
{
    public interface IStorage<T>
    {
        Dictionary<string, T> Load();
        void Remove(string key);
        void Store(T data);

        bool HasData { get; }
    }
}