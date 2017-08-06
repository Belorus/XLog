using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XLog.Formatters;

namespace XLog.NET.Targets
{
    public class PlainTcpSocketTarget : Target
    {
        private readonly string _hostName;
        private readonly int _port;

        private readonly BlockingCollection<string> _lines = new BlockingCollection<string>();

        public PlainTcpSocketTarget(string hostName, int port, IFormatter formatter = null) 
            : base(formatter)
        {
            _hostName = hostName;
            _port = port;

            Task.Run((Action)SendProc);
        }

        public override void Write(string content)
        {
            _lines.Add(content);
        }

        private void SendProc()
        {
            while (true)
            {
                try
                {
                    var client = new TcpClient();
                    client.Connect(_hostName, _port);

                    var writer = new StreamWriter(client.GetStream(), Encoding.ASCII);
                    writer.AutoFlush = true;

                    while (!_lines.IsCompleted)
                    {
                        var stringToSend = _lines.Take();

                        writer.Write(stringToSend);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("PlainTcpSocketTarget write failure: " + ex.ToString());

                    Thread.Sleep(3000);
                }
            }
        }
    }
}
