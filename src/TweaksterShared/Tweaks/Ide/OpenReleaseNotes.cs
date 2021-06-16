using System;
using System.ComponentModel.Design;
using Microsoft;
using Microsoft.VisualStudio.Setup.Configuration;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace Tweakster.Tweaks.Ide
{
    public class OpenReleaseNotes
    {
        private const string _releaseUrl = "https://docs.microsoft.com/visualstudio/releases/2019/release-notes?utm_source=tweaks";
        private const string _previewUrl = "https://docs.microsoft.com/visualstudio/releases/2019/release-notes-preview?utm_source=tweaks";

        public static async Task InitializeAsync(AsyncPackage package)
        {
            IMenuCommandService commandService = await package.GetServiceAsync<IMenuCommandService, IMenuCommandService>();
            Assumes.Present(commandService);


            var cmdId = new CommandID(PackageGuids.guidCommands, PackageIds.OpenReleaseNotes);
            var cmd = new MenuCommand(Execute, cmdId);
            commandService.AddCommand(cmd);
        }

        private static void Execute(object sender, EventArgs e)
        {
            var vsSetupConfig = new SetupConfiguration();
            ISetupInstance setupInstance = vsSetupConfig.GetInstanceForCurrentProcess();
            var setupInstanceCatalog = (ISetupInstanceCatalog)setupInstance;

            var url = setupInstanceCatalog.IsPrerelease() ? _previewUrl : _releaseUrl;

            System.Diagnostics.Process.Start(url);
        }
    }
}
