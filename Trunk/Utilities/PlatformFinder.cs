using System;

namespace Magecrawl.Utilities
{
    public static class PlatformFinder
    {
        static public bool IsRunningOnMac()
        {
            return Environment.OSVersion.Platform == PlatformID.MacOSX;
        }
    }
}
