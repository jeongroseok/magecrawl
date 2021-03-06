﻿using System;
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
using MageCrawl.Silverlight.ModelView;
using Magecrawl.Interfaces;

namespace MageCrawl.Silverlight
{
    public partial class CharacterInfo : UserControl
    {
        public CharacterInfo()
        {
            InitializeComponent();
        }

        public void Setup(IPlayer player)
        {
            Character = new CharacterViewModel(player);
            InfoPanel.DataContext = Character;
        }

        public CharacterViewModel Character
        {
            get;
            private set;
        }
    }
}
