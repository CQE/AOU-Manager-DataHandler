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
using Windows.ApplicationModel.DataTransfer;

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

        AOUSettings.FileSetting fileSetting;
        AOUSettings.RandomSetting randomSetting;
        AOUSettings.SerialSetting serialSetting;
        AOUSettings.RemoteSetting remoteSetting;

        const string fileSettingStr = "pictures,";
        const string randomSettingStr = "30,1000";
        const string serialSettingStr = "COM5,115200";
        const string remoteSettingStr = "Todo";

        const string DataSourceFileStr = "File";
        const string DataSourceSerialStr = "Serial";
        const string DataSourceRandomStr = "Random";
        const string DataSourceRemoteClientStr = "Remote Client";

        const string StreamNothingStr = "No streaming";
        const string StreamLogStr = "Log data stream";
        const string StreamPowerStr = "Power data stream";
        const string StreamBothStr = "Stream both Log and Power data";
        const string StreamRealDataStr = "Real data stream";

        public MainPage()
        {
            this.InitializeComponent();

            fileSetting = TextToFileSetting(fileSettingStr);
            randomSetting = TextToRandomSetting(randomSettingStr);
            serialSetting = TextToSerialSetting(serialSettingStr);
            remoteSetting = TextToRemoteSetting(remoteSettingStr);

            comboBoxDataSource.Items.Add(DataSourceFileStr);
            comboBoxDataSource.Items.Add(DataSourceSerialStr);
            comboBoxDataSource.Items.Add(DataSourceRandomStr);
            comboBoxDataSource.Items.Add(DataSourceRemoteClientStr);

            comboBoxStreaming.Items.Add(StreamNothingStr);
            comboBoxStreaming.Items.Add(StreamLogStr);
            comboBoxStreaming.Items.Add(StreamPowerStr);
            comboBoxStreaming.Items.Add(StreamBothStr);
            comboBoxStreaming.Items.Add(StreamRealDataStr);

            dTimerUpdateData = new DispatcherTimer();
            dTimerUpdateData.Tick += UpdateDataTick;
            dTimerUpdateData.Interval = new TimeSpan(0, 0, 0, 0, 250);
            dTimerUpdateData.Start();

            dTimerUpdateText = new DispatcherTimer();
            dTimerUpdateText.Tick += UpdateTextTick;
            dTimerUpdateText.Interval = new TimeSpan(0, 0, 0, 1, 0);
        }

        void UpdateDataTick(object sender, object e)
        {
            if (router != null)
            {
                router.Update();
                string log = router.GetLogStr();
                if (log.Length > 0)
                    textBox.Text += log + Environment.NewLine; //  if new router log messages
            }
        }

        private void AddLogMessagesToTextBox()
        {
            if (router.NewLogMessagesAreAvailable())
            {
                var logs = router.GetNewLogMessages();
                foreach (var log in logs)
                {
                    textBox.Text += log.ToString() + Environment.NewLine;
                }
            }
        }

        private void AddPowerValuesToTextBox(bool onlyLast = true)
        {
            if (router.NewPowerDataIsAvailable())
            {
                if (onlyLast)
                { 
                    var power = router.GetLastNewPowerValue();
                    textBox.Text += router.GetLastNewPowerValue().ToString() + Environment.NewLine;
                }
                else
                {
                    var powers = router.GetLastPowerValues(30);
                    foreach (var power in powers)
                    {
                        textBox.Text += power.ToString() + Environment.NewLine;
                    }
                }
            }
        }


        void UpdateTextTick(object sender, object e)
        {
            string errStr = "No valid streaming mode selected";
            
            if (router != null)
            {
                // Show new data in text box depending on selection
                if (comboBoxStreaming.SelectedValue == null) 
                {
                    if (!textBox.Text.Contains(errStr)) // No repeated messages
                    {
                        textBox.Text += errStr + Environment.NewLine; ;
                    }
                }
                else
                { 
                    switch (comboBoxStreaming.SelectedValue.ToString())
                    {
                        case StreamNothingStr:
                            if (textBox.Text.Contains("Stream Nothing")) // No repeated messages
                            {
                                textBox.Text += "Stream Nothing" + Environment.NewLine; ;
                            }
                            break; // No textBox.Text
                        case StreamLogStr:
                            AddLogMessagesToTextBox();
                            break;
                        case StreamPowerStr:
                            AddPowerValuesToTextBox();
                            break;
                        case StreamBothStr:
                            AddLogMessagesToTextBox();
                            AddPowerValuesToTextBox();
                            break;
                        case StreamRealDataStr:
                            textBox.Text += router.GetRawData();
                            break;
                        default:
                            if (!textBox.Text.Contains(errStr)) // No repeated messages
                            {
                                textBox.Text += errStr + Environment.NewLine;
                            }
                            break;
                    }
                }
            }
            else
            {
                if (!textBox.Text.Contains(errStr)) // No repeated messages
                {
                    textBox.Text += errStr + Environment.NewLine;
                }
            }
        }

        private void PickFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxDataSource.SelectedIndex == 0) PickFile();
        }

        private void comboBoxDataSource_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dTimerUpdateText.Stop();
            TextButton.IsEnabled = false;
            XMLButton.IsEnabled = false;
            switch (comboBoxDataSource.SelectedIndex)
            {
                case 0:
                    InitFileInput();
                    break;
                case 1:
                    InitSerialCom();
                    break;
                case 2:
                    InitRandomData();
                    TextButton.IsEnabled = true;
                    XMLButton.IsEnabled = true;
                    break;
            }
            if (comboBoxDataSource.SelectedIndex >= 0)
            {
                StartStopButton.Content = "Start";
            }
            else
            {
                StartStopButton.Content = "Select";
            }
        }

        private void comboBoxStreaming_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Dont needed for the moment
        }

        private void StartStopButton_Click(object sender, RoutedEventArgs e)
        {
            if (StartStopButton.Content.ToString() == "Start")
            {
                switch (comboBoxDataSource.SelectedIndex)
                {
                    case 0: StartFileData(); break;
                    case 1: StartSerialData(); break;
                    case 2: StartRandomData(); break;
                }

                textBox.Text = "";
                dTimerUpdateText.Start();
                StartStopButton.Content = "Stop";
                comboBoxDataSource.IsEnabled = false;
                dataSettings.IsEnabled = false;
            }
            else if (StartStopButton.Content.ToString() == "Stop")
            {
                dTimerUpdateText.Stop();
                switch (comboBoxDataSource.SelectedIndex)
                {
                    case 0: StopFileData(); break;
                    case 1: StopSerialData(); break;
                    case 2: StopRandomData(); break;
                }
                comboBoxDataSource.IsEnabled = true;
                dataSettings.IsEnabled = true;
                StartStopButton.Content = "Start";
            }
        }

        /* Init */
        private void InitFileInput()
        {
            dataSettings.Text = fileSettingStr;
            pickFileButton.Visibility = Visibility.Visible;
            if (dataSettings.Text.Length == 0) PickFile();
        }

        private void InitSerialCom()
        {
            dataSettings.Text = serialSettingStr;
            pickFileButton.Visibility = Visibility.Collapsed;
        }

        private void InitRandomData()
        {
            dataSettings.Text = randomSettingStr;
            pickFileButton.Visibility = Visibility.Collapsed;
        }

        private string FileSettingToText(AOUSettings.FileSetting setting)
        {
            return String.Format("{0},{1}", setting.SourceType, setting.FilePath);
        }

        private string SerialSettingToText(AOUSettings.SerialSetting setting)
        {
            return String.Format("{0},{1}", setting.ComPort, setting.BaudRate);
        }

        private string RandomSettingToText(AOUSettings.RandomSetting setting)
        {
            return String.Format("{0},{1}", setting.NumValues, setting.MsBetween);
        }

        private AOUSettings.FileSetting TextToFileSetting(string text)
        {
            string[] arr = text.Split(',');
            return new AOUSettings.FileSetting(arr[0], (arr[1]));
        }

        private AOUSettings.SerialSetting TextToSerialSetting(string text)
        {
            string[] arr = text.Split(',');
            return new AOUSettings.SerialSetting(arr[0], uint.Parse(arr[1]));
        }

        private AOUSettings.RandomSetting TextToRandomSetting(string text)
        {
            string[] arr = text.Split(',');
            return new AOUSettings.RandomSetting(uint.Parse(arr[0]), uint.Parse(arr[1]));
        }

        private AOUSettings.RemoteSetting TextToRemoteSetting(string text)
        {
            string[] arr = text.Split(',');
            if (arr.Length  == 4)
            {
                return new AOUSettings.RemoteSetting(arr[0], arr[1], arr[2], arr[3]);
            }
            return new AOUSettings.RemoteSetting("", "", "", "");
        }


        /* Start */
        private void StartFileData()
        {
            router = new AOURouter(TextToFileSetting(dataSettings.Text));
        }

        private void StartSerialData()
        {
            if (comboBoxStreaming.SelectedIndex == -1 || comboBoxStreaming.SelectedValue.ToString() == StreamRealDataStr)
                router = new AOURouter(TextToSerialSetting(dataSettings.Text), AOUSettings.DebugMode.rawData);
            else
                router = new AOURouter(TextToSerialSetting(dataSettings.Text), AOUSettings.DebugMode.noDebug);
        }

        private void StartRandomData()
        {
            router = new AOURouter(TextToRandomSetting(dataSettings.Text));
            TextButton.IsEnabled = true;
            XMLButton.IsEnabled = true;
        }


        /* Stop*/
        private void StopFileData()
        {
            router.Stop();
            router = null;
        }

        private void StopSerialData()
        {
            router.Stop();
            router = null;
        }

        private void StopRandomData()
        {
            router.Stop();
            router = null;
        }


        /* Test */
        private void TestFileData()
        {
            TestLogAndPowerLists();
        }

        private void TestSerialData()
        {
            TestLogAndPowerLists();
        }


        private void TestRandomData()
        {
            TestLogAndPowerLists();
        }

        private void TextButton_Click(object sender, RoutedEventArgs e)
        {
            this.textBox.Text = AOURandomData.CreateRandomText(30, 0, 1000);
        }

        private void XMLButton_Click(object sender, RoutedEventArgs e)
        {
            this.textBox.Text = AOURandomData.CreateRandomXML(30, 0, 1000);
        }

        // Todo
        private void TestLogAndPowerLists()
        {
            // Todo check
            if (router != null)
            {
                var logList = router.GetLastLogMessages(100);
                var pwrList = router.GetLastPowerValues(300);
                if (logList.Length > 0)
                {
                    foreach (var log in logList)
                    {
                        this.textBox.Text += String.Format("Log time:{0}, {1}, msg:{2}" + Environment.NewLine, log.time, log.prio, log.message);
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
                        TimeSpan ts = TimeSpan.FromMilliseconds(power.ElapsedTime);
                        this.textBox.Text += String.Format("Time:{0:c}({1}), State:{2}", ts, power.ElapsedTime, power.State);
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
            else
            {
                this.textBox.Text += "No type selected\r\n";
            }
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
                string fileNameStr = file.Path.Substring(file.Path.IndexOf("Pictures") + ("Pictures").Length);
                dataSettings.Text = "Pictures," + fileNameStr;
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            textBox.Text = "";
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            DataPackage dataPackage = new DataPackage();
            dataPackage.SetText(textBox.Text);
            Clipboard.SetContent(dataPackage);
        }
    }

}
