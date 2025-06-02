namespace ScreenReaderAccess.Observers
{
    // Generic observer interface for type-safe events
    public interface IEventObserver<TEvent>
    {
        void OnEvent(TEvent evt);
    }
}