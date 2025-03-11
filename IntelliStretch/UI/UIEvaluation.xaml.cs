using CsvHelper;
using IntelliStretch.Data;
//using NationalInstruments;
//using NationalInstruments.DAQmx;
using ScottPlot;
using ScottPlot.Plottables;
using ScottPlot.WPF;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace IntelliStretch.UI
{
    public partial class UIEvaluation : UserControl
    {
        #region Plot Components
        private ScottPlot.Plottables.DataStreamer Streamer1 = null;
        private ScottPlot.Plottables.DataStreamer Streamer2 = null;
        private DataStreamer[] dataStreamers = new DataStreamer[2];

        private WpfPlot[] plots = new WpfPlot[2];

        private ScottPlot.Plottables.VerticalLine VLine1;
        private ScottPlot.Plottables.VerticalLine VLine2;
        #endregion

        readonly System.Windows.Forms.Timer UpdatePlotTimer = new System.Windows.Forms.Timer() { Interval = 50, Enabled = true };

        public UIEvaluation()
        {
            InitializeComponent();

            //plot new data here using seperate timer function
            UpdatePlotTimer.Tick += (s, e) =>
            {
                if (Streamer1 != null && Streamer1.HasNewData)
                {
                    VLine1.IsVisible = Streamer1.Renderer is ScottPlot.DataViews.Wipe;
                    VLine1.Position = Streamer1.Data.NextIndex * Streamer1.Data.SamplePeriod + Streamer1.Data.OffsetX;

                    VLine2.IsVisible = Streamer2.Renderer is ScottPlot.DataViews.Wipe;
                    VLine2.Position = Streamer2.Data.NextIndex * Streamer2.Data.SamplePeriod + Streamer2.Data.OffsetX;

                    //vPlot1.Refresh();
                    //vPlot2.Refresh();
                    hPlot1.Refresh();
                    hPlot2.Refresh();
                }
            };
        }



        #region Variables
        IntelliSerialPort sp;
        //Streamer streamer;
        Protocols.IntelliProtocol intelliProtocol;
        MainApp mainApp;
        Interfaces.IUpdateUI currentUI;
        string measureMode;
        bool IsSavingData;

        private StreamWriter writer = null;
        private string fileContent = string.Empty;
        private string filePath = string.Empty;
        private CsvWriter csv = null;
        public string DataDir { get; set; }
        public string DataFilePrefix { get; set; }

        #endregion

        #region Dependency properties


        public string FlexionName
        {
            get { return (string)GetValue(FlexionNameProperty); }
            set { SetValue(FlexionNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FlexionName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FlexionNameProperty =
            DependencyProperty.Register("FlexionName", typeof(string), typeof(UIEvaluation), new UIPropertyMetadata("Flexion"));



        public string ExtensionName
        {
            get { return (string)GetValue(ExtensionNameProperty); }
            set { SetValue(ExtensionNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExtensionName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExtensionNameProperty =
            DependencyProperty.Register("ExtensionName", typeof(string), typeof(UIEvaluation), new UIPropertyMetadata("Extension"));

        public ImageSource FlexionImage
        {
            get { return (ImageSource)GetValue(FlexionImageProperty); }
            set { SetValue(FlexionImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FlexionImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FlexionImageProperty =
            DependencyProperty.Register("FlexionImage", typeof(ImageSource), typeof(UIEvaluation), new UIPropertyMetadata(null));


        public ImageSource ExtensionImage
        {
            get { return (ImageSource)GetValue(ExtensionImageProperty); }
            set { SetValue(ExtensionImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExtensionImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExtensionImageProperty =
            DependencyProperty.Register("ExtensionImage", typeof(ImageSource), typeof(UIEvaluation), new UIPropertyMetadata(null));

        #endregion

        public void Load_Evaluation(MainApp app, IntelliSerialPort port)
        {
            mainApp = app;
            sp = port;
            intelliProtocol = mainApp.IntelliProtocol;
            Protocols.GeneralSettings generalProtocol = intelliProtocol.General;
            Protocols.StretchingProtocol stretchProtocol = intelliProtocol.Stretching;
            IsSavingData = intelliProtocol.System.IsSavingData;

            // Initalize control
            switch (generalProtocol.Joint)
            {
                case Protocols.Joint.Ankle:
                case Protocols.Joint.Knee:
                    vAROM.Visibility = Visibility.Visible;
                    hAROM.Visibility = Visibility.Collapsed;

                    //vStrength.Visibility = Visibility.Visible;
                    hStrength.Visibility = Visibility.Collapsed;

                    //vStrengthLayout.Visibility = Visibility.Visible;
                    vAROMLayout.Visibility = Visibility.Visible;
                    hStrengthLayout.Visibility = Visibility.Collapsed;
                    hAROMLayout.Visibility = Visibility.Collapsed;

                    vAROM.Initialize_Layout(generalProtocol.FlexionMax, generalProtocol.ExtensionMax);
                    //vStrength.Initialize_Layout(stretchProtocol.FlexionTorqueMax, -stretchProtocol.ExtensionTorqueMax);
                    //InitializePlots(vPlot1, vPlot2);                 
                    break;

                case Protocols.Joint.Elbow:
                case Protocols.Joint.Wrist:
                    vAROM.Visibility = Visibility.Collapsed;
                    hAROM.Visibility = Visibility.Visible;

                    //vStrength.Visibility = Visibility.Collapsed;
                    hStrength.Visibility = Visibility.Visible;

                    //vStrengthLayout.Visibility = Visibility.Collapsed;
                    vAROMLayout.Visibility = Visibility.Collapsed;
                    hStrengthLayout.Visibility = Visibility.Visible;
                    hAROMLayout.Visibility = Visibility.Visible;

                    hAROM.Initialize_Layout(generalProtocol.FlexionMax, generalProtocol.ExtensionMax);
                    hStrength.Initialize_Layout(stretchProtocol.FlexionTorqueMax, -stretchProtocol.ExtensionTorqueMax);
                    InitializePlots(hPlot1, hPlot2);
                    /*
                    if (intelliProtocol.General.JointSide == Protocols.JointSide.Right)
                    {
                        strength_h_flexionBorder.SetValue(Grid.ColumnProperty, 0);
                        strength_h_extensionBorder.SetValue(Grid.ColumnProperty, 1);

                        arom_h_flexionGrid.SetValue(Grid.ColumnProperty, 0);
                        arom_h_extensionGrid.SetValue(Grid.ColumnProperty, 1);
                    }
                    else if (intelliProtocol.General.JointSide == Protocols.JointSide.Left)
                    {
                        strength_h_flexionBorder.SetValue(Grid.ColumnProperty, 1);
                        strength_h_extensionBorder.SetValue(Grid.ColumnProperty, 0);

                        arom_h_flexionGrid.SetValue(Grid.ColumnProperty, 1);
                        arom_h_extensionGrid.SetValue(Grid.ColumnProperty, 0);
                    }*/
                    break;

                default:
                    break;
            }

            Check_CurrentUI();
            cbDisplayEmg.IsChecked = true;
            //sliderLockPosition.Maximum = generalProtocol.FlexionMax;
            //sliderLockPosition.Minimum = generalProtocol.ExtensionMax;

            // Add event handler
            sp.UpdateData = new IntelliSerialPort.DelegateUpdateData(Update_UI);
            sp.WriteCmd("BK");
            
        }

        private void InitializePlots(ScottPlot.WPF.WpfPlot plot1, ScottPlot.WPF.WpfPlot plot2)
        {
            StylePlot(plot1);
            StylePlot(plot2);

            Streamer1 = plot1.Plot.Add.DataStreamer(3000);
            Streamer2 = plot2.Plot.Add.DataStreamer(3000);

            Streamer1.Color = ScottPlot.Color.FromHex("#e5ff24");
            Streamer2.Color = ScottPlot.Color.FromHex("#e5ff24");

            dataStreamers[0] = Streamer1;
            dataStreamers[1] = Streamer2;

            dataStreamers[0].LineWidth = 3;
            dataStreamers[1].LineWidth = 3;

            plots[0] = plot1;
            plots[1] = plot2;

            VLine1 = plot1.Plot.Add.VerticalLine(0, 2, ScottPlot.Colors.Red);
            VLine1.LineWidth = 5;
            VLine2 = plot2.Plot.Add.VerticalLine(0, 2, ScottPlot.Colors.Red);
            VLine2.LineWidth = 5;

            plot1.Plot.Axes.ContinuouslyAutoscale = false;
            Streamer1.ManageAxisLimits = true;

            plot2.Plot.Axes.ContinuouslyAutoscale = false;
            Streamer2.ManageAxisLimits = true;

            plot1.Plot.Axes.SetLimitsY(-3, 3);
            plot2.Plot.Axes.SetLimitsY(-3, 3);
        }

        public void StylePlot(ScottPlot.WPF.WpfPlot plot)
        {
            
            plot.Plot.FigureBackground.Color = ScottPlot.Colors.Transparent;
            plot.Plot.Grid.MajorLineColor = ScottPlot.Colors.LightSlateGray;

            //remove default frame and add left and bottom axes
            plot.Plot.Axes.Frameless();
            ScottPlot.AxisPanels.LeftAxis leftAxis = plot.Plot.Axes.AddLeftAxis();
            ScottPlot.AxisPanels.BottomAxis bottomAxis = plot.Plot.Axes.AddBottomAxis();

            //Style left axis
            leftAxis.FrameLineStyle.Width = 3;
            leftAxis.Color(ScottPlot.Colors.Gray);

            //Style bottom axis
            bottomAxis.FrameLineStyle.Width = 3;
            bottomAxis.Color(ScottPlot.Colors.Gray);

            plot.Refresh();
        }

        private void Update_UI(IntelliSerialPort.AnkleData newAnkleData) => this.Dispatcher.Invoke(new Action(delegate
                                                                                     {
                                                                                        // Motor torque value does not have polarity, assess within UI
                                                                                        if (measureMode == "Strength")
                                                                                        {
                                                                                            if (intelliProtocol.General.JointSide == Protocols.JointSide.Right && btnFlexion.IsChecked) newAnkleData.ankleTorque = -newAnkleData.ankleTorque;
                                                                                            else if (intelliProtocol.General.JointSide == Protocols.JointSide.Left && btnExtension.IsChecked) newAnkleData.ankleTorque = -newAnkleData.ankleTorque;
                                                                                        }                                                                        
                                                                                        currentUI.Update_UI(newAnkleData);
                                                                                     }));

        private void btnMeasure_Click(object sender, RoutedEventArgs e)
        {
            btnMeasure.IsPressed = !btnMeasure.IsPressed;  // Check or uncheck the button

            if (btnMeasure.IsPressed)
            {
                UI_Handler();
                currentUI.Set_Initial(true);
                if (measureMode == "AROM" && sp.IsConnected) sp.WriteCmd("BK");//Add BK , Yupeng 04.2013 

                if (IsSavingData) sp.Start_SaveData(measureMode);
                sp.IsUpdating = true;
            }
            else
            {
                WriterHandler();
                sp.IsUpdating = false;
                if (IsSavingData)
                {
                    sp.Stop_SaveData();
                }
                UI_Handler();
            }
        }

        private void Create_Writer()
        {
            string dataFile = DataDir + DataFilePrefix + "_EMG_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";


            //create new instance of writer
            if (writer == null) { writer = new StreamWriter(dataFile); }
            //create new instance of csv writer
            if (csv == null) { csv = new CsvWriter(writer, CultureInfo.InvariantCulture); }
        }

        private void WriterHandler()
        {
            if (writer != null)
            {
                csv = null;
                writer.Close();
                writer = null;
            }
        }

        static async System.Threading.Tasks.Task DataWriter(StreamWriter writer, CsvWriter csv, double[,] data)
        {
            for (int i = 0; i < data.GetLength(1); i++)
            {
                double ch1 = data[0, i];
                double ch2 = data[1, i];

                //write each line of data as tab-delimited
                csv.WriteRecord(ch1);
                csv.WriteRecord(ch2);
                csv.NextRecord();
            }
        }

        private void UI_Handler()
        {
            if (btnMeasure.IsPressed)
            {
                btnMeasure.Image = Utilities.GetImage("Stop.png");
                btnMeasure.Text = "Stop ";
                mainApp.Buttons_Enabled(false);
                tabItems_Enabled(false);
            }
            else
            {
                btnMeasure.Image = Utilities.GetImage("Start.png");
                btnMeasure.Text = "Measure ";
                if (btnLock.IsChecked) switch_Device_Mode();
                Apply_Measure();
                mainApp.Buttons_Enabled(true);
                tabItems_Enabled(true);
            }
        }

        private void switch_Device_Mode()
        {
            if (btnBackdrivable.IsChecked)
            {
                btnBackdrivable.IsChecked = false;
                btnLock.IsChecked = true;
                if (sp.IsConnected) sp.WriteCmd("LK");//change from BK to RL Yupeng 04.2013 //change from RL to BK, Michael 08.20.2024
            }
            else if (btnLock.IsChecked)
            {
                btnBackdrivable.IsChecked = true;
                btnLock.IsChecked = false;
                if (sp.IsConnected) sp.WriteCmd("BK"); //added Michael, 08.2024
            }
        }

        private void Apply_Measure()
        {
            if (measureMode == "AROM")
            {
                // Save AROM
                if (vAROM.Visibility == Visibility.Visible)
                {
                    intelliProtocol.General.ActiveFlexionMax = (int)vAROM.ActiveFlexionMax;
                    intelliProtocol.General.ActiveExtensionMax = (int)vAROM.ActiveExtensionMax;
                }
                else
                {
                    intelliProtocol.General.ActiveFlexionMax = (int)hAROM.ActiveFlexionMax;
                    intelliProtocol.General.ActiveExtensionMax = (int)hAROM.ActiveExtensionMax;
                }
            }
        }

        private void sliderLockPosition_ValueChanged(object sender, RoutedEventArgs e)
        {
            // Define function further
            if (btnLock.IsChecked == true)
            {
                btnLock.IsChecked = false;
                btnBackdrivable.IsChecked = true;
            }
        }

        private void tabEvaluation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Check_CurrentUI();
            e.Handled = true;
        }

        private void btnBackdrivable_Click(object sender, RoutedEventArgs e)
        {
            btnBackdrivable.IsChecked = true;
            btnLock.IsChecked = false;
            if (sp.IsConnected) sp.WriteCmd("BK");//change from BK to RL Yupeng 04.2013 //change from RL to BK, Michael 08.20.2024
        }
        private void btnLock_Click(object sender, RoutedEventArgs e)
        {
            btnBackdrivable.IsChecked = false;
            btnLock.IsChecked = true;
            if (sp.IsConnected) sp.WriteCmd("LK"); //added Michael, 08.2024
            //{
                //sp.WriteCmd("ML10"); //deleted, Michael 08.2024
                //sp.WriteCmd("PS" + ((int)-sliderLockPosition.Value).ToString()); //deleted, Michael, 08.2024
                //sp.WriteCmd("DT38"); //change from 30 to 38 , Yupeng, 04.2013 //deleted, Michael, 08.2024
                //sp.WriteCmd("PT38"); //change from 30 to 38 , Yupeng, 04.2013 //deleted, Michael, 08.2024
                //sp.WriteCmd("SC"); //deleted, Michael, 08.2024
            //}
        }

        private void Check_CurrentUI()
        {
            switch ((tabEvaluation.SelectedItem as TabItem).Name)
            {
                case "tabAROM":
                    measureMode = "AROM";
                    if (vAROM.Visibility == Visibility.Visible) currentUI = vAROM;
                    else currentUI = hAROM;
                    currentUI.Set_DataMode(DataInfo.DataMode.Position);
                    break;

                case "tabStrength":
                    measureMode = "Strength";
                    if (hStrength.Visibility == Visibility.Visible) currentUI = hStrength;
                    //else currentUI = vStrength;
                    currentUI.Set_DataMode(DataInfo.DataMode.Torque);


                    if (btnLock.IsChecked == true && sp.IsConnected) sp.WriteCmd("LK");
                    break;

                default:
                    break;
            }
        }

        private void sliderResistance_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (sp.IsConnected)
            {
                if (sliderResistFlex.Value == 0 && sliderResistExt.Value == 0)
                    sp.WriteCmd("BK"); // No loading => backdrivable
                else
                {
                    sp.WriteCmd("FC"); // Friction control
                    sp.WriteCmd($"FP{sliderResistFlex.Value}"); // Set Dorsi/flexion Resistance            
                    sp.WriteCmd($"FD{sliderResistExt.Value}"); // Set plantar/extension Resistance
                }
            }
            e.Handled = true;
        }

        private void tabItems_Enabled(bool IsEnabled)
        {
            foreach (TabItem item in tabEvaluation.Items)
            {
                if (!item.IsSelected) item.IsEnabled = IsEnabled;  // Disable other tab items except current one
            }
        }

        private void cbDisplayEmg_Changed(object sender, RoutedEventArgs e)
        {
            if (intelliProtocol.General.Joint == Protocols.Joint.Ankle | intelliProtocol.General.Joint == Protocols.Joint.Knee)
            {
                if (cbDisplayEmg.IsChecked == true)
                {
                    //vPlot1.Visibility = Visibility.Visible;
                    //vPlot2.Visibility = Visibility.Visible;

                    //strength_v_FlexionGrid.SetValue(Grid.ColumnProperty, 1);
                    //strength_v_ExtensionGrid.SetValue(Grid.ColumnProperty, 1);

                    //vStrength.SetValue(Grid.ColumnSpanProperty, 1);
                    btnRecord.Visibility = Visibility.Visible;
                }
                else
                {
                    //vPlot1.Visibility = Visibility.Collapsed;
                    //vPlot2.Visibility = Visibility.Collapsed;

                    //strength_v_FlexionGrid.SetValue(Grid.ColumnProperty, 2);
                    //strength_v_ExtensionGrid.SetValue(Grid.ColumnProperty, 2);

                    //vStrength.SetValue(Grid.ColumnSpanProperty, 2);
                    btnRecord.Visibility = Visibility.Collapsed;
                }
            }
            else if (intelliProtocol.General.Joint == Protocols.Joint.Elbow | intelliProtocol.General.Joint == Protocols.Joint.Wrist)
            {
                if (cbDisplayEmg.IsChecked == true)
                {
                    hPlot1.Visibility = Visibility.Visible;
                    hPlot2.Visibility = Visibility.Visible;

                    strength_h_FlexionGrid.SetValue(Grid.ColumnSpanProperty, 1);
                    strength_h_ExtensionGrid.SetValue(Grid.ColumnSpanProperty, 1);

                    //strength_h_flexionBorder.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    //strength_h_extensionBorder.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

                    btnRecord.Visibility = Visibility.Visible;
                }
                else
                {
                    hPlot1.Visibility = Visibility.Collapsed;
                    hPlot2.Visibility = Visibility.Collapsed;

                    strength_h_FlexionGrid.SetValue(Grid.ColumnSpanProperty, 2);
                    strength_h_ExtensionGrid.SetValue(Grid.ColumnSpanProperty, 2);

                    //strength_h_ExtensionGrid.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    //strength_h_FlexionGrid.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

                    if (intelliProtocol.General.JointSide == Protocols.JointSide.Left)
                    {
                        //strength_h_ExtensionGrid.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                        //strength_h_FlexionGrid.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                    
                        //strength_h_FlexionGrid.SetValue(Grid.ColumnProperty, 1);
                        //strength_h_ExtensionGrid.SetValue(Grid.ColumnProperty, 0);
                    }
                    else if (intelliProtocol.General.JointSide == Protocols.JointSide.Right)
                    {
                        //strength_h_ExtensionGrid.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                        //strength_h_FlexionGrid.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                
                        //strength_h_FlexionGrid.SetValue(Grid.ColumnProperty, 0);
                        //strength_h_ExtensionGrid.SetValue(Grid.ColumnProperty, 1);
                        // Console.WriteLine("Entered");
                    }
                }
            }
        }

        private void btnFlexion_Click(object sender, RoutedEventArgs e)
        {
            btnFlexion.IsChecked = true;
            btnExtension.IsChecked = false;
        }

        private void btnExtension_Click(object sender, RoutedEventArgs e)
        {
            btnFlexion.IsChecked = false;
            btnExtension.IsChecked = true;
        }
    }
}
