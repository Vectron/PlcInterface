using PlcInterface.OpcUa;

namespace PlcInterface.Sandbox.PLCCommands;

/// <summary>
/// Command for writing values with OPC.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="OpcWriteCommand"/> class.
/// </remarks>
/// <param name="readWrite">A <see cref="IOpcReadWrite"/> instance.</param>
/// <param name="symbolHandler">A <see cref="IOpcSymbolHandler"/> instance.</param>
/// <param name="typeConverter">A <see cref="IOpcTypeConverter"/> instance.</param>
internal sealed class OpcWriteCommand(IOpcReadWrite readWrite, IOpcSymbolHandler symbolHandler, IOpcTypeConverter typeConverter) : PlcWriteCommand("opc", readWrite)
{
    /// <inheritdoc/>
    protected override object ConvertToValidInputValue(string symbolName, string value)
    {
        if (!symbolHandler.TryGetSymbolInfo(symbolName, out var symbolInfo))
        {
            throw new InvalidOperationException("Symbol not found");
        }

        if (symbolInfo.IsArray)
        {
            if (!value.StartsWith('[') || !value.EndsWith(']'))
            {
                throw new InvalidOperationException("Arrays must have the following syntax: ['values']");
            }

            return value[1..^1]
                .Split(',')
                .Select(x => ConvertToManagedType(symbolInfo.BuiltInType, x))
                .ToArray();
        }

        if (symbolInfo.IsBigType)
        {
            throw new InvalidOperationException("object types are not supported");
        }

        return ConvertToManagedType(symbolInfo.BuiltInType, value)
            ?? throw new InvalidOperationException("Unknown data type");
    }

    private object? ConvertToManagedType(Opc.Ua.BuiltInType builtInType, object value)
        => builtInType switch
        {
            Opc.Ua.BuiltInType.Boolean => typeConverter.Convert<bool>(value),
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
            Opc.Ua.BuiltInType.Enumeration => null,
            _ => null,
        };
}
