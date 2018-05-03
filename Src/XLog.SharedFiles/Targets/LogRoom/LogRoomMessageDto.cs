namespace XLog.NET.Targets.LogRoom
{
    public class LogRoomMessageDto
    {
        public LogRoomEntryDto[] logs;
    }

    public class LogRoomEntryDto
    {
        public long timestamp;
        public int thread;

        public string level;
        public string tag;
        public string message;
        public string[] categories;
    }
}