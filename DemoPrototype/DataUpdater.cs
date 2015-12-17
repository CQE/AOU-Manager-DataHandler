using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;


namespace DemoPrototype
{

    public static class GlobalAppSettings
    {
        //Default is celsius
        private static bool _isCelsius = true;

        public static bool IsCelsius
        {
            get { return _isCelsius; }
            set { _isCelsius = value; }
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

        public LineChartViewModel()
        {
            //initialise
            ts = 0;
            this.power = new ObservableCollection<Power>();
            //add some datapoints
            power.Add(new Power() { ElapsedTime = ts++, THotTank = 220, TCoolTank = 45, TReturnValve = 37, TReturnActual = 45, TReturnForecasted = 67, State = 7 });
            power.Add(new Power() { ElapsedTime = ts++, THotTank = 227, TCoolTank = 41, TReturnValve = 87, TReturnActual = 43, TReturnForecasted = 77, State = 7 });
            power.Add(new Power() { ElapsedTime = ts++, THotTank = 235, TCoolTank = 37, TReturnValve = 77, TReturnActual = 45, TReturnForecasted = 67, State = 6 });
            power.Add(new Power() { ElapsedTime = ts++, THotTank = 215, TCoolTank = 33, TReturnValve = 87, TReturnActual = 46, TReturnForecasted = 61, State = 5 });
            power.Add(new Power() { ElapsedTime = ts++, THotTank = 204, TCoolTank = 40, TReturnValve = 77, TReturnActual = 43, TReturnForecasted = 77, State = 4 });
            power.Add(new Power() { ElapsedTime = ts++, THotTank = 205, TCoolTank = 39, TReturnValve = 67, TReturnActual = 42, TReturnForecasted = 87, State = 3 });
            power.Add(new Power() { ElapsedTime = ts++, THotTank = 204, TCoolTank = 38, TReturnValve = 57, TReturnActual = 46, TReturnForecasted = 61, State = 3 });
            power.Add(new Power() { ElapsedTime = ts++, THotTank = 204, TCoolTank = 35, TReturnValve = 58, TReturnActual = 42, TReturnForecasted = 87, State = 2 });
            power.Add(new Power() { ElapsedTime = ts++, THotTank = 214, TCoolTank = 31, TReturnValve = 59, TReturnActual = 41, TReturnForecasted = 63, State = 2 });
            power.Add(new Power() { ElapsedTime = ts++, THotTank = 224, TCoolTank = 38, TReturnValve = 57, TReturnActual = 41, TReturnForecasted = 63, State = 2 });
            power.Add(new Power() { ElapsedTime = ts++, THotTank = 220, TCoolTank = 45, TReturnValve = 37, TReturnActual = 42, TReturnForecasted = 87, State = 7 });
            power.Add(new Power() { ElapsedTime = ts++, THotTank = 227, TCoolTank = 41, TReturnValve = 87, TReturnActual = 40, TReturnForecasted = 45, State = 7 });
            power.Add(new Power() { ElapsedTime = ts++, THotTank = 235, TCoolTank = 37, TReturnValve = 77, TReturnActual = 41, TReturnForecasted = 44, State = 7 });
            power.Add(new Power() { ElapsedTime = ts++, THotTank = 215, TCoolTank = 33, TReturnValve = 87, TReturnActual = 42, TReturnForecasted = 43, State = 7 });
            power.Add(new Power() { ElapsedTime = ts++, THotTank = 204, TCoolTank = 40, TReturnValve = 77, TReturnActual = 43, TReturnForecasted = 42, State = 7 });
            power.Add(new Power() { ElapsedTime = ts++, THotTank = 205, TCoolTank = 39, TReturnValve = 67, TReturnActual = 44, TReturnForecasted = 41, State = 7 });
            power.Add(new Power() { ElapsedTime = ts++, THotTank = 204, TCoolTank = 38, TReturnValve = 57, TReturnActual = 45, TReturnForecasted = 40, State = 7 });
            power.Add(new Power() { ElapsedTime = ts++, THotTank = 204, TCoolTank = 35, TReturnValve = 58, TReturnActual = 44, TReturnForecasted = 41 });
            power.Add(new Power() { ElapsedTime = ts++, THotTank = 214, TCoolTank = 31, TReturnValve = 59, TReturnActual = 43, TReturnForecasted = 42 });
            power.Add(new Power() { ElapsedTime = ts++, THotTank = 224, TCoolTank = 38, TReturnValve = 57, TReturnActual = 43, TReturnForecasted = 42 });
            power.Add(new Power() { ElapsedTime = ts++, THotTank = 220, TCoolTank = 45, TReturnValve = 37, TReturnActual = 43, TReturnForecasted = 42 });
            power.Add(new Power() { ElapsedTime = ts++, THotTank = 227, TCoolTank = 41, TReturnValve = 87, TReturnActual = 43, TReturnForecasted = 60 });
            power.Add(new Power() { ElapsedTime = ts++, THotTank = 235, TCoolTank = 37, TReturnValve = 77, TReturnActual = 43, TReturnForecasted = 42 });
            power.Add(new Power() { ElapsedTime = ts++, THotTank = 215, TCoolTank = 33, TReturnValve = 87, TReturnActual = 43, TReturnForecasted = 60 });
            power.Add(new Power() { ElapsedTime = ts++, THotTank = 204, TCoolTank = 40, TReturnValve = 77, TReturnActual = 43, TReturnForecasted = 42 });
            power.Add(new Power() { ElapsedTime = ts++, THotTank = 205, TCoolTank = 39, TReturnValve = 67, TReturnActual = 43, TReturnForecasted = 42 });
            power.Add(new Power() { ElapsedTime = ts++, THotTank = 204, TCoolTank = 38, TReturnValve = 57, TReturnActual = 43, TReturnForecasted = 60 });
            power.Add(new Power() { ElapsedTime = ts++, THotTank = 214, TCoolTank = 31, TReturnValve = 59, TReturnActual = 43, TReturnForecasted = 42 });
            power.Add(new Power() { ElapsedTime = ts++, THotTank = 224, TCoolTank = 38, TReturnValve = 57, TReturnActual = 43, TReturnForecasted = 42 });
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
            var rnd = new Random();
            long valTHotTank = rnd.Next(204, 235);
            long valTCoolTank = rnd.Next(31, 45);
            long valTReturnValve = rnd.Next(40, 87);
            long valTReturnActual = rnd.Next(40, 46);
            long valTReturnForecasted = rnd.Next(40, 77);
            int valState = rnd.Next(2, 7);

            var power = new Power()
            {
                ElapsedTime = ts++,
                THotTank = valTHotTank,
                TCoolTank = valTCoolTank,
                TReturnValve = valTReturnValve,
                TReturnActual = valTReturnActual,
                TReturnForecasted = valTReturnForecasted,
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
        public int State
        {
            get;
            set;
        }

    }
}
