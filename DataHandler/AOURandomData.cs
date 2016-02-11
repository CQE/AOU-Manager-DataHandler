using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataHandler
{
    class AOURandomData
    {
        public const int MaxRandomCount = 50;

        private string logstr;

        public string GetLogText()
        {
            string text = logstr;
            logstr = "";

            return text;
        }

        public AOURandomData(string settings)
        {
            logstr = "Random Data Ready: " + settings;
        }

        public Power GetRandomPower()
        {
            return ValueGenerator.GetRandomPower();
        }

        public Power[] GetRandomPowerList(int num = 1)
        {
            Power[] lst = new Power[num];
            for (int i = 0; i < num; num++)
            {
                lst[i] = GetRandomPower();
            }
            return lst;
        }

        public bool NewRandomLogMessageAvailable()
        {
            return ValueGenerator.GetRandomOk(50);
        }

        public AOULogMessage GetRandomLogMsg()
        {
            return ValueGenerator.GetRandomLogMsg();
        }

        public AOULogMessage[] GetRandomLogMsgList(int num = 1)
        {
            AOULogMessage[] lst = new AOULogMessage[num];
            for (int i = 0; i < num; num++)
            {
                lst[i] = GetRandomLogMsg();
            }
            return lst;
        }

    }
}
