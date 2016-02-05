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

        AOURouter router = new AOURouter(AOURouter.RunType.File);

        private void AOURoterRandomTest()
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

        public MainPage()
        {
            this.InitializeComponent();

            //AOUSerialData sdata = new AOUSerialData();
            router = new AOURouter(AOURouter.RunType.File);
        }

        private void FileTestButton_Click(object sender, RoutedEventArgs e)
        {
            long time_ms = 0;

            double hot_tank_temp = 0;
            double cold_tank_temp = 0;
            double valve_return_temp = 0;
            double cool_temp = 0;

            int seqState = 0;

            string logMsg = "";

            double valvesRetPrev = 0;
            double valvesRetNew = 0;

            double feedHotPrev = 0;
            double feedHotNew = 0;
            double feedColdPrev = 0;
            double feedColdNew = 0;

            int count = 0;
            bool ok = true;
            string nextTag = "dummy";

            if (router.IsFileDataAvailable())
            {
                string text = router.GetFileData();
                while (ok && nextTag.Length > 0 &&  text.Length > 0)
                {
                    count = 0;
                    nextTag = AOUInputParser.GetNextTag(text);
                    if (AOUInputParser.IsNextTag(AOUInputParser.tagTemperature, nextTag))
                    {
                        ok = AOUInputParser.ParseTemperature(text, out time_ms, out hot_tank_temp, out cold_tank_temp,
                                                                  out valve_return_temp, out cool_temp, out count);
                    }
                    else if (AOUInputParser.IsNextTag(AOUInputParser.tagSequence, nextTag))
                    {
                        ok = AOUInputParser.ParseSequence(text, out time_ms, out seqState, out count);
                    }
                    else if (AOUInputParser.IsNextTag(AOUInputParser.tagFeeds, nextTag))
                    {
                        ok = AOUInputParser.ParseFeeds(text, out time_ms, out feedHotPrev, out feedHotNew, out feedColdPrev, out feedColdNew, out count);
                    }
                    else if (AOUInputParser.IsNextTag(AOUInputParser.tagValves, nextTag))
                    {
                        ok = AOUInputParser.ParseValves(text, out time_ms, out valvesRetPrev, out valvesRetNew, out count);
                    }
                    else if (AOUInputParser.IsNextTag(AOUInputParser.tagLog, nextTag))
                    {
                        ok = AOUInputParser.ParseLog(text, out time_ms, out logMsg, out count);
                    }

                    if (ok)
                    {
                        if (AOUInputParser.IsNextTag(AOUInputParser.tagTemperature, nextTag))
                        {
                            this.textBox.Text += String.Format("Temperature time:{0}, hot tank:{1}, cold tank:{2}, valve return:{3}, cool temp:{4}\r\n",
                                                                time_ms, hot_tank_temp, cold_tank_temp, valve_return_temp, cool_temp);
                        }
                        else if (AOUInputParser.IsNextTag(AOUInputParser.tagSequence, nextTag))
                        {
                            this.textBox.Text += String.Format("Sequence time:{0},sequence:{1}\r\n", time_ms, seqState);
                        }
                        else if (AOUInputParser.IsNextTag(AOUInputParser.tagFeeds, nextTag))
                        {
                            this.textBox.Text += String.Format("Feeds time:{0}, hot tank:{1}, cold tank:{2}, valve return:{3}, cool temp:{4}\r\n",
                                                                time_ms, feedHotPrev, feedHotNew, feedColdPrev, feedColdNew);
                        }
                        else if (AOUInputParser.IsNextTag(AOUInputParser.tagValves, nextTag))
                        {
                            this.textBox.Text += String.Format("Valves time:{0}, valvesRetPrev:{1}, valvesRetNew:{2}\r\n",
                                                                time_ms, valvesRetPrev, valvesRetNew);
                        }
                        else if (AOUInputParser.IsNextTag(AOUInputParser.tagLog, nextTag))
                        {
                            this.textBox.Text += String.Format("Log time:{0}, Log:{1}\r\n", logMsg);
                        }
                        else
                        {
                            this.textBox.Text += "Tag not recognized: " + nextTag + "\r\n";
                        }
                    }
                    text = text.Substring(count);
                }

            }
            else
            {
                this.textBox.Text = "No Filedata loaded";
            }
        }
    }
}
