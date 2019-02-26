namespace PlcInterface
{
    public interface IConnected<out T> : IConnected
    {
        T Value
        {
            get;
        }
    }

    public interface IConnected
    {
        bool IsConnected
        {
            get;
        }
    }
}