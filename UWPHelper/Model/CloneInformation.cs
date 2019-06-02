using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UWPHelper.Model
{
    public class CloneInformation : IComparable
    {
        public int CloneID;
        public int CloneFileID;
        public string FileName { get; set; }
        public int BeginToken;
        public int EndToken;
        public float RNR { get; set; }
        public bool ifCompare;

        public CloneInformation(int CloneID, int CloneFileID, int BeginToken, int EndToken)
        {
            this.CloneFileID = CloneFileID;
            this.CloneID = CloneID;
            this.BeginToken = BeginToken;
            this.EndToken = EndToken;
            ifCompare = false;
        }

        public int CompareTo(Object obj)
        {
            CloneInformation cobj = (CloneInformation)obj;
            return BeginToken.CompareTo(cobj.BeginToken);
        }
    }
}
