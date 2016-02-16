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
using DataHandler;
using Windows.Storage.Pickers;
using Windows.Storage;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DataHandlerTestApp
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
{ 
    public sealed partial class MainPage : Page
    {
        AOURouter router;

        private DispatcherTimer dTimer;

        public MainPage()
        {
            this.InitializeComponent();

            dTimer = new DispatcherTimer();
            dTimer.Tick += UpdateTick;
            dTimer.Interval = new TimeSpan(0, 0, 1);
            dTimer.Start();
       }

        void UpdateTick(object sender, object e)
        {
            if (router != null)
            {
                textBox.Text += router.GetLogStr();
                router.Update();
                /*
                if (router.IsDataAvailable())
                {
                    textBox.Text += router.GetTextData() + "\r\n";
                }
                */
            }
        }

        private void TestSerialComButton_Click(object sender, RoutedEventArgs e)
        {
            router = new AOURouter(AOURouter.RunType.Serial, "com3, 9600");
            loadFileButton.Content = "Send Data";
        }

        private void TestRandomDataButton_Click(object sender, RoutedEventArgs e)
        {
            router = new AOURouter(AOURouter.RunType.Random, "30, 1000");

            AOULogMessage[] msg = router.GetLastLogMessages(10);
            Power[] pwr = router.GetLastPowerValues(10);

            for (int i = 0; i < 160; i++)
            {
                router.Update();
            }

            bool bp = router.NewPowerDataIsAvailable();
            Power p = router.GetLastPowerValue();

            bool bm = router.NewLogMessagesAreAvailable();
            AOULogMessage[] msgn = router.GetNewLogMessages();

            AOULogMessage[] msg2 = router.GetLastLogMessages(10);
            Power[] pwr2 = router.GetLastPowerValues(10);

            AOULogMessage[] msg3 = router.GetLastLogMessages(100);
            Power[] pwr3 = router.GetLastPowerValues(100);

            bool ok = bm == bp;
        }

        private async void PickFile()
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = PickerViewMode.List;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary; // Todo: usb, cloud
            picker.FileTypeFilter.Add(".txt");

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                // Save only path relative to User Pictures folder
                fileName.Text = file.Path.Substring(file.Path.IndexOf("Pictures") + ("Pictures").Length);
                router = new AOURouter(AOURouter.RunType.File, fileName.Text);
            }
        }

        private void LoadFileButton_Click(object sender, RoutedEventArgs e)
        {
            PickFile();
        }

        private void TestLoadedFileButton_Click(object sender, RoutedEventArgs e)
        {
            var logList = router.GetLastLogMessages(30);
            var pwrList = router.GetLastPowerValues(30);
            if (logList.Length > 0)
            {
                foreach (var log in logList)
                {
                    this.textBox.Text += String.Format("Log time:{0}, Log:{1}\r\n", log.time, log.message);
                }
            }
            else
            {
                this.textBox.Text += "No Log data\r\n";
            }

            if (pwrList.Length> 0)
            {
                foreach (var power in pwrList)
                {
                    this.textBox.Text += String.Format("Time:{0}, State:{1}", power.ElapsedTime, power.State);
                    this.textBox.Text += String.Format(", Hot tank temp:{0}, Cold tank temp:{1}, cool temp:{2}", power.THotTank, power.TColdTank, 0);
                    this.textBox.Text += String.Format(", Return Actual:{0}, Return Forecasted:{1}, Valve return:{2}\r\n", power.TReturnActual, power.TReturnForecasted, power.TReturnValve);
                    //this.textBox.Text += String.Format("Valves time:{0}, valvesRetPrev:{1}, valvesRetNew:{2}\r\n");
                }
            }
            else
            {
                this.textBox.Text += "No Power data\r\n";
            }
        }

        private void CreateFileButton_Click(object sender, RoutedEventArgs e)
        {
            this.textBox.Text = AOURandomData.CreateRandomXML(30, 0, 1000);
        }

    }
}
