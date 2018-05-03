using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using XLog.Categories;
using XLog.Formatters;

namespace XLog.NET.Targets.LogRoom
{
    public class LogRoomTarget : Target
    {
        private readonly IJsonFormatter _jsonFormatter;
        private readonly ICategoryFormatter _categoryFormatter;
        private readonly BufferingDispatcher<Entry> _dispatcher;
        private readonly string _uri;
        private StoringSenderProxy<Entry> _storingProxy;

        public LogRoomTarget(
            IJsonFormatter jsonFormatter,
            ICategoryFormatter categoryFormatter,
            string uri)
        {
            _jsonFormatter = jsonFormatter;
            _categoryFormatter = categoryFormatter;
            _storingProxy = new StoringSenderProxy<Entry>(SendAsync, new InMemoryStorage<IList<Entry>>());
            _dispatcher = new BufferingDispatcher<Entry>(_storingProxy.SendEvents)
            {
                FlushDelay = TimeSpan.FromSeconds(5)
            };
            _uri = uri;
        }

        private async void SendAsync(IList<Entry> items, Action<bool> callback)
        {
            using (var httpClient = new HttpClient())
            {
                var dto = new LogRoomMessageDto
                {
                    logs = items.Select(entry => new LogRoomEntryDto
                    {
                        timestamp = (long) (entry.TimeStamp.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds,
                        categories = _categoryFormatter.GetAsStringArray(entry.Category),
                        level = LogLevels.Levels[(int) entry.Level],
                        message = entry.Message,
                        tag = entry.Tag,
                        thread = LogEnvironment.CurrentManagedThreadId
                    }).ToArray()
                };

                string jsonString = _jsonFormatter.Serialize(dto);

                try
                {
                    await httpClient.PostAsync(_uri, new StringContent(jsonString));
                    callback(true);
                }
                catch
                {
                    callback(false);
                }
            }
        }

        public override void Write(Entry entry, IFormatter formatter)
        {
            _dispatcher.Dispatch(entry);
        }

        public override void Write(string content)
        {
            throw new NotImplementedException();
        }
    }
}
