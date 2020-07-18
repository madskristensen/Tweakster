namespace Tweakster
{
    internal partial class Options : BaseOptionModel<Options>
    {
        /* ------------------------------------------

        This class is empty, because each tweak folder contain their own partial option class.

        --------------------------------------------*/

        private class Category
        {
            public const string General = "General";
            public const string Build = "Build";
            public const string Debug = "Debug";
            public const string Editor = "Editor";
        }
    }
}
