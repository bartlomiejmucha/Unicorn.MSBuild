using FluentAssertions;
using NSubstitute;
using Unicorn.MSBuild.Tasks.Unicorn.MSBuild.Tasks;
using Unicorn.MSBuild.Unicorn;
using Xunit;

namespace Unicorn.MSBuild.Tests.Tasks
{
    public class SyncUnicornTests
    {
        [Fact]
        public void WhenExecuted_ExecuteSyncMethodWithUnicornManager()
        {
            // Arrange
            var manager = Substitute.For<IUnicornManager>();
            var sut = new SyncUnicorn(manager);

            // Act
            sut.Execute();

            // Assert
            manager.Received(1).Sync();
        }

        [Fact]
        public void WhenUserManagerReturnsTrue_ReturnsTrue()
        {
            // Arrange
            var manager = Substitute.For<IUnicornManager>();
            manager.Sync().Returns(true);
            var sut = new SyncUnicorn(manager);

            // Act
            var result = sut.Execute();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void WhenUserManagerReturnsFalse_ReturnsFalse()
        {
            // Arrange
            var manager = Substitute.For<IUnicornManager>();
            manager.Sync().Returns(false);
            var sut = new SyncUnicorn(manager);

            // Act
            var result = sut.Execute();

            // Assert
            result.Should().BeFalse();
        }
    }
}