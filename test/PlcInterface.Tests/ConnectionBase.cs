using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace PlcInterface.Tests
{
    public abstract class ConnectionBase
    {
        protected static ILogger<T> GetLoggerMock<T>()
        {
            var mock = new Mock<ILogger<T>>();
            mock.Setup(x => x.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.IsAny<object>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<object, Exception, string>>()))
                .Callback<LogLevel, EventId, object, Exception, Func<object, Exception, string>>((l, e, s, ex, f) =>
                {
                    if (ex != null)
                    {
                        Console.WriteLine($"{Enum.GetName(typeof(LogLevel), l)}: {f(s, ex)}\n{ex?.StackTrace}");
                        Assert.Fail($"{ex.Message} \n {ex.StackTrace}");
                    }
                    else
                    {
                        Console.WriteLine($"{Enum.GetName(typeof(LogLevel), l)}: {f(s, ex)}");
                    }
                });
            return mock.Object;
        }

        protected static IOptions<T> GetOptionsMoq<T>(T options) where T : class, new()
        {
            var connectionSettingsMoq = new Mock<IOptions<T>>();
            connectionSettingsMoq.Setup(x => x.Value).Returns(options);
            return connectionSettingsMoq.Object;
        }

        protected abstract IMonitor GetMonitor();

        protected abstract IPlcConnection GetPLCConnection();

        protected abstract IReadWrite GetReadWrite();

        protected abstract ISymbolHandler GetSymbolHandler();
    }
}