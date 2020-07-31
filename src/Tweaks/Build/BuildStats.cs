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
    public class BuildStats
    {
        private static BuildEvents _buildEvents;
        private static DateTime _startTime;

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            IVsOutputWindow outWindow = await package.GetServiceAsync<SVsOutputWindow, IVsOutputWindow>();
            Assumes.Present(outWindow);

            outWindow.GetPane(VSConstants.GUID_BuildOutputWindowPane, out IVsOutputWindowPane buildPane);

            DTE2 dte = await package.GetServiceAsync<DTE, DTE2>();
            Assumes.Present(dte);

            _buildEvents = dte.Events.BuildEvents;
            _buildEvents.OnBuildBegin += (s, a) => _startTime = DateTime.Now;
            _buildEvents.OnBuildDone += (s, a) => OnBuildDone(a, buildPane);
        }

        private static void OnBuildDone(vsBuildAction action, IVsOutputWindowPane buildPane)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (Options.Instance.ShowBuildStats)
            {
                TimeSpan time = DateTime.Now - _startTime;

                var format = "> {0} started at {1} and took {2}";
                var msg = string.Format(format, ActionName(action), _startTime.ToShortTimeString(), TimeFormat(time));

                buildPane.OutputStringThreadSafe(Environment.NewLine + msg);
            }
        }

        private static string ActionName(vsBuildAction action)
        {
            switch (action)
            {
                case vsBuildAction.vsBuildActionBuild:
                    return "Build";
                case vsBuildAction.vsBuildActionRebuildAll:
                    return "Rebuild";
                case vsBuildAction.vsBuildActionClean:
                    return "Clean";
                case vsBuildAction.vsBuildActionDeploy:
                    return "Deploy";
            }

            return "Build";
        }

        private static string TimeFormat(TimeSpan time)
        {
            if (time.Minutes == 0)
            {
                return $"{time:s\\.fff} seconds";
            }
            if (time.Hours == 0)
            {
                return $"{time:mm\\:ss\\.fff} minutes";
            }

            return string.Format("{0:hh\\:m\\:ss\\.fff}", time);
        }
    }
}
