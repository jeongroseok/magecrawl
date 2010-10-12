using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MageCrawl.Silverlight
{
    public partial class OneButtonDialog : ChildWindowNoFade
    {
        public OneButtonDialog(GameWindow window, string text)
        {
            InitializeComponent();
            window.DisableFocusPopup();
            Title = "";
            Text.Text = text;
            Closing += (o, e) => window.EnableFocusPopup();
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

        private void OnKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                DialogResult = true;
            }
            else if (e.Key == Key.Escape)
            {
                DialogResult = true;
            }
        }
    }
}

