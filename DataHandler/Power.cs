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
        public enum StateType { NOTHING = 0, SQ_WAIT_COLD_AT_MOULD_ENTRY, SQ_WAIT_FOR_INJECTION_BEGIN,
                                SQ_WAIT_FOR_INJECTION_END, SQ_WAIT_FOR_COOLING_END, SQ_WAIT_FOR_OPEN_BEGIN,
                                SQ_WAIT_FOR_EJECT_BEGIN, SQ_WAIT_FOR_EJECT_END, SQ_WAIT_FOR_OPEN_END
                              };

        public const int NumStates = 8;

        public static StateType StringToStateType(string state_str)
        {
            switch (state_str)
            {
                case "SQ_WAIT_COLD_AT_MOULD_ENTRY": return StateType.SQ_WAIT_COLD_AT_MOULD_ENTRY;
                case "SQ_WAIT_FOR_INJECTION_BEGIN": return StateType.SQ_WAIT_FOR_INJECTION_BEGIN;
                case "SQ_WAIT_FOR_INJECTION_END": return StateType.SQ_WAIT_FOR_INJECTION_END;
                case "SQ_WAIT_FOR_COOLING_END": return StateType.SQ_WAIT_FOR_COOLING_END;
                case "SQ_WAIT_FOR_OPEN_BEGIN": return StateType.SQ_WAIT_FOR_OPEN_BEGIN;
                case "SQ_WAIT_FOR_EJECT_BEGIN": return StateType.SQ_WAIT_FOR_EJECT_BEGIN;
                case "SQ_WAIT_FOR_EJECT_END": return StateType.SQ_WAIT_FOR_EJECT_END;
                case "SQ_WAIT_FOR_OPEN_END": return StateType.SQ_WAIT_FOR_OPEN_END;
                default: return StateType.NOTHING;
            }
        }

    }

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

        public double THeaterOilOut { get; set; }

        public double ValveFeedHot { get; set; }

        public double ValveFeedCold { get; set; }

        public double ValveReturn { get; set; }

        public double ValveCoolant { get; set; }

        public double PowerHeating { get; set; }
    }
}
