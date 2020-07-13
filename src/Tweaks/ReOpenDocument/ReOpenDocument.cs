using System.ComponentModel.Design;
using System.IO;
using EnvDTE;
using EnvDTE80;
using Microsoft;
using Microsoft.VisualStudio.Shell;
using ShellEvents = Microsoft.VisualStudio.Shell.Events;
using Task = System.Threading.Tasks.Task;

namespace Tweakster
{
    internal sealed class ReOpenDocument
    {
        private static WindowEvents _events;
        private static string _closedFile;

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            await RegisterCommandAsync(package);
            await RegisterEventHandlerAsync(package);
        }

        private static async Task RegisterCommandAsync(AsyncPackage package)
        {
            var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Assumes.Present(commandService);

            var cmdId = new CommandID(PackageGuids.guidCommands, PackageIds.ReOpenDocument);
            var menuItem = new OleMenuCommand((s, e) => Execute(package), cmdId);
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);
        }

        private static void MenuItem_BeforeQueryStatus(object sender, System.EventArgs e)
        {
            var command = (OleMenuCommand)sender;

            command.Enabled = !string.IsNullOrWhiteSpace(_closedFile);

            if (command.Enabled)
            {
                var fileName = Path.GetFileName(_closedFile);
                command.Text = $"Re-Open {fileName}";
            }
            else
            {
                command.Text = "Re-Open Closed File";
            }
        }

        private static async Task RegisterEventHandlerAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            var dte = await package.GetServiceAsync(typeof(DTE)) as DTE2;
            Assumes.Present(dte);

            _events = dte.Events.WindowEvents;
            _events.WindowClosing += _events_WindowClosing;

            ShellEvents.SolutionEvents.OnAfterCloseSolution += (s, e) =>
            {
                TextFileCreationListener.OpenFiles.Clear();
            };
        }

        private static void _events_WindowClosing(Window Window)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (TextFileCreationListener.OpenFiles.TryGetValue(Window.Caption, out var fileName))
            {
                _closedFile = fileName;
                TextFileCreationListener.OpenFiles.Remove(Window.Caption);
            }
        }

        private static void Execute(AsyncPackage package)
        {
            if (!string.IsNullOrWhiteSpace(_closedFile))
            {
                VsShellUtilities.OpenDocument(package, _closedFile);
            }
        }
    }
}
