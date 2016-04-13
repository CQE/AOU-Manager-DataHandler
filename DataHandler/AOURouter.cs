using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace DataHandler 
{
    public class AOURouter
    {
        public const int MaxTotalValuesInMemory = 60;
        public const int MaxTotalLogMessagesInMemory = 300;

        private DateTime startTime;

        // Different Run types. File and Random are test modes
        public enum RunType {None, Serial, File, Random, Client};
        public RunType runMode
        {
            get; private set;
        }

        private AOUData aouData;

        private AOULogFile aouLogFile;

        private List<AOULogMessage> logMessages;
        private List<Power> powerValues;

        private int lastLogMessageCount;
        private int lastPowerValuesCount;

        private string applogstr = "AOURouter. No run mode selected";

        public AOURouter()
        {
            logMessages = new List<AOULogMessage>();
            powerValues = new List<Power>();
            lastLogMessageCount = 0; // Ready to get from start
            lastPowerValuesCount = 0;
            runMode = RunType.None;
            startTime = DateTime.Now;
            aouLogFile = new AOULogFile(startTime);
        }

        public AOURouter(AOUSettings.RandomSetting randomSetting) : this()
        {
            runMode = RunType.Random;
            aouData = new AOURandomData(randomSetting);
            aouData.Connect();
        }

        public AOURouter(AOUSettings.FileSetting fileSetting) : this()
        {
            runMode = RunType.File;
            aouData = new AOUFileData(fileSetting);
            aouData.Connect();
        }

        public AOURouter(AOUSettings.SerialSetting serialSetting, AOUSettings.DebugMode dbgMode) : this()
        {
            runMode = RunType.Serial;
            aouData = new AOUSerialData(serialSetting, dbgMode);
            aouData.Connect();
        }

        public AOURouter(AOUSettings.RemoteSetting remoteSetting) : this()
        {
            runMode = RunType.Serial;
            aouData = new AOURemoteData(remoteSetting);
            aouData.Connect();
        }

        ~AOURouter()
        {
            Stop();
        }

        public void Stop()
        {
            if (aouData != null)
                aouData.Disconnect();
        }

        public string GetLogStr()
        {
            if (aouData != null)
                return aouData.GetDataLogText();
            else
            { 
                string text = applogstr;
                applogstr = "";
                return text;
            }

        }

        public string GetRawData()
        {
            if (aouData != null)
                return aouData.GetRawData();
            else
                return "";
        }

        public bool SendToPlc(string text)
        {

            logMessages.Add(new AOULogMessage(AOUHelper.GetNowToMs(), "SendToPlc: "+ text, 12, 0));
            if (aouData != null)
                return aouData.SendData(text);
            else
                return false;
        }

        public void SendCommandToPlc(AOUTypes.CommandType cmd, int value)
        {
            /*
                (temperature in C)	<cmd><tempHotTankFeedSet>195</tempHotTankFeedSet></cmd>	
                (temperature in C)	<cmd><tempColdTankFeedSet>25</tempColdTankFeedSet></cmd>	
                (s/cycle)	        <cmd><coolingTime>15</coolingTime></cmd>	
                (s/cycle)	        <cmd><heatingTime>10</heatingTime></cmd>	
                (s/cycle)	        <cmd><toolHeatingFeedPause>5</toolHeatingFeedPause></cmd>	
            */

            switch (cmd)
            {
                case AOUTypes.CommandType.tempHotTankFeedSet:
                    SendToPlc(String.Format("<cmd><tempHotTankFeedSet>{0}</tempHotTankFeedSet></cmd>", value)); break;
                case AOUTypes.CommandType.tempColdTankFeedSet:
                    SendToPlc(String.Format("<cmd><tempColdTankFeedSet>{0}</tempColdTankFeedSet></cmd>", value)); break;
                case AOUTypes.CommandType.coolingTime:
                    SendToPlc(String.Format("<cmd><coolingTime>{0}</coolingTime></cmd>", value)); break;
                case AOUTypes.CommandType.heatingTime:
                    SendToPlc(String.Format("<cmd><heatingTime>{0}</heatingTime></cmd>", value)); break;
                case AOUTypes.CommandType.toolHeatingFeedPause:
                    SendToPlc(String.Format("<cmd><toolHeatingFeedPause>{0}</toolHeatingFeedPause></cmd>", value)); break;
                default:
                    SendToPlc(String.Format("<cmd><{0}>{1}</{0}></cmd>", cmd, value)); break;
            }
        }

        private void AddLogToFile(AOULogMessage[] logs)
        {
            aouLogFile.AddLogMessages(logs);
        }

        public void Update()
        {
            if (aouData == null) return;

            aouData.UpdateData();

            if (aouData.AreNewValuesAvailable())
            {
                powerValues.AddRange(aouData.GetNewValues());
                if (powerValues.Count > MaxTotalValuesInMemory)
                {
                    powerValues.RemoveRange(0, powerValues.Count - MaxTotalValuesInMemory);
                }
            }

            if (aouData.AreNewLogMessagesAvailable())
            {
                int maxTotalLogsInMemory = 100;
                var logs = aouData.GetNewLogMessages();
                logMessages.AddRange(logs);
                AddLogToFile(logs);
                if (logMessages.Count > maxTotalLogsInMemory)
                {
                    logMessages.RemoveRange(0, logMessages.Count - maxTotalLogsInMemory);
                }
            }
        }

        /************************
            Value Handling

            if (aouData.AreNewValuesAvailable())
            {
                powerValues.AddRange(aouData.GetNewValues());
                if (powerValues.Count > MaxTotalValuesInMemory)
                {
                    powerValues.RemoveRange(0, powerValues.Count - MaxTotalValuesInMemory);
                }
}

        ************************/

        public List<Power> GetLastPowerValues(int count)
        {
            if (powerValues.Count > 0)
            {
                if (count > powerValues.Count)
                {
                    count = powerValues.Count;
                }
                lastPowerValuesCount = powerValues.Count;
                return powerValues.GetRange(lastPowerValuesCount - count, count);
            }
            else
            {
                return new List<Power>();
            }
        }

        public bool NewPowerDataIsAvailable()
        {
            return (powerValues.Count  > lastPowerValuesCount);
        }

        public Power GetLastNewPowerValue()
        {
            if (powerValues.Count > 0)
            {
                lastPowerValuesCount = powerValues.Count;
                return powerValues[lastPowerValuesCount - 1];
            }
            else
            {
                return new Power();
            }
        }

        public int GetNumPowerValues()
        {
            return powerValues.Count;
        }

        /*
        public List<Power> GetNewPowerValues()
        {
            var powerList = new List<Power>();
            if (powerValues.Count > 0)
            {
                lastPowerValuesCount = powerValues.Count;
                powerList.Add(powerValues[lastPowerValuesCount - 1]); // Todo. All last power messages
            }

            return powerList;
        }

         public List<Power> GetPowerValuesFromStartTime(long time, int count)
         {
             var res = powerValues.Find(item => item.ElapsedTime == time);
             return res;
         }
         */

        /**************************
            Log Message Handling

            if (aouData.AreNewLogMessagesAvailable())
            {
                var logs = aouData.GetNewLogMessages();
        logMessages.AddRange(logs);
                AddLogToFile(logs);
                if (logMessages.Count > MaxTotalLogMessagesInMemory)
                {
                    logMessages.RemoveRange(0, logMessages.Count - MaxTotalLogMessagesInMemory);
                }
            }


        **************************/
        public AOULogMessage[] GetLastLogMessages(int count)
        {
            if (logMessages.Count > 0)
            { 
                if (count > logMessages.Count)
                {
                    count = logMessages.Count;
                }
                lastLogMessageCount = logMessages.Count;
                return logMessages.GetRange(lastLogMessageCount - count, count).ToArray();
            }
            else
            {
                return new AOULogMessage[0];
            }
        }

        public bool NewLogMessagesAreAvailable()
        {
            return (logMessages.Count > lastLogMessageCount);
        }

        public AOULogMessage[] GetNewLogMessages()
        {
            if (logMessages.Count > 0)
            { 
                int lmc = lastLogMessageCount;
                lastLogMessageCount = logMessages.Count;
                return logMessages.GetRange(lmc, logMessages.Count - lmc).ToArray();
            }
            else
            {
                return new AOULogMessage[0];
            }
        }
    }
}
