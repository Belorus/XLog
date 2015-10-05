namespace XLog.NET.Targets
{
    public interface IFileTarget
    {
        byte[][] CollectLastLogs(int count);
    }
}