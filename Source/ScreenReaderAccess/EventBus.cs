using System;
using System.Collections.Generic;
using System.Linq;
using ScreenReaderAccess.Observers;

namespace ScreenReaderAccess
{
    // Type-safe, generic EventBus
    public class EventBus
    {
        private readonly Dictionary<Type, List<object>> observers = new Dictionary<Type, List<object>>();

        public void RegisterObserver<TEvent>(IEventObserver<TEvent> observer)
        {
            var type = typeof(TEvent);
            if (!observers.TryGetValue(type, out var list))
            {
                list = new List<object>();
                observers[type] = list;
            }
            if (!list.Contains(observer))
            {
                list.Add(observer);
            }
        }

        public void UnregisterObserver<TEvent>(IEventObserver<TEvent> observer)
        {
            var type = typeof(TEvent);
            if (observers.TryGetValue(type, out var list))
            {
                list.Remove(observer);
            }
        }

        public void RaiseEvent<TEvent>(TEvent evt)
        {
            var type = typeof(TEvent);
            if (observers.TryGetValue(type, out var list))
            {
                foreach (var observer in list.Cast<IEventObserver<TEvent>>())
                {
                    observer.OnEvent(evt);
                }
            }
        }

        public void ClearObservers<TEvent>()
        {
            var type = typeof(TEvent);
            if (observers.ContainsKey(type))
            {
                observers.Remove(type);
            }
        }
    }
}
