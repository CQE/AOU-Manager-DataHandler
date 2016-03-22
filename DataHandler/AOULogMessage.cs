using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataHandler
{

    public struct AOULogMessage
    {
        public string time
        {
            get; set;
        }

        public string message
        {
            get; set;
        }

        public uint pid
        {
            get; set;
        }

        public uint prio
        {
            get; set;
        }

        public AOULogMessage(long logTime, string logMsg)
        {
            time = AOUHelper.msToTimeSpanStr(logTime);
            message = logMsg;
            prio = 0;
            pid = 0;
        }

        public AOULogMessage(long logTime, string logMsg, uint logPrio, uint logProcessId)
        {
            time = AOUHelper.msToTimeSpanStr(logTime);
            message = logMsg;
            prio = logPrio;
            pid = logProcessId;
        }

        public override string ToString()
        {
            return String.Format("{0}, {1}, {2}, {3}", time, message, prio, pid);
        }
    }
}
