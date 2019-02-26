namespace PlcInterface
{
    public interface IMonitorResult
    {
        string Name
        {
            get;
        }

        object Value
        {
            get;
        }
    }
}