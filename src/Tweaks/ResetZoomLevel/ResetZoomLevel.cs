using System;
using System.ComponentModel.Design;
using EnvDTE;
using Microsoft;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Task = System.Threading.Tasks.Task;

namespace Tweakster
{
    internal sealed class ResetZoomLevel
    {
        private readonly AsyncPackage _package;

        private ResetZoomLevel(AsyncPackage package, OleMenuCommandService commandService)
        {
            _package = package;

            var menuCommandID = new CommandID(PackageGuids.guidCommands, PackageIds.ResetZoom);
            var menuItem = new MenuCommand(Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        public static ResetZoomLevel Instance
        {
            get;
            private set;
        }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new ResetZoomLevel(package, commandService);
        }

        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            DTE dte = _package.GetService<DTE, DTE>();

            if (dte?.ActiveDocument == null)
            {
                return;
            }

            try
            {
                IWpfTextView view = GetCurrentTextView();

                if (view != null)
                {
                    ResetZoom(dte, view);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }
        }

        private static void ResetZoom(_DTE dte, IWpfTextView view)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            view.ZoomLevel = Options.Instance.DefaultZoomLevel;
            dte.ExecuteCommand("View.ZoomOut");
            dte.ExecuteCommand("View.ZoomIn");
        }

        public IWpfTextView GetCurrentTextView()
        {
            return GetTextView();
        }

        public IWpfTextView GetTextView()
        {
            IComponentModel compService = _package.GetService<SComponentModel, IComponentModel>();
            Assumes.Present(compService);

            IVsEditorAdaptersFactoryService editorAdapter = compService.GetService<IVsEditorAdaptersFactoryService>();
            return editorAdapter.GetWpfTextView(GetCurrentNativeTextView());
        }

        public IVsTextView GetCurrentNativeTextView()
        {
            IVsTextManager textManager = _package.GetService<SVsTextManager, IVsTextManager>();
            Assumes.Present(textManager);

            ErrorHandler.ThrowOnFailure(textManager.GetActiveView(1, null, out IVsTextView activeView));
            return activeView;
        }
    }
}
