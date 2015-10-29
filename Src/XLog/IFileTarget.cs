namespace XLog
{
    public interface IFileTarget
    {
        byte[][] CollectLastLogs(int count);
    }
}