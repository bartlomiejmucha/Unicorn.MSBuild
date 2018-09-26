using FluentAssertions;
using Unicorn.MSBuild.Logging;
using Unicorn.MSBuild.Unicorn;
using Xunit;

namespace Unicorn.MSBuild.Tests.Unicorn
{
    public class UnicornLogLineAnalyzerTests
    {
        private readonly ILogLineAnalyzer _sut;

        public UnicornLogLineAnalyzerTests()
        {
            _sut = new UnicornLogLineAnalyzer();
        }

        [Fact]
        public void WhenLineContainsErrorMessage_ReturnsTrue()
        {
            // Arrange
            var line = "****ERROR OCCURRED****";

            // Act
            var result = _sut.IsError(line);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void WhenLineContainsOtherMessage_ReturnsFalse()
        {
            // Arrange
            var line = "****OTHER ERROR MESSAGE****";

            // Act
            var result = _sut.IsError(line);

            // Assert
            result.Should().BeFalse();
        }
    }
}