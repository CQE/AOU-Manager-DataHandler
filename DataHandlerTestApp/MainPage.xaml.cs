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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DataHandlerTestApp
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
{ 
    public sealed partial class MainPage : Page
    {
        AOURouter router;

        public MainPage()
        {
            this.InitializeComponent();
            router = new AOURouter(AOURouter.RunType.File);
        }

        private void AOUSerielTest()
        {
            if (router.IsFileDataAvailable())
            {
                this.textBox.Text = router.GetFileData();
            }
            else
            {
                this.textBox.Text = "File not loaded";
            }
        }

        private void TestLoadedFileButton_Click(object sender, RoutedEventArgs e)
        {

            if (router.IsFileDataAvailable())
            {
                List<AOULogMessage> logList;
                List<Power> pwrList = router.GetFileDataList(out logList);
                if (logList.Count > 0)
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
            else
            {
                this.textBox.Text = "No Filedata loaded\r\n";
            }
        }

        private void TestRandomData_Click(object sender, RoutedEventArgs e)
        {
            AOURouter r = new AOURouter(AOURouter.RunType.Random);

            AOULogMessage[] msg = r.GetLastLogMessages(10);
            Power[] pwr = r.GetLastPowerValues(10);

            r.Update(40);

            Power p = r.GetLastPowerValue();
            bool bp = r.NewPowerDataIsAvailable();

            bool bm = r.NewLogMessagesAreAvailable();
            AOULogMessage[] msgn = r.GetNewLogMessages();

            AOULogMessage[] msg2 = r.GetLastLogMessages(10);
            Power[] pwr2 = r.GetLastPowerValues(10);

            bool ok = bm == bp;
        }

        private void LoadFileButton_Click(object sender, RoutedEventArgs e)
        {
            router.LoadTestFile(this.fileName.Text);
        }

        private void TestSerialComButton_Click(object sender, RoutedEventArgs e)
        {
            AOUSerialData sdata = new AOUSerialData("com3");

        }
    }
}
