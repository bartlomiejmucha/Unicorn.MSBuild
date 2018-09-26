using FluentAssertions;
using System.IO;
using System.Text;
using Unicorn.MSBuild.Network;
using Xunit;

namespace Unicorn.MSBuild.Tests.Network
{
    public class DefaultResponseStreamProcessorTests
    {
        private readonly IResponseStreamProcessor<string> _sut;

        public DefaultResponseStreamProcessorTests()
        {
            _sut = new DefaultResponseStreamProcessor();
        }

        [Fact]
        public void WhenStreamIsNull_ReturnNull()
        {
            // Arrange

            // Act
            var result = _sut.Process(null);

            // Assert
            result.Should().Be(null);
        }

        [Fact]
        public void WhenStreamIsNotNull_ReturnItsContent()
        {
            // Arrange
            var content = "stream content";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

            // Act
            var result = _sut.Process(stream);

            // Assert
            result.Should().Be(content);
        }
    }
}