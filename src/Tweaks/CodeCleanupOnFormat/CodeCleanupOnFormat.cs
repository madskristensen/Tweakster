using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using Microsoft.VisualStudio.Commanding;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Editor.Commanding;
using Microsoft.VisualStudio.Text.Editor.Commanding.Commands;
using Microsoft.VisualStudio.Utilities;

namespace Tweakster.Tweaks.CodeCleanupOnFormat
{
    [Export(typeof(ICommandHandler))]
    [Name(nameof(CodeCleanupOnFormat))]
    [ContentType("Csharp")]
    [ContentType("Basic")]
    [TextViewRole(PredefinedTextViewRoles.PrimaryDocument)]
    public class CodeCleanupOnFormat : ICommandHandler<FormatDocumentCommandArgs>
    {
        public string DisplayName => nameof(CodeCleanupOnFormat);

        [Import]
        private readonly IEditorCommandHandlerServiceFactory _commandService = default;

        public bool ExecuteCommand(FormatDocumentCommandArgs args, CommandExecutionContext executionContext)
        {
            if (!Options.Instance.RunCodeCleanupOnFormat)
            {
                return false;
            }

            try
            {
                IEditorCommandHandlerService service = _commandService.GetService(args.TextView);

                var cmd = new CodeCleanUpDefaultProfileCommandArgs(args.TextView, args.SubjectBuffer);
                service.Execute((v, b) => cmd, null);
            }
            catch (Exception ex)
            {
                Trace.Write(ex);
            }

            return false;
        }

        public CommandState GetCommandState(FormatDocumentCommandArgs args)
        {
            return CommandState.Available;
        }
    }
}
