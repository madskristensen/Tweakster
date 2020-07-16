using System;
using System.ComponentModel.Design;
using System.Diagnostics;
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
        private static readonly string[] _names = Enum.GetNames(typeof(LoggerVerbosity));
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
                using (RegistryKey key = _package.UserRegistryRoot.OpenSubKey("ApplicationPrivateSettings\\BuildAndRunOptions"))
                {
                    var value = key.GetValue("MSBuildLoggerVerbosity", (int)LoggerVerbosity.Minimal) as string;

                    if (!string.IsNullOrEmpty(value) && int.TryParse(value.Substring(value.Length - 1), out var number))
                    {
                        return (LoggerVerbosity)number;
                    }
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
                using (RegistryKey key = _package.UserRegistryRoot.OpenSubKey("ApplicationPrivateSettings\\BuildAndRunOptions", true))
                {
                    key.SetValue("MSBuildLoggerVerbosity", $"0*System.UInt32*{value}", RegistryValueKind.String);
                }
            }
            catch (Exception ex)
            {
                Trace.Write(ex);
            }
        }
    }
}
