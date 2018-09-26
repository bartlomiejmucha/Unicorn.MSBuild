using System.IO;

namespace Unicorn.MSBuild.Network
{
    public interface IResponseStreamProcessor<out T>
    {
        T Process(Stream stream);
    }
}