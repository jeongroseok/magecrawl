using System.Threading;
using libtcod;
using Magecrawl.GameUI;

namespace Magecrawl
{
    internal class LoadingScreen : System.IDisposable
    {
        private Timer m_timer;
        private TCODConsole m_console;

        internal LoadingScreen(TCODConsole console, string text)
        {
            console.printFrame(0, 0, UIHelper.ScreenWidth, UIHelper.ScreenHeight, true);
            console.printRectEx(UIHelper.ScreenWidth / 2, UIHelper.ScreenHeight / 2, UIHelper.ScreenWidth, UIHelper.ScreenHeight, TCODBackgroundFlag.Set, TCODAlignment.CenterAlignment, text);
            TCODConsole.flush();

            m_console = console;
            m_timer = new Timer(OnTick, null, 0, 50);
        }

        public void Dispose()
        {
            if (m_timer != null)
                m_timer.Dispose();
            m_timer = null;
        }

        private void OnTick(object o)
        {
            if (m_timer != null)
                TCODConsole.flush();
        }
    }
}
