using System;
using EnvDTE;
using EnvDTE80;
using Microsoft;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace Tweakster
{
    public class BuildOrdered
    {
        private static BuildEvents _buildEvents;

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            IVsOutputWindow outWindow = await package.GetServiceAsync<SVsOutputWindow, IVsOutputWindow>();
            Assumes.Present(outWindow);

            outWindow.GetPane(VSConstants.OutputWindowPaneGuid.SortedBuildOutputPane_guid, out IVsOutputWindowPane buildPane);

            DTE2 dte = await package.GetServiceAsync<DTE, DTE2>();
            Assumes.Present(dte);

            _buildEvents = dte.Events.BuildEvents;
            _buildEvents.OnBuildDone += (s, a) => OnBuildDone(a, buildPane);
        }

        private static void OnBuildDone(vsBuildAction action, IVsOutputWindowPane buildPane)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (Options.Instance.ShowBuildOrder)
            {
                buildPane.Activate();
            }
        }
    }
}
