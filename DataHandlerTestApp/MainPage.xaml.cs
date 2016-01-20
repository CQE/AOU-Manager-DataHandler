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
        public MainPage()
        {
            this.InitializeComponent();

            AOURouter r = new AOURouter(AOURouter.RunType.Random);

            AOULogMessage[] msg = r.GetLastLogMessages(10);
            Power[] pwr = r.GetLastPowerValues(10);

            for (int i = 0; i < 300; i++)
                r.Update();
            
            Power p = r.GetLastPowerValue();
            bool bp = r.NewPowerDataIsAvailable();

            bool bm = r.NewLogMessagesAreAvailable();
            AOULogMessage[] msgn = r.GetNewLogMessages();

            AOULogMessage[] msg2 = r.GetLastLogMessages(10);
            Power[] pwr2 = r.GetLastPowerValues(10);

            bool ok = bm == bp;
        }
    }
}
