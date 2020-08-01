using System;
using System.Runtime.InteropServices;

namespace Tweakster
{
    [ComImport]
    [Guid("9B164E40-C3A2-4363-9BC5-EB4039DEF653")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface SVsSettingsPersistenceManager
    {
        // Dummy class to get to the internal SVsSettingsPersistenceManager service
    }
}