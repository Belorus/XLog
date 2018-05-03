namespace XLog.Categories
{
    public interface ICategoryFormatter
    {
        string[] GetAsStringArray(long categories);

        string GetString(long category);
    }
}