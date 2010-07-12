using System;

namespace Magecrawl.Utilities
{
    // This is a wonderful hack borrowed from here:
    // http://anonsvn.mono-project.com/viewvc/trunk/monodevelop/main/src/core/MonoDevelop.Core/MonoDevelop.Core/PropertyService.cs
    // It uess some uname awesomeness to figure out if we're running on a Mac. Thanks mono guys for pointing me to it!
    public static class PlatformFinder
    {
        static public bool IsRunningOnMac()
        {
            IntPtr buf = IntPtr.Zero;
            try
            {
                buf = System.Runtime.InteropServices.Marshal.AllocHGlobal(8192);
                
                // This is a hacktastic way of getting sysname from uname()
                if (uname(buf) == 0)
                {
                    string os = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(buf);
                    if (os == "Darwin")
                        return true;
                }
            }
            catch
            {
            }
            finally
            {
                if (buf != IntPtr.Zero)
                    System.Runtime.InteropServices.Marshal.FreeHGlobal(buf);
            }
            return false;
        }

        [System.Runtime.InteropServices.DllImport("libc")]
        private static extern int uname(IntPtr buf);
    }
}
