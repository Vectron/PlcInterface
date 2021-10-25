using System;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Reactive.Subjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        var symbolHandlerSettings = new SymbolHandlerSettings();
        var fileSystemMock = new Mock<IFileSystem>();
        var twincatSymbolLoader = new Mock<ISymbolLoader>();
        var symbolLoaderFactory = new Mock<ISymbolLoaderFactory>();
        _ = symbolLoaderFactory.Setup(x => x.Create(It.IsAny<IConnection>(), It.IsAny<ISymbolLoaderSettings>())).Callback(() => throw new NotSupportedException()).Returns(twincatSymbolLoader.Object);
        using var symbolHandler = new SymbolHandler(adsPlcConnection.Object, MockHelpers.GetOptionsMoq(symbolHandlerSettings), MockHelpers.GetLoggerMock<SymbolHandler>(), fileSystemMock.Object, symbolLoaderFactory.Object);
        var adsClient = new Mock<IAdsDisposableConnection>();
        var connected = new Mock<IConnected<IAdsConnection>>();
        _ = connected.SetupGet(x => x.IsConnected).Returns(true);
        _ = connected.SetupGet(x => x.Value).Returns(adsClient.Object);

        // Act
        subject.OnNext(connected.Object);

        // Assert
        Assert.IsTrue(subject.HasObservers);
        Assert.IsFalse(subject.IsDisposed);
    }

    [TestMethod]
    public void GetSymbolInfoReturnsSymbolWhenAvailible()
    {
        // Arrange
        var ioName = "DummyVar1";
        var symbols = new SymbolCollection()
            {
                CreateSymbolMock(ioName, Array.Empty<string>()),
            };
        using var subject = new Subject<IConnected<IAdsConnection>>();
        var adsPlcConnection = new Mock<IAdsPlcConnection>();
        _ = adsPlcConnection.SetupGet(x => x.SessionStream).Returns(subject);
        var symbolHandlerSettings = new SymbolHandlerSettings();
        var fileSystemMock = new Mock<IFileSystem>();
        var twincatSymbolLoader = new Mock<ISymbolLoader>();
        _ = twincatSymbolLoader.SetupGet(x => x.Symbols).Returns(symbols);
        var symbolLoaderFactory = new Mock<ISymbolLoaderFactory>();
        _ = symbolLoaderFactory.Setup(x => x.Create(It.IsAny<IConnection>(), It.IsAny<ISymbolLoaderSettings>())).Returns(twincatSymbolLoader.Object);
        using var symbolHandler = new SymbolHandler(adsPlcConnection.Object, MockHelpers.GetOptionsMoq(symbolHandlerSettings), MockHelpers.GetLoggerMock<SymbolHandler>(), fileSystemMock.Object, symbolLoaderFactory.Object);
        var adsClient = new Mock<IAdsDisposableConnection>();
        var connected = new Mock<IConnected<IAdsConnection>>();
        _ = connected.SetupGet(x => x.IsConnected).Returns(true);
        _ = connected.SetupGet(x => x.Value).Returns(adsClient.Object);

        // Act
        subject.OnNext(connected.Object);
        var symbolInfo = symbolHandler.GetSymbolinfo(ioName);
        var symbolInfo2 = ((ISymbolHandler)symbolHandler).GetSymbolinfo(ioName);

        // Assert
        Assert.AreSame(symbolInfo, symbolInfo2);
        Assert.AreEqual(ioName, symbolInfo.Name);
    }

    [TestMethod]
    public void GetSymbolinfoThrowsSymbolExceptionWhenNoSymbolsAreLoaded()
    {
        // Arrange
        var adsPlcConnection = new Mock<IAdsPlcConnection>();
        _ = adsPlcConnection.SetupGet(x => x.SessionStream).Returns(Mock.Of<IObservable<IConnected<IAdsConnection>>>());
        var symbolHandlerSettings = new SymbolHandlerSettings();
        var fileSystemMock = new Mock<IFileSystem>();
        var symbolLoaderFactory = new Mock<ISymbolLoaderFactory>();
        using var symbolHandler = new SymbolHandler(adsPlcConnection.Object, MockHelpers.GetOptionsMoq(symbolHandlerSettings), MockHelpers.GetLoggerMock<SymbolHandler>(), fileSystemMock.Object, symbolLoaderFactory.Object);

        // Act
        // Assert
        _ = Assert.ThrowsException<SymbolException>(() => symbolHandler.GetSymbolinfo("dummyTag"));
    }

    [TestMethod]
    public void OnConnectSymbolListIsUpdated()
    {
        // Arrange
        var symbols = new SymbolCollection()
            {
                CreateSymbolMock("DummyVar1", Array.Empty<string>()),
                CreateSymbolMock("DummyVar2", Array.Empty<string>()),
                CreateSymbolMock("DummyVar3", new[] { "DummyVar3.Sub1", "DummyVar3.Sub2", "DummyVar3.Sub3" }),
            };
        using var subject = new Subject<IConnected<IAdsConnection>>();
        var adsPlcConnection = new Mock<IAdsPlcConnection>();
        _ = adsPlcConnection.SetupGet(x => x.SessionStream).Returns(subject);
        var symbolHandlerSettings = new SymbolHandlerSettings();
        var fileSystemMock = new Mock<IFileSystem>();
        var twincatSymbolLoader = new Mock<ISymbolLoader>();
        _ = twincatSymbolLoader.SetupGet(x => x.Symbols).Returns(symbols);
        var symbolLoaderFactory = new Mock<ISymbolLoaderFactory>();
        _ = symbolLoaderFactory.Setup(x => x.Create(It.IsAny<IConnection>(), It.IsAny<ISymbolLoaderSettings>())).Returns(twincatSymbolLoader.Object);
        using var symbolHandler = new SymbolHandler(adsPlcConnection.Object, MockHelpers.GetOptionsMoq(symbolHandlerSettings), MockHelpers.GetLoggerMock<SymbolHandler>(), fileSystemMock.Object, symbolLoaderFactory.Object);
        var adsClient = new Mock<IAdsDisposableConnection>();
        var connected = new Mock<IConnected<IAdsConnection>>();
        _ = connected.SetupGet(x => x.IsConnected).Returns(true);
        _ = connected.SetupGet(x => x.Value).Returns(adsClient.Object);

        // Act
        subject.OnNext(connected.Object);

        // Assert
        Assert.AreEqual(6, symbolHandler.AllSymbols.Count);
    }

    [TestMethod]
    public void SymbolsAreStoredOnDisk()
    {
        // Arrange
        var ioTag = "DummyVar1";
        var symbols = new SymbolCollection()
            {
                CreateSymbolMock(ioTag, Array.Empty<string>()),
            };
        using var subject = new Subject<IConnected<IAdsConnection>>();
        var adsPlcConnection = new Mock<IAdsPlcConnection>();
        _ = adsPlcConnection.SetupGet(x => x.SessionStream).Returns(subject);
        var symbolHandlerSettings = new SymbolHandlerSettings()
        {
            StoreSymbolsToDisk = true,
            OutputPath = "DummyPath",
        };
        var fileSystemMock = new MockFileSystem();

        var twincatSymbolLoader = new Mock<ISymbolLoader>();
        _ = twincatSymbolLoader.SetupGet(x => x.Symbols).Returns(symbols);
        var symbolLoaderFactory = new Mock<ISymbolLoaderFactory>();
        _ = symbolLoaderFactory.Setup(x => x.Create(It.IsAny<IConnection>(), It.IsAny<ISymbolLoaderSettings>())).Returns(twincatSymbolLoader.Object);
        using var symbolHandler = new SymbolHandler(adsPlcConnection.Object, MockHelpers.GetOptionsMoq(symbolHandlerSettings), MockHelpers.GetLoggerMock<SymbolHandler>(), fileSystemMock, symbolLoaderFactory.Object);
        var adsClient = new Mock<IAdsDisposableConnection>();
        var connected = new Mock<IConnected<IAdsConnection>>();
        _ = connected.SetupGet(x => x.IsConnected).Returns(true);
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
                CreateSymbolMock(ioTag, Array.Empty<string>()),
            };
        using var subject = new Subject<IConnected<IAdsConnection>>();
        var adsPlcConnection = new Mock<IAdsPlcConnection>();
        _ = adsPlcConnection.SetupGet(x => x.SessionStream).Returns(subject);
        var symbolHandlerSettings = new SymbolHandlerSettings()
        {
            StoreSymbolsToDisk = true,
            OutputPath = string.Empty,
        };
        var fileSystemMock = new MockFileSystem();

        var twincatSymbolLoader = new Mock<ISymbolLoader>();
        _ = twincatSymbolLoader.SetupGet(x => x.Symbols).Returns(symbols);
        var symbolLoaderFactory = new Mock<ISymbolLoaderFactory>();
        _ = symbolLoaderFactory.Setup(x => x.Create(It.IsAny<IConnection>(), It.IsAny<ISymbolLoaderSettings>())).Returns(twincatSymbolLoader.Object);
        using var symbolHandler = new SymbolHandler(adsPlcConnection.Object, MockHelpers.GetOptionsMoq(symbolHandlerSettings), MockHelpers.GetLoggerMock<SymbolHandler>(), fileSystemMock, symbolLoaderFactory.Object);
        var adsClient = new Mock<IAdsDisposableConnection>();
        var connected = new Mock<IConnected<IAdsConnection>>();
        _ = connected.SetupGet(x => x.IsConnected).Returns(true);
        _ = connected.SetupGet(x => x.Value).Returns(adsClient.Object);

        // Act
        subject.OnNext(connected.Object);

        // Assert
        Assert.AreEqual(1, fileSystemMock.AllFiles.Count());
        Assert.IsNotNull(fileSystemMock.GetFile("logs"));
        var fileDate = fileSystemMock.GetFile(fileSystemMock.AllFiles.First());
        StringAssert.Contains(fileDate.TextContents, ioTag, StringComparison.Ordinal);
    }

    private ISymbol CreateSymbolMock(string name, string[] subSymbols)
    {
        var symbol = new Mock<ISymbol>();
        _ = symbol.SetupGet(x => x.InstancePath).Returns(name);
        _ = subSymbols.Length == 0
            ? symbol.SetupGet(x => x.SubSymbols).Returns(SymbolCollection.Empty)
            : symbol.SetupGet(x => x.SubSymbols).Returns(new SymbolCollection(subSymbols.Select(x => CreateSymbolMock(x, Array.Empty<string>()))));

        return symbol.Object;
    }
}