using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Reactive.Subjects;
using Moq;
using TestUtilities;
using TwinCAT;
using TwinCAT.Ads;
using TwinCAT.TypeSystem;

namespace PlcInterface.Ads.Tests;

[TestClass]
public class SymbolHandlerTests
{
    [TestMethod]
    public void ExceptionWhenGeneratingSymbolsDowsNotDisposeStream()
    {
        // Arrange
        using var subject = new Subject<IConnected<IAdsConnection>>();
        var adsPlcConnection = new Mock<IAdsPlcConnection>();
        _ = adsPlcConnection.SetupGet(x => x.SessionStream).Returns(subject);
        var symbolHandlerSettings = new AdsSymbolHandlerOptions();
        var fileSystemMock = new Mock<IFileSystem>();
        var twinCATSymbolLoader = new Mock<ISymbolLoader>();
        var symbolLoaderFactory = new Mock<ISymbolLoaderFactory>();
        _ = symbolLoaderFactory.Setup(x => x.Create(It.IsAny<IConnection>(), It.IsAny<ISymbolLoaderSettings>())).Callback(() => throw new NotSupportedException()).Returns(twinCATSymbolLoader.Object);
        using var symbolHandler = new SymbolHandler(adsPlcConnection.Object, MockHelpers.GetOptionsMoq(symbolHandlerSettings), MockHelpers.GetLoggerMock<SymbolHandler>(), fileSystemMock.Object, symbolLoaderFactory.Object);
        var adsClient = new Mock<IAdsDisposableConnection>();
        var connected = new Mock<IConnected<IAdsConnection>>();
        _ = connected.SetupGet(x => x.IsConnected).Returns(value: true);
        _ = connected.SetupGet(x => x.Value).Returns(adsClient.Object);

        // Act
        subject.OnNext(connected.Object);

        // Assert
        Assert.IsTrue(subject.HasObservers);
        Assert.IsFalse(subject.IsDisposed);
    }

    [TestMethod]
    public void GetSymbolInfoReturnsSymbolWhenAvailable()
    {
        // Arrange
        var ioName = "DummyVar1";
        var symbols = new SymbolCollection()
            {
                CreateSymbolMock(ioName, []),
            };
        using var subject = new Subject<IConnected<IAdsConnection>>();
        var adsPlcConnection = new Mock<IAdsPlcConnection>();
        _ = adsPlcConnection.SetupGet(x => x.SessionStream).Returns(subject);
        var symbolHandlerSettings = new AdsSymbolHandlerOptions();
        var fileSystemMock = new Mock<IFileSystem>();
        var twinCATSymbolLoader = new Mock<ISymbolLoader>();
        _ = twinCATSymbolLoader.SetupGet(x => x.Symbols).Returns(symbols);
        var symbolLoaderFactory = new Mock<ISymbolLoaderFactory>();
        _ = symbolLoaderFactory.Setup(x => x.Create(It.IsAny<IConnection>(), It.IsAny<ISymbolLoaderSettings>())).Returns(twinCATSymbolLoader.Object);
        using var symbolHandler = new SymbolHandler(adsPlcConnection.Object, MockHelpers.GetOptionsMoq(symbolHandlerSettings), MockHelpers.GetLoggerMock<SymbolHandler>(), fileSystemMock.Object, symbolLoaderFactory.Object);
        var adsClient = new Mock<IAdsDisposableConnection>();
        var connected = new Mock<IConnected<IAdsConnection>>();
        _ = connected.SetupGet(x => x.IsConnected).Returns(value: true);
        _ = connected.SetupGet(x => x.Value).Returns(adsClient.Object);

        // Act
        subject.OnNext(connected.Object);
        var symbolInfo = symbolHandler.GetSymbolInfo(ioName);
        var symbolInfo2 = ((ISymbolHandler)symbolHandler).GetSymbolInfo(ioName);

        // Assert
        Assert.AreSame(symbolInfo, symbolInfo2);
        Assert.AreEqual(ioName, symbolInfo.Name);
    }

