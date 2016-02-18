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

        private DispatcherTimer dTimerUpdateData;
        private DispatcherTimer dTimerUpdateText;

        string filenamestr = "";
        int numRandom = 30;
        int msBetween = 1000;

        public MainPage()
        {
            this.InitializeComponent();

            comboBox.Items.Add("File");
            comboBox.Items.Add("Serial");
            comboBox.Items.Add("Random");

            dTimerUpdateData = new DispatcherTimer();
            dTimerUpdateData.Tick += UpdateDataTick;
            dTimerUpdateData.Interval = new TimeSpan(0, 0, 0, 0, 250);

            dTimerUpdateText = new DispatcherTimer();
            dTimerUpdateText.Tick += UpdateTextTick;
            dTimerUpdateText.Interval = new TimeSpan(0, 0, 0, 1, 0);

            dTimerUpdateData.Start();
            dTimerUpdateText.Start();
        }

        void UpdateDataTick(object sender, object e)
        {
            if (router != null)
            {
                textBox.Text += router.GetLogStr(); // Show if new log messages
                router.Update();
            }
        }

        void UpdateTextTick(object sender, object e)
        {
            if (router != null)
            {
                if (router.runMode == AOURouter.RunType.Random)
                {
                    textBox.Text += GetNewRandomText();
                }
                textBox.Text += router.GetLogStr(); // Show if new log messages
            }
        }

        private void InitSerialCom()
        {
            string comsettings = "COM3, 9600";
            if (this.fileName.Text.ToLower().StartsWith("com"))
            {
                comsettings = this.fileName.Text;
            }
            else
            {
                this.fileName.Text = comsettings;
            }
            pickFileButton.Visibility = Visibility.Visible;
            pickFileButton.Content = "Start";
        }

        private void TestLogAndPowerLists()
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

            if (pwrList.Count > 0)
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

        private void TestSerialData()
        {
            TestLogAndPowerLists();
        }


        private void TestRandomData()
        {
            TestLogAndPowerLists();
        }


        private void StopSerialData()
        {
//            router.s;
            pickFileButton.Content = "Start";
        }

        private void StartSerialData()
        {
            router = new AOURouter(AOURouter.RunType.Serial, fileName.Text);
            pickFileButton.Content = "Stop";
        }

        private void StopRandomData()
        {
//            router.s;
            pickFileButton.Content = "Start";
        }

        private void StartRandomData()
        {
            router = new AOURouter(AOURouter.RunType.Random, fileName.Text);
            pickFileButton.Content = "Stop";
        }

        private void InitRandomData()
        {
            string randomSettings = String.Format("{0}, {1}", numRandom, msBetween);
            if (fileName.Text.Length  > 0 && fileName.Text[0] >= '1' && fileName.Text[0] <= '9')
            {
                randomSettings = this.fileName.Text;
            }
            else
            {
                this.fileName.Text = randomSettings;
            }
            pickFileButton.Visibility = Visibility.Visible;
            pickFileButton.Content = "Start";
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
                filenamestr = file.Path.Substring(file.Path.IndexOf("Pictures") + ("Pictures").Length);
                fileName.Text = filenamestr;
                router = new AOURouter(AOURouter.RunType.File, fileName.Text);
            }
        }

        private void PickFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (comboBox.SelectedIndex == 0)
                PickFile();
            else if (comboBox.SelectedIndex == 1 && pickFileButton.Content == "Start")
                StartSerialData();
            else if (comboBox.SelectedIndex == 1)
                StopSerialData();
            else if (comboBox.SelectedIndex == 2 && pickFileButton.Content == "Start")
                StartRandomData();
            else if (comboBox.SelectedIndex == 2)
                StopRandomData();
        }

        private string GetNewRandomText()
        {
            string text = "";

            return text;
        }

        private void TestLoadedFile()
        {
            pickFileButton.Visibility = Visibility.Visible;
            if (filenamestr.Length == 0)
            {
                PickFile();
            }
            else
            {
                TestLogAndPowerLists();
            }
        }

        private void CreateFileButton_Click(object sender, RoutedEventArgs e)
        {
            this.textBox.Text = AOURandomData.CreateRandomXML(30, 0, 1000);
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (comboBox.SelectedIndex)
            {
                case 0: TestLoadedFile(); break;
                case 1: InitSerialCom(); break;
                case 2: InitRandomData();  break;
            }
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            switch (comboBox.SelectedIndex)
            {
                case 0: TestLoadedFile(); break;
                case 1: TestSerialData(); break;
                case 2: TestRandomData(); break;
            }
        }
    }
}
