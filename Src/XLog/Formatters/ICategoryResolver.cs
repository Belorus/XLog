namespace XLog.Formatters
{
    public interface ICategoryResolver
    {
        string GetString(long category);
    }
}