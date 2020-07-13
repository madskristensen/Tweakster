using EnvDTE;
using EnvDTE80;
using Microsoft;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace Tweakster.Tweaks.AutoSave
{
    public class AutoSave
    {
        private static DTE2 _dte;
        private static WindowEvents _windowEvents;
        private static ProjectItemsEvents _projectsEvents;

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            _dte = await package.GetServiceAsync(typeof(DTE)) as DTE2;
            Assumes.Present(_dte);

            _windowEvents = _dte.Events.WindowEvents;
            _windowEvents.WindowActivated += OnWindowActivated;

            _projectsEvents = (_dte.Events as Events2).ProjectItemsEvents;
            _projectsEvents.ItemAdded += (p) => OnProjectItemChange(p);
            _projectsEvents.ItemRemoved += (p) => OnProjectItemChange(p);
            _projectsEvents.ItemRenamed += (p, n) => OnProjectItemChange(p);
        }


        private static void OnProjectItemChange(ProjectItem item)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (ShouldExecute() && item?.ContainingProject?.IsDirty == true)
            {
                item.ContainingProject.Save();
            }
        }

        private static void OnWindowActivated(Window gotFocus, Window lostFocus)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (ShouldExecute())
            {
                lostFocus?.Document?.Save();

                // Test IsDirty to filter out temp and misc project types
                if (lostFocus?.Project?.IsDirty == true)
                {
                    lostFocus.Project.Save();
                }
            }
        }

        private static bool ShouldExecute()
        {
            return Options.Instance.AutoSave &&
                   _dte.Mode == vsIDEMode.vsIDEModeDesign;
        }
    }
}
