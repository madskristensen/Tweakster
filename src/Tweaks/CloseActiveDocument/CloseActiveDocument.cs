using System;
using System.ComponentModel.Design;
using EnvDTE;
using EnvDTE80;
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
        private readonly DTE2 _dte;

        private static Guid _cmdGuid = new Guid("{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}");
        private static readonly uint _cmdSelectWordId = 0x5a;
        private static readonly uint _cmdNextKeyId = 0x4d1;

        private CloseActiveDocument(DTE2 dte)
        {
            _dte = dte;
        }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Assumes.Present(commandService);

            var dte = await package.GetServiceAsync(typeof(DTE)) as DTE2;
            var pct = await package.GetServiceAsync(typeof(SVsRegisterPriorityCommandTarget)) as IVsRegisterPriorityCommandTarget;
            Assumes.Present(pct);

            var interceptor = new CloseActiveDocument(dte);
            pct.RegisterPriorityCommandTarget(0, interceptor, out _);
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            return (int)Microsoft.VisualStudio.OLE.Interop.Constants.MSOCMDEXECOPT_DODEFAULT;
        }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (pguidCmdGroup == _cmdGuid && (nCmdID == _cmdSelectWordId || nCmdID == _cmdNextKeyId))
            {
                if (IsDocumentWindowActive())
                {
                    Command cmd = _dte.Commands.Item("File.Close");

                    if (cmd != null && cmd.IsAvailable)
                    {
                        _dte.ExecuteCommand("File.Close");
                        return VSConstants.S_FALSE;
                    }
                }
            }

            return (int)Microsoft.VisualStudio.OLE.Interop.Constants.MSOCMDERR_E_FIRST;
        }

        private bool IsDocumentWindowActive()
        {
            return Options.Instance.CloseTabOnControlW &&
                    (
                        _dte.ActiveWindow?.Type == vsWindowType.vsWindowTypeCodeWindow ||
                        _dte.ActiveWindow?.Type == vsWindowType.vsWindowTypeDesigner ||
                        _dte.ActiveWindow?.Type == vsWindowType.vsWindowTypeDocument
                    );
        }
    }
}
