using System;
using System.IO;
using System.Text;
using XLog.Formatters;

namespace XLog.NET.Targets
{
    public class SyncFileTarget : Target, ILogStorage
    {
        private readonly object _syncRoot = new object();
        private readonly StreamWriter _writer;

        private readonly string _logFileDirectory;
        private readonly string _logFilePath;

        public SyncFileTarget(string logFilePath)
            : this(null, logFilePath)
        {
        }

        public SyncFileTarget(IFormatter formatter, string logFilePath, bool autoFlush = false)
            : base(formatter)
        {
            _logFilePath = logFilePath;
            _logFileDirectory = Path.GetDirectoryName(logFilePath);

            if (!string.IsNullOrEmpty(_logFileDirectory))
                Directory.CreateDirectory(_logFileDirectory);

            if (File.Exists(logFilePath))
            {
                File.Delete(logFilePath);
            }

            try
            {
                var file = File.Open(logFilePath, FileMode.Create, FileAccess.Write, FileShare.Read);
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

        public string GetLastLogs()
        {
            lock (_syncRoot)
            {
                Flush();

                var file = new FileInfo(_logFilePath);

                int numOfRetries = 3;
                do
                {
                    try
                    {
                        return ReadFileContentsSafe(file);
                    }
                    catch (IOException)
                    {
                    }
                } while (--numOfRetries > 0);

                return string.Empty;
            }
        }

        private static string ReadFileContentsSafe(FileInfo f)
        {
            string copyName = f.FullName + ".copy";

            File.Copy(f.FullName, copyName);
            try
            {
                using (var fileStream = File.OpenText(copyName))
                {
                    return fileStream.ReadToEnd();
                }
            }
            finally
            {
                File.Delete(copyName);
            }
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
