﻿using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace IntelliStretch.UI
{
    public partial class UIPrelimSettings : UserControl
    {
        public UIPrelimSettings()
        {
            InitializeComponent();
        }

        #region Variables

        IntelliSerialPort sp;
        MainApp mainApp;
        Protocols.GeneralSettings generalSettings;
        Protocols.WristSettings wristSettings;
        Protocols.MCPSettings mcpSettings;
        Image currentJoinImage;
        UserControls.FlatTextButton currentTxtButton;
        bool IsMeasuring;
        
        #endregion

        #region Dependency Properties

        public ImageSource NeutralImage
        {
            get { return (ImageSource)GetValue(NeutralImageProperty); }
            set { SetValue(NeutralImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NeutralImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NeutralImageProperty =
            DependencyProperty.Register("NeutralImage", typeof(ImageSource), typeof(UIPrelimSettings), new UIPropertyMetadata(null));


        public ImageSource FlexionImage
        {
            get { return (ImageSource)GetValue(FlexionImageProperty); }
            set { SetValue(FlexionImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FlexionImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FlexionImageProperty =
            DependencyProperty.Register("FlexionImage", typeof(ImageSource), typeof(UIPrelimSettings), new UIPropertyMetadata(null));


        public ImageSource ExtensionImage
        {
            get { return (ImageSource)GetValue(ExtensionImageProperty); }
            set { SetValue(ExtensionImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExtensionImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExtensionImageProperty =
            DependencyProperty.Register("ExtensionImage", typeof(ImageSource), typeof(UIPrelimSettings), new UIPropertyMetadata(null));


        public ImageSource GuideLinesImage
        {
            get { return (ImageSource)GetValue(GuideLinesImageProperty); }
            set { SetValue(GuideLinesImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GuideLinesImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GuideLinesImageProperty =
            DependencyProperty.Register("GuideLinesImage", typeof(ImageSource), typeof(UIPrelimSettings), new UIPropertyMetadata(null));



        public bool IsReflected
        {
            get { return (bool)GetValue(IsReflectedProperty); }
            set { SetValue(IsReflectedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsReflected.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsReflectedProperty =
            DependencyProperty.Register("IsReflected", typeof(bool), typeof(UIPrelimSettings), new UIPropertyMetadata(false));




        public string FlexionName
        {
            get { return (string)GetValue(FlexionNameProperty); }
            set { SetValue(FlexionNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FlexionName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FlexionNameProperty =
            DependencyProperty.Register("FlexionName", typeof(string), typeof(UIPrelimSettings), new UIPropertyMetadata("Flexion"));



        public string ExtensionName
        {
            get { return (string)GetValue(ExtensionNameProperty); }
            set { SetValue(ExtensionNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExtensionName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExtensionNameProperty =
            DependencyProperty.Register("ExtensionName", typeof(string), typeof(UIPrelimSettings), new UIPropertyMetadata("Extension"));



        #endregion

        #region Routed Events
        public event RoutedEventHandler Settings_Done
        {
            add { AddHandler(Settings_DoneEvent, value); }
            remove { RemoveHandler(Settings_DoneEvent, value); }
        }

        public static readonly RoutedEvent Settings_DoneEvent =
            EventManager.RegisterRoutedEvent("Settings_Done", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UIPrelimSettings));


        #endregion


        public void Load_GeneralSettings(MainApp app, IntelliSerialPort port)
        {
            sp = port;
            mainApp = app;
            generalSettings = mainApp.IntelliProtocol.General;
            wristSettings = mainApp.IntelliProtocol.Wrist;
            mcpSettings = mainApp.IntelliProtocol.MCP;

            // Wrist settings
            setFlexion.TextBoxText = wristSettings.FlexionMax.ToString();
            if (generalSettings.ExtensionMax > 0) setExtension.TextBoxText = wristSettings.ExtensionMax.ToString();
            else setExtension.TextBoxText = (-wristSettings.ExtensionMax).ToString();
            setExRange.Text = generalSettings.ExtraRange.ToString();

            // MCP Settings
            //setMCPFlexion.TextBoxText = mcpSettings.FlexionMax.ToString();
            //if (generalSettings.ExtensionMax > 0) setMCPExtension.TextBoxText = mcpSettings.ExtensionMax.ToString();
            //else setMCPExtension.TextBoxText = (-mcpSettings.ExtensionMax).ToString();
            //mcpRange.Text = generalSettings.ExtraRange.ToString();

            this.IsReflected = (generalSettings.JointSide == Protocols.JointSide.Left); // Reflect display images if left arm side is selected, right arm and ankle remain unchanged
        }

        public void Update_DataInfo(IntelliSerialPort.AnkleData newAnkleData)
        {
            txtDataInfo.Dispatcher.Invoke(new Action(delegate
            {
                txtDataInfo.Content = "Position (deg): " + newAnkleData.anklePos.ToString("#0.0")
                                + "\r\nTorque (Nm): " + newAnkleData.ankleTorque.ToString("#0.0")
                                + "\r\nCurrent Level: " + (newAnkleData.ankleAm * 100).ToString() + "%";
            }));

            if (IsMeasuring && currentTxtButton != null)
            {
                currentTxtButton.Dispatcher.Invoke(new Action(delegate
                {
                    int newPos = (int)newAnkleData.anklePos;
                    if (newPos < 0) newPos = -newPos;
                    currentTxtButton.TextBoxText = newPos.ToString();
                }));
                IsMeasuring = false;
            }
        }

        private void btnApplyGeneralSettings_Click(object sender, RoutedEventArgs e)
        {
            /*
            switch ((tabPreliminary.SelectedItem as TabItem).Name)
            {
                case "tabWrist":
                   Console.WriteLine("Sending Wrist Settings");
                    //Update wrist settings
                    int newWristFlexion = Convert.ToInt32(setWristFlexion.TextBoxText);
                    int newWristExtension = Convert.ToInt32(setWristExtension.TextBoxText);
                    int newWristExRange = Convert.ToInt32(wristRange.Text);

                    // Check sign of Extension and flip to negative if positive
                    if (newWristExtension > 0) newWristExtension = -newWristExtension;

                    wristSettings.FlexionMax = newWristFlexion;
                    wristSettings.ExtensionMax = newWristExtension; // Flipped sign, added by Michael, 27.08.2024
                                                               // generalSettings.ExtraRange = newExRange;
                    wristSettings.ActiveFlexionMax = newWristFlexion;
                    wristSettings.ActiveExtensionMax = newWristExtension; // Flipped sign, added by Michael, 27.08.2024
                    generalSettings.GameFlexionMax = newWristFlexion;
                    generalSettings.GameExtensionMax = newWristExtension; // Flipped sign, added by Michael, 27.08.2024
                    
                    mainApp.IntelliProtocol.General = generalSettings;  // Apply changes
                    mainApp.IntelliProtocol.Wrist = wristSettings;

                    if (sp.IsConnected)
                    {
                        // Send wrist tag
                        sp.WriteCmd($"WF{newWristFlexion}"); // Set max Flexion            
                        sp.WriteCmd($"WE{newWristExtension}"); // Set max Extension            
                        sp.WriteCmd($"WX{newWristExRange}"); // Set extra range
                    }

                    break;

                case "tabMCP":
                    Console.WriteLine("Sending MCP Settings");
                    //Update mcp settings
                    int newMCPFlexion = Convert.ToInt32(setMCPFlexion.TextBoxText);
                    int newMCPExtension = Convert.ToInt32(setMCPExtension.TextBoxText);
                    int newMCPExRange = Convert.ToInt32(mcpRange.Text);

                    // Check sign of Extension and flip to negative if positive
                    if (newMCPExtension > 0) newMCPExtension = -newMCPExtension;

                    mcpSettings.FlexionMax = newMCPFlexion;
                    mcpSettings.ExtensionMax = newMCPExtension; // Flipped sign, added by Michael, 27.08.2024
                                                                // generalSettings.ExtraRange = newExRange;
                    mcpSettings.ActiveFlexionMax = newMCPFlexion;
                    mcpSettings.ActiveExtensionMax = newMCPExtension; // Flipped sign, added by Michael, 27.08.2024
                    generalSettings.GameFlexionMax = newMCPFlexion;
                    generalSettings.GameExtensionMax = newMCPExtension; // Flipped sign, added by Michael, 27.08.2024

                    mainApp.IntelliProtocol.General = generalSettings;  // Apply changes
                    mainApp.IntelliProtocol.MCP = mcpSettings;

                    if (sp.IsConnected)
                    {
                        // Send MCP tag
                        sp.WriteCmd($"MF{newMCPFlexion}"); // Set max flexion            
                        sp.WriteCmd($"ME{newMCPExtension}"); // Set max extension            
                        sp.WriteCmd($"MX{newMCPExRange}"); // Set extra range
                    }

                    break;

                default:
                    break;
            }*/

            // Raise Done event
            RaiseEvent(new RoutedEventArgs(Settings_DoneEvent)); 
        }

        
        private void setting_GotFocus(object sender, RoutedEventArgs e)
        {
            if (e.Source == setFlexion)
            {
                Update_JointImage(imgFlexion);
                currentTxtButton = setFlexion;
                Update_SliderPicker(0, 100, 5, Convert.ToInt32(currentTxtButton.TextBoxText));// Chang Max. ROM from 90degree to 40 degree April.2013       
            }
            else if (e.Source == setExtension)
            {
                Update_JointImage(imgExtension);
                currentTxtButton = setExtension;
                Update_SliderPicker(0, 100, 5, Convert.ToInt32(currentTxtButton.TextBoxText));// Chang Max. ROM from 60degree to 30 degree April.2013          
            }
            else
            {
                Update_JointImage(imgNeutral);
                currentTxtButton = null;
            }
        }

        private void Update_JointImage(Image newImage)
        {
            if (currentJoinImage != null && currentJoinImage != newImage)
            {
                currentJoinImage.Visibility = Visibility.Collapsed;
                newImage.Visibility = Visibility.Visible;
            }
            currentJoinImage = newImage;
        }

        private void Update_SliderPicker(int minValue, int maxValue, int largeChange, int defaultValue)
        {
            sliderValuePicker.Minimum = minValue;
            sliderValuePicker.Maximum = maxValue;
            sliderValuePicker.LargeChange = largeChange;
            sliderValuePicker.Value = defaultValue;
            sliderValuePicker.BeginAnimation(OpacityProperty, new System.Windows.Media.Animation.DoubleAnimation(0d, 1d, new Duration(new TimeSpan(0,0,1))));
        }

        private void sliderValuePicker_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (currentTxtButton != null)
                currentTxtButton.Dispatcher.Invoke(new Action(delegate
                {
                    currentTxtButton.TextBoxText = sliderValuePicker.Value.ToString();
                }));

        }

        private void Measure_ButtonClick(object sender, RoutedEventArgs e)
        {
            currentTxtButton = e.Source as UserControls.FlatTextButton;
            IsMeasuring = true;
        }

        private void uiPrelimSettings_Loaded(object sender, RoutedEventArgs e)
        {
            currentJoinImage = imgNeutral;
        }

        private void saveCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            mainApp.IntelliProtocol.System.IsSavingData = true;
        }

        private void saveCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            mainApp.IntelliProtocol.System.IsSavingData = false;
        }
        

        private void setNeutral_Click(object sender, RoutedEventArgs e)
        {
            // Check selected tab 
            // If Wrist tab selected
                // send wrist tag
                // send zero position tag

            // If MCP tab selected
                // send mcp tag
                // send zero position tag

            // Set neutral postion
            if (sp.IsConnected) sp.WriteCmd("ZP");
        }

        #region Range Scroller Focus Functions
        private void wristExRangeBdr_GotFocus(object sender, RoutedEventArgs e)
        {
            //wristExRangeBdr.BorderBrush = new SolidColorBrush(Colors.Red);

            Update_JointImage(imgNeutral);
            sliderValuePicker.Visibility = Visibility.Hidden;
        }

        private void wristExRangeBdr_LostFocus(object sender, RoutedEventArgs e)
        {
            //wristExRangeBdr.BorderBrush = null;
            sliderValuePicker.Visibility = Visibility.Visible;
        }

        private void mcpExRangeBdr_GotFocus(object sender, RoutedEventArgs e)
        {
            //mcpExRangeBdr.BorderBrush = new SolidColorBrush(Colors.Red);
            Update_JointImage(imgNeutral);
            sliderValuePicker.Visibility = Visibility.Hidden;
        }

        private void mcpExRangeBdr_LostFocus(object sender, RoutedEventArgs e)
        {
            //mcpExRangeBdr.BorderBrush = null;
            sliderValuePicker.Visibility = Visibility.Visible;
        }

        #endregion

        #region Range Scroller Functions
        private void mcpRangeScroller_ButtonMinClick(object sender, RoutedEventArgs e)
        {
            //int range_val = Int32.Parse(mcpRange.Text);

            //if (range_val > 0) range_val = range_val - 1;
            //mcpRange.Text = range_val.ToString();
        }

        private void mcpRangeScroller_ButtonMaxClick(object sender, RoutedEventArgs e)
        {
            //int range_val = Int32.Parse(mcpRange.Text);

            //if (range_val < 5) range_val++;
           // mcpRange.Text = range_val.ToString();
        }

        private void wristRangeScroller_ButtonMinClick(object sender, RoutedEventArgs e)
        {
            //int range_val = Int32.Parse(wristRange.Text);

           // if (range_val > 0) range_val = range_val - 1;
            //wristRange.Text = range_val.ToString();
        }

        private void wristRangeScroller_ButtonMaxClick(object sender, RoutedEventArgs e)
        {
            //int range_val = Int32.Parse(wristRange.Text);

           // if (range_val < 5) range_val++;
           // wristRange.Text = range_val.ToString();
        }

        #endregion

        private void exRangeBdr_GotFocus(object sender, RoutedEventArgs e)
        {
            sliderValuePicker.Visibility = Visibility.Hidden;
            exRangeBdr.BorderBrush = new SolidColorBrush(Colors.Yellow);
            Update_JointImage(imgNeutral);
        }

        private void exRangeBdr_LostFocus(object sender, RoutedEventArgs e)
        {
            sliderValuePicker.Visibility = Visibility.Visible;
            exRangeBdr.BorderBrush = null;
        }

        private void MCPControl_Click(object sender, RoutedEventArgs e)
        {

        }

        private void JointControl_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source == WristControl)
            {
                Console.WriteLine("Wrist clicked");
                WristControl.IsChecked = true;
                MCPControl.IsChecked = false;
            }
            else if (e.Source == MCPControl)
            {
                Console.WriteLine("MCP Clicked");
                WristControl.IsChecked = false;
                MCPControl.IsChecked = true;
            }
        }
    }
}
