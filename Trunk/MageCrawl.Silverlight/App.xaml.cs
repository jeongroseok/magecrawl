using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Magecrawl.GameEngine.Interface;
using Magecrawl.Interfaces;

namespace MageCrawl.Silverlight
{
    public partial class App : Application
    {
        public IGameEngine m_engine;
        private GameWindow m_window;

        public App()
        {
            this.Startup += this.Application_Startup;
            this.Exit += this.Application_Exit;
            this.UnhandledException += this.Application_UnhandledException;

            InitializeComponent();
            m_window = new GameWindow();
            this.RootVisual = m_window;
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            m_engine = new PublicGameEngineInterface();

            m_engine.PlayerDiedEvent += new PlayerDied(PlayerDiedEvent);
            m_engine.RangedAttackEvent += new RangedAttack(RangedAttackEvent);
            m_engine.TextOutputEvent += new TextOutputFromGame(TextOutputEvent);

            m_engine.CreateNewWorld("Donblas", "Scholar");
            m_window.Setup(m_engine);
        }

        void TextOutputEvent(string s)
        {
            throw new NotImplementedException();
        }

        void RangedAttackEvent(object attackingMethod, ShowRangedAttackType type, object data, bool targetAtEndPoint)
        {
            throw new NotImplementedException();
        }

        void PlayerDiedEvent()
        {
            throw new NotImplementedException();
        }

        private void Application_Exit(object sender, EventArgs e)
        {

        }

        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            // If the app is running outside of the debugger then report the exception using
            // the browser's exception mechanism. On IE this will display it a yellow alert 
            // icon in the status bar and Firefox will display a script error.
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                // NOTE: This will allow the application to continue running after an exception has been thrown
                // but not handled. 
                // For production applications this error handling should be replaced with something that will 
                // report the error to the website and stop the application.
                e.Handled = true;
                Deployment.Current.Dispatcher.BeginInvoke(delegate { ReportErrorToDOM(e); });
            }
        }

        private void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
        {
            try
            {
                string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
                errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

                System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight Application " + errorMsg + "\");");
            }
            catch (Exception)
            {
            }
        }
    }
}
