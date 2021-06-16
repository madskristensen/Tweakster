using System;
using System.ComponentModel.Design;
using Microsoft;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using static Microsoft.VisualStudio.VSConstants;
using Task = System.Threading.Tasks.Task;

namespace Tweakster
{
    public class ClearRecentFilesAndProjects
    {
        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            IVsMRUItemsStore store = await package.GetServiceAsync<SVsMRUItemsStore, IVsMRUItemsStore>();
            IVsUIShell uiShell = await package.GetServiceAsync<SVsUIShell, IVsUIShell>();
            ISettingsManager manager = await package.GetServiceAsync<SVsSettingsPersistenceManager, ISettingsManager>();
            IMenuCommandService commandService = await package.GetServiceAsync<IMenuCommandService, IMenuCommandService>();
            Assumes.Present(commandService);

            var cmdFilesId = new CommandID(PackageGuids.guidCommands, PackageIds.ClearRecentFiles);
            var cmdFiles = new OleMenuCommand((s, e) => DeleteRecentFiles(store, uiShell), cmdFilesId);
            commandService.AddCommand(cmdFiles);

            var cmdProjectsId = new CommandID(PackageGuids.guidCommands, PackageIds.ClearRecentProjects);
            var cmdProjects = new OleMenuCommand((s, e) => DeleteRecentProjects(manager, uiShell), cmdProjectsId);
            commandService.AddCommand(cmdProjects);
        }

        private static void DeleteRecentFiles(IVsMRUItemsStore store, IVsUIShell uiShell)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (UserConfirmation("Are you sure you with to clear the list of recently opened files?", uiShell, true))
            {
                Guid guid = MruList.Files;
                store.DeleteMRUItems(ref guid);
            }
        }

        private static void DeleteRecentProjects(ISettingsManager manager, IVsUIShell uiShell)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (UserConfirmation("Are you sure you with to clear the list of recently opened solutions and projects?", uiShell))
            {
                ISettingsList list = manager.GetOrCreateList("CodeContainers.Offline", true);
                list.ClearAsync().FireAndForget();
            }
        }

        private static bool UserConfirmation(string msg, IVsUIShell uiShell, bool restartRequired = false)
        {
            if (restartRequired)
            {
                msg += Environment.NewLine + Environment.NewLine + "Restart required for changes to take effect.";
            }

            return VsShellUtilities.PromptYesNo(
                msg,
                "Do you wish to continue?",
                OLEMSGICON.OLEMSGICON_QUERY,
                uiShell);
        }
    }
}
