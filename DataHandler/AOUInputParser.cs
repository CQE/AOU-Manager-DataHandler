﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;

namespace DemoPrototype
{

    public class AOUInputParser
    {

        static string valvesStr = "3F01";
        static int seqNr = 0;

        #region Tag Constants
        public const string tagSubTagTime = "Time"; //


        public const string tagState = "state";
        public const string tagTemp = "temp";

        public const string tagTempBuHot = "BuHot";
        public const string tagTempBuMid = "BuMid";
        public const string tagTempBuCold = "BuCold";

        public const string tagTempSubTagHot = "Hot";
        public const string tagTempSubTagCold = "Cold";
        public const string tagTempSubTagRet = "Ret";

        public const string tagTempSubTagCool = "Cool";
        public const string tagTempSubTagHeat = "Heat";
        public const string tagTempSubTagBearHot = "BearHot";

        public const string tagPower = "Pow";
        public const string tagValves = "Valves";
        public const string tagEnergy = "Energy";
        public const string tagUI = "UI";
        public const string tagIMM = "IMM";
        public const string tagMode = "Mode";
        public const string tagSeqState = "Seq";

        public const string tagLog = "log";
        public const string tagLogSubTagMsg = "Msg";

        #endregion


        public static bool ValidPowerTag(string tag)
        {
            return (tag == tagState || tag == tagLog);
        }

        #region Common
        public static string GetNextTag(string text, out string tagContent, out List<string> logs, out int numHandled)
        {
            Regex rTag = new Regex("<[a-zA-Z]+>");
            logs = new List<string>();
            tagContent = "";
            numHandled = 0;

            int lastTextPos = 0;
            bool eot = false;
            string tag = "";
            int tlen = text.Length;
            string textLine = "";
            do
            {
                tag = "";
                textLine = "";
                if (text.IndexOf("\r\n") > 0)
                {
                    int endPos = text.IndexOf("\r\n", lastTextPos + 1);
                    if (endPos >= 0)
                    {
                        textLine = text.Substring(lastTextPos, endPos - lastTextPos).Trim();
                        lastTextPos = endPos + 1;
                    }
                    else
                    {
                        lastTextPos = lastTextPos + 2;
                    }
                }
                /* If only LF*/
                else if (text.IndexOf("\n") > 0)
                {
                    if ((lastTextPos + 1) < tlen)
                    {
                        int endPos = text.IndexOf("\n", lastTextPos + 1);
                        if (endPos >= 0)
                        {
                            textLine = text.Substring(lastTextPos, endPos - lastTextPos).Trim();
                            lastTextPos = endPos + 1;
                        }
                        else
                        {
                            lastTextPos = lastTextPos + 1;
                        }
                    }
                    else
                    {
                        int err = lastTextPos;
                    }
                }

                if (!eot && textLine.Length > 0)
                {
                    Match m = rTag.Match(textLine, 0);
                    if (m.Success)
                    {
                        tag = m.Groups[0].Value.Substring(1, m.Groups[0].Value.Length - 2);
                        int tagEndPos = 0;
                        FindTagAndExtractText(tag, textLine, out tagContent, out tagEndPos);
                        break;
                    }
                    else
                    {
                        logs.Add(textLine);
                        textLine = "";
                    }
                }
                else
                {
                    bool dbg = true;
                }
            } while (tag == "" && textLine.Length > 0);
            numHandled = lastTextPos;
            return tag;
        }

