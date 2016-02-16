using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace DataHandler
{
        // Can this be more effective ??
        [StructLayout(LayoutKind.Sequential, Pack = 2)] // Pack = 0, 1, 2, 4, 8, 16, 32, 64, or 128:
        public struct AOUTemperatureData
        {
            public UInt16 AOUTempDataHeader;
            public UInt16 time_min_of_week; // 60 * 24 * 7 min = 0 - 10 080 min
            public UInt16 time_ms_of_min; // 60 * 1000 = 60 000
            public UInt16 coldTankTemp; 
            public UInt16 hotTankTemp;
            public UInt16 retTemp;
            public UInt16 coolerTemp;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        public struct AOUFeedData
        {
            public UInt16 AOUFeedDataHeader;
            public UInt16 time_min_of_week; 
            public UInt16 time_ms_of_min; 
            public UInt16 prevFeedTemp;
            public UInt16 newFeedTemp;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        public struct AOULevelData
        {
            public UInt16 AOULevelDataHeader;
            public UInt16 time_min_of_week; 
            public UInt16 time_ms_of_min;
            public UInt16 prevLevel;
            public UInt16 newLevel;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        public struct AOUValvesData
        {
            public UInt16 AOUValvesDataHeader;
            public UInt16 time_min_of_week; 
            public UInt16 time_ms_of_min; 
            public UInt16 newValveReturnTemp;
            public UInt16 prevValveReturnTemp;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        public struct AOUSeqData
        {
            public UInt16 AOUSeqDataHeader;
            public UInt16 time_min_of_week; 
            public UInt16 time_ms_of_min; 
            public UInt16 state;
            public UInt16 cycle;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
        public struct AOUIMMData
        {
            public UInt16 AOUIMMDataHeader;
            public UInt16 time_min_of_week; 
            public UInt16 time_ms_of_min; 
            public UInt16 imm_setting_type;
            public UInt16 imm_setting_val;
    }


}
