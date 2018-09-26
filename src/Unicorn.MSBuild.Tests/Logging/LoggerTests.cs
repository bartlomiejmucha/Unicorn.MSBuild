using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using NSubstitute;
using Xunit;
using ILogger = Unicorn.MSBuild.Logging.ILogger;
using Logger = Unicorn.MSBuild.Logging.Logger;

namespace Unicorn.MSBuild.Tests.Logging
{
    public class LoggerTests
    {
        private readonly IBuildEngine _buildEngine;
        private readonly ILogger _sut;

        public LoggerTests()
        {
            _buildEngine = Substitute.For<IBuildEngine>();
            _sut = new Logger(Substitute.ForPartsOf<TaskLoggingHelper>(_buildEngine, "SyncUnicorn"));
        }

        [Fact]
        public void WhenNoLineLogged_NoMessageLogged()
        {
            // Arrange

            // Act

            // Assert
            _buildEngine.DidNotReceive().LogErrorEvent(Arg.Any<BuildErrorEventArgs>());
            _buildEngine.DidNotReceive().LogWarningEvent(Arg.Any<BuildWarningEventArgs>());
            _buildEngine.DidNotReceive().LogMessageEvent(Arg.Any<BuildMessageEventArgs>());
        }

        [Fact]
        public void WhenLineProvided_MessageLogged()
        {
            // Arrange
            var line = "some message";

            // Act
            _sut.LogLine(line);

            // Assert
            _buildEngine.DidNotReceive().LogErrorEvent(Arg.Any<BuildErrorEventArgs>());
            _buildEngine.DidNotReceive().LogWarningEvent(Arg.Any<BuildWarningEventArgs>());
            _buildEngine.Received(1).LogMessageEvent(Arg.Any<BuildMessageEventArgs>());
        }

        [Fact]
        public void WhenWarningLineProvided_WarningLogged()
        {
            // Arrange
            var line = "Warning:some message";

            // Act
            _sut.LogLine(line);

            // Assert
            _buildEngine.DidNotReceive().LogErrorEvent(Arg.Any<BuildErrorEventArgs>());
            _buildEngine.Received(1).LogWarningEvent(Arg.Any<BuildWarningEventArgs>());
            _buildEngine.DidNotReceive().LogMessageEvent(Arg.Any<BuildMessageEventArgs>());
        }

        [Fact]
        public void WhenErrorLineProvided_ErrorLogged()
        {
            // Arrange
            var line = "Error:some message";

            // Act
            _sut.LogLine(line);

            // Assert
            _buildEngine.Received(1).LogErrorEvent(Arg.Any<BuildErrorEventArgs>());
            _buildEngine.DidNotReceive().LogWarningEvent(Arg.Any<BuildWarningEventArgs>());
            _buildEngine.DidNotReceive().LogMessageEvent(Arg.Any<BuildMessageEventArgs>());
        }
    }
}