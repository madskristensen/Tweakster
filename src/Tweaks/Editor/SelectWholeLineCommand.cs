using System;
using System.ComponentModel.Design;
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
    public class SelectWholeLineCommand
    {
        private static AsyncPackage _package;

        public static async Task InitializeAsync(AsyncPackage package)
        {
            _package = package;
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Assumes.Present(commandService);

            var cmdId = new CommandID(PackageGuids.guidCommands, PackageIds.SelectWholeLine);
            var cmd = new OleMenuCommand(Execute, cmdId);
            commandService.AddCommand(cmd);
        }

        private static void Execute(object sender, EventArgs e)
        {
            var view = GetTextView();

            if (view == null)
            {
                return;
            }

            var position = view.Selection.Start.Position;
            var line = view.GetTextViewLineContainingBufferPosition(position);
            var span = line.Extent;

            view.Selection.Select(span, false);
        }

        public static IWpfTextView GetTextView()
        {
            IComponentModel compService = _package.GetService<SComponentModel, IComponentModel>();
            Assumes.Present(compService);

            IVsEditorAdaptersFactoryService editorAdapter = compService.GetService<IVsEditorAdaptersFactoryService>();
            return editorAdapter.GetWpfTextView(GetCurrentNativeTextView());
        }

        public static IVsTextView GetCurrentNativeTextView()
        {
            IVsTextManager textManager = _package.GetService<SVsTextManager, IVsTextManager>();
            Assumes.Present(textManager);

            ErrorHandler.ThrowOnFailure(textManager.GetActiveView(1, null, out IVsTextView activeView));
            return activeView;
        }
    }
}