namespace XLog.NET.Targets.LogRoom
{
    public interface IJsonFormatter
    {
        string Serialize(object data);
        T Deserialize<T>(string jsonString);
    }
}