using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

using Windows.Web;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DemoPrototype
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            //MyFrame.Navigate(typeof(OperatorPage));
            TitleTextBlock.Text = "AOU Control System Main View";
            BackButton.Visibility = Visibility.Collapsed;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (MyFrame.CanGoBack)
            {
                MyFrame.GoBack();
            }
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        private void ListBox_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (OperatorListBox.IsSelected)
            {
                MyFrame.Navigate(typeof(OperatorPage));
                TitleTextBlock.Text = "Run Injection Moulding";
                BackButton.Visibility = Visibility.Collapsed;
            }
           
            else if (SettingsListBoxItem.IsSelected)
            {
                MyFrame.Navigate(typeof(SettingsPage));
                TitleTextBlock.Text = "HAOU Control System Settings View";
                BackButton.Visibility = Visibility.Visible;
            }
            else if (CalibrateListBoxItem.IsSelected)
            {
                MyFrame.Navigate(typeof(CalibratePage));
                TitleTextBlock.Text = "AOU Control System Calibrate View";
                BackButton.Visibility = Visibility.Visible;
            }
            else if (MaintenanceListBoxItem.IsSelected)
            {
                MyFrame.Navigate(typeof(MaintenancePage));
                TitleTextBlock.Text = "AOU Control System Maintenance View";
                BackButton.Visibility = Visibility.Visible;
            }
            else if (HelpListBoxItem.IsSelected)
            {
                TitleTextBlock.Text = "AOU Control System Help";
                BackButton.Visibility = Visibility.Collapsed;
                DisplayHtml("Help");
            }
            else if (AboutListBoxItem.IsSelected)
            {
                TitleTextBlock.Text = "About AOU Control System";
                BackButton.Visibility = Visibility.Collapsed;
                DisplayHtml("About");
            }
        }

        public void DisplayHtml(string htmlPage)
        {
            WebView webView = new WebView();
            Uri url = webView.BuildLocalStreamUri("page3", "/Assets/"+ htmlPage + ".html");
            StreamUriWinRTResolver myResolver = new StreamUriWinRTResolver();
            webView.NavigateToLocalStreamUri(url, myResolver);
            MyFrame.Content = webView;
        }

    }

    public class LineChartViewModel
        //Our class for handling chart data
    {
        public LineChartViewModel()
        {
            //initialise
            long MyTime = 0; //should read time from server here
            this.power = new ObservableCollection<Power>();
            //add some datapoints
            power.Add(new Power() { ElapsedTime = MyTime + 1, THotTank = 220, TCoolTank = 45, TReturnValve = 37, TReturnActual = 45, TReturnForecasted = 67, State=7   });
            power.Add(new Power() { ElapsedTime = MyTime + 2, THotTank = 227, TCoolTank = 41, TReturnValve = 87, TReturnActual = 43, TReturnForecasted = 77, State =7 });
            power.Add(new Power() { ElapsedTime = MyTime + 3, THotTank = 235, TCoolTank = 37, TReturnValve = 77, TReturnActual = 45, TReturnForecasted = 67, State = 6 });
            power.Add(new Power() { ElapsedTime = MyTime + 4, THotTank = 215, TCoolTank = 33, TReturnValve = 87, TReturnActual = 46, TReturnForecasted = 61, State = 5 });
            power.Add(new Power() { ElapsedTime = MyTime + 5, THotTank = 204, TCoolTank = 40, TReturnValve = 77, TReturnActual = 43, TReturnForecasted = 77, State = 4 });
            power.Add(new Power() { ElapsedTime = MyTime + 6, THotTank = 205, TCoolTank = 39, TReturnValve = 67, TReturnActual = 42, TReturnForecasted = 87, State = 3 });
            power.Add(new Power() { ElapsedTime = MyTime + 7, THotTank = 204, TCoolTank = 38, TReturnValve = 57, TReturnActual = 46, TReturnForecasted = 61, State = 3 });
            power.Add(new Power() { ElapsedTime = MyTime + 8, THotTank = 204, TCoolTank = 35, TReturnValve = 58, TReturnActual = 42, TReturnForecasted = 87, State = 2 });
            power.Add(new Power() { ElapsedTime = MyTime + 9, THotTank = 214, TCoolTank = 31, TReturnValve = 59, TReturnActual = 41, TReturnForecasted = 63, State = 2 });
            power.Add(new Power() { ElapsedTime = MyTime + 10, THotTank = 224, TCoolTank = 38, TReturnValve = 57, TReturnActual = 41, TReturnForecasted = 63, State =2 });
            power.Add(new Power() { ElapsedTime = MyTime + 11, THotTank = 220, TCoolTank = 45, TReturnValve = 37, TReturnActual = 42, TReturnForecasted = 87, State = 7 });
            power.Add(new Power() { ElapsedTime = MyTime + 12, THotTank = 227, TCoolTank = 41, TReturnValve = 87, TReturnActual = 40, TReturnForecasted = 45, State = 7 });
            power.Add(new Power() { ElapsedTime = MyTime + 13, THotTank = 235, TCoolTank = 37, TReturnValve = 77, TReturnActual = 41, TReturnForecasted = 44, State = 7 });
            power.Add(new Power() { ElapsedTime = MyTime + 14, THotTank = 215, TCoolTank = 33, TReturnValve = 87, TReturnActual = 42, TReturnForecasted = 43, State = 7 });
            power.Add(new Power() { ElapsedTime = MyTime + 15, THotTank = 204, TCoolTank = 40, TReturnValve = 77, TReturnActual = 43, TReturnForecasted = 42, State = 7 });
            power.Add(new Power() { ElapsedTime = MyTime + 16, THotTank = 205, TCoolTank = 39, TReturnValve = 67, TReturnActual = 44, TReturnForecasted = 41, State = 7 });
            power.Add(new Power() { ElapsedTime = MyTime + 17, THotTank = 204, TCoolTank = 38, TReturnValve = 57, TReturnActual = 45, TReturnForecasted = 40, State = 7 });
            power.Add(new Power() { ElapsedTime = MyTime + 18, THotTank = 204, TCoolTank = 35, TReturnValve = 58, TReturnActual = 44, TReturnForecasted = 41 });
            power.Add(new Power() { ElapsedTime = MyTime + 19, THotTank = 214, TCoolTank = 31, TReturnValve = 59, TReturnActual = 43, TReturnForecasted = 42 });
            power.Add(new Power() { ElapsedTime = MyTime + 20, THotTank = 224, TCoolTank = 38, TReturnValve = 57, TReturnActual = 43, TReturnForecasted = 42 });
            power.Add(new Power() { ElapsedTime = MyTime + 21, THotTank = 220, TCoolTank = 45, TReturnValve = 37, TReturnActual = 43, TReturnForecasted = 42 });
            power.Add(new Power() { ElapsedTime = MyTime + 22, THotTank = 227, TCoolTank = 41, TReturnValve = 87, TReturnActual = 43, TReturnForecasted = 60 });
            power.Add(new Power() { ElapsedTime = MyTime + 23, THotTank = 235, TCoolTank = 37, TReturnValve = 77, TReturnActual = 43, TReturnForecasted = 42 });
            power.Add(new Power() { ElapsedTime = MyTime + 24, THotTank = 215, TCoolTank = 33, TReturnValve = 87, TReturnActual = 43, TReturnForecasted = 60 });
            power.Add(new Power() { ElapsedTime = MyTime + 25, THotTank = 204, TCoolTank = 40, TReturnValve = 77, TReturnActual = 43, TReturnForecasted = 42 });
            power.Add(new Power() { ElapsedTime = MyTime + 26, THotTank = 205, TCoolTank = 39, TReturnValve = 67, TReturnActual = 43, TReturnForecasted = 42 });
            power.Add(new Power() { ElapsedTime = MyTime + 27, THotTank = 204, TCoolTank = 38, TReturnValve = 57, TReturnActual = 43, TReturnForecasted = 60 });
            power.Add(new Power() { ElapsedTime = MyTime + 29, THotTank = 214, TCoolTank = 31, TReturnValve = 59, TReturnActual = 43, TReturnForecasted = 42 });
            power.Add(new Power() { ElapsedTime = MyTime + 30, THotTank = 224, TCoolTank = 38, TReturnValve = 57, TReturnActual = 43, TReturnForecasted = 42 });
        }
        public ObservableCollection<Power> power
        {
            get;
            set;
        }
        //public void AddPoint(Power newPoint)});
        //    power.Add(new Power() { ElapsedTime = MyTime + 28, THotTank = 204, TCoolTank = 35, TReturnValve = 58, TReturnActual = 43, TReturnForecasted = 42 
        //{
        //    power.Add(newPoint);
       // }
    }

    public class Power
        //Represens one data collection
    {
        public long ElapsedTime
        {
            get;
            set;
        }

        public double THotTank
        {
            get;
            set;
        }
        public double TCoolTank
        {
            get;
            set;
        }
        public double TReturnValve
        {
            get;
            set;
        }
        public double TReturnActual
        {
            get;
            set;
        }
        public double TReturnForecasted
        {
            get;
            set;
        }
        public int State
        {
            get;
            set;
        }

    }

    static class Global
    {
        //Default is celsius
        private static bool _isCelsius = true;

        public static bool IsCelsius
        {
            get { return _isCelsius; }
            set { _isCelsius = value; }
        }


    }

    public sealed class StreamUriWinRTResolver : IUriToStreamResolver
    {
        public IAsyncOperation<IInputStream> UriToStreamAsync(Uri uri)
        {
            if (uri == null)
            {
                throw new Exception();
            }
            string path = uri.AbsolutePath;

            // Because of the signature of the this method, it can't use await, so we 
            // call into a seperate helper method that can use the C# await pattern.
            return GetContent(path).AsAsyncOperation();
        }

        private async Task<IInputStream> GetContent(string path)
        {
            try
            {
                Uri localUri = new Uri("ms-appx://" + path);
                StorageFile f = await StorageFile.GetFileFromApplicationUriAsync(localUri);
                IRandomAccessStream stream = await f.OpenAsync(FileAccessMode.Read);
                return stream;
            }
            catch (Exception) { throw new Exception("Invalid path"); }
        }
    }

}
