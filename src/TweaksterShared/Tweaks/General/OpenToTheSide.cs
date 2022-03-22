using System.Windows.Input;
using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace Tweakster
{
    [Command(PackageGuids.guidCommandsString, PackageIds.OpenToTheSide)]
    internal sealed class OpenToTheSide : BaseCommand<OpenToTheSide>
    {
        private bool _handled;
        
        protected override Task InitializeCompletedAsync()
        {
            VS.Events.DocumentEvents.Opened += OnOpened;

            return base.InitializeCompletedAsync();
        }

        private void OnOpened(string obj)
        {
            if (!_handled && (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)))
            {
                ThreadHelper.JoinableTaskFactory.StartOnIdle(async () =>
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    await VS.Commands.ExecuteAsync("Window.NewVerticalTabGroup");
                    
                }, VsTaskRunContext.UIThreadNormalPriority).FireAndForget();
            }
        }

        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            try
            {
                _handled = true;
                SolutionItem item = await VS.Solutions.GetActiveItemAsync();

                if (item is PhysicalFile file)
                {
                    DocumentView docView = await VS.Documents.OpenAsync(file.FullPath);

                    if (docView != null)
                    {
                        await VS.Commands.ExecuteAsync("Window.NewVerticalTabGroup");
                    }
                }
            }
            finally
            {
                _handled = false;
            }
        }
    }
}
