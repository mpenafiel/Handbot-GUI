using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace IntelliStretch.UserControls
{
    public partial class FlatToggleButton : UserControl
    {
        public FlatToggleButton()
        {
            InitializeComponent();
        }

        #region Dependency Properties

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(FlatToggleButton), new UIPropertyMetadata(""));


        public double TextFontSize
        {
            get { return (double)GetValue(TextFontSizeProperty); }
            set { SetValue(TextFontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextFontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextFontSizeProperty =
            DependencyProperty.Register("TextFontSize", typeof(double), typeof(FlatToggleButton), new UIPropertyMetadata(16d));

        public double ButtonCornerRadius
        {
            get { return (double)GetValue(ButtonCornerRadiusProperty); }
            set { SetValue(ButtonCornerRadiusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextFontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ButtonCornerRadiusProperty =
            DependencyProperty.Register("ButtonCornerRadius", typeof(double), typeof(FlatToggleButton), new UIPropertyMetadata(5d));


        public Color TopColor
        {
            get { return (Color)GetValue(TopColorProperty); }
            set { SetValue(TopColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Color.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TopColorProperty =
            DependencyProperty.Register("TopColor", typeof(Color), typeof(FlatToggleButton), new UIPropertyMetadata(Colors.Black));

        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Color.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(FlatToggleButton), new UIPropertyMetadata(Colors.Black));

        public Color TextColor
        {
            get { return (Color)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Color.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextColorProperty =
            DependencyProperty.Register("TextColor", typeof(Color), typeof(FlatToggleButton), new UIPropertyMetadata(Colors.Black));

        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsChecked.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(FlatToggleButton), new UIPropertyMetadata(false));

        #endregion

        #region Routed Events
        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        public static readonly RoutedEvent ClickEvent =
            EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(FlatToggleButton));
        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            IntelliStretch.Globals.Sound.buttonSound.Play();
            RaiseEvent(new RoutedEventArgs(ClickEvent));
        }
    }
}
