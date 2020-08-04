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

            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control &&
                Options.Instance.CloseTabOnControlW &&
                e.Key == Key.W)
            {
                e.Handled = true;
                Guid guid = typeof(VSConstants.VSStd97CmdID).GUID;
                var id = (uint)VSConstants.VSStd97CmdID.FileClose;
                _uiShell.PostExecCommand(guid, id, 0, null);
            }
        }
    }
}
