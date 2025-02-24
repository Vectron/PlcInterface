using Moq;

namespace PlcInterface.Abstraction.Tests;

[TestClass]
public class IReadWriteExtensionTests
{
    [TestMethod]
    public async Task WaitForValueAsyncBlocksUntilValueIsTriggeredAsync()
    {
        // Arrange
        var ioName = "test.var";
        var readWriteMock = new Mock<IReadWrite>();
        var counter = 0;
        _ = readWriteMock.Setup(x => x.ReadAsync<bool>(It.Is<string>(x => x.Equals(ioName, StringComparison.Ordinal)))).Returns(() => Task.FromResult(counter++ == 2));

        // Act
        await readWriteMock.Object.WaitForValueAsync(ioName, filterValue: true, TimeSpan.FromMilliseconds(1000));

        // Assert
        readWriteMock.Verify(x => x.ReadAsync<bool>(It.Is<string>(x => x.Equals(ioName, StringComparison.Ordinal))), Times.Exactly(3));
    }

    [TestMethod]
    public void WaitForValueBlocksUntilValueIsTriggered()
    {
        // Arrange
        var ioName = "test.var";
        var readWriteMock = new Mock<IReadWrite>();
        var counter = 0;
        _ = readWriteMock.Setup(x => x.Read<bool>(It.Is<string>(x => x.Equals(ioName, StringComparison.Ordinal)))).Returns(() => counter++ == 2);

        // Act
        readWriteMock.Object.WaitForValue(ioName, filterValue: true, TimeSpan.FromMilliseconds(1000));

        // Assert
        readWriteMock.Verify(x => x.Read<bool>(It.Is<string>(x => x.Equals(ioName, StringComparison.Ordinal))), Times.Exactly(3));
    }

    [TestMethod]
    public void WaitForValueIgnoresNullValues()
    {
        // Arrange
        var ioName = "test.var";
        var result = "test";
        var readWriteMock = new Mock<IReadWrite>();
        var counter = 0;
        _ = readWriteMock.Setup(x => x.Read<string>(It.Is<string>(x => x.Equals(ioName, StringComparison.Ordinal)))).Returns(() => counter++ == 0 ? null! : result);

        // Act
        readWriteMock.Object.WaitForValue(ioName, result, TimeSpan.FromMilliseconds(1000));

        // Assert
        readWriteMock.Verify(x => x.Read<string>(It.Is<string>(x => x.Equals(ioName, StringComparison.Ordinal))), Times.Exactly(2));
    }

    [TestMethod]
    public async Task WaitForValueIgnoresNullValuesAsync()
    {
        // Arrange
        var ioName = "test.var";
        var result = "test";
        var readWriteMock = new Mock<IReadWrite>();
        var counter = 0;
        _ = readWriteMock.Setup(x => x.ReadAsync<string>(It.Is<string>(x => x.Equals(ioName, StringComparison.Ordinal)))).Returns(() => counter++ == 0 ? Task.FromResult<string>(null!) : Task.FromResult(result));

        // Act
        await readWriteMock.Object.WaitForValueAsync(ioName, result, TimeSpan.FromMilliseconds(1000));

        // Assert
        readWriteMock.Verify(x => x.ReadAsync<string>(It.Is<string>(x => x.Equals(ioName, StringComparison.Ordinal))), Times.Exactly(2));
    }

    [TestMethod]
    public void WaitForValueThrowsExceptionOnTimeout()
    {
        // Arrange
        var ioName = "test.var";
        var readWrite = Mock.Of<IReadWrite>();

        // Act Assert
        _ = Assert.ThrowsException<TimeoutException>(() => readWrite.WaitForValue(ioName, filterValue: true, TimeSpan.FromMilliseconds(2)));
    }

    [TestMethod]
    public void WaitForValueThrowsExceptionOnTimeoutAsync()
    {
        // Arrange
        var ioName = "test.var";
        var readWrite = Mock.Of<IReadWrite>();

        // Act Assert
        _ = Assert.ThrowsExceptionAsync<TimeoutException>(() => readWrite.WaitForValueAsync(ioName, filterValue: true, TimeSpan.FromMilliseconds(2)));
    }
}
