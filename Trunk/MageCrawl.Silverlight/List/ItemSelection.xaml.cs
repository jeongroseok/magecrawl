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
using Magecrawl.Interfaces;
using MageCrawl.Silverlight.KeyboardHandlers;

namespace MageCrawl.Silverlight.List
{
    public partial class ItemSelection : ChildWindowNoFade
    {
        private IItem m_item;

        public ItemSelection(IItem item)
        {
            InitializeComponent();

            ParentWindow = null;

            m_item = item;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (KeyboardConverter.GetConvertedKey(e.Key, e.PlatformKeyCode))
            {
                case MagecrawlKey.Escape:
                    Close();
                    break;
            }
        }


        protected override Control InitialFocusItem
        {
            get
            {
                return null;
            }
        }
    }
}

