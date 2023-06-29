using System;
using PlcInterface.OpcUa;

namespace PlcInterface.Sandbox.PLCCommands;

/// <summary>
/// Command for writing values with OPC.
/// </summary>
internal sealed class OpcWriteCommand : PlcWriteCommand
{
    private readonly IOpcSymbolHandler symbolHandler;
    private readonly IOpcTypeConverter typeConverter;

    /// <summary>
    /// Initializes a new instance of the <see cref="OpcWriteCommand"/> class.
    /// </summary>
    /// <param name="readWrite">A <see cref="IOpcReadWrite"/> instance.</param>
    /// <param name="symbolHandler">A <see cref="IOpcSymbolHandler"/> instance.</param>
    /// <param name="typeConverter">A <see cref="IOpcTypeConverter"/> instance.</param>
    public OpcWriteCommand(IOpcReadWrite readWrite, IOpcSymbolHandler symbolHandler, IOpcTypeConverter typeConverter)
        : base("opc", readWrite)
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

        var boxedValue = symbolInfo.BuiltInType switch
        {
            Opc.Ua.BuiltInType.Boolean => (object)typeConverter.Convert<bool>(value),
            Opc.Ua.BuiltInType.SByte => typeConverter.Convert<sbyte>(value),
            Opc.Ua.BuiltInType.Byte => typeConverter.Convert<byte>(value),
            Opc.Ua.BuiltInType.Int16 => typeConverter.Convert<short>(value),
            Opc.Ua.BuiltInType.UInt16 => typeConverter.Convert<ushort>(value),
            Opc.Ua.BuiltInType.Integer or Opc.Ua.BuiltInType.Int32 => typeConverter.Convert<int>(value),
            Opc.Ua.BuiltInType.UInteger or Opc.Ua.BuiltInType.UInt32 => typeConverter.Convert<uint>(value),
            Opc.Ua.BuiltInType.Int64 => typeConverter.Convert<long>(value),
            Opc.Ua.BuiltInType.UInt64 => typeConverter.Convert<ulong>(value),
            Opc.Ua.BuiltInType.Float => typeConverter.Convert<float>(value),
            Opc.Ua.BuiltInType.Double => typeConverter.Convert<double>(value),
            Opc.Ua.BuiltInType.String => value,
            Opc.Ua.BuiltInType.DateTime => typeConverter.Convert<DateTime>(value),
            Opc.Ua.BuiltInType.Null => null,
            Opc.Ua.BuiltInType.Guid => null,
            Opc.Ua.BuiltInType.ByteString => null,
            Opc.Ua.BuiltInType.XmlElement => null,
            Opc.Ua.BuiltInType.NodeId => null,
            Opc.Ua.BuiltInType.ExpandedNodeId => null,
            Opc.Ua.BuiltInType.StatusCode => null,
            Opc.Ua.BuiltInType.QualifiedName => null,
            Opc.Ua.BuiltInType.LocalizedText => null,
            Opc.Ua.BuiltInType.ExtensionObject => null,
            Opc.Ua.BuiltInType.DataValue => null,
            Opc.Ua.BuiltInType.Variant => null,
            Opc.Ua.BuiltInType.DiagnosticInfo => null,
            Opc.Ua.BuiltInType.Number => null,
            Opc.Ua.BuiltInType.Enumeration => typeConverter.Convert<int>(value),
            _ => null,
        };

        return boxedValue
            ?? throw new InvalidOperationException("Unknown data type");
    }
}