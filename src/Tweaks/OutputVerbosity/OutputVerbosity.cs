using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using Microsoft;
using Microsoft.Build.Framework;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace Tweakster
{
    [Guid("9B164E40-C3A2-4363-9BC5-EB4039DEF653")]
    public class SVsSettingsPersistenceManager
    {
        // Dummy class to get to the internal SVsSettingsPersistenceManager service
    }

    internal sealed class OutputVerbosity
    {
        private const string _settingName = "BuildAndRunOptions.MSBuildLoggerVerbosity";
        private static readonly string[] _names = Enum.GetNames(typeof(LoggerVerbosity));
        private static ISettingsManager _settings;

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Assumes.Present(commandService);

            _settings = await package.GetServiceAsync(typeof(SVsSettingsPersistenceManager)) as ISettingsManager;
            Assumes.Present(_settings);

            var id1 = new CommandID(PackageGuids.guidCommands, PackageIds.OuputVerbosityList);
            var cmd1 = new OleMenuCommand(FillDropdownWithValues, id1);
            commandService.AddCommand(cmd1);

            var id2 = new CommandID(PackageGuids.guidCommands, PackageIds.OuputVerbosity);
            var cmd2 = new OleMenuCommand(SetSelectedValue, id2);
            commandService.AddCommand(cmd2);
        }

        private static void FillDropdownWithValues(object sender, EventArgs e)
        {
            if (e is OleMenuCmdEventArgs args && args.OutValue != IntPtr.Zero)
            {
                Marshal.GetNativeVariantForObject(_names, args.OutValue);
            }
        }

        private static void SetSelectedValue(object sender, EventArgs e)
        {
            if (e is OleMenuCmdEventArgs args)
            {
                if (args.OutValue != IntPtr.Zero)
                {
                    // Set the initial value on load
                    LoggerVerbosity verbosity = _settings.GetValueOrDefault(_settingName, LoggerVerbosity.Minimal);
                    Marshal.GetNativeVariantForObject(verbosity.ToString(), args.OutValue);
                }
                else if (args.InValue is int index)
                {
                    // Save the value on manual selection from dropdown
                    _settings.SetValueAsync(_settingName, index, false).FileAndForget(nameof(OutputVerbosity));
                }
            }
        }
    }
}