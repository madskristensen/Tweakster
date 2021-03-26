using System;
using Community.VisualStudio.Toolkit;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Task = System.Threading.Tasks.Task;

namespace Tweakster
{
    internal sealed class ResetZoomLevel : BaseCommand<ResetZoomLevel>
    {
        public ResetZoomLevel() : base(PackageGuids.guidCommands, PackageIds.ResetZoom)
        { }

        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            DTE2 dte = await VS.GetDTEAsync();

            if (dte?.ActiveDocument == null)
            {
                return;
            }

            try
            {
                IWpfTextView view = await VS.Editor.GetCurrentWpfTextViewAsync();

                if (view != null)
                {
                    ResetZoom(dte, view);
                }
            }
            catch (Exception ex)
            {
                await ex.LogAsync();
            }
        }

        private static void ResetZoom(DTE2 dte, IWpfTextView view)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            view.ZoomLevel = Options.Instance.DefaultZoomLevel;
            dte.ExecuteCommand("View.ZoomOut");
            dte.ExecuteCommand("View.ZoomIn");
        }
    }
}
