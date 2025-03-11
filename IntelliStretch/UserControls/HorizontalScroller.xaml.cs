using System.Windows;
using System.Windows.Controls;

namespace IntelliStretch.UserControls
{
    public partial class HorizontalScroller : UserControl
    {
        public HorizontalScroller()
        {
            InitializeComponent();
        }

        #region Routed Events
        public event RoutedEventHandler ButtonMinClick
        {
            add { AddHandler(ButtonMinClickEvent, value); }
            remove { RemoveHandler(ButtonMinClickEvent, value); }
        }

        public static readonly RoutedEvent ButtonMinClickEvent =
            EventManager.RegisterRoutedEvent("ButtonMinClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(HorizontalScroller));

        public event RoutedEventHandler ButtonMaxClick
        {
            add { AddHandler(ButtonMaxClickEvent, value); }
            remove { RemoveHandler(ButtonMaxClickEvent, value); }
        }

        public static readonly RoutedEvent ButtonMaxClickEvent =
            EventManager.RegisterRoutedEvent("ButtonMaxClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(HorizontalScroller));

        #endregion

        private void btnMin_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(ButtonMinClickEvent));
        }

        private void btnMax_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(ButtonMaxClickEvent));
        }
    }
}
