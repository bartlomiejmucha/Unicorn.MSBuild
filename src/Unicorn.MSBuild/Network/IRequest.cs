using System.Net;

namespace Unicorn.MSBuild.Network
{
    public interface IRequest
    {
        string Url { get; }
        int Timeout { get; }
        WebHeaderCollection Headers { get; }

        string Execute();
        T Execute<T>(IResponseStreamProcessor<T> processor);
    }
}