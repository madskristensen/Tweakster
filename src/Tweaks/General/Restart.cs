using System.ComponentModel.Design;
using Microsoft;
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

            IVsShell4 shell = await package.GetServiceAsync<SVsShell, IVsShell4>();
            Assumes.Present(shell);

            IMenuCommandService commandService = await package.GetServiceAsync<IMenuCommandService, IMenuCommandService>();
            Assumes.Present(commandService);

            var cmdNormalId = new CommandID(PackageGuids.guidCommands, PackageIds.RestartNormal);
            var cmdNormal = new OleMenuCommand((s, e) => Execute(false, shell), cmdNormalId);
            commandService.AddCommand(cmdNormal);

            var cmdElevatedId = new CommandID(PackageGuids.guidCommands, PackageIds.RestartElevated);
            var cmdElevated = new OleMenuCommand((s, e) => Execute(true, shell), cmdElevatedId);
            commandService.AddCommand(cmdElevated);
        }

        private static void Execute(bool forceElevated, IVsShell4 shell)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (forceElevated)
            {
                shell.Restart((uint)__VSRESTARTTYPE.RESTART_Elevated);
            }
            else
            {
                ((IVsShell3)shell).IsRunningElevated(out var elevated);
                __VSRESTARTTYPE type = elevated ? __VSRESTARTTYPE.RESTART_Elevated : __VSRESTARTTYPE.RESTART_Normal;
                shell.Restart((uint)type);
            }
        }
    }
}
