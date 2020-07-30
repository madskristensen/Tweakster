using System;
using System.Diagnostics;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using task = System.Threading.Tasks.Task;

internal class OutputWindowTraceListener : TraceListener
{
    private IVsOutputWindowPane _pane;
    private readonly string _paneTitle;
    private readonly string _filterText;

    private OutputWindowTraceListener(string paneTitle, string filterText)
    {
        _paneTitle = paneTitle;
        _filterText = filterText;
    }

    public static void Register(string paneTitle, string filterText)
    {
        var instance = new OutputWindowTraceListener(paneTitle, filterText);
        Trace.Listeners.Add(instance);
    }

    public override void Write(string message)
        => WriteLine(message);

    public override void WriteLine(string message)
    {
        if (!string.IsNullOrEmpty(message) && message.Contains(_filterText))
        {
            LogAsync(message + Environment.NewLine)
                .FileAndForget(nameof(OutputWindowTraceListener));
        }
    }

    private async task LogAsync(object message)
    {
        try
        {
            if (await EnsurePaneAsync())
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                _pane.OutputString(message + Environment.NewLine);
            }
        }
        catch (Exception ex)
        {
            Debug.Write(ex);
        }
    }

    private async System.Threading.Tasks.Task<bool> EnsurePaneAsync()
    {
        if (_pane == null)
        {
            try
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                if (_pane == null)
                {
                    IVsOutputWindow output = await ServiceProvider.GetGlobalServiceAsync<SVsOutputWindow, IVsOutputWindow>();
                    var guid = new Guid();

                    ErrorHandler.ThrowOnFailure(output.CreatePane(ref guid, _paneTitle, 1, 1));
                    ErrorHandler.ThrowOnFailure(output.GetPane(ref guid, out _pane));
                }
            }
            catch
            {
                // Nothing to do if this fails
            }
        }

        return _pane != null;
    }
}