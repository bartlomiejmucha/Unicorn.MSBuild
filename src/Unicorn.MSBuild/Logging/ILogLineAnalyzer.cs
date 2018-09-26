namespace Unicorn.MSBuild.Logging
{
    public interface ILogLineAnalyzer
    {
        bool IsError(string line);
    }
}