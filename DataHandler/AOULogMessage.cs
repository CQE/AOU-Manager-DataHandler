using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataHandler
{
 
    public struct AOULogMessage
    {
        public long time
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
            time = logTime;
            message = logMsg;
            prio = 0;
            pid = 0;
        }

        public AOULogMessage(long logTime, string logMsg, uint logPrio, uint logProcessId)
        {
            time = logTime;
            message = logMsg;
            prio = logPrio;
            pid = logProcessId;
        }
    }
}
