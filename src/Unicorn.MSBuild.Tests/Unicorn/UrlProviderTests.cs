using FluentAssertions;
using Unicorn.MSBuild.Unicorn;
using Xunit;

namespace Unicorn.MSBuild.Tests.Unicorn
{
    public class UrlProviderTests
    {
        private const string PanelUrl = "https://test.sc/unicorn.aspx";

        private readonly UrlProvider _sut;

        public UrlProviderTests()
        {
            _sut = new UrlProvider(PanelUrl);
        }

        [Fact]
        public void WhenSyncVerbRequested_ReturnsFormattedUrl()
        {
            // Arrange

            // Act
            var result = _sut.GetUrl(Verb.Sync);

            // Assert
            result.Should().Be($"{PanelUrl}?verb=Sync&configuration=&skipTransparentConfigs=False");
        }

        [Fact]
        public void WhenChallengeVerbRequested_ReturnsFormattedUrl()
        {
            // Arrange

            // Act
            var result = _sut.GetUrl(Verb.Challenge);

            // Assert
            result.Should().Be($"{PanelUrl}?verb=Challenge");
        }
    }
}