using System;
using System.ComponentModel.Design;
using Microsoft;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace Tweakster.Tweaks.Ide
{
    public class BrowseSuggestions
    {
        private const string _url = "https://aka.ms/browsesuggestions";

        public static async Task InitializeAsync(AsyncPackage package)
        {
            IMenuCommandService commandService = await package.GetServiceAsync<IMenuCommandService, IMenuCommandService>();
            Assumes.Present(commandService);

            var cmdId = new CommandID(PackageGuids.guidCommands, PackageIds.ExploreSuggestions);
            var cmd = new MenuCommand(Execute, cmdId);
            commandService.AddCommand(cmd);
        }

        private static void Execute(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(_url);
        }
    }
}
