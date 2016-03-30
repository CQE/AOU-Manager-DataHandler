using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataHandler
{
    class AOURemoteData : AOUData
    {
        private AOUSettings.RemoteSetting setting;
        private string textToSend = "";
        private string receivedText = "";


        public AOURemoteData(AOUSettings.RemoteSetting remoteSetting) : base()
        {
            setting = remoteSetting;
        }

        public override bool SendData(string data)
        {
            textToSend += data;
            // Send();
            return true;
        }

        public override void UpdateData()
        {
            base.GetTextDataList();
        }

        protected override string GetTextData()
        {
            string text = receivedText;
            receivedText = "";
            return text;
        }


    }
}