        public static bool FindTag(string tag, string textLine)
        {
            string startTag = "<" + tag + ">";
            string endTag = "</" + tag + ">";
            int pos1 = textLine.IndexOf(startTag);
            int pos2 = textLine.IndexOf(endTag);

            return (pos1 != -1 && pos2 != -1);
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

        public static bool ParseMMSS(string tag, string textline, out byte mask, out byte state)
        {
            int endpos = 0;
            string tagValue = "";
            mask = 0; state = 0;

            if (FindTagAndExtractText(tag, textline, out tagValue, out endpos) && tagValue.Length == 4)
            {
                string mm = tagValue.Substring(0, 2);
                string ss = tagValue.Substring(2, 2);
                return (byte.TryParse(mm, System.Globalization.NumberStyles.HexNumber, null, out mask) &&
                        byte.TryParse(ss, System.Globalization.NumberStyles.HexNumber, null, out state));
            }
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

        /*
        public static bool ParseWordTime(string textline, out UInt16 time_min_of_week, out UInt16 time_ms_of_min) 
        {
            long time_ms = 0;
            if (ParseLong(tagSubTagTime, textline, out time_ms))
            {
                if (time_ms > 283200)
                {
                    long t2 = time_ms;
                }
                AOUDataTypes.AOUModelTimeSecX10_to_TimeMs(time_ms, out time_min_of_week, out time_ms_of_min);
                return true;
            }
            else
            {
                time_min_of_week = 0;
                time_ms_of_min = 0;
                return false;
            }
        }
        */

        public static bool ParseWordTime_sek_x_10(string textline, out UInt16 time_hours, out UInt16 time_sek_x_10)
        {
            long time_s_x_10 = 0;
            if (ParseLong(tagSubTagTime, textline, out time_s_x_10))
            {
                AOUDataTypes.Time_ms_to_AOUModelTimeSecX10(time_s_x_10 * 100, out time_hours, out time_sek_x_10);
                return true;
            }
            else
            {
                time_sek_x_10 = 0;
                time_hours = 0;
                return false;
            }
        }

        public static bool ParseLongTime(string textline, out long time_ms) // Not to be misunderstood
        {
            return ParseLong(tagSubTagTime, textline, out time_ms);
        }

        public static List<string> ParseBetweenTagsMessages(string tagText)
        {
            List<string> logs = new List<string>();

            long time_ms = 0;
            ParseLongTime(tagText, out time_ms); // <Time> value before message

            var r = new Regex("<\\/([a-zA-Z]+)>([^<]+)<"); // match "</tag>message<"
            var matches = r.Matches(tagText, 0);
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
                        logs.Add(between); // Todo time
                    }
                }
            }
            return logs;
        }


