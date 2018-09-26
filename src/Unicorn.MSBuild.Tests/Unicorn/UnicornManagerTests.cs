using FluentAssertions;
using MicroCHAP;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Net;
using Unicorn.MSBuild.Network;
using Unicorn.MSBuild.Unicorn;
using Xunit;

namespace Unicorn.MSBuild.Tests.Unicorn
{
    public class UnicornManagerTests
    {
        [Fact]
        public void WhenUrlAndSecretAreEmpty_ThrowException()
        {
            // Arrange
            var sut = new UnicornManager(null, null, null);

            // Act
            Action act = () => sut.Sync();

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void WhenUrlIsEmpty_ThrowException()
        {
            // Arrange
            var sut = new UnicornManager(null, "secret", null);

            // Act
            Action act = () => sut.Sync();

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void WhenSecretIsEmpty_ThrowException()
        {
            // Arrange
            var sut = new UnicornManager("url", null, null);

            // Act
            Action act = () => sut.Sync();

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void WhenExecuteSync_SetRequiredSecurityProtocolType()
        {
            // Arrange
            var urlProvider = Substitute.For<IUrlProvider>();
            var syncUrl = "http://test.sc?Sync";
            var challengeUrl = "http://test.sc?Challenge";
            urlProvider.GetUrl(Verb.Sync).Returns(syncUrl);
            urlProvider.GetUrl(Verb.Challenge).Returns(challengeUrl);

            var challengeRequest = Substitute.For<IRequest>();
            var challenge = "challenge";
            challengeRequest.Execute().Returns(challenge);

            var streamProcessor = Substitute.For<IResponseStreamProcessor<bool>>();

            var syncRequest = Substitute.For<IRequest>();
            syncRequest.Execute(streamProcessor).Returns(true);

            var requestFactory = Substitute.For<IRequestFactory>();
            requestFactory.Create(challengeUrl, Arg.Any<int>(), null).Returns(challengeRequest);
            requestFactory.Create(syncUrl, Arg.Any<int>(), null).Returns(syncRequest);

            var signatureService = Substitute.For<ISignatureService>();
            var signatureResult = new SignatureResult();
            signatureService.CreateSignature(challenge, syncUrl, null).Returns(signatureResult);

            var sut = new UnicornManager("http://test.sc", "secret", null, urlProvider, requestFactory,
                signatureService, streamProcessor);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.SystemDefault;

            // Act
            sut.Sync();

            // Assert
            ServicePointManager.SecurityProtocol.Should().Be(SecurityProtocolType.Tls12);
        }

        [Fact]
        public void WhenExecuteSync_DoRequestForChallengeWithChallengeUrl()
        {
            // Arrange
            var urlProvider = Substitute.For<IUrlProvider>();
            var syncUrl = "http://test.sc?Sync";
            var challengeUrl = "http://test.sc?Challenge";
            urlProvider.GetUrl(Verb.Sync).Returns(syncUrl);
            urlProvider.GetUrl(Verb.Challenge).Returns(challengeUrl);

            var challengeRequest = Substitute.For<IRequest>();
            var challenge = "challenge";
            challengeRequest.Execute().Returns(challenge);

            var streamProcessor = Substitute.For<IResponseStreamProcessor<bool>>();

            var syncRequest = Substitute.For<IRequest>();
            syncRequest.Execute(streamProcessor).Returns(true);

            var requestFactory = Substitute.For<IRequestFactory>();
            requestFactory.Create(challengeUrl, Arg.Any<int>(), null).Returns(challengeRequest);
            requestFactory.Create(syncUrl, Arg.Any<int>(), null).Returns(syncRequest);

            var signatureService = Substitute.For<ISignatureService>();
            var signatureResult = new SignatureResult();
            signatureService.CreateSignature(challenge, syncUrl, null).Returns(signatureResult);

            var sut = new UnicornManager("http://test.sc", "secret", null, urlProvider, requestFactory,
                signatureService, streamProcessor);

            // Act
            sut.Sync();

            // Assert
            urlProvider.Received(1).GetUrl(Verb.Sync);
            urlProvider.Received(1).GetUrl(Verb.Challenge);
            requestFactory.Received(1).Create(challengeUrl, 360000, null);
            challengeRequest.Received(1).Execute();
        }

        [Fact]
        public void WhenExecuteSync_SignatureIsCreatedWithChallengeAndSyncUrl()
        {
            // Arrange
            var urlProvider = Substitute.For<IUrlProvider>();
            var syncUrl = "http://test.sc?Sync";
            var challengeUrl = "http://test.sc?Challenge";
            urlProvider.GetUrl(Verb.Sync).Returns(syncUrl);
            urlProvider.GetUrl(Verb.Challenge).Returns(challengeUrl);

            var challengeRequest = Substitute.For<IRequest>();
            var challenge = "challenge";
            challengeRequest.Execute().Returns(challenge);

            var streamProcessor = Substitute.For<IResponseStreamProcessor<bool>>();

            var syncRequest = Substitute.For<IRequest>();
            syncRequest.Execute(streamProcessor).Returns(true);

            var requestFactory = Substitute.For<IRequestFactory>();
            requestFactory.Create(challengeUrl, Arg.Any<int>(), null).Returns(challengeRequest);
            requestFactory.Create(syncUrl, Arg.Any<int>(), null).Returns(syncRequest);

            var signatureService = Substitute.For<ISignatureService>();
            var signatureResult = new SignatureResult();
            signatureService.CreateSignature(challenge, syncUrl, null).Returns(signatureResult);

            var sut = new UnicornManager("http://test.sc", "secret", null, urlProvider, requestFactory,
                signatureService, streamProcessor);

            // Act
            sut.Sync();

            // Assert
            signatureService.Received(1).CreateSignature(challenge, syncUrl, null);
        }

        [Fact]
        public void WhenExecuteSync_RequestToSyncUrlIsCreatedWithCorrectHeaders()
        {
            // Arrange
            var urlProvider = Substitute.For<IUrlProvider>();
            var syncUrl = "http://test.sc?Sync";
            var challengeUrl = "http://test.sc?Challenge";
            urlProvider.GetUrl(Verb.Sync).Returns(syncUrl);
            urlProvider.GetUrl(Verb.Challenge).Returns(challengeUrl);

            var challengeRequest = Substitute.For<IRequest>();
            var challenge = "challenge";
            challengeRequest.Execute().Returns(challenge);

            var streamProcessor = Substitute.For<IResponseStreamProcessor<bool>>();

            var syncRequest = Substitute.For<IRequest>();
            syncRequest.Execute(streamProcessor).Returns(true);

            var signatureService = Substitute.For<ISignatureService>();
            var signatureResult = new SignatureResult() {SignatureHash = "signature hash"};
            signatureService.CreateSignature(challenge, syncUrl, null).Returns(signatureResult);

            var requestFactory = Substitute.For<IRequestFactory>();
            requestFactory.Create(challengeUrl, 360000, null).Returns(challengeRequest);
            requestFactory.Create(syncUrl, 10800000,
                    Arg.Is<Dictionary<string, string>>(x =>
                        x.Keys.Count == 2 && x.ContainsKey("X-MC-MAC") && x.ContainsKey("X-MC-Nonce") &&
                        x["X-MC-MAC"] == signatureResult.SignatureHash && x["X-MC-Nonce"] == challenge))
                .Returns(syncRequest);

            var sut = new UnicornManager("http://test.sc", "secret", Substitute.For<TaskLoggingHelper>(Substitute.For<ITask>()), urlProvider, requestFactory,
                signatureService, streamProcessor);

            // Act
            sut.Sync();

            // Assert
            requestFactory.Received(1).Create(syncUrl, 10800000, Arg.Is<Dictionary<string, string>>(x =>
                x.Keys.Count == 2 && x.ContainsKey("X-MC-MAC") && x.ContainsKey("X-MC-Nonce") &&
                x["X-MC-MAC"] == signatureResult.SignatureHash && x["X-MC-Nonce"] == challenge));
            syncRequest.Received(1).Execute(streamProcessor);
        }

        [Fact]
        public void WhenExecuteSyncRequestReturnsTrueAndNoLoggedErrors_ReturnsTrue()
        {
            // Arrange
            var urlProvider = Substitute.For<IUrlProvider>();
            var syncUrl = "http://test.sc?Sync";
            var challengeUrl = "http://test.sc?Challenge";
            urlProvider.GetUrl(Verb.Sync).Returns(syncUrl);
            urlProvider.GetUrl(Verb.Challenge).Returns(challengeUrl);

            var challengeRequest = Substitute.For<IRequest>();
            var challenge = "challenge";
            challengeRequest.Execute().Returns(challenge);

            var streamProcessor = Substitute.For<IResponseStreamProcessor<bool>>();

            var syncRequest = Substitute.For<IRequest>();
            syncRequest.Execute(streamProcessor).Returns(true);

            var signatureService = Substitute.For<ISignatureService>();
            var signatureResult = new SignatureResult() { SignatureHash = "signature hash" };
            signatureService.CreateSignature(challenge, syncUrl, null).Returns(signatureResult);

            var requestFactory = Substitute.For<IRequestFactory>();
            requestFactory.Create(challengeUrl, 360000, null).Returns(challengeRequest);
            requestFactory.Create(syncUrl, 10800000,
                    Arg.Is<Dictionary<string, string>>(x =>
                        x.Keys.Count == 2 && x.ContainsKey("X-MC-MAC") && x.ContainsKey("X-MC-Nonce") &&
                        x["X-MC-MAC"] == signatureResult.SignatureHash && x["X-MC-Nonce"] == challenge))
                .Returns(syncRequest);

            var sut = new UnicornManager("http://test.sc", "secret", Substitute.For<TaskLoggingHelper>(Substitute.For<ITask>()), urlProvider, requestFactory,
                signatureService, streamProcessor);

            // Act
            var result = sut.Sync();

            // Assert
            result.Should().Be(true);
        }

        [Fact]
        public void WhenExecuteSyncRequestReturnsTrueButThereAreLoggedErrors_ReturnsFalse()
        {
            // Arrange
            var urlProvider = Substitute.For<IUrlProvider>();
            var syncUrl = "http://test.sc?Sync";
            var challengeUrl = "http://test.sc?Challenge";
            urlProvider.GetUrl(Verb.Sync).Returns(syncUrl);
            urlProvider.GetUrl(Verb.Challenge).Returns(challengeUrl);

            var challengeRequest = Substitute.For<IRequest>();
            var challenge = "challenge";
            challengeRequest.Execute().Returns(challenge);

            var streamProcessor = Substitute.For<IResponseStreamProcessor<bool>>();

            var syncRequest = Substitute.For<IRequest>();
            syncRequest.Execute(streamProcessor).Returns(true);

            var signatureService = Substitute.For<ISignatureService>();
            var signatureResult = new SignatureResult() { SignatureHash = "signature hash" };
            signatureService.CreateSignature(challenge, syncUrl, null).Returns(signatureResult);

            var requestFactory = Substitute.For<IRequestFactory>();
            requestFactory.Create(challengeUrl, 360000, null).Returns(challengeRequest);
            requestFactory.Create(syncUrl, 10800000,
                    Arg.Is<Dictionary<string, string>>(x =>
                        x.Keys.Count == 2 && x.ContainsKey("X-MC-MAC") && x.ContainsKey("X-MC-Nonce") &&
                        x["X-MC-MAC"] == signatureResult.SignatureHash && x["X-MC-Nonce"] == challenge))
                .Returns(syncRequest);

            var log = Substitute.For<TaskLoggingHelper>(Substitute.For<ITask>());
            log.LogError("error");

            var sut = new UnicornManager("http://test.sc", "secret", log, urlProvider, requestFactory,
                signatureService, streamProcessor);

            // Act
            var result = sut.Sync();

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public void WhenExecuteSyncRequestReturnsFalseAndThereAreLoggedErrors_ReturnsFalse()
        {
            // Arrange
            var urlProvider = Substitute.For<IUrlProvider>();
            var syncUrl = "http://test.sc?Sync";
            var challengeUrl = "http://test.sc?Challenge";
            urlProvider.GetUrl(Verb.Sync).Returns(syncUrl);
            urlProvider.GetUrl(Verb.Challenge).Returns(challengeUrl);

            var challengeRequest = Substitute.For<IRequest>();
            var challenge = "challenge";
            challengeRequest.Execute().Returns(challenge);

            var streamProcessor = Substitute.For<IResponseStreamProcessor<bool>>();

            var syncRequest = Substitute.For<IRequest>();
            syncRequest.Execute(streamProcessor).Returns(false);

            var signatureService = Substitute.For<ISignatureService>();
            var signatureResult = new SignatureResult() { SignatureHash = "signature hash" };
            signatureService.CreateSignature(challenge, syncUrl, null).Returns(signatureResult);

            var requestFactory = Substitute.For<IRequestFactory>();
            requestFactory.Create(challengeUrl, 360000, null).Returns(challengeRequest);
            requestFactory.Create(syncUrl, 10800000,
                    Arg.Is<Dictionary<string, string>>(x =>
                        x.Keys.Count == 2 && x.ContainsKey("X-MC-MAC") && x.ContainsKey("X-MC-Nonce") &&
                        x["X-MC-MAC"] == signatureResult.SignatureHash && x["X-MC-Nonce"] == challenge))
                .Returns(syncRequest);

            var log = Substitute.For<TaskLoggingHelper>(Substitute.For<ITask>());
            log.LogError("error");

            var sut = new UnicornManager("http://test.sc", "secret", log, urlProvider, requestFactory,
                signatureService, streamProcessor);

            // Act
            var result = sut.Sync();

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public void WhenExecuteSyncRequestReturnsFalseButNoLoggedErrors_ReturnsFalse()
        {
            // Arrange
            var urlProvider = Substitute.For<IUrlProvider>();
            var syncUrl = "http://test.sc?Sync";
            var challengeUrl = "http://test.sc?Challenge";
            urlProvider.GetUrl(Verb.Sync).Returns(syncUrl);
            urlProvider.GetUrl(Verb.Challenge).Returns(challengeUrl);

            var challengeRequest = Substitute.For<IRequest>();
            var challenge = "challenge";
            challengeRequest.Execute().Returns(challenge);

            var streamProcessor = Substitute.For<IResponseStreamProcessor<bool>>();

            var syncRequest = Substitute.For<IRequest>();
            syncRequest.Execute(streamProcessor).Returns(false);

            var signatureService = Substitute.For<ISignatureService>();
            var signatureResult = new SignatureResult() { SignatureHash = "signature hash" };
            signatureService.CreateSignature(challenge, syncUrl, null).Returns(signatureResult);

            var requestFactory = Substitute.For<IRequestFactory>();
            requestFactory.Create(challengeUrl, 360000, null).Returns(challengeRequest);
            requestFactory.Create(syncUrl, 10800000,
                    Arg.Is<Dictionary<string, string>>(x =>
                        x.Keys.Count == 2 && x.ContainsKey("X-MC-MAC") && x.ContainsKey("X-MC-Nonce") &&
                        x["X-MC-MAC"] == signatureResult.SignatureHash && x["X-MC-Nonce"] == challenge))
                .Returns(syncRequest);

            var log = Substitute.For<TaskLoggingHelper>(Substitute.For<ITask>());

            var sut = new UnicornManager("http://test.sc", "secret", log, urlProvider, requestFactory,
                signatureService, streamProcessor);

            // Act
            var result = sut.Sync();

            // Assert
            result.Should().Be(false);
        }
    }
}