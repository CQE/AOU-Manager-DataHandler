using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataHandler
{
 
    public class AOULogMessage
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

        public AOULogMessage(uint logTime, string logMsg, uint logPrio, uint logProcessId)
        {
            time = logTime;
            message = logMsg;
            prio = logPrio;
            pid = logProcessId;
        }
    }
}
