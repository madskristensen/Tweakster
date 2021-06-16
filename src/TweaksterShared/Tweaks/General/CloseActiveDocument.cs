using System;
using Microsoft;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace Tweakster
{
    internal sealed class CloseActiveDocument : IOleCommandTarget
    {
        private static IVsUIShell _uiShell;

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            _uiShell = await package.GetServiceAsync<SVsUIShell, IVsUIShell>();
            Assumes.Present(_uiShell);

            var pct = await package.GetServiceAsync(typeof(SVsRegisterPriorityCommandTarget)) as IVsRegisterPriorityCommandTarget;
            Assumes.Present(pct);

            var interceptor = new CloseActiveDocument();
            pct.RegisterPriorityCommandTarget(0, interceptor, out _);
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            return (int)Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED;
        }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (pguidCmdGroup == typeof(VSConstants.VSStd2KCmdID).GUID)
            {
                switch (nCmdID)
                {
                    case (uint)VSConstants.VSStd2KCmdID.CAPTUREKEYSTROKE:
                    case (uint)VSConstants.VSStd2KCmdID.SELECTCURRENTWORD:
                        if (Options.Instance.CloseTabOnControlW)
                        {
                            Guid guid = typeof(VSConstants.VSStd97CmdID).GUID;
                            var id = (uint)VSConstants.VSStd97CmdID.FileClose;
                            _uiShell.PostExecCommand(guid, id, 0, null);
                        }

                        break;
                }
            }

            return (int)Microsoft.VisualStudio.OLE.Interop.Constants.MSOCMDERR_E_FIRST;
        }
    }
}
