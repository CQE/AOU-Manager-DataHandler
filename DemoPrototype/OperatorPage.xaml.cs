using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace DemoPrototype
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OperatorPage : Page
    {
        private bool isInitSelection = true; // ToDo. Better check
        Timer updateTimer;
        DispatcherTimer dTimer;

        public OperatorPage()
        {
            this.InitializeComponent();

            InitBackgroundTimer();

            InitDispatcherTimer();

        }

        private void InitDispatcherTimer()
        {
            dTimer = new DispatcherTimer();
            dTimer.Tick += UpdateTick;
            dTimer.Interval = new TimeSpan(0, 0, 1);
            dTimer.Start();
        }

        void UpdateTick(object sender, object e)
        {
            PhaseShiftValue.Text = DateTime.Now.ToString("mm:ss");
            // dTimer.Stop();
        }


        private void InitBackgroundTimer()
        {
            AutoResetEvent autoEvent = new AutoResetEvent(false);
            StatusChecker statusChecker = new StatusChecker();

            // Create an inferred delegate that invokes methods for the timer.
            TimerCallback tcb = statusChecker.CheckStatus;

            // Create a timer that signals the delegate to invoke 
            updateTimer = new Timer(tcb, autoEvent, 1000, 1000);
        }

        private async void modalDlg(string title, string message)
        {
            var dlg = new ContentDialog();
            dlg.Title = title;
            dlg.Content = message;
            dlg.PrimaryButtonText = "Enable";
            dlg.SecondaryButtonText = "Cancel";

            ContentDialogResult res = await dlg.ShowAsync();
            if (res == ContentDialogResult.Primary)
            {
                switch (title)
                {
                    //kommunicera med PLC?
                    case "": break; // example. sendToPLC 
                }

            }

        }

        private void ShowHotTankSlider(object sender, RoutedEventArgs e)
        {
       //     SetHotTankSlider.Visibility="True";
        }

        private void NewModeSelected(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem item = (ComboBoxItem)e.AddedItems[0];
            string modeTitle = item.Content.ToString();
            string message = "You are about to change running mode";
            if (!isInitSelection)
            {
                modalDlg(modeTitle, message);
            }
            else
            {
                isInitSelection = false;
            }
        }

        private void PhaseShiftButton(object sender, RoutedEventArgs e)
        {
            //Just testing
            double firstSlope = (double)PhaseVLine1.X1;

            PhaseShiftResultText.Text = firstSlope.ToString();
            PhaseShiftValue.Text = 4.ToString();
        }

        private void PhaseLine1_Dragged(object sender, Syncfusion.UI.Xaml.Charts.AnnotationDragCompletedEventArgs e)
        {
            //Urban please replace this code with code showing diff between the lines, and center the Chartstripline
        }

        private void PhaseLine2_Dragged(object sender, Syncfusion.UI.Xaml.Charts.AnnotationDragCompletedEventArgs e)
        {
            //Urban please replace this code with code showing diff between the lines, and center the Chartstripline
        }
    }


    class StatusChecker
    {
        private int count;

        public StatusChecker()
        {
            count = 0;
        }

        // This method is called by the timer delegate.
        public void CheckStatus(Object stateInfo)
        {
            count++;
            string curTime = DateTime.Now.ToString("hh:mm:ss");

            if (false) {
                //AutoResetEvent autoEvent = (AutoResetEvent)stateInfo;
                // Reset and signal Main.
                // autoEvent.Set();
            }

        }
    }


}
