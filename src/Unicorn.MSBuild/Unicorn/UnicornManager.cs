using MicroCHAP;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.Net;
using Unicorn.MSBuild.Network;

namespace Unicorn.MSBuild.Unicorn
{
    public class UnicornManager : IUnicornManager
    {
        private readonly string _panelUrl;
        private readonly string _secret;
        private readonly IUrlProvider _urlProvider;
        private readonly IRequestFactory _requestFactory;
        private readonly TaskLoggingHelper _log;
        private readonly ISignatureService _signatureService;
        private readonly IResponseStreamProcessor<bool> _streamProcessor;

        public UnicornManager(string panelUrl, string secret, TaskLoggingHelper log)
            : this(panelUrl, secret, log, new UrlProvider(panelUrl), new RequestFactory(), new SignatureService(secret),
                new Logging.LogStreamProcessor(new Logging.Logger(log), new UnicornLogLineAnalyzer()))
        {
        }

        public UnicornManager(string panelUrl, string secret, TaskLoggingHelper log, IUrlProvider urlProvider,
            IRequestFactory requestFactory, ISignatureService signatureService,
            IResponseStreamProcessor<bool> streamProcessor)
        {
            _streamProcessor = streamProcessor;
            _signatureService = signatureService;
            _requestFactory = requestFactory;
            _urlProvider = urlProvider;
            _panelUrl = panelUrl;
            _secret = secret;
            _log = log;
        }

        public bool Sync()
        {
            if (string.IsNullOrWhiteSpace(_panelUrl) || string.IsNullOrWhiteSpace(_secret))
            {
                throw new ArgumentNullException();
            }

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var syncUrl = _urlProvider.GetUrl(Verb.Sync);
            var challenge = _requestFactory.Create(_urlProvider.GetUrl(Verb.Challenge), 360000, null).Execute();
            var signature = _signatureService.CreateSignature(challenge, syncUrl, null);

            return _requestFactory
                       .Create(syncUrl, 10800000,
                           new Dictionary<string, string>
                               {{"X-MC-MAC", signature.SignatureHash}, {"X-MC-Nonce", challenge}})
                       .Execute(_streamProcessor)
                   && !_log.HasLoggedErrors;
        }
    }
}