using System.Windows.Input;
using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace Tweakster
{
    [Command(PackageGuids.guidCommandsString, PackageIds.OpenToTheSide)]
    internal sealed class OpenToTheSide : BaseCommand<OpenToTheSide>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
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
    }
}
