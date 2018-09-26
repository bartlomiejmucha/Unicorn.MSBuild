using FluentAssertions;
using NSubstitute;
using System.IO;
using System.Net;
using System.Text;
using Unicorn.MSBuild.Network;
using Xunit;

namespace Unicorn.MSBuild.Tests.Network
{
    public class RequestTests
    {
        [Fact]
        public void WhenExecuteWithDefaultProcessor_ReturnsStreamContent()
        {
            // Arrange
            var content = "stream content";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

            var request = Substitute.For<WebRequest>();
            var response = Substitute.For<WebResponse>();
            request.GetResponse().Returns(response);
            response.GetResponseStream().Returns(stream);

            var sut = new Request(request);

            // Act
            var result = sut.Execute();

            // Assert
            result.Should().Be(content);
        }

        [Fact]
        public void WhenExecuteWithCustomProcessor_ReturnsStreamContent()
        {
            // Arrange
            var content = "stream content";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

            var processor = Substitute.For<IResponseStreamProcessor<string>>();
            processor.Process(stream).Returns(content);

            var request = Substitute.For<WebRequest>();
            var response = Substitute.For<WebResponse>();
            request.GetResponse().Returns(response);
            response.GetResponseStream().Returns(stream);

            var sut = new Request(request);

            // Act
            var result = sut.Execute(processor);

            // Assert
            result.Should().Be(content);
        }
    }
}