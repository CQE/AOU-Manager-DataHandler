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
            long[] onOffArr = new long[] { 50, 70 };
            if (ts_start == -1)
                ts_start = DateTime.Now.Ticks;

            TimeSpan ts = new TimeSpan(DateTime.Now.Ticks - ts_start);
            long tsSeconds = ts.Hours * 60 * 60 + ts.Minutes * 60 + ts.Seconds;
            // Todo tsDay - or add day

            /*
            int index = rnd.Next(1, 7);
            AOUTypes.StateType state;
            switch (index)
            {
                case 2: state = AOUTypes.StateType.state2; break;
                case 3: state = AOUTypes.StateType.state3; break;
                case 4: state = AOUTypes.StateType.state4; break;
                case 5: state = AOUTypes.StateType.state5; break;
                case 6: state = AOUTypes.StateType.state6; break;
                case 7: state = AOUTypes.StateType.state7; break;
                default: state = AOUTypes.StateType.state1; break;

            }
            */

            long valState = rnd.Next(1, 7);

            long valTHotTank = rnd.Next(204, 235);
            long valTCoolTank = rnd.Next(31, 45);
            long valTReturnValve = rnd.Next(40, 87);
            long valTReturnActual = rnd.Next(40, 46);
            long valTReturnForecasted = rnd.Next(40, 77);
            long valTBufferHot = rnd.Next(180, 200);
            long valTBufferMid = rnd.Next(75, 110);
            long valTBufferCold = rnd.Next(20, 40);
            long valTheaterOilOut = rnd.Next(250, 290);
            long valValveFeedHot = RandomFromIntArray(onOffArr, 0); // Off=50, On=70
            long valValveFeedCold = RandomFromIntArray(onOffArr, 0); // Off=50, On=70
            long valValveReturn = RandomFromIntArray(onOffArr, 0); // Cold=50, Hot=70
            long valValveCoolant = rnd.Next(50, 90); // 0-100%, 50-90
            long valPowerHeating = rnd.Next(5, 12); // 0-14kW ?

            var power = new Power()
            {
                // To milliseconds
                ElapsedTime = ts.Milliseconds + tsSeconds * 1000,

                State = (int)valState,

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
            // long okTo = rnd.Next(0, period);
            // if (okTo)
            long index = rnd.Next(0, intArr.Length);
            return intArr[index];
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
