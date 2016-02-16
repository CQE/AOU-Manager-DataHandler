﻿using System;
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

        // Different Run types. File and Random are test modes
        public enum RunType {Serial, File, Random};
        public RunType runMode
        {
            get; private set;
        }

        // Object to read and write text to serial port
        private AOUSerialData serialData;

        // Object to read and write to text files in User Image folder
        private TextFile dataFile;

        // Object to read random text data
        private AOURandomData randomData;

        // Text to parse from file, serial or random
        private string textDataStream = "";

        private List<AOULogMessage> logMessages;
        private List<Power> powerValues;

        private int lastLogMessageCount;
        private int lastPowerValuesCount;

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

        #region PrivateMethods
        private bool IsDataAvailable()
        {
            if (runMode == RunType.Serial && serialData != null)
            {
                return serialData.IsDataAvailable();
            }
            else if (runMode == RunType.File && dataFile != null)
            {
                return dataFile.IsDataAvailable();
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

        private int GetTextData()
        {
            string text = "error";
            if (runMode == RunType.Serial && serialData != null)
            {
                text = serialData.GetTextData();
            }
            else if (runMode == RunType.File && dataFile != null)
            {
                text = dataFile.GetTextData();
            }
            text = text.Replace("\t", "");
            text = text.Replace("\r", "");
            text = text.Replace("\n", "");

            textDataStream += text;

            return textDataStream.Length;
       }

        public void SendToPlc(string text)
        {
            if (runMode == RunType.Random)
            {
                logMessages.Add(new AOULogMessage(1000 * 4, "SendToPlc: "+ text));
                // ToDo: Save to log file
            }
            else if (runMode == RunType.File)
            {
                logMessages.Add(new AOULogMessage(1000 * 4, "SendToPlc: " + text));
                // ToDo: Save to log file
            }
            else if (runMode == RunType.Serial && serialData != null)
            {
                serialData.SendData(text);
            }
        }

        public void SendCommandToPlc(AOUTypes.CommandType cmd, int value)
        {
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

        private List<Power> GetTextDataList(out List<AOULogMessage> logList)
        {
            long time_ms = 0;
            string logMsg = "";

            AOUSeqData seqData;
            AOUTemperatureData tempData;
            AOUFeedData feedData;
            AOULevelData levelData;
            AOUValvesData valvesData;
            AOUIMMData immData;


            long min_ms = long.MaxValue;
            long max_ms = 0;

            List<Power> listData = new List<Power>();

            List<AOULogMessage> newLogs = new List<AOULogMessage>();
            logList = new List<AOULogMessage>();

            int prevTextLength = GetTextData();

            while (prevTextLength > 0)
            {
                Power tempPower = new Power();
                int count = 0;

                string nextTag = AOUInputParser.GetNextTag(textDataStream);
                if (nextTag.Length > 0)
                {
                    logList.AddRange(AOUInputParser.ParseTagLogMessages(nextTag, textDataStream));
                }

                if (nextTag == AOUInputParser.tagTemperature)
                {
                    if (AOUInputParser.ParseTemperature(textDataStream, out tempData, out newLogs, out count))
                    {
                        tempPower.ElapsedTime = AOUTypes.AOUModelTimeToTimeMs(tempData.time_min_of_week, tempData.time_ms_of_min);
                        tempPower.THotTank = tempData.hotTankTemp;
                        tempPower.TColdTank = tempData.coldTankTemp;
                        tempPower.TReturnActual = tempData.retTemp;
                        tempPower.ValveCoolant = tempData.coolerTemp;
                    }
                }
                else if (nextTag == AOUInputParser.tagSequence)
                {
                    if (AOUInputParser.ParseSequence(textDataStream, out seqData, out count))
                    {
                        tempPower.ElapsedTime = AOUTypes.AOUModelTimeToTimeMs(seqData.time_min_of_week, seqData.time_ms_of_min);
                        tempPower.State = (AOUTypes.StateType)seqData.state;
                        // tempPower.Cycle = seqData.cycle;
                    }
                }
                else if (nextTag == AOUInputParser.tagIMM)
                {
                    if (AOUInputParser.ParseIMM(textDataStream, out immData, out count))
                    {
                        tempPower.ElapsedTime = AOUTypes.AOUModelTimeToTimeMs(immData.time_min_of_week, immData.time_ms_of_min);
                        AOUTypes.IMMSettings set = (AOUTypes.IMMSettings)immData.imm_setting_type;
                        long value = immData.imm_setting_val;
                        // tempPower.State = Types(immData;
                    }
                }
                else if (nextTag == AOUInputParser.tagFeeds)
                {
                    if (AOUInputParser.ParseFeeds(textDataStream, out feedData, out count))
                    {
                        tempPower.ElapsedTime = AOUTypes.AOUModelTimeToTimeMs(feedData.time_min_of_week, feedData.time_ms_of_min);
                        tempPower.TReturnActual = feedData.prevFeedTemp;
                        tempPower.TReturnForecasted = feedData.newFeedTemp;
                    }
                }
                else if (nextTag == AOUInputParser.tagLevels)
                {
                    if (AOUInputParser.ParseLevels(textDataStream, out levelData, out count))
                    {
                        tempPower.ElapsedTime = AOUTypes.AOUModelTimeToTimeMs(levelData.time_min_of_week, levelData.time_ms_of_min);
                        // tempPower.TReturnActual = levelData.prevLevel;
                        // tempPower.TReturnForecasted = levelData.newLevel;
                    }
                }
                else if (nextTag == AOUInputParser.tagValves)
                {
                    if (AOUInputParser.ParseValves(textDataStream, out valvesData, out count))
                    {
                        tempPower.ElapsedTime = AOUTypes.AOUModelTimeToTimeMs(valvesData.time_min_of_week, valvesData.time_ms_of_min);
                        tempPower.TReturnValve = valvesData.prevValveReturnTemp;
                        tempPower.TReturnActual = valvesData.prevValveReturnTemp;
                    }
                }
                else if (nextTag == AOUInputParser.tagLog)
                {
                    if (AOUInputParser.ParseLog(textDataStream, out time_ms, out logMsg, out count))
                    {
                        AOULogMessage msg = new AOULogMessage((uint)time_ms, logMsg);
                        logList.Add(msg);
                    }
                }
                else if (nextTag.Length > 0)
                {
                    string unknownTagText;
                    AOUInputParser.FindTagAndExtractText(nextTag, textDataStream, out unknownTagText, out count);
                    logList.Add(new AOULogMessage(1000, "Unknown tag:"+ nextTag, 10, 0));
                }

                if (AOUInputParser.ValidPowerTag(nextTag))
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
                                if (pwr.State == AOUTypes.StateType.NOTHING) pwr.State = tempPower.State;
                                if (pwr.TColdTank == double.NaN) pwr.TColdTank = tempPower.TColdTank;
                                if (pwr.THotTank == double.NaN) pwr.THotTank = tempPower.THotTank;
                                if (pwr.TReturnActual == double.NaN) pwr.TReturnActual = tempPower.TReturnActual;
                                if (pwr.TReturnForecasted == double.NaN) pwr.TReturnForecasted = tempPower.TReturnForecasted;
                                if (pwr.TReturnValve == double.NaN) pwr.TReturnValve = tempPower.TReturnValve;
                                if (pwr.TBufferCold == double.NaN) pwr.TBufferCold = tempPower.TBufferCold;
                                if (pwr.TBufferMid == double.NaN) pwr.TBufferMid = tempPower.TBufferMid;
                                if (pwr.TBufferHot == double.NaN) pwr.TBufferHot = tempPower.TBufferHot;
                                if (pwr.THeaterOilOut == double.NaN) pwr.THeaterOilOut = tempPower.THeaterOilOut;
                                if (pwr.THeatExchangerCoolantOut == double.NaN) pwr.THeatExchangerCoolantOut = tempPower.THeatExchangerCoolantOut;

                                if (pwr.PowerHeating == double.NaN) pwr.PowerHeating = tempPower.PowerHeating;

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
                if (count == 0) // No more valid tags. Wait for more data
                {
                   break; 
                }
                else
                {
                    textDataStream = textDataStream.Substring(count); // Delete handled tag
                }
            }
            return listData;
        }

        
        private void SaveValuesToFile(Power[] powers)
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
        #endregion

        #region PublicMethods
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
                    powerValues = GetTextDataList(out logMessages);
                    /*
                    var dt = DateTime.Now;
                    uint time = (uint)(dt.Hour * 60 * 1000 + dt.Minute * 1000 + DateTime.Now.Millisecond);
                    string msg = serialData.GetTextData();
                    logMessages.Add(new AOULogMessage(time, msg));
                    // powerValues.Add(serialData.GetLatestValues());
                    */
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
        #endregion
    }
}
