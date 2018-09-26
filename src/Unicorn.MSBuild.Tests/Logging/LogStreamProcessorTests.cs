using FluentAssertions;
using NSubstitute;
using System;
using System.IO;
using System.Text;
using Unicorn.MSBuild.Logging;
using Unicorn.MSBuild.Network;
using Xunit;

namespace Unicorn.MSBuild.Tests.Logging
{
    public class LogStreamProcessorTests
    {
        private const string ErrorLine = "line contains error";

        private readonly ILogger _logger;
        private readonly IResponseStreamProcessor<bool> _sut;

        public LogStreamProcessorTests()
        {
            _logger = Substitute.For<ILogger>();

            var logLineAnalyzer = Substitute.For<ILogLineAnalyzer>();
            logLineAnalyzer.IsError(ErrorLine).Returns(true);

            _sut = new LogStreamProcessor(_logger, logLineAnalyzer);
        }

        [Fact]
        public void WhenStreamIsNull_ThrowsArgumentException()
        {
            // Arrange

            // Act
            Action act = () => _sut.Process(null);

            // Assert
            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("stream");
        }

        [Fact]
        public void WhenEmptyStream_LogNoLines()
        {
            // Arrange
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(""));

            // Act
            var result = _sut.Process(stream);

            // Assert
            result.Should().BeTrue();
            _logger.DidNotReceive().LogLine(Arg.Any<string>());
        }

        [Fact]
        public void WhenOneLineInStream_LogOneLine()
        {
            // Arrange
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("first log line"));

            // Act
            var result = _sut.Process(stream);

            // Assert
            result.Should().BeTrue();
            _logger.Received(1).LogLine(Arg.Any<string>());
        }

        [Fact]
        public void WhenTwoLinesInStream_LogTwoLines()
        {
            // Arrange
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("first log line\nsecond line"));

            // Act
            var result = _sut.Process(stream);

            // Assert
            result.Should().BeTrue();
            _logger.Received(2).LogLine(Arg.Any<string>());
        }

        [Fact]
        public void WhenLineContainsError_ReturnsFalse()
        {
            // Arrange
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("first log line\nsecond line\n" + ErrorLine));

            // Act
            var result = _sut.Process(stream);

            // Assert
            result.Should().BeFalse();
            _logger.Received(3).LogLine(Arg.Any<string>());
        }
    }
}