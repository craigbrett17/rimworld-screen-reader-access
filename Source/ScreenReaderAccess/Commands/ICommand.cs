namespace ScreenReaderAccess.Commands
{
    public interface ICommand<TArgs>
    {
        void Execute(TArgs args);
    }
}
