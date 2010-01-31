using System;
using System.Collections.Generic;
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
                using (TextWriter tw = new StreamWriter("DebuggingLog.txt"))
                {
                    tw.WriteLine("Data - " + e.Data);
                    tw.WriteLine("Message - " + e.Message);
                    tw.WriteLine("Source - " + e.Source);
                    tw.WriteLine("StackTrace - " + e.StackTrace);
                    tw.WriteLine("TargetSite - " + e.TargetSite);
                }
            }
        }
    }
}
