using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Windows.Storage;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DemoPrototype
{


    public static class GlobalAppSettings
    {
        static public bool IsCelsius
        {
            get
            {
                return ApplicationData.Current.LocalSettings.Values.ContainsKey("IsCelsius") ?
                       (bool)ApplicationData.Current.LocalSettings.Values["IsCelsius"] : true;
            }

            set
            {
                ApplicationData.Current.LocalSettings.Values["IsCelsius"] = (bool)value;
            }
        }

        public static double SafeConvertToDouble(object value)
        {
            if (value is String)
            {
                double res = Double.NaN;
                Double.TryParse((string)value, out res);
                return res;
            }
            else
            {
                try
                {
                    return (double)value;
                }
                catch (Exception)
                {
                    return Double.NaN;
                }
            }
        }

    }

    public class DataUpdater
    {
        public DataUpdater()
        {
            /*
            ToDo More Initializing
            */
        }

        public void UpdateInputData(object dataContext)
        {
            ((LineChartViewModel)dataContext).DeleteFirstPoint();
            ((LineChartViewModel)dataContext).AddRandomPoint();
        }
    }

    public class LineChartViewModel
    //Our class for handling chart data
    {
        long ts; // Time, Timestamp or ...
        Random rnd;


        public LineChartViewModel()
        {
            //initialise
            ts = 0;
            this.rnd = new Random();
            this.power = new ObservableCollection<Power>();

            //add some datapoints
            for (int i = 0; i < 30; i++)
            {
                AddRandomPoint();
            }
        }

        public ObservableCollection<Power> power
        {
            get;
            set;
        }

        public void AddPoint(Power newPoint)
        {
            power.Add(newPoint);
        }

        public void DeleteFirstPoint()
        {
            power.RemoveAt(0);
        }

        public void AddRandomPoint()
        {
            long valTHotTank = rnd.Next(204, 235);
            long valTCoolTank = rnd.Next(31, 45);
            long valTReturnValve = rnd.Next(40, 87);
            long valTReturnActual = rnd.Next(40, 46);
            long valTReturnForecasted = rnd.Next(40, 77);
            int valState = rnd.Next(2, 7);
            long valTBufferHot = rnd.Next(180, 200);
            long valTBufferMid = rnd.Next(75, 110);
            long valTBufferCold = rnd.Next(20, 40);

            var power = new Power()
            {
                ElapsedTime = ts++,
                THotTank = valTHotTank,
                TCoolTank = valTCoolTank,
                TReturnValve = valTReturnValve,
                TReturnActual = valTReturnActual,
                TReturnForecasted = valTReturnForecasted,
                TBufferHot = valTBufferHot,
                TBufferMid = valTBufferMid,
                TBufferCold = valTBufferCold,
                State = valState
            };

            AddPoint(power);
        }
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

        public double TBufferHot
        {
            get;
            set;
        }

        public double TBufferMid
        {
            get;
            set;
        }

        public double TBufferCold
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

    public class AOULogMessage
    {
        //check current solution for correct format
        public long AOUTimeStamp;
        public string AOUMessage;
        private int v1;
        private string v2;

        public AOULogMessage(int v1, string v2)
        {
            this.v1 = v1;
            this.v2 = v2;
        }
    }
    public class AOULogMessageHandler
    {
        //replace this with correct error handler later, this code is for GUI testing only
        public static List<AOULogMessage> GetAOULogMessages()
        {
            var logMessages = new List<AOULogMessage>();
            logMessages.Add(new AOULogMessage(1, "message 1"));
            logMessages.Add(new AOULogMessage(2, "message 2"));
            logMessages.Add(new AOULogMessage(3, "message 3"));
            logMessages.Add(new AOULogMessage(4, "message 4"));
            return logMessages;
        }
    }

}
