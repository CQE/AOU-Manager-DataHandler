using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataHandler
{
    class AOUFileData : AOUData
    {
        private TextFile dataFile;
        private AOUSettings.FileSetting setting;

        public AOUFileData(AOUSettings.FileSetting fileSetting) : base()
        {
            setting = fileSetting;
            dataFile = new TextFile();
            dataFile.OpenFileIfExistAndGetText(fileSetting.FilePath);
        }

        protected override string GetTextData()
        {
            return dataFile.GetTextData();
        }

        public override bool SendData(string data)
        {
            return true;
        }

        public override void UpdateData()
        {
            base.GetTextDataList();
        }

    }
}
