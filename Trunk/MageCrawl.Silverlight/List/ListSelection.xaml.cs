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
using MageCrawl.Silverlight.KeyboardHandlers;
using Magecrawl.Interfaces;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace MageCrawl.Silverlight.List
{
    public partial class ListSelection : ChildWindowNoFade
    {
        public delegate void OnSelection(INamedItem itemSelected);

        public bool DismissOnSelection { get; set; }

        public OnSelection SelectionDelegate;

        public ListSelection(GameWindow window, IEnumerable<INamedItem> items, string title)
        {
            InitializeComponent();

            DismissOnSelection = true;
            Title = title;

            window.DisableFocusPopup();
            Closed += (o, e) => { window.EnableFocusPopup(); };

            List.ItemsSource = items;
            if (items.Count() > 0)
                List.SelectedIndex = 0;
            Focus();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (KeyboardConverter.GetConvertedKey(e.Key, e.PlatformKeyCode))
            {
                case MagecrawlKey.Escape:
                    Close();
                    break;
                case MagecrawlKey.Enter:
                    if (SelectionDelegate != null && List.SelectedItem != null)
                        SelectionDelegate((INamedItem)List.SelectedItem);
                    if (DismissOnSelection)
                        Close();
                    break;
            }
        }

        protected override Control InitialFocusItem
        {
            get
            {
                if (List.SelectedItem != null)
                    return (Control)List.GetContainers().ToList()[List.SelectedIndex];
                else
                    return List;
            }
        }
    }

    // This is the converter used to split 3'tabed strings into ones that are aligned, or just return text blocks for normal strings.
    // Later it might learn tricks like placing icons near elements or such
    public class ListItemValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            INamedItem namedItem = value as INamedItem;
            if (namedItem == null)
            {
                TextBlock block = new TextBlock();
                block.Text = "";
                return block;
            }

            string[] tabSeperatedParts = namedItem.DisplayName.Split('\t');

            if (tabSeperatedParts.Count() != 3)
            {
                TextBlock block = new TextBlock();
                block.Text = namedItem.DisplayName;
                return block;
            }
            else
            {
                Grid grid = new Grid();
                grid.RowDefinitions.Add(new RowDefinition());
                for (int i = 0; i < 3; ++i)
                {
                    ColumnDefinition col = new ColumnDefinition();
                    col.Width = new GridLength(1, GridUnitType.Star);
                    grid.ColumnDefinitions.Add(col);
                    TextBlock text = new TextBlock();
                    text.Text = tabSeperatedParts[i];
                    grid.Children.Add(text);
                    Grid.SetColumn(text, i);
                }
                ((TextBlock)grid.Children[0]).TextAlignment = TextAlignment.Left;
                ((TextBlock)grid.Children[1]).TextAlignment = TextAlignment.Center;
                ((TextBlock)grid.Children[2]).TextAlignment = TextAlignment.Right;
                return grid;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}

