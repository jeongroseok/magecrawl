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

namespace MageCrawl.Silverlight.List
{
    public class ChildWindowNoFade : ChildWindow
    {
        public ChildWindowNoFade ParentWindow;

        public ChildWindowNoFade()
        {
            this.DefaultStyleKey = typeof(ChildWindowNoFade);
            this.Loaded += OnLoad;
            this.Closing += OnClose;
        }

        void OnClose(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (ParentWindow != null)
                {
                    ParentWindow.Focus();
                    if (ParentWindow.InitialFocusItem != null)
                        ParentWindow.InitialFocusItem.Focus();
                }
            });
        }

        void OnLoad(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(() => 
            {
                Focus();
                if (InitialFocusItem != null)
                    InitialFocusItem.Focus();
            });
        }

        protected virtual Control InitialFocusItem
        {
            get
            {
                return null;
            }
        }
    }
}
