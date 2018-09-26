using System.IO;

namespace Unicorn.MSBuild.Network
{
    public class DefaultResponseStreamProcessor : IResponseStreamProcessor<string>
    {
        public string Process(Stream stream)
        {
            if (stream != null)
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }

            return null;
        }
    }
}