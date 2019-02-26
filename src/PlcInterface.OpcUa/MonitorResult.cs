namespace PlcInterface.OpcUa
{
    internal class MonitorResult : IMonitorResult
    {
        public MonitorResult(string name, object value)
        {
        }

        public string Name
        {
            get;
        }

        public object Value
        {
            get;
        }
    }
}