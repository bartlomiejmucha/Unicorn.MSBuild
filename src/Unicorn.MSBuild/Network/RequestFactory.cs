using System.Collections.Generic;
using System.Net;

namespace Unicorn.MSBuild.Network
{
    public class RequestFactory : IRequestFactory
    {
        public IRequest Create(string url, int timeout, Dictionary<string, string> headers)
        {
            var request = WebRequest.Create(url);
            request.Timeout = timeout;

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers[header.Key] = header.Value;
                }
            }

            return new Request(request);
        }
    }
}