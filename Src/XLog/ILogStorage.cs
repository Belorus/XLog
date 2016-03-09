namespace XLog
{
    public interface ILogStorage
    {
        byte[][] GetLastLogs(int count);
    }
}