using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;

namespace DataHandler
{

    public class AOUInputParser
    {
        public const string tagSubTagTime = "Time"; //

        public const string tagTemperature = "temperature";
        public const string tagTempSubTagHot = "Hot";
        public const string tagTempSubTagCold = "Cold";
        public const string tagTempSubTagRet = "Ret";
        public const string tagTempSubTagCool = "Cool";
        public const string tagTempSubTagSpare1 = "Spare1";

        public const string tagFeeds = "feeds";
        public const string tagFeedsHot = "Hot";
        public const string tagFeedsCold = "Cold";
        public const string tagFeedsPrev = "Prev";
        public const string tagFeedsNew = "New";

        public const string tagSequence = "seq";
        public const string tagSeqSubTagState = "State";
        public const string tagSeqSubTagCycle = "Cycle";
        public const string tagSeqSubTagDesc = "Descr";
        public const string tagSeqSubTagLeave = "Leave";

        public const string tagValves = "valves";
        public const string tagValvesSubTagRet = "Ret";
        public const string tagValvesSubTagRetPrev = "Prev";
        public const string tagValvesSubTagRetNew = "New";

        public const string tagIMM = "imm";
        public const string tagIMMSubTagSetIMMError = "SetIMMError";
        public const string tagIMMSubTagIMMBlockInject = "SetIMMBlockInject";
        public const string tagIMMSubTagIMMBlockOpen = "SetIMMBlockOpen";
        public const string tagIMMSubTagIMMStop = "IMMStop";
        public const string tagIMMSubTagCycleAuto = "CycleAuto";
        public const string tagIMMSubTagIMMInjecting = "IMMInjecting";
        public const string tagIMMSubTagIMMEjecting = "IMMEjecting";
        public const string tagIMMSubTagIMMToolClosed = "IMMToolClosed";

        public const string tagLevels = "levels";
        public const string tagLevelsSubTagCold = "Cold";
        public const string tagLevelsSubTagHot = "Hot";
        public const string tagLevelsSubTagPrev = "Prev";
        public const string tagLevelsSubTagNew = "New";

        public const string tagLog = "log";
        public const string tagLogSubTagMsg = "Msg";

        public static bool ValidPowerTag(string tag)
        {
            if (tag == tagTemperature || tag == tagFeeds || tag == tagSequence ||
                tag == tagValves)
            {
                return true;
            }
            return false;
        }

        #region Common
        public static string GetNextTag(string textLine, out string strBeforeTag)
        {
            Regex r = new Regex("<[a-zA-Z]+>");
            Match m = r.Match(textLine, 0);
            strBeforeTag = "";
            if (m.Success)
            {
                string tag = m.Groups[0].Value.Substring(1, m.Groups[0].Value.Length - 2);
                if (textLine.IndexOf(tag) > 1)
                {
                    strBeforeTag = textLine.Substring(0, textLine.IndexOf(tag)-1);
                }
                return tag;
            }
            else
                return "";
        }

