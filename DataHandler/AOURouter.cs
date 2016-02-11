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
        public const int MaxTotalValuesInMemory = 90;

        // "Idle","Heating","Cooling","Fixed Cycling", "Auto with IMM"
        public enum AOUCommandType { idleMode, heatingMode, coolingMode, fixedCyclingMode, autoWidthIMMMode,
                                     tempHotTankFeedSet, tempColdTankFeedSet, coolingTime, heatingTime,
                                     toolHeatingFeedPause, toolCoolingFeedPause
                                   }

        // Different Run types. File and Random are test modes
        public enum RunType {Serial, File, Random};
        public RunType runMode
        {
            get; private set;
        }

        // Object to read and write text to serial port
        AOUSerialData serialData;

        // Object to read and write to text files in User Image folder
        private TextFile dataFile;

        // Object to read random text data
        AOURandomData randomData;

        List<AOULogMessage> logMessages;
        List<Power> powerValues;

        int lastLogMessageCount;
        int lastPowerValuesCount;

        private string logstr = "AOURouter. No run mode selected";

        public AOURouter()
        {
            logMessages = new List<AOULogMessage>();
            powerValues = new List<Power>();
            lastLogMessageCount = 0; // Ready to get from start
            lastPowerValuesCount = 0;
            runMode = RunType.Serial; // Normal
        }

        public AOURouter(RunType mode, string metaData = "") : this()
        {
            runMode = mode;
            if (runMode == RunType.Random)
            {
                randomData = new AOURandomData(metaData);
            }
            else if (runMode == RunType.File)
            {
                dataFile = new TextFile();
                dataFile.OpenFileIfExistAndGetText(metaData);
            }
            else if (runMode == RunType.Serial)
            {
                serialData = new AOUSerialData(metaData);
            }
        }

        public string GetLogStr()
        {
            if (runMode == RunType.Serial && serialData != null)
            {
                return serialData.GetLogText();
            }
            else if (runMode == RunType.File && dataFile != null)
            {
                return dataFile.GetLogText();
            }
            else if (runMode == RunType.Random && randomData != null)
            {
                return randomData.GetLogText();
            }
            else
            {
                string text = logstr;
                logstr = "";
                return text;
            }

        }

        public bool IsDataAvailable()
        {
            if (runMode == RunType.Serial && serialData != null)
            {
                return serialData.IsDataAvailable();
            }
            else if (runMode == RunType.File && dataFile != null)
            {
                return dataFile.StrData.Length > 0;
            }
            else if (runMode == RunType.Random && randomData != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string GetTextData()
        {
            if (runMode == RunType.Serial && serialData != null)
            {
                return serialData.GetTextData();
            }
            else if (runMode == RunType.File && dataFile != null)
            {
                return dataFile.StrData;
            }
            else
            {
                return "ToDo";
            }
        }

        public void SendToPlc(string text)
        {
            if (runMode == RunType.Random)
            {
                // ToDo: Save to log file
            }
            else if (runMode == RunType.File)
            {
                // ToDo: Save to log file
            }
            else if (runMode == RunType.Serial && serialData != null)
            {
                serialData.SendData(text);
            }
        }

        public void SendCommandToPlc(AOUCommandType cmd, int value)
        {
            switch (cmd)
            {
                case AOUCommandType.tempHotTankFeedSet:
                    SendToPlc(String.Format("<cmd><tempHotTankFeedSet>{0}</tempHotTankFeedSet></cmd>", value)); break;
                case AOUCommandType.tempColdTankFeedSet:
                    SendToPlc(String.Format("<cmd><tempColdTankFeedSet>{0}</tempColdTankFeedSet></cmd>", value)); break;
                case AOUCommandType.coolingTime:
                    SendToPlc(String.Format("<cmd><coolingTime>{0}</coolingTime></cmd>", value)); break;
                case AOUCommandType.heatingTime:
                    SendToPlc(String.Format("<cmd><heatingTime>{0}</heatingTime></cmd>", value)); break;
                case AOUCommandType.toolHeatingFeedPause:
                    SendToPlc(String.Format("<cmd><toolHeatingFeedPause>{0}</toolHeatingFeedPause></cmd>", value)); break;
                default:
                    SendToPlc(String.Format("<cmd>{0}</cmd>", cmd)); break;
            }

        }

        public List<Power> GetTextDataList(out List<AOULogMessage> logList)
        {
            long time_ms = 0;

            double hot_tank_temp = double.NaN;
            double cold_tank_temp = double.NaN;
            double valve_return_temp = double.NaN;
            double cool_temp = double.NaN;

            string seqState = "";

            string logMsg = "";

            double valvesRetPrev = double.NaN;
            double valvesRetNew = double.NaN;

            long min_ms = long.MaxValue;
            long max_ms = 0;

            AOUInputParser.FeedType feed = AOUInputParser.FeedType.unknown;
            double feedPrev = 0;
            double feedNew = 0;
            List<Power> listData = new List<Power>();
            logList = new List<AOULogMessage>();

            if (IsDataAvailable())
            {
                string text = GetTextData();
                text = text.Replace("\t", "");
                text = text.Replace("\r", "");
                text = text.Replace("\n", "");

                string nextTag = "Dummy";
                while (nextTag.Length > 0)
                {
                    Power tempPower = new Power();
                    int count = 0;

                    nextTag = AOUInputParser.GetNextTag(text);
                    bool validTag = nextTag.Length > 0;

                    if (nextTag == AOUInputParser.tagTemperature)
                    {
                        if (AOUInputParser.ParseTemperature(text, out time_ms, out hot_tank_temp, out cold_tank_temp,
                                                                  out valve_return_temp, out cool_temp, out count))
                        {
                            tempPower.ElapsedTime = time_ms;
                            tempPower.THotTank = hot_tank_temp;
                            tempPower.TColdTank = cold_tank_temp;
                            tempPower.TReturnValve = valve_return_temp;
                            tempPower.ValveCoolant = cool_temp;
                        }
                    }
                    else if (nextTag == AOUInputParser.tagSequence)
                    {
                        if (AOUInputParser.ParseSequence(text, out time_ms, out seqState, out count))
                        {
                            tempPower.ElapsedTime = time_ms;
                            tempPower.State = AOUTypes.StringToStateType(seqState);
                        }
                    }
                    else if (nextTag == AOUInputParser.tagFeeds)
                    {
                        if (AOUInputParser.ParseFeeds(text, out time_ms, out feed, out feedNew, out feedPrev, out count))
                        {
                            tempPower.ElapsedTime = time_ms;
                            tempPower.TReturnActual = feedPrev;
                            tempPower.TReturnForecasted = feedNew;
                        }
                    }
                    else if (nextTag == AOUInputParser.tagValves)
                    {
                        if (AOUInputParser.ParseValves(text, out time_ms, out valvesRetPrev, out valvesRetNew, out count))
                        {
                            tempPower.ElapsedTime = time_ms;
                            tempPower.TReturnValve = valvesRetPrev;
                        }
                    }
                    else if (nextTag == AOUInputParser.tagLog)
                    {
                        if (AOUInputParser.ParseLog(text, out time_ms, out logMsg, out count))
                        {
                            AOULogMessage msg = new AOULogMessage((uint)time_ms, logMsg);
                            logList.Add(msg);
                        }
                    }
                    else if (nextTag.Length > 0)
                    {
                        string unknownTagText;
                        AOUInputParser.FindTagAndExtractText(nextTag, text, out unknownTagText, out count);
                        validTag = false;
                        // Todo: if not found valid tag
                    }

                    if (validTag && nextTag != AOUInputParser.tagLog)
                    {
                        if (max_ms == 0 || tempPower.ElapsedTime > max_ms)
                        {
                            listData.Add(tempPower);
                        }
                        else if (tempPower.ElapsedTime < min_ms)
                        {
                            listData.Insert(0, tempPower);
                        }
                        else {
                            for (int i = 0; i < listData.Count; i++)
                            {
                                if (listData[i].ElapsedTime == tempPower.ElapsedTime)
                                {
                                    Power pwr = listData[i];
                                    if (pwr.TColdTank == double.NaN) pwr.TColdTank = tempPower.TColdTank;
                                    if (pwr.THotTank == double.NaN) pwr.THotTank = tempPower.THotTank;
                                    if (pwr.TReturnActual == double.NaN) pwr.TReturnActual = tempPower.TReturnActual;
                                    if (pwr.TReturnForecasted == double.NaN) pwr.TReturnForecasted = tempPower.TReturnForecasted;
                                    if (pwr.TReturnValve == double.NaN) pwr.TReturnValve = tempPower.TReturnValve;
                                    if (pwr.TBufferCold == double.NaN) pwr.TBufferCold = tempPower.TBufferCold;
                                    if (pwr.TBufferMid == double.NaN) pwr.TBufferMid = tempPower.TBufferMid;
                                    if (pwr.TBufferHot == double.NaN) pwr.TBufferHot = tempPower.TBufferHot;
                                    if (pwr.PowerHeating == double.NaN) pwr.PowerHeating = tempPower.PowerHeating;
                                    if (pwr.THeaterOilOut == double.NaN) pwr.THeaterOilOut = tempPower.THeaterOilOut;
                                    if (pwr.ValveCoolant == double.NaN) pwr.ValveCoolant = tempPower.ValveCoolant;
                                    if (pwr.ValveFeedCold == double.NaN) pwr.ValveFeedCold = tempPower.ValveFeedCold;
                                    if (pwr.ValveFeedHot == double.NaN) pwr.ValveFeedHot = tempPower.ValveFeedHot;
                                    if (pwr.ValveReturn == double.NaN) pwr.ValveReturn = tempPower.ValveReturn;
                                    listData[i] = pwr;
                                    break;
                                }
                                else if (tempPower.ElapsedTime < listData[i].ElapsedTime)
                                {
                                    listData.Insert(i, tempPower);
                                    break;
                                }
                                else if (i == (listData.Count-1))
                                {
                                    bool err = true;
                                }
                            }
                        }

                        if (tempPower.ElapsedTime < min_ms)
                            min_ms = tempPower.ElapsedTime;

                        if (tempPower.ElapsedTime > max_ms)
                            max_ms = tempPower.ElapsedTime;
                    }
                    text = text.Substring(count);
                }
            }
            return listData;
        }

        
        public void SaveValuesToFile(Power[] powers)
        {
            string dateStr = DateTime.Today.ToString("yyMMdd");

            foreach (var pwr in powers) {
                string ts = pwr.ElapsedTime.ToString() + ',';
                dataFile.AddToFile("AOU\\TBufferCold\\", "TBufferCold-" + dateStr + ".txt", ts + pwr.TBufferCold);
                dataFile.AddToFile("AOU\\TBufferMid\\", "TBufferMid-" + dateStr + ".txt", ts + pwr.TBufferMid);
                dataFile.AddToFile("AOU\\TBufferHot\\", "TBufferHot-" + dateStr + ".txt", ts + pwr.TBufferHot);
                dataFile.AddToFile("AOU\\TCoolTank\\", "TCoolTank-" + dateStr + ".txt", ts + pwr.TColdTank);
                dataFile.AddToFile("AOU\\THotTank\\", "THotTank-" + dateStr + ".txt", ts + pwr.THotTank);
                dataFile.AddToFile("AOU\\State\\", "State-" + dateStr + ".txt", ts + pwr.State.ToString());
                dataFile.AddToFile("AOU\\TReturnActual\\", "TReturnActual-" + dateStr + ".txt", ts + pwr.TReturnActual);
                dataFile.AddToFile("AOU\\TReturnForecasted\\", "TReturnForecasted-" + dateStr + ".txt", ts + pwr.TReturnForecasted);
                dataFile.AddToFile("AOU\\TReturnValve\\", "TReturnValve-" + dateStr + ".txt", ts + pwr.TReturnValve);
            }
        }

        public void Update()
        {
            if (runMode == RunType.Random && randomData != null)
            {
                if (randomData.NewRandomLogMessageAvailable())
                { 
                    logMessages.Add(randomData.GetRandomLogMsg());
                    if (logMessages.Count > MaxTotalValuesInMemory)
                    {
                        logMessages.RemoveAt(0);
                    }
                }

                powerValues.Add(randomData.GetRandomPower());
                if (powerValues.Count > MaxTotalValuesInMemory)
                {
                    powerValues.RemoveAt(0);
                }
            }
            else if (runMode == RunType.File && dataFile != null)
            {
                if (IsDataAvailable())
                {
                    powerValues = GetTextDataList(out logMessages);
                }

            }
            else if (runMode == RunType.Serial)
            {
                if (IsDataAvailable())
                { 
                    var dt = DateTime.Now;
                    uint time = (uint)(dt.Hour * 60 * 1000 + dt.Minute * 1000 + DateTime.Now.Millisecond);
                    string msg = serialData.GetTextData();
                    logMessages.Add(new AOULogMessage(time, msg));
                    // powerValues.Add(serialData.GetLatestValues());
                }
            }

            // SaveValuesToFile(new Power[] { pwr });
        }

        /************************
            Value Handling
        ************************/
        public Power[] GetLastPowerValues(int count)
        {
            if (powerValues.Count > 0)
            {
                if (count > powerValues.Count)
                {
                    count = powerValues.Count;
                }
                lastPowerValuesCount = powerValues.Count;
                return powerValues.GetRange(lastPowerValuesCount - count, count).ToArray();
            }
            else
            {
                return new Power[0];
            }
        }

        public bool NewPowerDataIsAvailable()
        {
            return (powerValues.Count  > lastPowerValuesCount);
        }

        public Power GetLastPowerValue()
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

        public Power GetPowerValuesFromStartTime(long time, int count)
        {
            int index = 0;
            var res = powerValues.Find(item => item.ElapsedTime == time);
            return res;

            // return powerValues.GetRange(lastPowerValuesCount - count, count).ToArray();
        }

        /**************************
            Log Message Handling
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
