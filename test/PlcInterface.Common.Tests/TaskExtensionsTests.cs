using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace PlcInterface.Common.Tests;

[TestClass]
public class TaskExtensionsTests
{
    [TestMethod]
    public void LogExceptionsAsyncLogsExceptionsToErrorStream()
    {
        // Arrange
        var loggerMock = new Mock<ILogger>();

        // Act
        var task2 = Task.Run(() => throw new NotSupportedException()).LogExceptionsAsync(loggerMock.Object);
        task2.Wait();

        // Assert
        loggerMock.Verify(x => x.Log(It.Is<LogLevel>(x => x == LogLevel.Error), It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<NotSupportedException>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }

    [TestMethod]
    public void LogExceptionsAsyncThrowsArgumentNullExceptionWhenArgumentsAreNull()
    {
        // Arrange
        var loggerMock = new Mock<ILogger>();
        var task = new Task(() => { });

        // Act
        // Assert
        _ = Assert.ThrowsExceptionAsync<ArgumentNullException>(() => TaskExtensions.LogExceptionsAsync(null!, loggerMock.Object));
        _ = Assert.ThrowsExceptionAsync<ArgumentNullException>(() => task.LogExceptionsAsync(null!));
    }
}