using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataHandler
{
    public static class AOUTypes
    {
       public const long msInWeek = 60000 * 10080; // 1 week = 7*24*60 = 10 080 min

        public const UInt16 UInt16_NaN = UInt16.MaxValue;

        public static void TimeMsToAOUModelTime(long time_ms, out UInt16 time_minutes_of_week, out UInt16 time_ms_of_minute)
        {
            time_ms_of_minute = (UInt16)(time_ms % 60000); // 1 min = 60 000 ms
            time_minutes_of_week = (UInt16)((time_ms % msInWeek) / 60000); 
        }

        public static long AOUModelTimeToTimeMs(UInt16 time_minutes_of_week, UInt16 time_ms_of_minute)
        {
            return time_minutes_of_week * 60000 + time_ms_of_minute;
        }

        public enum CommandType
        {
            // "Idle","Heating","Cooling","Fixed Cycling", "Auto with IMM"
            CmdTypeToDo,
            RunningModeIdle, RunningModeHeating, RunningModeCooling,
            RunningModefixedCycling, RunningModeAutoWidthIMM,

            tempHotTankFeedSet, tempColdTankFeedSet, coolingTime, heatingTime,
            toolHeatingFeedPause, toolCoolingFeedPause,  hotDelayTime, coldDelayTime,

            THotTankAlarmLowThreshold, TColdTankAlarmHighThreshold,
            TReturnThresholdCold2Hot, TReturnThresholdHot2Cold,
            TBufferHotLowerLimit, TBufferMidRefThreshold, TBufferColdUpperLimit
        }

        public enum FeedType { Unknown, Cold, Hot };

        public enum StateType
        {
            NOTHING = 0, SQ_INITIAL, IDLE, SQ_WAIT_HOT_AT_MOULD_ENTRY, SQ_WAIT_COLD_AT_MOULD_ENTRY, SQ_WAIT_FOR_INJECTION_BEGIN,
            SQ_WAIT_FOR_INJECTION_END, SQ_WAIT_FOR_COOLING_END, SQ_WAIT_FOR_OPEN_BEGIN,
            SQ_WAIT_FOR_EJECT_BEGIN, SQ_WAIT_FOR_EJECT_END, SQ_WAIT_FOR_OPEN_END
        };

        public enum IMMSettings
        {
            Nothing, SetIMMError, SetIMMBlockInject, SetIMMBlockOpen, IMMStop,
            CycleAuto, IMMInjecting, IMMEjecting, IMMToolClosed
        };

        public enum AOUDataType
        {
            AOUTempData, AOUColdFeedData, AOUHotFeedData, AOUColdLevelData,
            AOUHotLevelData, AOUValvesData, AOUSeqData, AOUIMMData
        }

        public const int NumStates = 8;

        public static StateType StringToStateType(string state_str)
        {
            switch (state_str)
            {
                case "IDLE": return StateType.IDLE;
                case "SQ_INITIAL": return StateType.SQ_INITIAL;
                case "SQ_WAIT_HOT_AT_MOULD_ENTRY": return StateType.SQ_WAIT_HOT_AT_MOULD_ENTRY;
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
}
