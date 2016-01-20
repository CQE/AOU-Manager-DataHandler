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
        // Different Run types. File and Random are test modes
        public enum RunType {Serial, File, Random};

        public RunType runMode {
            get; set;
        }

        // Object to read and write to text files. Default in User Image folder
        private TextFile dataFile;

        List<AOULogMessage> logMessages;
        List<Power> powerValues;

        int lastLogMessageCount;
        int lastPowerValuesCount;

        public AOURouter()
        {
            dataFile = new TextFile();
            logMessages = new List<AOULogMessage>();
            powerValues = new List<Power>();
            lastLogMessageCount = 0; // Ready to get from start
            lastPowerValuesCount = 0;
            runMode = RunType.Serial; // Normal

            // System.Diagnostics.
        }

        public AOURouter(RunType mode) : this()
        {
            runMode = mode;
            if (runMode==RunType.Random)
            {
                InitRandom();
            }
        }

        public void InitRandom()
        {
            lastLogMessageCount = 0; // Ready to get
            for (int i = 0; i < 50; i++)
            {
                logMessages.Add(ValueGenerator.GetRandomLogMsg());
            }

            for (int i = 0; i < 50; i++)
            {
                powerValues.Add(ValueGenerator.GetRandomPower());
            }

        }

        public void LoadTestFile()
        {

        }

        public void SaveValuesToFile(Power[] powers)
        {
            string dateStr = DateTime.Today.ToString("yyMMdd");

            foreach (var pwr in powers) {
                string ts = pwr.ElapsedTime.ToString() + ',';
                dataFile.AddToFile("AOU\\TBufferCold\\", "TBufferCold-" + dateStr + ".txt", ts + pwr.TBufferCold);
                dataFile.AddToFile("AOU\\TBufferMid\\", "TBufferMid-" + dateStr + ".txt", ts + pwr.TBufferMid);
                dataFile.AddToFile("AOU\\TBufferHot\\", "TBufferHot-" + dateStr + ".txt", ts + pwr.TBufferHot);
                dataFile.AddToFile("AOU\\TCoolTank\\", "TCoolTank-" + dateStr + ".txt", ts + pwr.TCoolTank);
                dataFile.AddToFile("AOU\\THotTank\\", "THotTank-" + dateStr + ".txt", ts + pwr.THotTank);
                dataFile.AddToFile("AOU\\State\\", "State-" + dateStr + ".txt", ts + pwr.State);
                dataFile.AddToFile("AOU\\TReturnActual\\", "TReturnActual-" + dateStr + ".txt", ts + pwr.TReturnActual);
                dataFile.AddToFile("AOU\\TReturnForecasted\\", "TReturnForecasted-" + dateStr + ".txt", ts + pwr.TReturnForecasted);
                dataFile.AddToFile("AOU\\TReturnValve\\", "TReturnValve-" + dateStr + ".txt", ts + pwr.TReturnValve);
            }
        }

        public void Update()
        {
            if (runMode == RunType.Random)
            {
                if (ValueGenerator.GetRandomOk(50))
                {
                    logMessages.Add(ValueGenerator.GetRandomLogMsg());
                }

                powerValues.Add(ValueGenerator.GetRandomPower());
 
            }
            else if (runMode == RunType.File)
            {
                // ToDo: From  file
            }
            else if (runMode == RunType.Serial)
            {
                // ToDo: From Serial Port
            }
            // SaveValuesToFile(new Power[] { pwr });
        }

        /************************
            Value Handling
        ************************/
        public Power[] GetLastPowerValues(int count)
        {
            lastPowerValuesCount = powerValues.Count;
            return powerValues.GetRange(lastPowerValuesCount - count, count).ToArray();
        }

        public bool NewPowerDataIsAvailable()
        {
            return (powerValues.Count  > lastPowerValuesCount);
        }

        public Power GetLastPowerValue()
        {
            lastPowerValuesCount = powerValues.Count;
            return powerValues[lastPowerValuesCount-1];
        }

        public int[] GetPowerValuesFastData()
        {
            return new int[] { 1, 2, 3, 4, 5, 6 };
        }

        /**************************
            Log Message Handling
        **************************/
        public AOULogMessage[] GetLastLogMessages(int count)
        {
            lastLogMessageCount = logMessages.Count;
            return logMessages.GetRange(lastLogMessageCount - count, count).ToArray();
        }

        public bool NewLogMessagesAreAvailable()
        {
            return (logMessages.Count > lastLogMessageCount);
        }

        public AOULogMessage[] GetNewLogMessages()
        {
            int lmc = lastLogMessageCount;
            lastLogMessageCount = logMessages.Count;
            return logMessages.GetRange(lmc, logMessages.Count - lmc).ToArray();
        }
    }
}
