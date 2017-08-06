using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using XLog.Formatters;

namespace XLog.W10.Targets
{
    public class PlainStreamSocketTarget : Target
    {
        private readonly string _hostName;

        private readonly BlockingCollection<string> _lines = new BlockingCollection<string>();
        private readonly int _port;

        public PlainStreamSocketTarget(string hostName, int port, IFormatter formatter = null)
            : base(formatter)
        {
            _hostName = hostName;
            _port = port;

            Task.Run((Action) SendProc);
        }

        public override void Write(string content)
        {
            _lines.Add(content);
        }

        private async void SendProc()
        {
            while (true)
                try
                {
                    var client = new StreamSocket();
                    await client.ConnectAsync(new HostName(_hostName), _port.ToString());

                    var writer = new StreamWriter(client.OutputStream.AsStreamForWrite(), Encoding.ASCII);
                    writer.AutoFlush = true;

                    while (!_lines.IsCompleted)
                    {
                        var stringToSend = _lines.Take();

                        writer.Write(stringToSend);
                    }
                }
                catch
                {
                    Task.Delay(3000).Wait();
                }
        }
    }
}