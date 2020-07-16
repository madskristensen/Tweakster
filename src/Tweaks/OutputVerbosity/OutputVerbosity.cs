using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using Microsoft;
using Microsoft.Build.Framework;
using Microsoft.VisualStudio.Shell;
using Microsoft.Win32;
using Task = System.Threading.Tasks.Task;

namespace Tweakster
{
    internal sealed class OutputVerbosity
    {
        private static AsyncPackage _package;
        public static async Task InitializeAsync(AsyncPackage package)
        {
            _package = package;

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Assumes.Present(commandService);

            var id1 = new CommandID(PackageGuids.guidCommands, PackageIds.OuputVerbosityList);
            var cmd1 = new OleMenuCommand(OnPortDropDownComboList, id1);
            commandService.AddCommand(cmd1);

            var id2 = new CommandID(PackageGuids.guidCommands, PackageIds.OuputVerbosity);
            var cmd2 = new OleMenuCommand(OnPortDropDownCombo, id2);
            commandService.AddCommand(cmd2);
        }

        private static void OnPortDropDownCombo(object sender, EventArgs e)
        {
            if (e is OleMenuCmdEventArgs eventArgs)
            {
                var inParam = eventArgs.InValue;
                IntPtr vOut = eventArgs.OutValue;

                if (vOut != IntPtr.Zero)
                {
                    var verbosity = GetVerbosityValue().ToString();
                    Marshal.GetNativeVariantForObject(verbosity, vOut);
                }
                else if (inParam is int index)
                {
                    SetVerbosityValue(index);
                }
            }
        }

        private static LoggerVerbosity GetVerbosityValue()
        {
            using (RegistryKey key = _package.UserRegistryRoot.OpenSubKey("ApplicationPrivateSettings\\BuildAndRunOptions"))
            {
                var value = key.GetValue("MSBuildLoggerVerbosity", (int)LoggerVerbosity.Minimal) as string;

                if (!string.IsNullOrEmpty(value) && int.TryParse(value.Substring(value.Length - 1), out var number))
                {
                    return (LoggerVerbosity)number;
                }
            }

            return LoggerVerbosity.Minimal;
        }

        private static void SetVerbosityValue(int value)
        {
            using (RegistryKey key = _package.UserRegistryRoot.OpenSubKey("ApplicationPrivateSettings\\BuildAndRunOptions", true))
            {
                key.SetValue("MSBuildLoggerVerbosity", $"0*System.UInt32*{value}", RegistryValueKind.String);
            }
        }

        private static void OnPortDropDownComboList(object sender, EventArgs e)
        {
            var names = Enum.GetNames(typeof(LoggerVerbosity));

            if (e is OleMenuCmdEventArgs eventArgs)
            {
                var inParam = eventArgs.InValue;
                IntPtr vOut = eventArgs.OutValue;

                if (vOut != IntPtr.Zero)
                {
                    Marshal.GetNativeVariantForObject(names, vOut);
                }
            }
        }


    }
}
