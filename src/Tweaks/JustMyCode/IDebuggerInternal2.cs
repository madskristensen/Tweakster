using System.Runtime.InteropServices;

namespace Tweakster
{
    [Guid("1DA40549-8CCC-48CF-B99B-FC22FE3AFEDF")]
    public interface IDebuggerInternal2
    {
        int GetDebuggerOption(DEBUGGER_OPTIONS2 option, out uint value);
        int SetDebuggerOption(DEBUGGER_OPTIONS2 option, uint value);
    }

    public enum DEBUGGER_OPTIONS2
    {
        Option_ConfirmDeleteAllBreakpoints,
        Option_StopAllProcesses,
        Option_StopOnExceptionCrossingManagedBoundary,
        Option_EnableAddressLevelDebugging,
        Option_ShowDisassemblyWhenNoSource,
        Option_EnableBreakpointConstraints,
        Option_UseExceptionHelper,
        Option_AutoUnwindOnException,
        Option_JustMyCode,
    }
}
