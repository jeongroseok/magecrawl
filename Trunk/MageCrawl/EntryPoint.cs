using System.IO;

namespace Magecrawl
{
    public static class EntryPoint
    {
        public static void Main()
        {
            try
            {
                using (GameInstance inst = new GameInstance())
                {
                    inst.Go();
                }
            }
            catch (System.Exception e)
            {
                // In debug builds, we want the exception to be rethrown to make debugging easier. In release builds, we want it to get written to a file.
#if DEBUG
                throw e;
#else
                using (TextWriter tw = new StreamWriter("DebuggingLog.txt"))
                {
                    tw.WriteLine("Message - " + e.Message);
                    tw.WriteLine("Source - " + e.Source);
                    tw.WriteLine("StackTrace - " + e.StackTrace);
                    tw.WriteLine("TargetSite - " + e.TargetSite);
                }
#endif
            }
        }
    }
}
