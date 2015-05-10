using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace XLog.ConsoleApp
{
    public class FileTarget : Target
    {
        private readonly AsyncSemaphore _semaphore;
        private readonly FileStream _file;

        public FileTarget(string path, string fileNamePrefix)
            : this(null, path, fileNamePrefix)
        {
        }

        public FileTarget(IFormatter formatter, string path, string fileNamePrefix)
            : base(formatter)
        {
            Path = path;
            FileNamePrefix = fileNamePrefix;

            Directory.CreateDirectory(Path);

#if DEV_BUILD
            var fileName = System.IO.Path.Combine(Path, FileNamePrefix) + DateTime.Now.ToString("s").Replace(":", "_") + ".log";
#else
            var fileName = System.IO.Path.Combine(Path, FileNamePrefix) + ".log";
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
#endif

            _file = File.Open(fileName, FileMode.Append, FileAccess.Write, FileShare.Read);
            _semaphore = new AsyncSemaphore(1);
        }

        public string Path { get; private set; }
        public string FileNamePrefix;


        public override async void Write(string content)
        {
            await _semaphore.WaitAsync();

            try
            {
                for (int i = 0; i < 3; i++)
                {
                    if (i > 0)
                    {
                        await Task.Delay(20);
                    }

                    try
                    {
                        using (var writer = new StreamWriter(_file, Encoding.UTF8, 4096, true))
                        {
                            if (content.Length > 5000)
                            {
                                content = ">>>>>> " + content.Replace(Environment.NewLine, string.Empty);
                            }

                            writer.WriteLine(content);
                            writer.Flush();
                        }

                        break;
                    }
                    catch (IOException)
                    {
                    }
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public byte[][] CollectLastLogs(int count)
        {
            FileInfo[] logFiles = Directory.GetFiles(Path).Select(f => new FileInfo(f)).OrderByDescending(x => x.CreationTime).Take(count).ToArray();
            byte[][] logsContent = null;
            do
            {
                try
                {
                    logsContent = logFiles.Select(ReadFileContentsSafe).ToArray();
                }
                catch (IOException)
                {

                }
            } while (logsContent == null);

            return logsContent;
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
    }
}
