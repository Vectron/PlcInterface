using TwinCAT.Ads;

namespace PlcInterface.Ads
{
    /// <summary>
    /// The Ads implementation of a <see cref="IPlcConnection"/>.
    /// </summary>
    public interface IAdsPlcConnection : IPlcConnection<IAdsConnection>
    {
    }
}