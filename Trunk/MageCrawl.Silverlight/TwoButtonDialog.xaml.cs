using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MageCrawl.Silverlight
{
    public partial class TwoButtonDialog : ChildWindowNoFade
    {
        public TwoButtonDialog(GameWindow window, string text)
        {
            InitializeComponent();
            window.DisableFocusPopup();
            Title = "";
            Text.Text = text;
        }

        protected override Control InitialFocusItem
        {
            get
            {
                return OKButton;
            }
        }

        private void OKButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void OnKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                OKButton.Focus();
            }
            else if (e.Key == Key.Right)
            {
                CancelButton.Focus();
            }
            else if (e.Key == Key.Escape)
            {
                DialogResult = false;
            }
        }
    }
}

