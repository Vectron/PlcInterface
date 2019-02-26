namespace PlcInterface.Ads
{
    internal class MonitorResult : IMonitorResult
    {
        public MonitorResult(string name, object value)
        {
            Name = name;
            Value = value;
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