using System.Diagnostics;
using EnvDTE;
using EnvDTE80;
using Microsoft;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace Tweakster
{
    public class AutoSave
    {
        private static DTE2 _dte;
        private static WindowEvents _windowEvents;
        private static ProjectItemsEvents _projectsEvents;
        private static BuildEvents _buildEvents;

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            _dte = await package.GetServiceAsync(typeof(DTE)) as DTE2;
            Assumes.Present(_dte);

            _buildEvents = _dte.Events.BuildEvents;
            _buildEvents.OnBuildBegin += OnBuildBegin;

            _windowEvents = _dte.Events.WindowEvents;
            _windowEvents.WindowActivated += OnWindowActivated;

            _projectsEvents = (_dte.Events as Events2).ProjectItemsEvents;
            _projectsEvents.ItemAdded += (p) => OnProjectItemChange(p);
            _projectsEvents.ItemRemoved += (p) => OnProjectItemChange(p);
            _projectsEvents.ItemRenamed += (p, n) => OnProjectItemChange(p);


        }

        private static void OnBuildBegin(vsBuildScope Scope, vsBuildAction Action)
        {
            if (Options.Instance.SaveAllOnBuild)
            {
                _dte.ExecuteCommand("File.SaveAll");
            }
        }

        private static void OnProjectItemChange(ProjectItem item)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (ShouldExecute(item))
            {
                try
                {
                    item.ContainingProject.Save();
                }
                catch (System.Exception ex)
                {
                    Trace.Write(ex);
                }
            }
        }

        private static void OnWindowActivated(Window gotFocus, Window lostFocus)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (ShouldExecute(lostFocus?.Document?.ProjectItem))
            {
                if (!lostFocus.Document.Saved)
                {
                    lostFocus.Document.Save();
                }

                // Test IsDirty to filter out temp and misc project types
                if (lostFocus.Project?.IsDirty == true)
                {
                    lostFocus.Project.Save();
                }
            }
        }

        private static bool ShouldExecute(ProjectItem item)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            return Options.Instance.AutoSave &&
                   _dte.Mode == vsIDEMode.vsIDEModeDesign &&
                   item?.ContainingProject != null &&
                   item?.ContainingProject?.Kind != ProjectKinds.vsProjectKindSolutionFolder;
        }
    }
}
