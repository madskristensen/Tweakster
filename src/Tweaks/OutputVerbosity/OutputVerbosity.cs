using System;
using System.ComponentModel.Design;
using System.Diagnostics;
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
        private static ISettingsManager _settingsManager;

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Assumes.Present(commandService);

            _settingsManager = await package.GetServiceAsync(typeof(SVsSettingsPersistenceManager)) as ISettingsManager;
            Assumes.Present(_settingsManager);

            var id1 = new CommandID(PackageGuids.guidCommands, PackageIds.OuputVerbosityList);
            var cmd1 = new OleMenuCommand(OnPortDropDownComboList, id1);
            commandService.AddCommand(cmd1);

            var id2 = new CommandID(PackageGuids.guidCommands, PackageIds.OuputVerbosity);
            var cmd2 = new OleMenuCommand(OnPortDropDownCombo, id2);
            commandService.AddCommand(cmd2);
        }

        private static void OnPortDropDownCombo(object sender, EventArgs e)
        {
            if (e is OleMenuCmdEventArgs args)
            {
                if (args.OutValue != IntPtr.Zero)
                {
                    var verbosity = GetVerbosityValue().ToString();
                    Marshal.GetNativeVariantForObject(verbosity, args.OutValue);
                }
                else if (args.InValue is int index)
                {
                    SetVerbosityValue(index);
                }
            }
        }

        private static void OnPortDropDownComboList(object sender, EventArgs e)
        {
            if (e is OleMenuCmdEventArgs args && args.OutValue != IntPtr.Zero)
            {
                Marshal.GetNativeVariantForObject(_names, args.OutValue);
            }
        }

        private static LoggerVerbosity GetVerbosityValue()
        {
            try
            {
                if (_settingsManager.TryGetValue(_settingName, out LoggerVerbosity value) == GetValueResult.Success)
                {
                    return value;
                }
            }
            catch (Exception ex)
            {
                Trace.Write(ex);
            }

            return LoggerVerbosity.Minimal;
        }

        private static void SetVerbosityValue(int value)
        {
            try
            {
                _settingsManager.SetValueAsync(_settingName, value, false)
                    .FileAndForget(nameof(OutputVerbosity));
            }
            catch (Exception ex)
            {
                Trace.Write(ex);
            }
        }
    }
}
