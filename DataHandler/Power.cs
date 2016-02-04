using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataHandler
{
    /*
     Run view Parameter monitoring:

         THotTank, TColdTank
         TReturnActual
         TReturnForecasted (high/low)
         TBufferHot, TBufferMid, TBufferCold
         New:
             HeatExchangerCoolantOut, THeaterOilOut
             ValveFeedHot (on/off) / preferably curve vertically draggable in graph
             ValveFeedCold (on/off) / preferably curve vertically draggable in graph
             ValveReturn (on/off) / preferably curve vertically draggable in graph
             ValveCoolant (on/off curve & average duty cycle % as number) / preferably curve vertically draggable in graph
             PowerHeating (on/off curve & average duty cycle % as number) / preferably curve vertically draggable in graph

     Calibrate view:
         Feed and Return timings(s):
             THotTank (see how much of this TReturnActual has reached)
             TColdTank (see how much of this TReturnActual has reached)
             TReturnActual (see time diff with TReturnForecasted)
             TReturnForecasted (see time diff with TReturnActual)

         Energy balance (C):
             THotTank
             TColdTank
             TReturnActual

         Volume balance (C):
             TBufferHot
             TBufferMid
             TBufferCold
     */

    public class AOUTypes
    {
        public enum StateType { state1, state2, state3, state4, state5, state6, state7 };

    }

    public struct Power
    {
        public Power(long etime = 0)
        {
            ElapsedTime = etime;
            State = 0;
            THotTank = double.NaN;
            TColdTank = double.NaN;
            TReturnValve = double.NaN;
            TReturnActual = double.NaN;
            TReturnForecasted = double.NaN;
            TBufferHot = double.NaN;
            TBufferMid = double.NaN;
            TBufferCold = double.NaN;
            THeaterOilOut = double.NaN;
            ValveFeedHot = 0;
            ValveFeedCold = 0;
            ValveReturn = 0;
            ValveCoolant = 0;
            PowerHeating = 0;
        }

    // Time in ms
    public long ElapsedTime { get; set; }

        /*
        */
//        public AOUTypes.StateType State { get; set; }
        public int State { get; set; }

        public double THotTank { get; set; }

        public double TColdTank { get; set; }

        public double TReturnValve { get; set; }

        public double TReturnActual { get; set; }

        public double TReturnForecasted { get; set; }

        public double TBufferHot { get; set; }

        public double TBufferMid { get; set; }

        public double TBufferCold { get; set; }

        public double THeaterOilOut { get; set; }

        public long ValveFeedHot { get; set; }

        public long ValveFeedCold { get; set; }

        public long ValveReturn { get; set; }

        public long ValveCoolant { get; set; }

        public long PowerHeating { get; set; }
    }
}
