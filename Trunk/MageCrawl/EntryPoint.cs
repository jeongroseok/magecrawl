using libtcod;
using Magecrawl.GameUI;

namespace Magecrawl
{
    public static class EntryPoint
    {
        public static void Main()
        {
            try
            {
                UIHelper.SetupUI();
                WelcomeWindow.Result result;
                
                using (WelcomeWindow welcomeWindow = new WelcomeWindow())
                {
                    result = welcomeWindow.Run();
                }

                if (result.Quitting || TCODConsole.isWindowClosed())
                    return;

                using (GameInstance inst = new GameInstance())
                {
                    inst.Go(result.CharacterName, result.LoadCharacter);
                }
            }
            catch (System.Exception e)
            {
                // In debug builds, we want the exception to be rethrown to make debugging easier. In release builds, we want it to get written to a file.
#if DEBUG
                throw;
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
