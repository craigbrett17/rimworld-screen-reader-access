using ScreenReaderAccess.Observers;
using System.Collections.Generic;

namespace ScreenReaderAccess
{
    // EventBus: manages observer registration and event dispatching
    public class EventBus
    {
        private readonly List<IEventObserver> observers = new List<IEventObserver>();

        public void RegisterObserver(IEventObserver observer)
        {
            if (!observers.Contains(observer))
                observers.Add(observer);
        }

        public void UnregisterObserver(IEventObserver observer)
        {
            observers.Remove(observer);
        }

        public void RaiseEvent(string eventName, object eventArgs)
        {
            foreach (var observer in observers)
            {
                observer.OnEvent(eventName, eventArgs);
            }
        }
    }
}