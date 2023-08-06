// Tag the library
[assembly: NativeLibrary("SampleLibrary")]

namespace SampleLibrary;

public partial class Common
{
    static bool _init;

    public static void InitLibrary()
    {
        if (_init)
            return;

        SLInit();
        initPlatform();
    }

    static partial void initPlatform();

#if NETSTANDARD
    static partial void initPlatform()
    {

    }
#endif

    [DllImport("SampleLibrary", CallingConvention = CallingConvention.Cdecl), Order]
    static extern void SLInit();

}
