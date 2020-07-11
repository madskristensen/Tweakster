using EnvDTE;
using EnvDTE80;
using Microsoft;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace Tweakster.Tweaks.AutoSave
{
    public class AutoSave
    {
        private static WindowEvents _windowEvents;
        private static ProjectItemsEvents _projectsEvents;

        public static async Task RegisterAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var dte = await package.GetServiceAsync(typeof(DTE)) as DTE2;
            Assumes.Present(dte);

            _windowEvents = dte.Events.WindowEvents;
            _windowEvents.WindowActivated += OnWindowActivated;

            _projectsEvents = (dte.Events as Events2).ProjectItemsEvents;
            _projectsEvents.ItemAdded += (p) => OnProjectItemChange(p);
            _projectsEvents.ItemRemoved += (p) => OnProjectItemChange(p);
            _projectsEvents.ItemRenamed += (p, n) => OnProjectItemChange(p);

        }


        private static void OnProjectItemChange(ProjectItem item)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (item?.ContainingProject?.IsDirty == true)
            {
                item.ContainingProject.Save();
            }
        }

        private static void OnWindowActivated(Window gotFocus, Window lostFocus)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            lostFocus?.Document?.Save();

            // Test IsDirty to filter out temp and misc project types
            if (lostFocus?.Project?.IsDirty == true)
            {
                lostFocus.Project.Save();
            }
        }
    }
}
