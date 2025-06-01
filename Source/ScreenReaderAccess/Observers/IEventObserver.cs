namespace ScreenReaderAccess.Observers
{
    public interface IEventObserver
    {
        void OnEvent(string eventName, object eventArgs);
    }
}