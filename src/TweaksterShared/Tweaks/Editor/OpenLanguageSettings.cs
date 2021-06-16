using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using EnvDTE;
using EnvDTE80;
using Microsoft;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace Tweakster
{
    public class OpenLanguageSettings
    {
        private static readonly Dictionary<string, string> _map = new Dictionary<string, string>() {
            { "Basic", "{f1e1021e-a781-4862-9f4b-88746a288a67}" },
            { "C/C++", "{2F4DC042-B440-42cc-A65F-888316531A02}" },
            { "CSharp", "{8fd0b177-b244-4a97-8e37-6fb7b27de3af}" },
            { "F#", "{9007718c-357a-4327-a193-ab3ec38d7ee8}" },
            { "TypeScript", "{08f3fd83-881a-4990-89c9-57ba96eafded}" },
            { "XAML", "{dac05320-0c3a-4ead-a332-8c23b0cfc130}" },
            { "XML", "{6d677434-2ccb-4e82-b67b-1b4839cdba81}" },
        };

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync<IMenuCommandService, OleMenuCommandService>();
            Assumes.Present(commandService);

            IVsUIShell shell = await package.GetServiceAsync<SVsUIShell, IVsUIShell>();
            Assumes.Present(shell);

            DTE2 dte = await package.GetServiceAsync<DTE, DTE2>();
            Assumes.Present(dte);

            var cmdId = new CommandID(PackageGuids.guidCommands, PackageIds.LanguageSetting);
            var menuItem = new OleMenuCommand((s, e) => Execute(shell, dte), cmdId);
            menuItem.BeforeQueryStatus += (s, e) => OnBeforeQueryStatus(menuItem, dte);

            commandService.AddCommand(menuItem);
        }

        private static void OnBeforeQueryStatus(OleMenuCommand menuItem, DTE2 dte)
        {
            var language = dte.ActiveDocument?.Language;
            menuItem.Visible = menuItem.Enabled = language != null && _map.ContainsKey(language);
        }

        public static void Execute(IVsUIShell shell, DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (_map.TryGetValue(dte.ActiveDocument?.Language, out var guid))
            {
                Guid cmdGuid = VSConstants.GUID_VSStandardCommandSet97;
                object guidObject = guid;
                shell.PostExecCommand(ref cmdGuid, VSConstants.cmdidToolsOptions, 1, ref guidObject);
            }
        }
    }
}
