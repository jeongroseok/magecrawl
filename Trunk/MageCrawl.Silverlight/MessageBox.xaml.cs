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
    public partial class MessageBox : UserControl
    {
        private ScrollViewer m_scroll;
        private TextBlock m_messages;

        public MessageBox()
        {
            InitializeComponent();

            m_scroll = (ScrollViewer)FindName("MessageScrollViewer");
            m_messages = (TextBlock)FindName("Messages");
        }

        public void AddMessage(string s)
        {
            if (m_messages.Text == "")
                m_messages.Text = s;
            else
                m_messages.Text += "\n" + s;
            ToBottom();
        }

        public void Clear()
        {
            m_messages.Text = "";
            ToBottom();
        }

        public void PageUp()
        {
            m_scroll.PageUp();
        }

        public void PageDown()
        {
            m_scroll.PageDown();
        }

        public void ToBottom()
        {
            m_scroll.ScrollToBottom();
        }
    }
}
