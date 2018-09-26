using Unicorn.MSBuild.Logging;

namespace Unicorn.MSBuild.Unicorn
{
    public class UnicornLogLineAnalyzer : ILogLineAnalyzer
    {
        private const string ErrorOccurredMessage = "****ERROR OCCURRED****";

        public bool IsError(string line)
        {
            return line != null && line.Contains(ErrorOccurredMessage);
        }
    }
}