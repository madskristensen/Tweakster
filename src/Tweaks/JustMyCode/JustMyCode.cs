using System.ComponentModel.Design;
using Microsoft;
using Microsoft.VisualStudio.Debugger.Interop.Internal;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace Tweakster
{
    internal sealed class JustMyCode
    {
        private static IDebuggerInternal _debugger;

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Assumes.Present(commandService);

            _debugger = await package.GetServiceAsync<IVsDebugger, IDebuggerInternal>();

            var cmdId = new CommandID(PackageGuids.guidCommands, PackageIds.JustMyCode);
            var menuItem = new OleMenuCommand((s, e) => Execute((OleMenuCommand)s), cmdId);
            menuItem.BeforeQueryStatus += (s, e) => OnBeforeQueryStatus((OleMenuCommand)s);
            commandService.AddCommand(menuItem);
        }

        private static void OnBeforeQueryStatus(OleMenuCommand menuItem)
        {
            _debugger.GetDebuggerOption(DEBUGGER_OPTIONS.Option_JustMyCode, out var value);

            menuItem.Checked = value == 1;
            menuItem.Enabled = true; // Enable on package load
        }

        private static void Execute(OleMenuCommand menuItem)
        {
            var enabled = !menuItem.Checked ? (uint)1 : 0;

            _debugger.SetDebuggerOption(DEBUGGER_OPTIONS.Option_JustMyCode, enabled);
        }
    }
}
