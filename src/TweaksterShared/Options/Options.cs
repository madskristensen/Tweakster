using System.Runtime.InteropServices;
using Community.VisualStudio.Toolkit;

namespace Tweakster
{
    internal class DialogPageProvider
    {
        [ComVisible(true)]
        public class General : BaseOptionPage<Options> { }
    }

    internal partial class Options : BaseOptionModel<Options>
    {
        /* ------------------------------------------

        This class is empty, because each tweak folder contain their own partial option class.

        --------------------------------------------*/
    }
}
