using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Tweakster.Tweaks.General;
using Task = System.Threading.Tasks.Task;

namespace Tweakster
{
    [Guid(PackageGuids.guidTweaksterPackageString)]
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
    [ProvideOptionPage(typeof(DialogPageProvider.General), "Environment", Vsix.Name, 0, 0, true, new[] { "save", "open", "close", "clear", "focus", "restart", "output", "verbosity", "delete", "bin", "obj", "build", "debug", "copy", "empty", "line", "zoom", "format", "select", "paste" }, ProvidesLocalizedCategoryName = false)]
    [ProvideProfile(typeof(DialogPageProvider.General), "Environment", Vsix.Name, 0, 0, true)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    public sealed class TweaksterPackage : AsyncPackage
    {
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            WindowsJumpLists.Initialize();

            await NoStepDebuggingInDesignMode.InitializeAsync(this);
            await Restart.InitializeAsync(this);
            await DeleteOutputArtifacts.InitializeAsync(this);
            await OutputVerbosity.InitializeAsync(this);
            await ResetZoomLevel.InitializeAsync(this);
            await JustMyCode.InitializeAsync(this);
            await BuildStats.InitializeAsync(this);
            await BuildOrdered.InitializeAsync(this);
            await FindInSolutionExplorer.InitializeAsync(this);
            await OpenLanguageSettings.InitializeAsync(this);
            await CloseActiveDocument.InitializeAsync(this);
            await FocusSolutionExplorer.InitializeAsync(this);
            await DuplicateWindow.InitializeAsync(this);
            await ClearRecentFilesAndProjects.InitializeAsync(this);
            await SelectWholeLineCommand.InitializeAsync(this);
            await OpenToTheSide.InitializeAsync(this);

#if VS16
            await AutoSave.InitializeAsync(this);
            await ReOpenDocument.InitializeAsync(this);
#endif
        }
    }
}
