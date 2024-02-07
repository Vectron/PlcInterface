using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace TestUtilities;

public static class MockHelpers
{
    public static ILogger<T> GetLoggerMock<T>()
    {
        var mock = new Mock<ILogger<T>>();
        _ = mock.Setup(x => x.Log(
                  It.IsAny<LogLevel>(),
                  It.IsAny<EventId>(),
                  It.IsAny<object>(),
                  It.IsAny<Exception>(),
                  It.IsAny<Func<object, Exception?, string>>()));
        return mock.Object;
    }

    public static IOptions<T> GetOptionsMoq<T>(T options)
        where T : class, new()
    {
        var connectionSettingsMoq = new Mock<IOptions<T>>();
        _ = connectionSettingsMoq.Setup(x => x.Value).Returns(options);
        return connectionSettingsMoq.Object;
    }
}
