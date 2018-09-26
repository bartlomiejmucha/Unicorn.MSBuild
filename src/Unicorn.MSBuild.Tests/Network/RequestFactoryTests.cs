using FluentAssertions;
using System;
using System.Collections.Generic;
using Unicorn.MSBuild.Network;
using Xunit;

namespace Unicorn.MSBuild.Tests.Network
{
    public class RequestFactoryTests
    {
        private readonly IRequestFactory _sut;

        public RequestFactoryTests()
        {
            _sut = new RequestFactory();
        }

        [Fact]
        public void WhenUrlIsNull_ThrowsException()
        {
            // Arrange

            // Act
            Action act = () => _sut.Create(null, 0, new Dictionary<string, string>());

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void WhenUrlIsNotValid_ThrowsException()
        {
            // Arrange
            var url = "url in wrong format";

            // Act
            Action act = () => _sut.Create(url, 0, new Dictionary<string, string>());

            // Assert
            act.Should().Throw<UriFormatException>();
        }

        [Fact]
        public void WhenUrlIsValid_ReturnsRequestObject()
        {
            // Arrange
            var url = "https://test.sc/";

            // Act
            var result = _sut.Create(url, 0, new Dictionary<string, string>());

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void WhenHeaderIsNull_ReturnsRequestObject()
        {
            // Arrange
            var url = "https://test.sc/";

            // Act
            var result = _sut.Create(url, 0, null);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void CreateMethod_ReturnsRequestWithCorrectParameters()
        {
            // Arrange
            var url = "http://test.sc/";
            var timeout = 1000;
            var headers = new Dictionary<string, string> {{"headerA", "valueA"}, {"headerB", "valueB"}};

            // Act
            var result = _sut.Create(url, timeout, headers);

            // Assert
            result.Should().NotBeNull();
            result.Url.Should().Be(url);
            result.Timeout.Should().Be(timeout);
            result.Headers["headerA"].Should().Be("valueA");
            result.Headers["headerB"].Should().Be("valueB");
        }
    }
}