using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using Community.VisualStudio.Toolkit;

namespace Tweakster
{
    [Export(typeof(ITaggerProvider))]
    [TagType(typeof(IStructureTag))]
    [Name(nameof(CollapseAttributeTaggerProvider))]
    [ContentType(ContentTypes.CSharp)]
    internal sealed class CollapseAttributeTaggerProvider : ITaggerProvider
    {
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            if (!Options.Instance.CollapseMemberAttributes)
            {
                return null;
            }

            return buffer.Properties.GetOrCreateSingletonProperty(() => new CollapseAttributeTagger(buffer, Options.Instance.CollapseMemberAttributesShortForm)) as ITagger<T>;
        }
    }
}