    [TestMethod]
    public void GetSymbolInfoThrowsSymbolExceptionWhenNoSymbolsAreLoaded()
    {
        // Arrange
        var ioName = "DummyVar1";
        var adsPlcConnection = new Mock<IAdsPlcConnection>();
        _ = adsPlcConnection.SetupGet(x => x.SessionStream).Returns(Mock.Of<IObservable<IConnected<IAdsConnection>>>());
        _ = adsPlcConnection.SetupGet(x => x.IsConnected).Returns(value: true);
        var symbolHandlerSettings = new AdsSymbolHandlerOptions();
        var fileSystemMock = new Mock<IFileSystem>();
        var symbolLoaderFactory = new Mock<ISymbolLoaderFactory>();
        using var symbolHandler = new SymbolHandler(adsPlcConnection.Object, MockHelpers.GetOptionsMoq(symbolHandlerSettings), MockHelpers.GetLoggerMock<SymbolHandler>(), fileSystemMock.Object, symbolLoaderFactory.Object);

        // Act

        // Assert
        var exception = Assert.ThrowsExactly<SymbolException>(() => symbolHandler.GetSymbolInfo(ioName));
        Assert.AreEqual(exception.Message, $"{ioName} Does not exist in the PLC");
    }

    [TestMethod]
    public void OnConnectSymbolListIsUpdated()
    {
        // Arrange
        var symbols = new SymbolCollection()
            {
                CreateSymbolMock("DummyVar1", []),
                CreateSymbolMock("DummyVar2", []),
                CreateSymbolMock("DummyVar3", ["DummyVar3.Sub1", "DummyVar3.Sub2", "DummyVar3.Sub3"]),
            };
        using var subject = new Subject<IConnected<IAdsConnection>>();
        var adsPlcConnection = new Mock<IAdsPlcConnection>();
        _ = adsPlcConnection.SetupGet(x => x.SessionStream).Returns(subject);
        var symbolHandlerSettings = new AdsSymbolHandlerOptions();
        var fileSystemMock = new Mock<IFileSystem>();
        var twinCATSymbolLoader = new Mock<ISymbolLoader>();
        _ = twinCATSymbolLoader.SetupGet(x => x.Symbols).Returns(symbols);
        var symbolLoaderFactory = new Mock<ISymbolLoaderFactory>();
        _ = symbolLoaderFactory.Setup(x => x.Create(It.IsAny<IConnection>(), It.IsAny<ISymbolLoaderSettings>())).Returns(twinCATSymbolLoader.Object);
        using var symbolHandler = new SymbolHandler(adsPlcConnection.Object, MockHelpers.GetOptionsMoq(symbolHandlerSettings), MockHelpers.GetLoggerMock<SymbolHandler>(), fileSystemMock.Object, symbolLoaderFactory.Object);
        var adsClient = new Mock<IAdsDisposableConnection>();
        var connected = new Mock<IConnected<IAdsConnection>>();
        _ = connected.SetupGet(x => x.IsConnected).Returns(value: true);
        _ = connected.SetupGet(x => x.Value).Returns(adsClient.Object);

        // Act
        subject.OnNext(connected.Object);

        // Assert
        Assert.AreEqual(6, symbolHandler.AllSymbols.Count);
    }

    [TestMethod]
    public void SymbolExceptionThrownWhenNoPlcConnected()
    {
        // Arrange
        var ioName = "DummyVar1";
        var adsPlcConnection = new Mock<IAdsPlcConnection>();
        _ = adsPlcConnection.SetupGet(x => x.SessionStream).Returns(Mock.Of<IObservable<IConnected<IAdsConnection>>>());
        _ = adsPlcConnection.SetupGet(x => x.IsConnected).Returns(value: false);
        var symbolHandlerSettings = new AdsSymbolHandlerOptions();
        var fileSystemMock = new Mock<IFileSystem>();
        var symbolLoaderFactory = new Mock<ISymbolLoaderFactory>();
        using var symbolHandler = new SymbolHandler(adsPlcConnection.Object, MockHelpers.GetOptionsMoq(symbolHandlerSettings), MockHelpers.GetLoggerMock<SymbolHandler>(), fileSystemMock.Object, symbolLoaderFactory.Object);

        // Act

        // Assert
        var exception = Assert.ThrowsExactly<SymbolException>(() => symbolHandler.GetSymbolInfo(ioName));
        Assert.AreEqual("PLC not connected", exception.Message);
    }

