using System.Windows;
using System.Windows.Controls;

namespace IntelliStretch.UserControls
{
    public partial class FlatTextBox : UserControl
    {
        public FlatTextBox()
        {
            InitializeComponent();
        }

        public string TextBoxText
        {
            get { return (string)GetValue(TextBoxTextProperty); }
            set { SetValue(TextBoxTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextBoxTextProperty =
            DependencyProperty.Register("TextBoxText", typeof(string), typeof(TextButton), new UIPropertyMetadata(null));
    }
}
