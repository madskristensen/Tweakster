using System.Diagnostics;
using System.Windows;
using System.Windows.Shell;

namespace Tweakster.Tweaks.General
{
    public class WindowsJumpLists
    {
        public static void Initialize()
        {
            var presentationMode = new JumpTask
            {
                ApplicationPath = Process.GetCurrentProcess().MainModule.FileName,
                IconResourcePath = Process.GetCurrentProcess().MainModule.FileName,
                Title = "Presentation Mode",
                Arguments = "/RootSuffix Present"
            };

            var safeMode = new JumpTask
            {
                ApplicationPath = Process.GetCurrentProcess().MainModule.FileName,
                IconResourcePath = Process.GetCurrentProcess().MainModule.FileName,
                Title = "Safe Mode",
                Arguments = "/safemode"
            };

            JumpList list = JumpList.GetJumpList(Application.Current) ?? new JumpList();
            list.JumpItems.Add(presentationMode);
            list.JumpItems.Add(safeMode);
            list.Apply();
        }
    }
}
