using System.ComponentModel.Design;
using System.IO;
using Microsoft;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace Tweakster
{
    internal sealed class ReOpenDocument
    {
        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Assumes.Present(commandService);

            var cmdId = new CommandID(PackageGuids.guidCommands, PackageIds.ReOpenDocument);
            var menuItem = new OleMenuCommand((s, e) => Execute(package), cmdId);
            menuItem.BeforeQueryStatus += BeforeQueryStatus;
            commandService.AddCommand(menuItem);
        }

        private static void BeforeQueryStatus(object sender, System.EventArgs e)
        {
            var command = (OleMenuCommand)sender;

            command.Enabled = !string.IsNullOrWhiteSpace(ReOpenCreationListener.LastClosed);

            if (command.Enabled)
            {
                var fileName = Path.GetFileName(ReOpenCreationListener.LastClosed);
                command.Text = $"Re-Open {fileName}";
            }
            else
            {
                command.Text = "Re-Open Closed File";
            }
        }

        private static void Execute(AsyncPackage package)
        {
            var lastClosed = ReOpenCreationListener.LastClosed;

            if (!string.IsNullOrWhiteSpace(lastClosed) && File.Exists(lastClosed))
            {
                VsShellUtilities.OpenDocument(package, lastClosed);
            }
        }
    }
}
