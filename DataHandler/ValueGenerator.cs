using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataHandler
{
    static public class ValueGenerator
    {
        static Random rnd = new Random();
        static long ts_start = -1;

        static public Power GetRandomPower()
        {
            double[] onOffArr = new double[] { 50, 70 };

            if (ts_start == -1)
                ts_start = DateTime.Now.Ticks;

            TimeSpan ts = new TimeSpan(DateTime.Now.Ticks - ts_start);
            int tsSeconds = ts.Hours * 60 * 60 + ts.Minutes * 60 + ts.Seconds;
            // Todo tsDay - or add day

            AOUTypes.StateType valState = (AOUTypes.StateType)rnd.Next(1, AOUTypes.NumStates + 1);

            double valTHotTank = rnd.Next(204, 235);
            double valTCoolTank = rnd.Next(31, 45);
            double valTReturnValve = rnd.Next(40, 87);
            double valTReturnActual = rnd.Next(40, 46);
            double valTReturnForecasted = rnd.Next(40, 77);
            double valTBufferHot = rnd.Next(180, 200);
            double valTBufferMid = rnd.Next(75, 110);
            double valTBufferCold = rnd.Next(20, 40);
            double valTheaterOilOut = rnd.Next(250, 290);
            double valValveFeedHot = RandomFromDoubleArray(onOffArr, 0); // Off=50, On=70
            double valValveFeedCold = RandomFromDoubleArray(onOffArr, 0); // Off=50, On=70
            double valValveReturn = RandomFromDoubleArray(onOffArr, 0); // Cold=50, Hot=70
            double valValveCoolant = rnd.Next(50, 90); // 0-100%, 50-90
            double valPowerHeating = rnd.Next(5, 12); // 0-14kW ?

            var power = new Power()
            {
                // To milliseconds
                ElapsedTime = ts.Milliseconds + tsSeconds * 1000,

                State = valState, 

                THotTank = valTHotTank,
                TColdTank = valTCoolTank,
                TReturnValve = valTReturnValve,
                TReturnActual = valTReturnActual,
                TReturnForecasted = valTReturnForecasted,
                TBufferHot = valTBufferHot,
                TBufferMid = valTBufferMid,
                TBufferCold = valTBufferCold,
                THeaterOilOut = valTheaterOilOut,

                ValveFeedHot = valValveFeedHot,
                ValveFeedCold = valValveFeedCold,
                ValveReturn = valValveReturn,
                ValveCoolant = valValveCoolant,
                PowerHeating = valPowerHeating,

            };

            return power;
        }

        static public long RandomFromIntArray(long[] intArr, int period)
        {
            long index = rnd.Next(0, intArr.Length);
            return intArr[index];
        }

        static public double RandomFromDoubleArray(double[] dblArr, int period)
        {
            long index = rnd.Next(0, dblArr.Length);
            return dblArr[index];
        }

        static public bool GetRandomOk(int max)
        {
            long value = rnd.Next(0, max);
            return (value == 0);
        }

        static public char GetRandomAsciiChar(char lowestChar, char highestChar)
        {
            return (char)(byte)rnd.Next((int)lowestChar, (int)highestChar);
        }

        static public AOULogMessage GetRandomLogMsg()
        {
            uint ts = (uint)((ulong)DateTime.Now.Ticks / (ulong)10000); // 100 ns to s
            uint prio = (uint)rnd.Next(1, 3);
            uint pid = (uint)rnd.Next(1038, 1965);
            string logtext = "log-";
            for (int i = 0; i < 8; i++)
            {
                
                logtext += ValueGenerator.GetRandomAsciiChar('0', 'Z');
            }

            return new AOULogMessage(ts, logtext, prio, pid);
        }

    }
}
