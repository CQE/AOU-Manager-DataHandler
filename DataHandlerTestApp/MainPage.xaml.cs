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
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private void AOURoterTest()
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


        public MainPage()
        {
            this.InitializeComponent();

            //AOUSerialData sdata = new AOUSerialData();

            long time_ms = 0;
            double hot_tank_temp = 0;
            double cold_tank_temp = 0;
            double valve_return_temp = 0;
            double cool_temp = 0;

            bool ok = AOUInputParser.ParseTemperature("", out time_ms, out hot_tank_temp,out cold_tank_temp, out valve_return_temp, out cool_temp);

            int x = 0;
        }
    }
}
