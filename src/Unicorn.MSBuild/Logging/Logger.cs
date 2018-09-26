using Microsoft.Build.Utilities;

namespace Unicorn.MSBuild.Logging
{
    public class Logger : ILogger
    {
        private const string ErrorLinePrefix = "Error";
        private const string WarningLinePrefix = "Warning";

        private readonly TaskLoggingHelper _taskLoggingHelper;

        public Logger(TaskLoggingHelper taskLoggingHelper)
        {
            _taskLoggingHelper = taskLoggingHelper;
        }

        public void LogLine(string line)
        {
            var index = line?.IndexOf(':');
            if (index > 0)
            {
                var type = line.Substring(0, index.Value);
                var message = line.Substring(index.Value);

                switch (type)
                {
                    case ErrorLinePrefix:
                        _taskLoggingHelper.LogError(message);
                        return;
                    case WarningLinePrefix:
                        _taskLoggingHelper.LogWarning(message);
                        return;
                }
            }

            _taskLoggingHelper.LogMessage(line);
        }
    }
}