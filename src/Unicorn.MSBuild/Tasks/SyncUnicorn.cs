using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Unicorn.MSBuild.Unicorn;

namespace Unicorn.MSBuild.Tasks
{
    namespace Unicorn.MSBuild.Tasks
    {
        public class SyncUnicorn : Task
        {
            private readonly IUnicornManager _unicornManager;

            [Required] public string ControlPanelUrl { get; set; }

            [Required] public string SharedSecret { get; set; }

            public SyncUnicorn()
            {
                _unicornManager = new UnicornManager(ControlPanelUrl, SharedSecret, Log);
            }

            public SyncUnicorn(IUnicornManager unicornManager)
            {
                _unicornManager = unicornManager;
            }

            public override bool Execute()
            {
                return _unicornManager.Sync();
            }
        }
    }
}