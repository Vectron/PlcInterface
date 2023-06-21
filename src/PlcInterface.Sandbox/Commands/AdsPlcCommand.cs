using PlcInterface.Ads;
using PlcInterface.Sandbox.Interactive;

namespace PlcInterface.Sandbox.Commands;

/// <summary>
/// A <see cref="IApplicationCommand"/> implementation for interacting with a ads PLC.
/// </summary>
internal sealed class AdsPlcCommand : PlcCommandBase
{
    private const string CommandName = "ads";
    private readonly IAdsReadWrite readWrite;
    private readonly IAdsSymbolHandler symbolHandler;
    private readonly IAdsTypeConverter typeConverter;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdsPlcCommand"/> class.
    /// </summary>
    /// <param name="plcConnection">A <see cref="IAdsPlcConnection"/>.</param>
    /// <param name="readWrite">A <see cref="IAdsReadWrite"/>.</param>
    /// <param name="symbolHandler">A <see cref="IAdsSymbolHandler"/>.</param>
    /// <param name="monitor">A <see cref="IAdsMonitor"/>.</param>
    /// <param name="typeConverter">A <see cref="IAdsTypeConverter"/>.</param>
    public AdsPlcCommand(IAdsPlcConnection plcConnection, IAdsReadWrite readWrite, IAdsSymbolHandler symbolHandler, IAdsMonitor monitor, IAdsTypeConverter typeConverter)
        : base(CommandName, plcConnection, readWrite, symbolHandler, monitor)
    {
        this.readWrite = readWrite;
        this.symbolHandler = symbolHandler;
        this.typeConverter = typeConverter;
    }

    /// <inheritdoc/>
    protected override Response ExecuteWrite(string symbolName, string value)
    {
        if (!symbolHandler.TryGetSymbolinfo(symbolName, out var symbolInfo))
        {
            return new Response("Symbol not found");
        }

        if (symbolInfo.IsArray)
        {
            return new Response("Arrays are not supported");
        }

        if (symbolInfo.IsBigType)
        {
            return new Response("object types are not supported");
        }

        if (symbolInfo.Symbol.DataType == null)
        {
            return new Response("Unable to read data type.");
        }

        if (!symbolInfo.Symbol.DataType.IsPrimitive)
        {
            return new Response("only primitive types are supported");
        }

        if (symbolInfo.Symbol.DataType is not TwinCAT.Ads.TypeSystem.DataType dataType
            || dataType.ManagedType == null)
        {
            return new Response("Unable to read data type.");
        }

        var boxedValue = typeConverter.Convert(value, dataType.ManagedType);
        if (boxedValue == null)
        {
            return new Response("Unknown data type");
        }

        readWrite.Write(symbolName, boxedValue);
        return new Response("Value written to PLC");
    }
}