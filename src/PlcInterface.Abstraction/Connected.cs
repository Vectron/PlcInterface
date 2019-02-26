namespace PlcInterface
{
    public static class Connected
    {
        public static IConnected<T> No<T>()
        {
            return new Connected<T>();
        }

        public static IConnected<T> Yes<T>(T value)
        {
            return new Connected<T>(value);
        }
    }
}