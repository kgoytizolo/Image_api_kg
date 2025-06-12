using Microsoft.Extensions.Logging;
using Moq;

namespace ImageAppTest.Extensions;

public static class SerilogMockExtension
{
    public static Mock<ILogger<TValue>> ConfigureMockLogger<TValue>() where TValue : class
    {
        var loggerMock = new Mock<ILogger<TValue>>();

        List<LogLevel> logLevels = new List<LogLevel>();
        List<string> logMessages = new List<string>();

        loggerMock.Setup(
            x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<object>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<object, Exception?, string>>())
        ).Callback<LogLevel, EventId, object, Exception, Func<object, Exception, string>>(
            (level, eventId, state, exception, func) =>
            {
                logLevels.Add(level);
                logMessages.Add(func(state, exception));
            }
        );

        return loggerMock;
    }
}
