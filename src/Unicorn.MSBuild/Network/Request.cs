using System.Net;

namespace Unicorn.MSBuild.Network
{
    public class Request : IRequest
    {
        private readonly WebRequest _request;

        public string Url => _request.RequestUri.ToString();
        public int Timeout => _request.Timeout;
        public WebHeaderCollection Headers => _request.Headers;

        public Request(WebRequest request)
        {
            _request = request;
        }

        public string Execute()
        {
            return Execute(new DefaultResponseStreamProcessor());
        }

        public T Execute<T>(IResponseStreamProcessor<T> processor)
        {
            using (var response = _request.GetResponse())
            using (var stream = response.GetResponseStream())
            {
                return processor.Process(stream);
            }
        }
    }
}