using System;
using System.ComponentModel.Design;
using Microsoft;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using static Microsoft.VisualStudio.VSConstants;
using Task = System.Threading.Tasks.Task;

namespace Tweakster.Tweaks.General
{
    public class ClearRecentFilesAndProjects
    {
        private const string _registryKey = @"ApplicationPrivateSettings\_metadata\baselines\CodeContainers";

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            IVsMRUItemsStore store = await package.GetServiceAsync<SVsMRUItemsStore, IVsMRUItemsStore>();
            IVsUIShell uiShell = await package.GetServiceAsync<SVsUIShell, IVsUIShell>();
            IMenuCommandService commandService = await package.GetServiceAsync<IMenuCommandService, IMenuCommandService>();
            Assumes.Present(commandService);

            var cmdFilesId = new CommandID(PackageGuids.guidCommands, PackageIds.ClearRecentFiles);
            var cmdFiles = new OleMenuCommand((s, e) => DeleteRecentFiles(store, uiShell), cmdFilesId);
            commandService.AddCommand(cmdFiles);

            //var cmdProjectsId = new CommandID(PackageGuids.guidCommands, PackageIds.ClearRecentProjects);
            //var cmdProjects = new OleMenuCommand((s, e) => DeleteRecentProjects(store, package, uiShell), cmdProjectsId);
            //commandService.AddCommand(cmdProjects);
        }

        private static void DeleteRecentFiles(IVsMRUItemsStore store, IVsUIShell uiShell)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (UserConfirmation("Are you sure you with to clear the list of recently opened files?", uiShell))
            {
                Guid guid = MruList.Files;
                store.DeleteMRUItems(ref guid);
            }
        }

        private static void DeleteRecentProjects(IVsMRUItemsStore store, AsyncPackage package, IVsUIShell uiShell)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (UserConfirmation("Are you sure you with to clear the list of recently opened solutions and projects?", uiShell))
            {
                // MRU for projects is stored somewhere else. Find out where.
                Guid guid = MruList.Projects;
                store.DeleteMRUItems(ref guid);
            }
        }

        private static bool UserConfirmation(string msg, IVsUIShell uiShell)
        {
            return VsShellUtilities.PromptYesNo(
                msg + Environment.NewLine + Environment.NewLine + "Restart required for changes to take effect.",
                "Do you wish to continue?",
                OLEMSGICON.OLEMSGICON_QUERY,
                uiShell);
        }
    }
}
