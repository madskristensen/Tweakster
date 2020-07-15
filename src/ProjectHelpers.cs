using System;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Tweakster
{
    public static class ProjectHelpers
    {
        // Returns ProjectItem, Project, or Solution
        public static object GetSelectedItem()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            object selectedObject = null;
            var monitorSelection = (IVsMonitorSelection)Package.GetGlobalService(typeof(SVsShellMonitorSelection));

            try
            {
                monitorSelection.GetCurrentSelection(out IntPtr hierarchyPointer,
                                                 out var itemId,
                                                 out IVsMultiItemSelect multiItemSelect,
                                                 out IntPtr selectionContainerPointer);


                if (Marshal.GetTypedObjectForIUnknown(hierarchyPointer, typeof(IVsHierarchy)) is IVsHierarchy selectedHierarchy)
                {
                    ErrorHandler.ThrowOnFailure(selectedHierarchy.GetProperty(itemId, (int)__VSHPROPID.VSHPROPID_ExtObject, out selectedObject));
                }

                Marshal.Release(hierarchyPointer);
                Marshal.Release(selectionContainerPointer);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }

            return selectedObject;
        }

        public static string GetFullPath(this Project project)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                return project?.Properties.Item("FullPath")?.Value as string;
            }
            catch (ArgumentException)
            {
                try
                {
                    // MFC projects don't have FullPath, and there seems to be no way to query existence
                    return project?.Properties.Item("ProjectDirectory")?.Value as string;
                }
                catch (ArgumentException)
                {
                    // Installer projects have a ProjectPath.
                    return project?.Properties.Item("ProjectPath")?.Value as string;
                }
            }
        }
    }
}
