using System;

namespace Magecrawl.Utilities
{
    // At some point in the future, this should read from a file and contain user-visible preferences
    public class Preferences
    {
        private static Preferences m_instance = new Preferences();
        public static Preferences Instance
        {
            get
            {
                return m_instance;
            }
        }

        // Should things like FPS meter, opening story, and stuff be shown
        public bool DebuggingMode
        {
            get
            {
#if DEBUG
                return true;
#else
                return false;
#endif
            }
        }

    }
}
