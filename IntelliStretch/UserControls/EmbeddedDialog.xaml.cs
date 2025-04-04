﻿using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace IntelliStretch.UserControls
{
    public partial class EmbeddedDialog : UserControl
    {
        public EmbeddedDialog()
        {
            InitializeComponent();
        }

        #region DependencyProperties
        public string DialogTitle
        {
            get { return (string)GetValue(DialogTitleProperty); }
            set { SetValue(DialogTitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Caption.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DialogTitleProperty =
            DependencyProperty.Register("DialogTitle", typeof(string), typeof(EmbeddedDialog), new UIPropertyMetadata(null));

        public string DialogText
        {
            get { return (string)GetValue(DialogTextProperty); }
            set { SetValue(DialogTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Caption.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DialogTextProperty =
            DependencyProperty.Register("DialogText", typeof(string), typeof(EmbeddedDialog), new UIPropertyMetadata(null));

        public int DialogWidth
        {
            get { return (int)GetValue(DialogWidthProperty); }
            set { SetValue(DialogWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Caption.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DialogWidthProperty =
            DependencyProperty.Register("DialogWidth", typeof(int), typeof(EmbeddedDialog), new UIPropertyMetadata(null));
        #endregion
    }
}
