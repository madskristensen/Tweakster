using System.Runtime.InteropServices;

namespace Tweakster
{
    // This is a dummy interface for the internal COM IDebuggerInternal interface
    [Guid("1DA40549-8CCC-48CF-B99B-FC22FE3AFEDF")]
    public interface IDebuggerInternal
    {
        int GetDebuggerOption(DEBUGGER_OPTIONS option, out uint value);
        int SetDebuggerOption(DEBUGGER_OPTIONS option, uint value);
    }

    public enum DEBUGGER_OPTIONS
    {
        Option_JustMyCode = 8,
    }
}
