using System.IO;
using libtcod;
using Magecrawl.GameUI;

namespace Magecrawl
{
    public static class EntryPoint
    {
        public static void Main()
        {
#if !DEBUG
            try
#endif
            {
                UIHelper.SetupUI();
                WelcomeWindow welcomeWindow = new WelcomeWindow();
                WelcomeWindow.Result result = welcomeWindow.Run();

                if (result.Quitting || TCODConsole.isWindowClosed())
                    return;

                using (GameInstance inst = new GameInstance())
                {
                    if (result.LoadCharacter)
                        inst.StartGameFromFile(result.CharacterName);
                    else
                        inst.StartNewGame(result.CharacterName);
                }
            }
            
            // In debug builds, we want the exception to be rethrown to make debugging easier. In release builds, we want it to get written to a file.
#if !DEBUG
            catch (System.Exception e)
            {
                using (TextWriter tw = new StreamWriter("DebuggingLog.txt"))
                {
                    tw.WriteLine("Message - " + e.Message);
                    tw.WriteLine("Source - " + e.Source);
                    tw.WriteLine("StackTrace - " + e.StackTrace);
                    tw.WriteLine("TargetSite - " + e.TargetSite);
                }
            }
#endif
        }
    }
}