        public static bool ParseState(string tagText, out AOUStateData stateData)
        {
            /* 
            <state><Time>19</Time><temp><Heat>34</Heat><Hot>31</Hot><Ret>27</Ret><BuHot>30</BuHot><BuMid>29</BuMid><BuCold>27</BuCold><Cool>32</Cool><Cold>30</Cold><BearHot>0</BearHot>
            <ch9>0</ch9><ch10>0</ch10><ch11>0</ch11><ch12>0</ch12><ch13>0</ch13><ch14>0</ch14><ch15>0</ch15><avg>28</avg></temp></stateData> // Arduino data

            ASCII format from AOU. // MASK_STATE = 2 hex digits MASK (e.g. “3F”), and 2 hex digits STATE (e.g. “12”). Optional tags except Time
 
            <state><Time>104898416</Time>  // Number of 1/10 second ticks since RESET (32bits unsigned). Not Optional
            <temp>  <Heat>120</Heat><Hot>122</Hot><Ret>68</Ret><BuHot>56</BuHot><BuMid>56</BuMid><BuCold>56</BuCold><Cool>40</Cool><Cold>56</Cold><BearHot>40</BearHot> </temp> // // 16bits signed
               <Pow>127</Pow>           // 8bits unsigned
               <Valves>MMSS</Valves>    // MASK_STATE, Bits: 0/Hot valve, 1/Cold valve, 2/Return valve
               <Energy>MMSS</Energy>    // MASK_STATE, 

               <UI>MMSS</UI>            // MASK_STATE, BUTTON_ONOFF = 0x0001 (Soft on/Off); BUTTON_EMERGENCYOFF = 0x0002 (Hard Off); BUTTON_MANUALOPHEAT = 0x0004 (Forced Heating);
                                        // BUTTON_MANUALOPCOOL = 0x0008 (Forced Cooling); BUTTON_CYCLE = 0x0010 (Forced Cycling); BUTTON_RUN = 0x0020 (Run with IMM)

               <IMM>MMSS</IMM>          // MASK_STATE, IMM_OutIMMError = 0x01; IMM_OutIMMBlockInject = 0x02; IMM_OutIMMBlockOpen = 0x04; IMM_InIMMStop = 0x08
                                        // IMM_InCycleAuto = 0x10; IMM_InIMMInjecting = 0x20; IMM_InIMMEjecting = 0x40; IMM_InIMMToolClosed = 0x80

               <Mode>MMSS</Mode>        // MASK_STATE, <Mode>1</Mode>(int); HT_STATE_INVALID = -999; HT_STATE_COLD = -1; HT_STATE_UNKNOWN = 0; HT_STATE_HOT = 1
               <Seq>117</Seq>           // 
            </state>
 
            <state><Time>4711</Time>
               <Valves>0101</Valves>      // Example Hot feed valve “on” (i.e. feeds hot tempering fluid)
            </state>
            <state><Time>4721</Time>  // One second (or 10 x 1/10 second) later
               <Valves>0100</Valves>       // Hot feed valve “off” (i.e. stopped feeding hot tempering fluid)
            </state>
*/

            stateData.time_sek_x_10_of_hour = 0;
            stateData.time_hours = 0;

            stateData.coldTankTemp = AOUDataTypes.UInt16_NaN;
            stateData.hotTankTemp = AOUDataTypes.UInt16_NaN;
            stateData.retTemp = AOUDataTypes.UInt16_NaN;

            stateData.coolerTemp = AOUDataTypes.UInt16_NaN;
            stateData.heaterTemp = AOUDataTypes.UInt16_NaN;

            stateData.bufColdTemp = AOUDataTypes.UInt16_NaN;
            stateData.bufMidTemp = AOUDataTypes.UInt16_NaN;
            stateData.bufHotTemp = AOUDataTypes.UInt16_NaN;

            stateData.seqState = AOUDataTypes.UInt16_NaN;

            stateData.BearHot = AOUDataTypes.UInt16_NaN;
            stateData.Power = AOUDataTypes.UInt16_NaN;
            stateData.Energy = AOUDataTypes.UInt16_NaN;

            stateData.IMM = AOUDataTypes.UInt16_NaN;
            stateData.Valves = AOUDataTypes.UInt16_NaN;
            stateData.Mode = Int16.MaxValue;
            stateData.UIButtons = AOUDataTypes.UInt16_NaN;

            ParseWordTime_sek_x_10(tagText, out stateData.time_hours, out stateData.time_sek_x_10_of_hour);

            ParseWord(tagTempSubTagHot, tagText, out stateData.hotTankTemp);
            ParseWord(tagTempSubTagCold, tagText, out stateData.coldTankTemp);
            ParseWord(tagTempSubTagRet, tagText, out stateData.retTemp);
            ParseWord(tagTempBuCold, tagText, out stateData.bufColdTemp);
            ParseWord(tagTempBuMid, tagText, out stateData.bufMidTemp);
            ParseWord(tagTempBuHot, tagText, out stateData.bufHotTemp);

            ParseWord(tagTempSubTagCool, tagText, out stateData.coolerTemp);
            ParseWord(tagTempSubTagHeat, tagText, out stateData.heaterTemp);

            ParseWord(tagTempSubTagBearHot, tagText, out stateData.BearHot);

            
            ParseWord(tagSeqState, tagText, out stateData.seqState);

            ParseWord(tagPower, tagText, out stateData.Power); 

            byte mask; byte state; long temp;

            if (ParseMMSS(tagValves, tagText, out mask, out state))
            {
                stateData.Valves = state;
            }

            if (ParseMMSS(tagEnergy, tagText, out mask, out state))
            {
                stateData.Energy = state;
            }

            if (ParseMMSS(tagUI, tagText, out mask, out state))
            {
                int hiAndLow = ((int)mask << 8) | state;
                stateData.UIButtons = (UInt16)hiAndLow; 
            }

            if (ParseMMSS(tagIMM, tagText, out mask, out state))
            {
                stateData.IMM = state;
            }

            if (ParseMMSS(tagMode, tagText, out mask, out state))
            {
                stateData.Mode = state;
            }
            else if (ParseLong(tagMode, tagText, out temp))
            {
                stateData.Mode = (Int16)temp;
            }


            return true;
        }

