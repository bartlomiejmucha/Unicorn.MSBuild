namespace Unicorn.MSBuild.Unicorn
{
    public interface IUrlProvider
    {
        string GetUrl(Verb verb);
    }
}