using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace IntelliStretch.UserControls
{
    public partial class TextButton : UserControl
    {
        public TextButton()
        {
            InitializeComponent();
        }

        #region Dependency properties


        public string Caption
        {
            get { return (string)GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Caption.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CaptionProperty =
            DependencyProperty.Register("Caption", typeof(string), typeof(TextButton), new UIPropertyMetadata(null));


        public string ButtonText
        {
            get { return (string)GetValue(ButtonTextProperty); }
            set { SetValue(ButtonTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ButtonText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register("ButtonText", typeof(string), typeof(TextButton), new UIPropertyMetadata(null));

        public string TextBoxText
        {
            get { return (string)GetValue(TextBoxTextProperty); }
            set { SetValue(TextBoxTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextBoxTextProperty =
            DependencyProperty.Register("TextBoxText", typeof(string), typeof(TextButton), new UIPropertyMetadata(null));

        public string TextFontSize
        {
            get { return (string)GetValue(TextFontSizeProperty); }
            set { SetValue(TextFontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextFontSizeProperty =
            DependencyProperty.Register("TextFontSize", typeof(string), typeof(TextButton), new UIPropertyMetadata(null));

        public string TextBoxRadius
        {
            get { return (string)GetValue(TextBoxRadiusProperty); }
            set { SetValue(TextBoxRadiusProperty, value); }
        }

        // Using a Dependency Property as a backing store for TextBoxRadius
        public static readonly DependencyProperty TextBoxRadiusProperty =
            DependencyProperty.Register("TextRadiusBox", typeof(string), typeof(TextButton), new UIPropertyMetadata(null));


        public Visibility ButtonVisibility
        {
            get { return (Visibility)GetValue(ButtonVisibilityProperty); }
            set { SetValue(ButtonVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ButtonVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ButtonVisibilityProperty =
            DependencyProperty.Register("ButtonVisibility", typeof(Visibility), typeof(TextButton), new UIPropertyMetadata(Visibility.Visible));



        #endregion

        #region Routed Events
        public event RoutedEventHandler ButtonClick
        {
            add { AddHandler(ButtonClickEvent, value); }
            remove { RemoveHandler(ButtonClickEvent, value); }
        }

        public static readonly RoutedEvent ButtonClickEvent =
            EventManager.RegisterRoutedEvent("ButtonClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TextButton));

        #endregion


        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            Globals.Sound.buttonSound.Play();
            RaiseEvent(new RoutedEventArgs(ButtonClickEvent));
        }

        private void textButton_GotFocus(object sender, RoutedEventArgs e)
        {
            bdrHighlight.BorderBrush = new SolidColorBrush(Colors.Red);
        }

        private void textButton_LostFocus(object sender, RoutedEventArgs e)
        {
            bdrHighlight.BorderBrush = null;
        }
    }
}
