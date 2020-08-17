using System;
using System.Windows;
using System.Windows.Input;
using Microsoft;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace Tweakster
{
    internal sealed class CloseActiveDocument
    {
        private static IVsUIShell _uiShell;

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            _uiShell = await package.GetServiceAsync<SVsUIShell, IVsUIShell>();
            Assumes.Present(_uiShell);

            Application.Current.MainWindow.KeyUp += OnKeyUp;
        }

        private static void OnKeyUp(object sender, KeyEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (!Options.Instance.CloseTabOnControlW || e.Key != Key.W)
            {
                return;
            }

            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                Guid guid = typeof(VSConstants.VSStd97CmdID).GUID;
                var id = (uint)VSConstants.VSStd97CmdID.FileClose;
                _uiShell.PostExecCommand(guid, id, 0, null);
            }
            else if (e.KeyboardDevice.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
            {
                Guid guid = typeof(VSConstants.VSStd97CmdID).GUID;
                var id = (uint)VSConstants.VSStd97CmdID.CloseAllDocuments;
                _uiShell.PostExecCommand(guid, id, 0, null);
            }
        }
    }
}
