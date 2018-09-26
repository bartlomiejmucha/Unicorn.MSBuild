using System;
using System.IO;
using Unicorn.MSBuild.Network;

namespace Unicorn.MSBuild.Logging
{
    public class LogStreamProcessor : IResponseStreamProcessor<bool>
    {
        private readonly ILogger _logger;
        private readonly ILogLineAnalyzer _analyzer;

        public LogStreamProcessor(ILogger logger, ILogLineAnalyzer analyzer)
        {
            _logger = logger;
            _analyzer = analyzer;
        }

        public bool Process(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            var result = true;

            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    _logger.LogLine(line);

                    if (_analyzer.IsError(line))
                    {
                        result = false;
                    }
                }
            }

            return result;
        }
    }
}