using System;
using System.Net.Http;
using System.Net.Http.Headers;
using XLog.Formatters;

namespace XLog.NET.Targets
{
    public class HttpTarget : Target
    {
        private readonly string _uri;

        public HttpTarget(string uri, IFormatter formatter = null) : base(formatter)
        {
            _uri = uri;
        }

        public override async void Write(string content)
        {
            using (var client = new HttpClient())
            {
                var x = await client.PostAsync(_uri, new StringContent(content) { Headers = { ContentType = MediaTypeHeaderValue.Parse("text/plain") }});
            }
        }
    }
}