        public static bool ParseLog(string tagText, out long time_ms, out string logMsg)
        {
            time_ms = 0;
            logMsg = "-";
            // textLine = "<log><Time>94962045</Time><Msg>Setup AOU version 1.1 ready (Plastics Unbound Ltd, Cyprus)</Msg></log>";
            return ParseLongTime(tagText, out time_ms) && ParseString(tagLogSubTagMsg, tagText, out logMsg);
        }

        /*
        Create XML Strings
        */
        public static string CreateTimeXmlString(UInt16 time_hours, UInt16 time_sek_x_10_of_hour)
        {
            return String.Format("<Time>{0}</Time>", time_hours * 36000 + time_sek_x_10_of_hour);
        }

        public static string CreateTimeXmlString(uint time)
        {
            return String.Format("<Time>{0}</Time>", time);
        }

        public static string CreateStateXmlString(uint time, string content)
        {
            return "<state>" + CreateTimeXmlString(time) + content + "</state>";
        }

        public static string CreateValvesXmlString(uint time, uint hotValve, uint coldValve, uint retValve)
        {
            string valvesStr = "7F";
            int valves = 0;
            if (hotValve != 0) valves += 1;
            if (coldValve != 0) valves += 2;
            if (retValve != 0) valves += 4;
            valvesStr += String.Format("{0:X02}", valves);
            return CreateStateXmlString(time, String.Format("<Valves>{0}</Valves>", valvesStr));
        }

        public static string CreateUIXmlString(uint time, string ui)
        {
            // MASK_STATE, BUTTON_ONOFF = 0x0001 (Soft on/Off); BUTTON_EMERGENCYOFF = 0x0002 (Hard Off); BUTTON_MANUALOPHEAT = 0x0004 (Forced Heating);
            // BUTTON_MANUALOPCOOL = 0x0008 (Forced Cooling); BUTTON_CYCLE = 0x0010 (Forced Cycling); BUTTON_RUN = 0x0020 (Run with IMM)

            return CreateStateXmlString(time, String.Format("<UI>{0}</UI>", ui));
        }

        public static string CreateModeXmlString(uint time, int mode)
        {
            return CreateStateXmlString(time, String.Format("<Mode>{0}</Mode>", mode));
        }

        public static string CreatePowXmlString(uint time, uint pow)
        {
            return CreateStateXmlString(time, String.Format("<Pow>{0}</Pow>", pow));
        }

        public static string CreateSeqXmlString(uint time, uint seq)
        {
            return CreateStateXmlString(time, String.Format("<Seq>{0}</Seq>", seq));
        }

        public static string CreateLogXmlString(uint time, string msg)
        {
            return String.Format("<{0}><{1}>{3}</{1}><{2}>{4}</{2}></{0}>",
                                    tagLog, tagSubTagTime, tagLogSubTagMsg, // 0 - 2
                                    time, msg); // 3 - 4
        }

        public static string CreateStateTempXmlString(int Heat, int Hot, int Ret, int BuHot, int BuMid, int BuCold, int Cool, int Cold, int BearHot)
        {
            string content = String.Format(
                "<Heat>{0}</Heat><Hot>{1}</Hot><Ret>{2}</Ret><BuHot>{3}</BuHot><BuMid>{4}</BuMid><BuCold>{5}</BuCold><Cool>{6}</Cool><Cold>{7}</Cold><BearHot>{8}</BearHot>",
                       Heat, Hot, Ret, BuHot, BuMid, BuCold, Cool, Cold, BearHot);
            return "<temp>" + content + "</temp>";
        }

        public static string CreateStateXmlString(AOUStateData data)
        {
            string temp = CreateStateTempXmlString(data.heaterTemp, data.hotTankTemp, data.retTemp, data.bufHotTemp, data.bufMidTemp, data.bufColdTemp, data.coolerTemp, data.coldTankTemp, data.BearHot);
            return "<state>" + CreateTimeXmlString(data.time_hours, data.time_sek_x_10_of_hour) + temp + "</state>";
        }
    }
}
