namespace TestUtilities;

public static class MockDelegates
{
    public delegate void OutAction<TOut>(out TOut outVal);

    public delegate void OutAction<T1, TOut>(T1 arg1, out TOut outVal);

    public delegate void OutAction<T1, T2, TOut>(T1 arg1, T2 agr2, out TOut outVal);

    public delegate TReturn OutFunction<TOut, TReturn>(out TOut outVal);

    public delegate TReturn OutFunction<T1, TOut, TReturn>(T1 arg1, out TOut outVal);

    public delegate TReturn OutFunction<T1, T2, TOut, TReturn>(T1 arg1, T2 agr2, out TOut outVal);
}