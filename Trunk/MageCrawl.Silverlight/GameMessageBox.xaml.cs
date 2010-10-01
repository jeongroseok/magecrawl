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

namespace MageCrawl.Silverlight
{
    public partial class GameMessageBox : UserControl
    {
        public GameMessageBox()
        {
            InitializeComponent();
        }

        public void AddMessage(string s)
        {
            if (Messages.Text == "")
                Messages.Text = s;
            else
                Messages.Text += "\n" + s;

            ToBottom();
        }

        public void Clear()
        {
            Messages.Text = "";
            ToBottom();
        }

        public void PageUp()
        {
            MessageScrollViewer.PageUp();
        }

        public void PageDown()
        {
            MessageScrollViewer.PageDown();
        }

        public void ToBottom()
        {
            MessageScrollViewer.UpdateLayout();
            MessageScrollViewer.ScrollToVerticalOffset(double.MaxValue);
        }
    }
}