        public static bool FindTag(string tag, string textLine)
        {
            string startTag = "<" + tag + ">";
            string endTag = "</" + tag + ">";
            int pos1 = textLine.IndexOf(startTag);
            int pos2 = textLine.IndexOf(endTag);
            if (pos1 != -1 && pos2 != -1)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public static bool FindTagAndExtractText(string tag, string textLine, out string tagText, out int endPos)
        {
            string startTag = "<" + tag + ">";
            string endTag = "</" + tag + ">";
            int pos1 = textLine.IndexOf(startTag);
            int pos2 = textLine.IndexOf(endTag);
            if (pos1 != -1 && pos2 != -1)
            {
                pos1 += startTag.Length;
                tagText = textLine.Substring(pos1, pos2 - pos1);
                endPos = pos2 + endTag.Length;
                tagText = tagText.Trim();
                return true;
            }
            else
            {
                tagText = "";
                endPos = 0;
                return false;
            }

        }

        public static bool ParseString(string tagText, string textline, out string text)
        {
            int endpos = 0;
            return FindTagAndExtractText(tagText, textline, out text, out endpos);
        }

        public static bool ParseWord(string tag, string textline, out UInt16 value)
        {
            double dbl = double.NaN;
            if (Parsedouble(tag, textline, out dbl))
            {
                value = (UInt16)Math.Round(dbl);
                return true;
            }
            value = UInt16.MaxValue;
            return false;
        }

        public static bool Parsedouble(string tag, string textline, out double value)
        {
            int endpos = 0;
            string tagValue = "";

            if (FindTagAndExtractText(tag, textline, out tagValue, out endpos))
            {
                tagValue.Replace(',', '.');
                return double.TryParse(tagValue, out value);
            }
            value = 0;
            return false;
        }

        public static bool ParseLong(string tagText, string textline, out long value)
        {
            int endpos = 0;
            if (FindTagAndExtractText(tagText, textline, out tagText, out endpos) &&
                long.TryParse(tagText, out value))
            {
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }

        public static Byte LowByte(UInt16 word)
        {
            return (Byte)(word & 0xff);
        }

        public static Byte HighByte(UInt16 word)
        {
            return (Byte)(word >> 8);
        }

        #endregion

        #region CreateXML
        public static string CreateTempXmlString(long time_ms, AOUTemperatureData tempData)
        {
            return String.Format("<{0}><{1}>{6}</{1}><{2}>{7}</{2}><{3}>{8}</{3}><{4}>{9}</{4}><{5}>{10}</{5}></{0}>",
                                    tagTemperature,
                                    tagSubTagTime, tagTempSubTagHot, tagTempSubTagCold, tagTempSubTagRet, tagTempSubTagCool, // 1 - 5
                                    time_ms, 
                                    tempData.hotTankTemp, tempData.coldTankTemp, tempData.retTemp, tempData.coolerTemp); // 6 - 10
        }

        public static string CreateSeqXmlString(long time_ms, AOUSeqData tempData)
        {
            return String.Format("<{0}><{1}>{6}</{1}><{2}>{7}</{2}><{3}>{8}</{3}><{4}>{9}</{4}><{5}>{10}</{5}></{0}>",
                                    tagSequence, tagSubTagTime,
                                    tagSeqSubTagState, tagSeqSubTagCycle, tagSeqSubTagDesc, tagSeqSubTagLeave, // 2 - 5
                                    time_ms, tempData.state, tempData.cycle, "Desc...", "Leave..."); // 6 - 10
        }

        public static string CreateIMMXmlString(long time_ms, AOUIMMData tempData)
        {
            string IMMSettingsTag = null;

            switch ((AOUTypes.IMMSettings)tempData.imm_setting_type)
            {
                case AOUTypes.IMMSettings.CycleAuto: IMMSettingsTag = tagIMMSubTagCycleAuto; break;
                case AOUTypes.IMMSettings.IMMEjecting: IMMSettingsTag = tagIMMSubTagIMMEjecting; break;
                case AOUTypes.IMMSettings.IMMInjecting: IMMSettingsTag = tagIMMSubTagIMMInjecting; break;
                case AOUTypes.IMMSettings.IMMStop: IMMSettingsTag = tagIMMSubTagIMMStop; break;
                case AOUTypes.IMMSettings.IMMToolClosed: IMMSettingsTag = tagIMMSubTagIMMToolClosed; break;
                case AOUTypes.IMMSettings.SetIMMBlockInject: IMMSettingsTag = tagIMMSubTagIMMBlockInject; break;
                case AOUTypes.IMMSettings.SetIMMBlockOpen: IMMSettingsTag = tagIMMSubTagIMMBlockOpen; break;
                case AOUTypes.IMMSettings.SetIMMError: IMMSettingsTag = tagIMMSubTagSetIMMError;  break;
                default: IMMSettingsTag = tagIMMSubTagSetIMMError; break;
            }

            return String.Format("<{0}><{1}>{3}</{1}><{2}>{4}</{2}></{0}>",
                                    tagIMM, tagSubTagTime, IMMSettingsTag, // 0 - 2
                                    time_ms, tempData.imm_setting_val); // 3 - 4
        }

        public static string CreateFeedsXmlString(long time_ms, AOUFeedData data)
        {
            if (data.AOUFeedDataHeader == AOUTypes.AOUColdFeedDataId) // Cold feed
            {
                return String.Format("<{0}><{1}>{5}</{1}><{2}> <{3}>{6}</{3}><{4}>{7}</{4}> </{2}></{0}>",
                                        tagFeeds, tagSubTagTime, tagFeedsCold, // 0 - 2
                                        tagFeedsPrev, tagFeedsNew, // 3, 4
                                        time_ms, data.prevFeedTemp, data.newFeedTemp); // 5, 6, 7
            }
            else // Hot feed
            {
                return String.Format("<{0}><{1}>{5}</{1}><{2}> <{3}>{6}</{3}><{4}>{7}</{4}> </{2}></{0}>",
                                        tagFeeds, tagSubTagTime, tagFeedsCold, // 0 - 2
                                        tagFeedsPrev, tagFeedsNew, // 3, 4
                                        time_ms, data.prevFeedTemp, data.newFeedTemp); // 5, 6, 7
            }
        }

        public static string CreateLevelsXmlString(long time_ms, AOULevelData data)
        {
            if (data.AOULevelDataHeader == AOUTypes.AOUColdLevelDataId) // Cold feed
            {
                return String.Format("<{0}><{1}>{5}</{1}><{2}> <{3}>{6}</{3}><{4}>{7}</{4}> </{2}></{0}>",
                                        tagLevels, tagSubTagTime, tagLevelsSubTagHot, // 0 - 2
                                        tagLevelsSubTagPrev, tagLevelsSubTagNew, // 3, 4
                                        time_ms, data.prevLevel, data.newLevel); // 5, 6, 7
            }
            else // Hot feed
            {
                return String.Format("<{0}><{1}>{5}</{1}><{2}> <{3}>{6}</{3}><{4}>{7}</{4}> </{2}></{0}>",
                                        tagFeeds, tagSubTagTime, tagLevelsSubTagCold, // 0 - 2
                                        tagFeedsPrev, tagFeedsNew, // 3, 4
                                        time_ms, data.prevLevel, data.newLevel); // 5, 6, 7
            }
        }

        public static string CreateValvesXmlString(long time_ms, AOUValvesData data)
        {
            return String.Format("<{0}><{1}>{5}</{1}><{2}>{7}</{2}><{3}>{6}</{3}><{4}>{7}</{4}></{2}></{0}>",
                                    tagValves, tagSubTagTime, tagValvesSubTagRet, // 0 - 2
                                    tagValvesSubTagRetPrev, tagValvesSubTagRetNew, // 3 - 4
                                    time_ms, data.prevValveReturnTemp, data.newValveReturnTemp); // 5 - 7
        }

        public static string CreateLogXmlString(long time_ms, AOULogMessage data)
        {
            return String.Format("<{0}><{1}>{3}</{1}><{2}>{4}</{2}></{0}>",
                                    tagLog, tagSubTagTime, tagLogSubTagMsg, // 0 - 2
                                    time_ms, data.message); // 3 - 4
        }


        #endregion

        #region ParseXML
        public static bool ParseTime_ms(string textline, out TimeSpan time)
        {

            long time_ms = 0;
            if (ParseLong(tagSubTagTime, textline, out time_ms))
            {
                time = TimeSpan.FromMilliseconds(time_ms);
                return true;
            }
            else
            {
                time = TimeSpan.Zero;
                return false;
            }
        }

        public static bool ParseWordTime(string textline, out UInt16 time_min_of_week, out UInt16 time_ms_of_min) 
        {
            long time_ms = 0;
            if (ParseLong(tagSubTagTime, textline, out time_ms))
            {
                AOUTypes.TimeMsToAOUModelTime(time_ms, out time_min_of_week, out time_ms_of_min);
                return true;
            }
            else
            {
                time_min_of_week = 0;
                time_ms_of_min = 0;
                return false;
            }
        }

        public static bool ParseLongTime(string textline, out long time_ms) // Not to be misunderstood
        {
            return ParseLong(tagSubTagTime, textline, out time_ms);
        }

        public static List<AOULogMessage> ParseBetweenTagsMessages(string tag, string text)
        {
            List<AOULogMessage> logs = new List<AOULogMessage>();

            string tagtext;
            long time_ms = 0;
            if (ParseString(tag, text, out tagtext))
            {
                ParseLongTime(tagtext, out time_ms); // <Time> value before message

                var r = new Regex("<\\/([a-zA-Z]+)>([^<]+)<"); // match "</tag>message<"
                var matches = r.Matches(tagtext, 0);
                if (matches.Count > 0)
                {
                    foreach (var match in matches)
                    {
                        string s = match.ToString();
                        int n = s.IndexOf('>');
                        string tagBefore = s.Substring(2, n - 2);
                        string between = s.Substring(n + 1, s.Length - n - 2).Trim();
                        if (between.Length > 2)
                        {
                            logs.Add(new AOULogMessage((uint)time_ms, between, 10, 0));
                        }
                    }
                }
            }
            return logs;
        }

        public static bool ParseTemperature(string textLine, out AOUTemperatureData tempData, out List<AOULogMessage> logList, out int endpos)
        {
            // textLine = "<temperature><Time>104898416</Time><Hot>122</Hot><Cold>56</Cold><Ret>68</Ret><Cool>40</Cool></temperature>";
            string tempText = "";

            tempData.AOUTempDataHeader = AOUTypes.AOUTempDataId;
            tempData.time_min_of_week = 0;
            tempData.time_ms_of_min = 0;
            tempData.coldTankTemp = AOUTypes.UInt16_NaN; 
            tempData.hotTankTemp = AOUTypes.UInt16_NaN;
            tempData.retTemp = AOUTypes.UInt16_NaN;
            tempData.coolerTemp = AOUTypes.UInt16_NaN;

            logList = new List<AOULogMessage>();

            if (FindTagAndExtractText(tagTemperature, textLine, out tempText, out endpos))
            {
                return 
                    ParseWordTime(tempText, out tempData.time_min_of_week, out tempData.time_ms_of_min) &&
                    ParseWord(tagTempSubTagHot, tempText, out tempData.hotTankTemp) &&
                    ParseWord(tagTempSubTagCold, tempText, out tempData.coldTankTemp) &&
                    ParseWord(tagTempSubTagRet, tempText, out tempData.retTemp) &&
                    ParseWord(tagTempSubTagCool, tempText, out tempData.coolerTemp);
            }
            return false;
        }

        public static bool ParseValves(string textLine, out AOUValvesData valvesData, out int endpos)
        {
            // textLine = "<valves><Time>104903816</Time><Ret><Prev>93</Prev><New>80</New></Ret></valves>";
            string tempText = "";

            valvesData.AOUValvesDataHeader = AOUTypes.AOUValvesDataId;
            valvesData.time_min_of_week = 0;
            valvesData.time_ms_of_min = 0;
            valvesData.prevValveReturnTemp = AOUTypes.UInt16_NaN;
            valvesData.newValveReturnTemp = AOUTypes.UInt16_NaN;

            if (FindTagAndExtractText(tagValves, textLine, out tempText, out endpos))
            { // ToDo Check tagValvesSubTagRet
                if (ParseWordTime(tempText, out valvesData.time_min_of_week, out valvesData.time_ms_of_min) &&
                    ParseWord(tagValvesSubTagRetPrev, tempText, out valvesData.prevValveReturnTemp) &&
                    ParseWord(tagValvesSubTagRetNew, tempText, out valvesData.newValveReturnTemp))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        public static bool ParseFeeds(string textLine, out AOUFeedData feedData, out int endpos)
        {
            // textLine = "<feeds><Time>104894473</Time><Hot><Prev>60</Prev></New>63,75</New></Hot></feeds>";
            // textLine = "<feeds><Time>104878268</Time><Cold><Prev>65</Prev></New>60</New></Cold></feeds>";
            string tempText = "";

            feedData.AOUFeedDataHeader = AOUTypes.AOUHotFeedDataId;
            feedData.time_min_of_week = 0;
            feedData.time_ms_of_min = 0;
            feedData.prevFeedTemp = AOUTypes.UInt16_NaN;
            feedData.newFeedTemp = AOUTypes.UInt16_NaN;

            if (FindTagAndExtractText(tagFeeds, textLine, out tempText, out endpos))
            {
                /*
                if (FindTag(tagFeedsHot, tempText))
                {
                }
                else 
                */
                if (FindTag(tagFeedsCold, tempText))
                {
                    feedData.AOUFeedDataHeader = AOUTypes.AOUColdFeedDataId;
                }

                if (ParseWordTime(tempText, out feedData.time_min_of_week, out feedData.time_ms_of_min) &&
                    ParseWord(tagFeedsPrev, tempText, out feedData.prevFeedTemp) &&
                    ParseWord(tagFeedsNew, tempText, out feedData.newFeedTemp))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        public static bool ParseIMM(string textLine, out AOUIMMData immData, out int endpos)
        {
            string tempText = "";
            UInt16 tempValue = 0;

            immData.AOUIMMDataHeader = AOUTypes.AOUIMMDataId;
            immData.time_min_of_week = 0;
            immData.time_ms_of_min = 0;
            immData.imm_setting_val = AOUTypes.UInt16_NaN;
            immData.imm_setting_type = (UInt16)AOUTypes.IMMSettings.Nothing;

            if (FindTagAndExtractText(tagIMM, textLine, out tempText, out endpos) &&
               ParseWordTime(tempText, out immData.time_min_of_week, out immData.time_ms_of_min))
            {
                if (ParseWord(tagIMMSubTagSetIMMError, tempText, out tempValue))
                {
                    immData.imm_setting_type = (UInt16)AOUTypes.IMMSettings.SetIMMError;
                    immData.imm_setting_val = tempValue;
                }
                else if (ParseWord(tagIMMSubTagCycleAuto, tempText, out tempValue))
                {
                    immData.imm_setting_type = (UInt16)AOUTypes.IMMSettings.CycleAuto;
                    immData.imm_setting_val = tempValue;
                }
                else if (ParseWord(tagIMMSubTagIMMEjecting, tempText, out tempValue))
                {
                    immData.imm_setting_type = (UInt16)AOUTypes.IMMSettings.IMMEjecting;
                    immData.imm_setting_val = tempValue;
                }
                else if (ParseWord(tagIMMSubTagIMMInjecting, tempText, out tempValue))
                {
                    immData.imm_setting_type = (UInt16)AOUTypes.IMMSettings.IMMInjecting;
                    immData.imm_setting_val = tempValue;
                }
                else if (ParseWord(tagIMMSubTagIMMToolClosed, tempText, out tempValue))
                {
                    immData.imm_setting_type = (UInt16)AOUTypes.IMMSettings.IMMToolClosed;
                    immData.imm_setting_val = tempValue;
                }
                else if (ParseWord(tagIMMSubTagIMMStop, tempText, out tempValue))
                {
                    immData.imm_setting_type = (UInt16)AOUTypes.IMMSettings.IMMStop;
                    immData.imm_setting_val = tempValue;
                }
                else if (ParseWord(tagIMMSubTagIMMBlockInject, tempText, out tempValue))
                {
                    immData.imm_setting_type = (UInt16)AOUTypes.IMMSettings.SetIMMBlockInject;
                    immData.imm_setting_val = tempValue;
                }
                else if (ParseWord(tagIMMSubTagIMMBlockOpen, tempText, out tempValue))
                {
                    immData.imm_setting_type = (UInt16)AOUTypes.IMMSettings.SetIMMBlockOpen;
                    immData.imm_setting_val = tempValue;
                }
            }

            return immData.imm_setting_type != (UInt16)AOUTypes.IMMSettings.Nothing;
        }
 
        public static bool ParseLevels(string textLine, out AOULevelData levelData, out int endpos)
        {
            // textLine = "<feeds><Time>104894473</Time><Hot><Prev>60</Prev></New>63,75</New></Hot></feeds>";
            // textLine = "<feeds><Time>104878268</Time><Cold><Prev>65</Prev></New>60</New></Cold></feeds>";
            string tempText = "";

            levelData.AOULevelDataHeader = AOUTypes.AOUColdFeedDataId;
            levelData.time_min_of_week = 0;
            levelData.time_ms_of_min = 0;
            levelData.prevLevel = AOUTypes.UInt16_NaN;
            levelData.newLevel = AOUTypes.UInt16_NaN;

            if (FindTagAndExtractText(tagLevels, textLine, out tempText, out endpos))
            {
                if (FindTag(tagLevelsSubTagHot, tempText))
                {
                    levelData.AOULevelDataHeader = AOUTypes.AOUHotLevelDataId;
                }

                if (ParseWordTime(tempText, out levelData.time_min_of_week, out levelData.time_ms_of_min) &&
                    ParseWord(tagLevelsSubTagPrev, tempText, out levelData.prevLevel) &&
                    ParseWord(tagLevelsSubTagNew, tempText, out levelData.newLevel))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        public static bool ParseLog(string textLine, out long time_ms, out string logMsg, out int endpos)
        {
            string tempText;
            time_ms = 0;
            logMsg = "";
            // textLine = "<log><Time>94962045</Time><Msg>Setup AOU version 1.1 ready (Plastics Unbound Ltd, Cyprus)</Msg></log>";
            if (FindTagAndExtractText(tagLog, textLine, out tempText, out endpos))
            {
                if (ParseLongTime(tempText, out time_ms) &&
                    ParseString(tagLogSubTagMsg, tempText, out logMsg))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool ParseSequence(string textLine, out AOUSeqData seqData, out int endpos)
        {
            string tempText;

            seqData.AOUSeqDataHeader = AOUTypes.AOUSeqDataId;
            seqData.time_min_of_week = 0;
            seqData.time_ms_of_min = 0;
            seqData.state = (UInt16)AOUTypes.StateType.NOTHING;
            seqData.cycle = 0;

            if (FindTagAndExtractText(tagSequence, textLine, out tempText, out endpos))
            {
                if (ParseWordTime(tempText, out seqData.time_min_of_week, out seqData.time_ms_of_min) &&
                    ParseWord(tagSeqSubTagState, tempText, out seqData.state) &&
                    ParseWord(tagSeqSubTagCycle, tempText, out seqData.cycle))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

    }
}
