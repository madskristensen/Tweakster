using System.Diagnostics;
using System.Windows;
using System.Windows.Shell;

namespace Tweakster.Tweaks.General
{
    public class WindowsJumpLists
    {
        public static void Initialize()
        {
            var devenv = Process.GetCurrentProcess().MainModule.FileName;

            var presentationMode = new JumpTask
            {
                ApplicationPath = devenv,
                IconResourcePath = devenv,
                Title = "Presentation Mode",
                Description = "Starts a separate Visual Studio instance with its own settings, layout, extensions, and more...",
                Arguments = "/RootSuffix Demo"
            };

            var safeMode = new JumpTask
            {
                ApplicationPath = devenv,
                IconResourcePath = devenv,
                Title = "Safe Mode",
                Description = "Starts Visual Studio in limited functionality mode where all extensions are disabled.",
                Arguments = "/safemode"
            };

            JumpList list = JumpList.GetJumpList(Application.Current) ?? new JumpList();
            list.JumpItems.Add(presentationMode);
            list.JumpItems.Add(safeMode);
            list.Apply();
        }
    }
}
