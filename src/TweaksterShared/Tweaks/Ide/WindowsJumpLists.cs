using System.Diagnostics;
using System.Windows;
using System.Windows.Shell;

namespace Tweakster.Tweaks.General
{
    public class WindowsJumpLists
    {
        public static void Initialize()
        {
            Options.Saved += delegate { AddJumpListItems(); };

            AddJumpListItems();
        }

        private static void AddJumpListItems()
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
                Arguments = "/SafeMode"
            };

            JumpList list = JumpList.GetJumpList(Application.Current) ?? new JumpList();
            list.ShowRecentCategory = true;

            if (Options.Instance.EnablePresentationMode)
            {
                list.JumpItems.Add(presentationMode);
            }

            if (Options.Instance.EnableSafeMode)
            {
                list.JumpItems.Add(safeMode);
            }

            list.Apply();
        }
    }
}
