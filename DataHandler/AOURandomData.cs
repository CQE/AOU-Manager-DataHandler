using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataHandler
{
    public class AOURandomData
    {
        public const int MaxRandomCount = 50;

        private string logstr;

        public string GetLogText()
        {
            string text = logstr;
            logstr = "";

            return text;
        }

        public AOURandomData(string settings = "")
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

        public string CreateRandomTempXmlString(long time)
        {
            var rndData = ValueGenerator.GetRandomTempData(time);
            return AOUInputParser.CreateTempXmlString(time, rndData);
        }

        public string CreateRandomSeqXmlString(long time, AOUTypes.StateType state)
        {
            var rndData = ValueGenerator.GetRandomSeqData(time, state);
            return AOUInputParser.CreateSeqXmlString(time, rndData);
        }

        public string CreateRandomIMMXmlString(long time, AOUTypes.IMMSettings settings)
        {
            var rndData = ValueGenerator.GetRandomIMMData(time, settings);
            return AOUInputParser.CreateIMMXmlString(time, rndData);
        }

        public string CreateRandomFeedsXmlString(long time, AOUTypes.FeedType feedType)
        {
            var rndData = ValueGenerator.GetRandomFeedData(time, feedType);
            return AOUInputParser.CreateFeedsXmlString(time, rndData);
        }

        public string CreateRandomLevelString(long time, AOUTypes.FeedType feedType)
        {
            var rndData = ValueGenerator.GetRandomLevelData(time, feedType);
            return AOUInputParser.CreateLevelsXmlString(time, rndData);
        }

        public string CreateRandomValvesXmlString(long time)
        {
            var rndData = ValueGenerator.GetRandomValvesData(time);
            return AOUInputParser.CreateValvesXmlString(time, rndData);
        }

        public string CreateRandomLogXmlString(long time)
        {
            return AOUInputParser.CreateLogXmlString(time, GetRandomLogMsg());
        }

        public static string CreateRandomXML(int num, long startTime, long msBetween)
        {
            AOURandomData rndData = new AOURandomData();
            string xml = "";
            long time = startTime + msBetween;
            AOUTypes.StateType state = AOUTypes.StateType.SQ_WAIT_HOT_AT_MOULD_ENTRY;
            AOUTypes.IMMSettings immSetting = AOUTypes.IMMSettings.CycleAuto;

            for (int i = 0; i < num; i++)
            {
                xml += rndData.CreateRandomTempXmlString(time) + "\r\n";

                if (state == AOUTypes.StateType.SQ_WAIT_HOT_AT_MOULD_ENTRY)
                    xml += rndData.CreateRandomFeedsXmlString(time, AOUTypes.FeedType.Hot) + "\r\n";
                else if (state == AOUTypes.StateType.SQ_WAIT_COLD_AT_MOULD_ENTRY)
                    xml += rndData.CreateRandomFeedsXmlString(time, AOUTypes.FeedType.Cold) + "\r\n";

                if (state == AOUTypes.StateType.SQ_WAIT_HOT_AT_MOULD_ENTRY)
                    xml += rndData.CreateRandomLevelString(time, AOUTypes.FeedType.Hot) + "\r\n";
                else if (state == AOUTypes.StateType.SQ_WAIT_COLD_AT_MOULD_ENTRY)
                    xml += rndData.CreateRandomLevelString(time, AOUTypes.FeedType.Cold) + "\r\n";

                xml += rndData.CreateRandomValvesXmlString(time) + "\r\n";

                time += msBetween;

                if ((i % 5) == 0)
                {
                    xml += rndData.CreateRandomSeqXmlString(time, state) + "\r\n";
                    if (state == AOUTypes.StateType.SQ_WAIT_FOR_OPEN_END)
                    {
                        state = AOUTypes.StateType.SQ_WAIT_HOT_AT_MOULD_ENTRY;
                    }
                    else
                    {
                        state++;
                    }
                }

                if ((i % 8) == 0)
                {
                    xml += rndData.CreateRandomLogXmlString(time) + "\r\n";
                }

                if ((i % 10) == 0)
                {
                    xml += rndData.CreateRandomIMMXmlString(time, immSetting) + "\r\n";
                    if (immSetting == AOUTypes.IMMSettings.IMMToolClosed)
                    {
                        immSetting = AOUTypes.IMMSettings.SetIMMBlockInject;
                    }
                    else
                    {
                        immSetting++;
                    }
                }
            }
            return xml;
        }
    }

}

