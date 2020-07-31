using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using EnvDTE;
using EnvDTE80;
using Microsoft;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace Tweakster
{
    internal sealed class Restart
    {
        private static DTE2 _dte;
        private static DTEEvents _events;
        private static IVsShell4 _shell;
        private static IVsUIShell _uiShell;
        private static bool _openInSafeMode;

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            _shell = await package.GetServiceAsync<SVsShell, IVsShell4>();
            _uiShell = await package.GetServiceAsync<SVsUIShell, IVsUIShell>();
            _dte = await package.GetServiceAsync<DTE, DTE2>();
            _events = _dte.Events.DTEEvents;
            _events.OnBeginShutdown += OnBeginShutdown;

            IMenuCommandService commandService = await package.GetServiceAsync<IMenuCommandService, IMenuCommandService>();
            Assumes.Present(commandService);

            var cmdNormalId = new CommandID(PackageGuids.guidCommands, PackageIds.RestartNormal);
            var cmdNormal = new OleMenuCommand((s, e) => Execute(RestartType.Normal), cmdNormalId);
            commandService.AddCommand(cmdNormal);

            var cmdElevatedId = new CommandID(PackageGuids.guidCommands, PackageIds.RestartElevated);
            var cmdElevated = new OleMenuCommand((s, e) => Execute(RestartType.Elevated), cmdElevatedId);
            commandService.AddCommand(cmdElevated);

            var cmdSafemodeId = new CommandID(PackageGuids.guidCommands, PackageIds.RestartSafemode);
            var cmdSafemode = new OleMenuCommand((s, e) => Execute(RestartType.Safemode), cmdSafemodeId);
            commandService.AddCommand(cmdSafemode);
        }

        private static void Execute(RestartType restartType)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _openInSafeMode = false;

            switch (restartType)
            {
                case RestartType.Normal:
                    ((IVsShell3)_shell).IsRunningElevated(out var elevated);
                    __VSRESTARTTYPE type = elevated ? __VSRESTARTTYPE.RESTART_Elevated : __VSRESTARTTYPE.RESTART_Normal;
                    _shell.Restart((uint)type);
                    break;

                case RestartType.Elevated:
                    _shell.Restart((uint)__VSRESTARTTYPE.RESTART_Elevated);
                    break;

                case RestartType.Safemode:
                    _openInSafeMode = true;
                    Guid guid = typeof(VSConstants.VSStd97CmdID).GUID;
                    var id = (uint)VSConstants.VSStd97CmdID.Exit;
                    _uiShell.PostExecCommand(guid, id, 0, null);
                    break;
            }
        }

        private static void OnBeginShutdown()
        {
            if (_openInSafeMode)
            {
                var devenv = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                var args = _dte.CommandLineArguments;

                if (args.IndexOf(" /safemode", StringComparison.OrdinalIgnoreCase) == -1)
                {
                    args += " /safemode";
                }

                var start = new ProcessStartInfo
                {
                    FileName = devenv,
                    UseShellExecute = true,
                    ErrorDialog = true,
                    Arguments = args
                };

                System.Diagnostics.Process.Start(start);
            }
        }

        private enum RestartType
        {
            Normal,
            Elevated,
            Safemode
        }
    }
}
