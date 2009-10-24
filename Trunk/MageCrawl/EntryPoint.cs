using System.Collections.Generic;

namespace Magecrawl
{
    public static class EntryPoint
    {
        public static void Main()
        {
            using (GameInstance inst = new GameInstance())
            {
                inst.Go();
            }
        }
    }
}
