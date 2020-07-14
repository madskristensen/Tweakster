using System.ComponentModel.Design;
using Microsoft;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace Tweakster
{
    internal sealed class Restart
    {
        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Assumes.Present(commandService);

            var shell = await package.GetServiceAsync(typeof(SVsShell)) as IVsShell3;
            Assumes.Present(shell);

            var cmdId = new CommandID(PackageGuids.guidCommands, PackageIds.Restart);
            var menuItem = new OleMenuCommand((s, e) => Execute(shell), cmdId);
            commandService.AddCommand(menuItem);
        }

        private static void Execute(IVsShell3 shell3)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var shell4 = (IVsShell4)shell3;

            ErrorHandler.ThrowOnFailure(shell3.IsRunningElevated(out var elevated));
            __VSRESTARTTYPE type = elevated ? __VSRESTARTTYPE.RESTART_Elevated : __VSRESTARTTYPE.RESTART_Normal;

            shell4.Restart((uint)type);
        }
    }
}