    [TestMethod]
    public void SymbolsAreStoredOnDisk()
    {
        // Arrange
        var ioTag = "DummyVar1";
        var symbols = new SymbolCollection()
            {
                CreateSymbolMock(ioTag, []),
            };
        using var subject = new Subject<IConnected<IAdsConnection>>();
        var adsPlcConnection = new Mock<IAdsPlcConnection>();
        _ = adsPlcConnection.SetupGet(x => x.SessionStream).Returns(subject);
        var symbolHandlerSettings = new AdsSymbolHandlerOptions()
        {
            StoreSymbolsToDisk = true,
            OutputPath = "DummyPath",
        };
        var fileSystemMock = new MockFileSystem();

        var twinCATSymbolLoader = new Mock<ISymbolLoader>();
        _ = twinCATSymbolLoader.SetupGet(x => x.Symbols).Returns(symbols);
        var symbolLoaderFactory = new Mock<ISymbolLoaderFactory>();
        _ = symbolLoaderFactory.Setup(x => x.Create(It.IsAny<IConnection>(), It.IsAny<ISymbolLoaderSettings>())).Returns(twinCATSymbolLoader.Object);
        using var symbolHandler = new SymbolHandler(adsPlcConnection.Object, MockHelpers.GetOptionsMoq(symbolHandlerSettings), MockHelpers.GetLoggerMock<SymbolHandler>(), fileSystemMock, symbolLoaderFactory.Object);
        var adsClient = new Mock<IAdsDisposableConnection>();
        var connected = new Mock<IConnected<IAdsConnection>>();
        _ = connected.SetupGet(x => x.IsConnected).Returns(value: true);
        _ = connected.SetupGet(x => x.Value).Returns(adsClient.Object);

        // Act
        subject.OnNext(connected.Object);

        // Assert
        Assert.AreEqual(1, fileSystemMock.AllFiles.Count());
        var fileDate = fileSystemMock.GetFile(fileSystemMock.AllFiles.First());
        StringAssert.Contains(fileDate.TextContents, ioTag, StringComparison.Ordinal);
    }

    [TestMethod]
    public void SymbolsAreStoredOnDiskToDefaultFolder()
    {
        // Arrange
        var ioTag = "DummyVar1";
        var symbols = new SymbolCollection()
            {
                CreateSymbolMock(ioTag, []),
            };
        using var subject = new Subject<IConnected<IAdsConnection>>();
        var adsPlcConnection = new Mock<IAdsPlcConnection>();
        _ = adsPlcConnection.SetupGet(x => x.SessionStream).Returns(subject);
        var symbolHandlerSettings = new AdsSymbolHandlerOptions()
        {
            StoreSymbolsToDisk = true,
            OutputPath = string.Empty,
        };
        var fileSystemMock = new MockFileSystem();

        var twinCATSymbolLoader = new Mock<ISymbolLoader>();
        _ = twinCATSymbolLoader.SetupGet(x => x.Symbols).Returns(symbols);
        var symbolLoaderFactory = new Mock<ISymbolLoaderFactory>();
        _ = symbolLoaderFactory.Setup(x => x.Create(It.IsAny<IConnection>(), It.IsAny<ISymbolLoaderSettings>())).Returns(twinCATSymbolLoader.Object);
        using var symbolHandler = new SymbolHandler(adsPlcConnection.Object, MockHelpers.GetOptionsMoq(symbolHandlerSettings), MockHelpers.GetLoggerMock<SymbolHandler>(), fileSystemMock, symbolLoaderFactory.Object);
        var adsClient = new Mock<IAdsDisposableConnection>();
        var connected = new Mock<IConnected<IAdsConnection>>();
        _ = connected.SetupGet(x => x.IsConnected).Returns(value: true);
        _ = connected.SetupGet(x => x.Value).Returns(adsClient.Object);

        // Act
        subject.OnNext(connected.Object);

        // Assert
        Assert.AreEqual(1, fileSystemMock.AllFiles.Count());
        Assert.IsNotNull(fileSystemMock.GetFile("logs"));
        var fileDate = fileSystemMock.GetFile(fileSystemMock.AllFiles.First());
        StringAssert.Contains(fileDate.TextContents, ioTag, StringComparison.Ordinal);
    }

    private static ISymbol CreateSymbolMock(string name, string[] subSymbols)
    {
        var symbol = new Mock<ISymbol>();
        _ = symbol.SetupGet(x => x.InstancePath).Returns(name);
        _ = subSymbols.Length == 0
            ? symbol.SetupGet(x => x.SubSymbols).Returns(SymbolCollection.Empty)
            : symbol.SetupGet(x => x.SubSymbols).Returns(new SymbolCollection(subSymbols.Select(x => CreateSymbolMock(x, []))));

        return symbol.Object;
    }
}
