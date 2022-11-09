using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Shell;

namespace Tweakster.Tweaks.General
{
    public class WindowsJumpLists
    {
        private static string _devenvPath;
        private static string _installerPath;
        public static void Initialize(string DevEnvPath, string InstallerPath)
        {
            Options.Saved += delegate { AddJumpListItems(); };

            _devenvPath = DevEnvPath;
            _installerPath = InstallerPath;

            AddJumpListItems();
        }

        private static void AddJumpListItems()
        {
            var presentationMode = new JumpTask
            {
                ApplicationPath = _devenvPath,
                IconResourcePath = _devenvPath,
                Title = "Presentation Mode",
                Description = "Starts a separate Visual Studio instance with its own settings, layout, extensions, and more...",
                Arguments = "/RootSuffix Demo"
            };

            var safeMode = new JumpTask
            {
                ApplicationPath = _devenvPath,
                IconResourcePath = _devenvPath,
                Title = "Safe Mode",
                Description = "Starts Visual Studio in limited functionality mode where all extensions are disabled.",
                Arguments = "/SafeMode"
            };

            var installerShortcut = new JumpTask
            {
                ApplicationPath = _installerPath,
                IconResourcePath = _installerPath,
                Title = "Visual Studio Installer",
                Description = "Starts the Visual Studio installer.",
                Arguments = ""
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

            if(Options.Instance.EnableInstallerShortcut)
            {
                list.JumpItems.Add(installerShortcut);
            }

            list.Apply();
        }
    }
}
