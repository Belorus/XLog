using System;
using System.IO;
using System.Linq;
using System.Text;
using XLog.Formatters;

namespace XLog.NET.Targets
{
    public class SyncFileTarget : Target, ILogStorage
    {
        private readonly object _syncRoot = new object();
        private readonly StreamWriter _writer;

        private readonly string _logFilePath;
        private readonly string _logFileDirectory;

        public SyncFileTarget(string logFilePath)
            : this(null, logFilePath)
        {
        }

        public SyncFileTarget(IFormatter formatter, string logFilePath, bool autoFlush = false)
            : base(formatter)
        {
            _logFileDirectory = Path.GetDirectoryName(logFilePath);
            _logFilePath = logFilePath;

            if (!string.IsNullOrEmpty(_logFileDirectory))
                Directory.CreateDirectory(_logFileDirectory);

            if (File.Exists(_logFilePath))
            {
                File.Delete(_logFilePath);
            }

            try
            {
                var file = File.Open(_logFilePath, FileMode.Create, FileAccess.Write, FileShare.Read);
                _writer = new StreamWriter(file, Encoding.UTF8);
                _writer.AutoFlush = autoFlush;
            }
            catch (IOException)
            {
                _writer = StreamWriter.Null;
            }
        }

        public override void Write(string content)
        {
            lock (_syncRoot)
            {
                _writer.Write(content);
            }
        }

        public byte[][] GetLastLogs(int count)
        {
            lock (_syncRoot)
            {
                Flush();

                FileInfo[] logFiles = Directory.GetFiles(_logFileDirectory)
                                               .Select(f => new FileInfo(f))
                                               .OrderByDescending(x => x.CreationTime)
                                               .Take(count)
                                               .ToArray();

                byte[][] logsContent = new byte[logFiles.Length][];

                for (int i = 0; i < logFiles.Length; i++)
                {
                    int numOfRetries = 3;
                    do
                    {
                        try
                        {
                            logsContent[i] = ReadFileContentsSafe(logFiles[i]);
                        }
                        catch (IOException)
                        {
                        }
                    } while (logsContent[i] == null && --numOfRetries > 0);
                }

                return logsContent;
            }
        }

        private static byte[] ReadFileContentsSafe(FileInfo f)
        {
            string copyName = f.FullName + ".copy";
            byte[] bytes = new byte[f.Length];
            File.Copy(f.FullName, copyName);
            using (var stream = File.OpenRead(copyName))
            {
                stream.Read(bytes, 0, bytes.Length);
            }

            File.Delete(copyName);

            return bytes;
        }

        public override void Flush()
        {
            lock (_syncRoot)
            {
                try
                {
                    _writer.Flush();
                }
                catch (IOException)
                {
                    // If log file cannot be flushed - we shouldn't crash.
                    // Supressing finalization to avoid crash in finalizer
                    GC.SuppressFinalize(_writer);
                    GC.SuppressFinalize(_writer.BaseStream);
                }
            }
        }
    }
}
