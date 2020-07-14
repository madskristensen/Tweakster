using System;
using Microsoft;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace Tweakster
{
    public class NoStepDebuggingInDesignMode : IOleCommandTarget
    {
        private readonly System.IServiceProvider _serviceProvider;

        private NoStepDebuggingInDesignMode(System.IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var pct = await package.GetServiceAsync(typeof(SVsRegisterPriorityCommandTarget)) as IVsRegisterPriorityCommandTarget;
            Assumes.Present(pct);

            var interceptor = new NoStepDebuggingInDesignMode(package);
            pct.RegisterPriorityCommandTarget(0, interceptor, out _);
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            if (pguidCmdGroup == typeof(VSConstants.VSStd97CmdID).GUID)
            {
                for (var i = 0; i < cCmds; i++)
                {
                    switch (prgCmds[i].cmdID)
                    {
                        case (uint)VSConstants.VSStd97CmdID.StepOver:
                        case (uint)VSConstants.VSStd97CmdID.StepInto:
                            if (InterceptCommand())
                            {
                                return VSConstants.S_FALSE;
                            }

                            break;
                    }
                }
            }

            return (int)Microsoft.VisualStudio.OLE.Interop.Constants.MSOCMDEXECOPT_DODEFAULT;
        }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            if (pguidCmdGroup == typeof(VSConstants.VSStd97CmdID).GUID)
            {
                switch (nCmdID)
                {
                    case (uint)VSConstants.VSStd97CmdID.StepOver:
                    case (uint)VSConstants.VSStd97CmdID.StepInto:
                        if (InterceptCommand())
                        {
                            return VSConstants.S_FALSE;
                        }

                        break;
                }
            }

            return (int)Microsoft.VisualStudio.OLE.Interop.Constants.MSOCMDERR_E_FIRST;
        }

        private bool InterceptCommand()
        {
            return !Options.Instance.DebugOnF10F11 &&
                   VsShellUtilities.GetDebugMode(_serviceProvider) == DBGMODE.DBGMODE_Design;
        }
    }
}
