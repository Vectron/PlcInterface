using System;
using PlcInterface.Ads;

namespace PlcInterface.Sandbox.PLCCommands;

/// <summary>
/// Command for writing values with ADS.
/// </summary>
internal sealed class AdsWriteCommand : PlcWriteCommand
{
    private readonly IAdsSymbolHandler symbolHandler;
    private readonly IAdsTypeConverter typeConverter;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdsWriteCommand"/> class.
    /// </summary>
    /// <param name="readWrite">A <see cref="IAdsReadWrite"/> instance.</param>
    /// <param name="symbolHandler">A <see cref="IAdsSymbolHandler"/> instance.</param>
    /// <param name="typeConverter">A <see cref="IAdsTypeConverter"/> instance.</param>
    public AdsWriteCommand(IAdsReadWrite readWrite, IAdsSymbolHandler symbolHandler, IAdsTypeConverter typeConverter)
        : base("ads", readWrite)
    {
        this.symbolHandler = symbolHandler;
        this.typeConverter = typeConverter;
    }

    /// <inheritdoc/>
    protected override object ConvertToValidInputValue(string symbolName, string value)
    {
        if (!symbolHandler.TryGetSymbolInfo(symbolName, out var symbolInfo))
        {
            throw new InvalidOperationException("Symbol not found");
        }

        if (symbolInfo.IsArray)
        {
            throw new InvalidOperationException("Arrays are not supported");
        }

        if (symbolInfo.IsBigType)
        {
            throw new InvalidOperationException("object types are not supported");
        }

        if (symbolInfo.Symbol.DataType == null)
        {
            throw new InvalidOperationException("Unable to read data type.");
        }

        if (!symbolInfo.Symbol.DataType.IsPrimitive)
        {
            throw new InvalidOperationException("only primitive types are supported");
        }

        if (symbolInfo.Symbol.DataType is not TwinCAT.Ads.TypeSystem.DataType dataType
            || dataType.ManagedType == null)
        {
            throw new InvalidOperationException("Unable to read data type.");
        }

        var boxedValue = typeConverter.Convert(value, dataType.ManagedType)
            ?? throw new InvalidOperationException("Unknown data type");

        return boxedValue;
    }
}