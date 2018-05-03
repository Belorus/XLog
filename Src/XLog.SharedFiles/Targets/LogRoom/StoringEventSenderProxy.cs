using System;
using System.Collections.Generic;

namespace XLog.NET.Targets.LogRoom
{
    public class StoringSenderProxy<T>
    {
        private readonly BufferingDispatcher<T>.ProcessAsyncDelegate _eventSender;
        private readonly IStorage<IList<T>> _storage;

        public StoringSenderProxy(
            BufferingDispatcher<T>.ProcessAsyncDelegate eventSender,
            IStorage<IList<T>> storage)
        {
            _eventSender = eventSender;
            _storage = storage;
        }

        public void SendEvents(IList<T> eventsToSend, Action<bool> callback)
        {
            if (_storage.HasData)
            {
                SendOldEvents();
            }

            _eventSender(eventsToSend, isSuccess =>
            {
                if (isSuccess)
                {
                    callback(true);
                }
                else
                {
                    _storage.Store(eventsToSend);
                    callback(false);
                }
            });
        }

        private void SendOldEvents()
        {
            Dictionary<string, IList<T>> unsentEvents = _storage.Load();
            foreach (var kv in unsentEvents)
            {
                _eventSender(kv.Value, isSuccess =>
                {
                    if (isSuccess)
                    {
                        _storage.Remove(kv.Key);
                    }
                });
            }
        }
    }
}