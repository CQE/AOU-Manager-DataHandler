using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataHandler
{
    public class AOUSettings
    {

    }

    public class Power
    //Represens one data collection
    {
        public long ElapsedTime
        {
            get;
            set;
        }

        public double THotTank
        {
            get;
            set;
        }
        public double TCoolTank
        {
            get;
            set;
        }
        public double TReturnValve
        {
            get;
            set;
        }
        public double TReturnActual
        {
            get;
            set;
        }
        public double TReturnForecasted
        {
            get;
            set;
        }

        public double TBufferHot
        {
            get;
            set;
        }

        public double TBufferMid
        {
            get;
            set;
        }

        public double TBufferCold
        {
            get;
            set;
        }


        public int State
        {
            get;
            set;
        }


    }

    public class AOULogMessage
    {
        //check current solution for correct format
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
