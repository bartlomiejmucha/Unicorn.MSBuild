using System.Collections.Generic;

namespace Unicorn.MSBuild.Network
{
    public interface IRequestFactory
    {
        IRequest Create(string url, int timeout, Dictionary<string, string> headers);
    }
}