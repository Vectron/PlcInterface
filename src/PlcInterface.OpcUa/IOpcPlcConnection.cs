using Opc.Ua.Client;

namespace PlcInterface.OpcUa;

/// <summary>
/// The Opc implementation of a <see cref="IPlcConnection"/>.
/// </summary>
public interface IOpcPlcConnection : IPlcConnection<ISession>
{
}