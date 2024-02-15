using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TwinCAT.TypeSystem;

namespace PlcInterface.Ads.Tests;

[TestClass]
public class ISymbolInfoExtensionTests
{
    [TestMethod]
    public void CastAndValidateReturnsSymbolInfo()
    {
        // Arrange
        ISymbolInfo symbolInfo = new SymbolInfo(Mock.Of<ISymbol>(), string.Empty);

        // Act
        var actual = symbolInfo.CastAndValidate();

        // Assert
        Assert.IsInstanceOfType(actual, typeof(IAdsSymbolInfo));
        Assert.IsInstanceOfType(actual, typeof(ISymbolInfo));
        Assert.AreSame(symbolInfo, actual);
    }

    [TestMethod]
    public void CastAndValidateThrowsSymbolExceptionWhenNotAdsSymbol()
    {
        // Arrange
        var symbolMock = Mock.Of<ISymbolInfo>();

        // Act
        // Assert
        _ = Assert.ThrowsException<SymbolException>(symbolMock.CastAndValidate);
    }

    private sealed class DummySymbol : ISymbol
    {
        public ITypeAttributeCollection Attributes => null!;

        public int BitSize => 0;

        public int ByteSize => 0;

        public DataTypeCategory Category
        {
            get;
        }

        public string Comment => string.Empty;

        public IDataType? DataType
        {
            get;
        }

        public string InstanceName => "DummyVar1";

        public string InstancePath => "DummyVar1";

        public bool IsBitType => true;

        public bool IsByteAligned
        {
            get;
        }

        public bool IsContainerType
        {
            get;
        }

        public bool IsPersistent
        {
            get;
        }

        public bool IsPointer
        {
            get;
        }

        public bool IsPrimitiveType
        {
            get;
        }

        public bool IsReadOnly
        {
            get;
        }

        public bool IsRecursive
        {
            get;
        }

        public bool IsReference
        {
            get;
        }

        public bool IsStatic
        {
            get;
        }

        public ISymbol? Parent
        {
            get;
        }

        public int Size
        {
            get;
        }

        public ISymbolCollection<ISymbol> SubSymbols => new SymbolCollection();

        public string TypeName => "Boolean";

        public Encoding ValueEncoding => null!;
    }
}
