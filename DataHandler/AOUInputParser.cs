using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace DataHandler

{

    // Can this be more effective ??
    [StructLayout(LayoutKind.Sequential, Pack = 4)] // Pack = 0, 1, 2, 4, 8, 16, 32, 64, or 128:
    struct InputTempData
    {
        public byte tm;
        public byte cold;
        public byte hot;
    }

    public class AOUInputParser
    {
        public const string tagTemperature = "temperature";
        public const string tagFeeds = "feeds";
        public const string tagSequence = "seq";
        public const string tagValves = "valves";
        public const string tagLog = "log";

        public static string GetNextTag(string textLine)
        {
            Regex r = new Regex("<[a-zA-Z]+>");
            Match m = r.Match(textLine, 0);
            if (m.Success)
                return m.Groups[0].Value.Substring(1, m.Groups[0].Value.Length-2);
            else
                return "";
        }

        public static bool IsNextTag(string tag, string tagFound)
        {
            return tag == tagFound;
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
            if (FindTagAndExtractText(tagText, textline, out text, out endpos))
            {
                return true;
            }
            else
            {
                text = "";
                return false;
            }
        }

        public static bool Parsedouble(string tagText, string textline, out double value)
        {
            int endpos = 0;
            if (FindTagAndExtractText(tagText, textline, out tagText, out endpos) &&
                double.TryParse(tagText, out value))
            {
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
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

        public static bool ParseTime_ms(string textline, out TimeSpan time)
        {

            long time_ms = 0;
            if (ParseLong("Time", textline, out time_ms))
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

        public static bool ParseLongTime(string textline, out long time_ms) // Not to be misunderstood
        {

            if (ParseLong("Time", textline, out time_ms))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool ParseTemperature(string textLine, out long time_ms , out double hot_tank_temp, 
                                    out double cold_tank_temp, out double valve_return_temp, out double cool_temp, out int endpos)
        {
            // textLine = "<temperature><Time>104898416</Time><Hot>122</Hot><Cold>56</Cold><Ret>68</Ret><Cool>40</Cool></temperature>";

            string tempText = "";
            time_ms = 0;
            hot_tank_temp = 0.0;
            cold_tank_temp = 0.0;
            valve_return_temp = 0.0;
            cool_temp = 0.0;

            if (FindTagAndExtractText("temperature", textLine, out tempText, out endpos))
            {
                if (ParseLongTime(tempText, out time_ms) &&
                    Parsedouble("Hot", tempText, out hot_tank_temp) &&
                    Parsedouble("Cold", tempText, out cold_tank_temp) &&
                    Parsedouble("Ret", tempText, out valve_return_temp) &&
                    Parsedouble("Cool", tempText, out cool_temp)
                )
                {
                    return true;
                }
                else
                { 
                    return false;
                }

            }
            else
            {
                return false;
            }
        }

        public static bool ParseValves(string textLine, out long time_ms, out double valvesRetPrev, out double valvesRetNew, out int endpos)
        {
            textLine = "<valves><Time>104903816</Time><Ret><Prev>93</Prev><New>80</New></Ret></valves>";
            string tempText = "";
            time_ms = 0;
            valvesRetPrev = 0.0;
            valvesRetNew = 0.0;

            if (FindTagAndExtractText("valves", textLine, out tempText, out endpos))
            {
                if (ParseLongTime(tempText, out time_ms) &&
                    Parsedouble("Hot", tempText, out valvesRetPrev) &&
                    Parsedouble("Cold", tempText, out valvesRetNew))
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

        public static bool ParseFeeds(string textLine, out long time_ms, out double feedHotPrev, out double feedHotNew, out double feedColdPrev, out double feedcoldNew, out int endpos)
        {
            textLine = "<feeds><Time>104894473</Time><Hot><Prev>60</Prev></New>63,75</New></Hot></feeds>";
            textLine = "<feeds><Time>104878268</Time><Cold><Prev>65</Prev></New>60</New></Cold></feeds>";
            string tempText = "";
            time_ms = 0;
            feedHotPrev = 0;
            feedHotNew = 0;
            feedColdPrev = 0;
            feedcoldNew = 0;
            endpos = 0;
            return false;
        }

        public static bool ParseLog(string textLine, out long time_ms, out string logMsg, out int endpos)
        {
            string tempText;
            time_ms = 0;
            logMsg = "";
            textLine = "<log><Time>94962045</Time><Msg>Setup AOU version 1.1 ready (Plastics Unbound Ltd, Cyprus)</Msg></log>";
            if (FindTagAndExtractText("valves", textLine, out tempText, out endpos))
            {
                if (ParseLongTime(tempText, out time_ms) &&
                    ParseString("Hot", tempText, out logMsg))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static bool ParseSequence(string textLine, out long time_ms, out int state, out int endpos)
        {
            string tempText;
            time_ms = 0;
            state = 0;

            if (FindTagAndExtractText("seq", textLine, out tempText, out endpos))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
