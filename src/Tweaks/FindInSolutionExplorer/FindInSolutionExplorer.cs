using System.ComponentModel.Design;
using EnvDTE;
using EnvDTE80;
using Microsoft;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace Tweakster
{
    internal sealed class FindInSolutionExplorer
    {
        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync<IMenuCommandService, OleMenuCommandService>();
            Assumes.Present(commandService);

            DTE2 dte = await package.GetServiceAsync<DTE, DTE2>();
            Assumes.Present(dte);

            var cmdId = new CommandID(PackageGuids.guidCommands, PackageIds.FindInSolutionExplorer);
            var menuItem = new OleMenuCommand((s, e) => Execute(dte), cmdId)
            {
                Supported = false
            };
            commandService.AddCommand(menuItem);
        }

        private static void Execute(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            Command cmd = dte.Commands.Item("SolutionExplorer.SyncWithActiveDocument");

            if (cmd?.IsAvailable == true)
            {
                dte.ExecuteCommand(cmd.Name);
            }
        }
    }
}
