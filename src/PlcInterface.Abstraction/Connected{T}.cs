namespace PlcInterface
{
    public class Connected<T> : IConnected<T>
    {
        public Connected(T value)
        {
            Value = value;
            IsConnected = true;
        }

        public Connected()
        {
        }

        public bool IsConnected
        {
            get;
        }

        public T Value
        {
            get;
        }
    }
}