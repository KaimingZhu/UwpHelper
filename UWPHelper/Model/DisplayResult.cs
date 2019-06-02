using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UWPHelper.Model
{
    public class DisplayResult
    {
        public string data;
        public string filename;
        public int colorIndex;
        private static string[] color = { "yellowgreen", "indianred", "goldenrod"};
        
        public DisplayResult(string data,string filename,int colorIndex)
        {
            this.data = data;
            this.filename = filename;
            this.colorIndex = colorIndex;
        }

        public string getColor()
        {
            return color[colorIndex];
        }
    }

}
