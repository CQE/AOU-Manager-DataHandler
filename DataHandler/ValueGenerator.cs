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

        static public Power GetRandomPower()
        {
            long ts = (long) ((ulong)DateTime.Now.Ticks / (ulong)10000); // 100 ns to s

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
                ElapsedTime = ts,
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

            return power;
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
