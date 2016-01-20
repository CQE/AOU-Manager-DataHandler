using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;

namespace DataHandler
{
    /*
    ts = xxx xxx xxx ms;

    Sequence IMM/AOU : <seq> <Time>ms</Time> <State>str</State><Cycle>int</Cycle><Descr>str<Leave>str</Leave> </seq>

    Temperature data 10x/s
    <temperature> <Time>ms</Time> <Hot>int</Hot><Cold>int</Cold><Ret>int</Ret <Cool>int</Cool> </temperature>

    (Cool is AOU forecast Tret=High/low)
    <temperature> <Time>104 904 248</Time> <Hot>122</Hot> <Cold>60</Cold> <Ret>84 </Ret> <Cool>60</Cool> </temperature>
    -------------------------------------------------------------------------------------------------------------------
    Feed valves 2x/cycle
    <feeds>	<Time>104 894 473</Time>	<Hot>	<Prev>	60	</Prev>	</New>	63,75	</New>	</Hot>	</feeds>

    (on/off)
    <feeds>	<Time>104 906 094</Time>	<Hot>	<Prev>	63,75	</Prev>	</New>	60	</New>	</Hot>	</feeds>
    --------------------------------------------------------------------------------------------------------
    Return valve 2x/cycle
    <valves> <Time>104 903 816</Time> <Ret> <Prev>93></Prev><New>80</New> </Ret> </valves>

    (cold/hot)
    <valves> <Time>104 922 423</Time> <Ret>	<Prev>80</Prev> <New>93</New> </Ret> </valves>

    */

    [StructLayout(LayoutKind.Sequential, Pack = 4)] // Pack = 0, 1, 2, 4, 8, 16, 32, 64, or 128:
    struct InputTempData
    {
        public byte tm;
        public byte cold;
        public byte hot;
    }

    class InputParser
    {

    }
}
