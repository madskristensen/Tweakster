using System.ComponentModel.Design;
using Microsoft;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell.Settings;
using Task = System.Threading.Tasks.Task;

namespace Tweakster
{
    internal sealed class JustMyCode
    {
        private static IDebuggerInternal _debugger;
        private static ShellSettingsManager _settingsManager;

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Assumes.Present(commandService);

            // No reason to use settings manager if IDebuggerInternal can return the right values in GetDebuggerOption method
            _settingsManager = new ShellSettingsManager(package);

            // This is a dummy interface for the internal COM IDebuggerInternal interface
            _debugger = await package.GetServiceAsync<IVsDebugger, IDebuggerInternal>();

            var cmdId = new CommandID(PackageGuids.guidCommands, PackageIds.JustMyCode);
            var menuItem = new OleMenuCommand((s, e) => Execute((OleMenuCommand)s), cmdId);
            menuItem.BeforeQueryStatus += (s, e) => OnBeforeQueryStatus((OleMenuCommand)s);
            commandService.AddCommand(menuItem);
        }

        private static void OnBeforeQueryStatus(OleMenuCommand menuItem)
        {
            SettingsStore store = _settingsManager.GetReadOnlySettingsStore(SettingsScope.UserSettings);
            menuItem.Checked = store.GetBoolean("Debugger", "JustMyCode", true);
            menuItem.Enabled = true; // Enable on package load
        }

        private static void Execute(OleMenuCommand menuItem)
        {
            var enabled = !menuItem.Checked ? (uint)1 : 0;

            _debugger.SetDebuggerOption(DEBUGGER_OPTIONS.Option_JustMyCode, enabled);

            WritableSettingsStore store = _settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
            store.SetBoolean("Debugger", "JustMyCode", !menuItem.Checked);
        }
    }
}
