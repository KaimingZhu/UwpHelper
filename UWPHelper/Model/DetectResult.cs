using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UWPHelper.Model
{
    public class DetectResult
    {
        public int BeginRow;
        public int EndRow;
        public string CloneFileName;
        public float RNR;

        public DetectResult(int BeginRow,int EndRow,string CloneFileName,float RNR)
        {
            this.BeginRow = BeginRow;
            this.EndRow = EndRow;
            this.CloneFileName = CloneFileName;
            this.RNR = RNR;
        }
    }
}
