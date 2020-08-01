using System.Diagnostics;
using System.Windows;
using System.Windows.Shell;

namespace Tweakster.Tweaks.General
{
    public class WindowsJumpLists
    {
        public static void Initialize()
        {
            var task = new JumpTask
            {
                ApplicationPath = Process.GetCurrentProcess().MainModule.FileName,
                IconResourcePath = Process.GetCurrentProcess().MainModule.FileName,
                Title = "Start in SafeMode",
                Arguments = "/safemode"
            };

            JumpList list = JumpList.GetJumpList(Application.Current) ?? new JumpList();
            list.JumpItems.Add(task);
            list.Apply();
        }
    }
}
