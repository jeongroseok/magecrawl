using System.Threading;
using libtcodWrapper;
using Magecrawl.GameUI;

namespace Magecrawl
{
    internal class LoadingScreen : System.IDisposable
    {
        private Timer m_timer;
        private RootConsole m_console;

        internal LoadingScreen(RootConsole console, string text)
        {
            console.DrawFrame(0, 0, UIHelper.ScreenWidth, UIHelper.ScreenHeight, true);
            console.PrintLineRect(text, UIHelper.ScreenWidth / 2, UIHelper.ScreenHeight / 2, UIHelper.ScreenWidth, UIHelper.ScreenHeight, LineAlignment.Center);
            console.Flush();

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
            m_console.Flush();
        }
    }
}
