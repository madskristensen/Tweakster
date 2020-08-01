using System;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace Tweakster
{
    public static class ExtensionMethods
    {
        public static string GetFullPath(this Project project)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var properties = new[] { "FullPath", "ProjectDirectory", "ProjectPath" };

            foreach (var name in properties)
            {
                try
                {
                    if (project?.Properties.Item(name)?.Value is string fullPath)
                    {
                        return fullPath;
                    }
                }
                catch (Exception)
                { }
            }

            return null;
        }
    }
}