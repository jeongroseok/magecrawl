using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Magecrawl.Interfaces;
using MageCrawl.Silverlight.KeyboardHandlers;

namespace MageCrawl.Silverlight.List
{
    public partial class ItemSelection : ChildWindowNoFade
    {
        public delegate void OnSelection(IItem selectedItem, string optionName);

        private List<ItemOptions> m_options;
        private OnSelection m_selection;
        private IItem m_item;

        public ItemSelection(IGameEngine engine, IItem item, OnSelection selectionDelegate)
        {
            InitializeComponent();

            m_item = item;
            m_selection = selectionDelegate;

            m_options = engine.GameState.GetOptionsForInventoryItem(item);
            ActionList.ItemsSource = m_options;

            if (m_options.Count > 0)
                ActionList.SelectedIndex = 0;

            Description.Text = item.ItemDescription + "\n\n" + item.FlavorDescription;

            ParentWindow = null;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (KeyboardConverter.GetConvertedKey(e.Key, e.PlatformKeyCode))
            {
                case MagecrawlKey.Escape:
                    Close();
                    break;
                case MagecrawlKey.Enter:
                {
                    if (m_options.Count == 0)
                    {
                        Close();
                    }
                    else
                    {
                        ItemOptions option = (ItemOptions)ActionList.SelectedItem;
                        if (option.Enabled)
                        {
                            m_selection(m_item, option.Option);
                            Close();
                            ParentWindow.Close();
                        }
                    }
                    break;
                }
            }
        }

        protected override Control InitialFocusItem
        {
            get
            {
                return ActionList;
            }
        }
    }

    public class OptionEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ItemOptions option = (ItemOptions)value;
            if (option.Enabled)
                return new SolidColorBrush(Colors.Black);
            else
                return new SolidColorBrush(Colors.Red);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

