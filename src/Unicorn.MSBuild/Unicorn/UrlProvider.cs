using System;

namespace Unicorn.MSBuild.Unicorn
{
    public class UrlProvider : IUrlProvider
    {
        private readonly string _panelUrl;

        public UrlProvider(string panelUrl)
        {
            _panelUrl = panelUrl;
        }

        public string GetUrl(Verb verb)
        {
            switch (verb)
            {
                case Verb.Sync:
                    return $"{_panelUrl}?verb=Sync&configuration=&skipTransparentConfigs=False";
                case Verb.Challenge:
                    return $"{_panelUrl}?verb=Challenge";
            }

            throw new InvalidOperationException();
        }
    }
}