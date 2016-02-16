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
         
         TReturnActual, TReturnForecasted (high/low)

         TBufferHot, TBufferMid, TBufferCold

         New:
             TheatExchangerCoolantOut, TheaterOilOut

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

    public struct Power
    {
        public Power(long etime = 0)
        {
            ElapsedTime = etime;
            State = AOUTypes.StateType.NOTHING;

            THotTank = double.NaN;
            TColdTank = double.NaN;

            TReturnValve = double.NaN;
            TReturnActual = double.NaN;
            TReturnForecasted = double.NaN;

            TBufferHot = double.NaN;
            TBufferMid = double.NaN;
            TBufferCold = double.NaN;

            THeatExchangerCoolantOut = double.NaN;
            THeaterOilOut = double.NaN;

            ValveFeedHot = double.NaN;
            ValveFeedCold = double.NaN;
            ValveReturn = double.NaN;
            ValveCoolant = double.NaN;
            PowerHeating = double.NaN;
        }

        // Time in ms
        public long ElapsedTime { get; set; }

        /*
        */
//        public AOUTypes.StateType State { get; set; }
        public AOUTypes.StateType State { get; set; }

        public double THotTank { get; set; }
        public double TColdTank { get; set; }

        public double TReturnValve { get; set; }

        public double TReturnActual { get; set; }
        public double TReturnForecasted { get; set; }

        public double TBufferHot { get; set; }
        public double TBufferMid { get; set; }
        public double TBufferCold { get; set; }

        public double THeatExchangerCoolantOut { get; set; }
        public double THeaterOilOut { get; set; }

        public double ValveFeedHot { get; set; } // on/off
        public double ValveFeedCold { get; set; } // on/off
        public double ValveReturn { get; set; } // on/off
        public double ValveCoolant { get; set; } // on/off

        public double PowerHeating { get; set; } // on/off
    }
}
