using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Tweakster.Tweaks.AutoSave;
using Task = System.Threading.Tasks.Task;

namespace Tweakster
{
    [Guid(PackageGuids.guidTweaksterPackageString)]
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [ProvideOptionPage(typeof(DialogPageProvider.General), "Environment", Vsix.Name, 0, 0, true, ProvidesLocalizedCategoryName = false)]
    [ProvideProfile(typeof(DialogPageProvider.General), "Environment", Vsix.Name, 0, 0, true)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string, PackageAutoLoadFlags.BackgroundLoad)]
    //[ProvideAutoLoad(VSConstants.UICONTEXT.NoSolution_string, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    public sealed class TweaksterPackage : AsyncPackage
    {
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            await AutoSave.InitializeAsync(this);
            await ReOpenDocument.InitializeAsync(this);
            await NoStepDebuggingInDesignMode.InitializeAsync(this);
            await Restart.InitializeAsync(this);
        }
    }
}
