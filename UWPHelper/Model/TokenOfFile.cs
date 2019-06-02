using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UWPHelper.Model
{
    public class TokenOfFile
    {
        public int BeginRow;//开始行
        public int BeginCol;//开始列
        public int BeginIndex;//开始位置
        public int EndRow;//结束行
        public int EndCol;//结束列
        public int EndIndex;//结束位置
        public string TokenPrep;//字符串

        public TokenOfFile(int BeginRow,int BeginCol,int BeginIndex,int EndRow,int EndCol,int EndIndex,string TokenPrep)
        {
            this.BeginCol = BeginCol;
            this.BeginRow = BeginRow;
            this.BeginIndex = BeginIndex;
            this.EndCol = EndCol;
            this.EndRow = EndRow;
            this.EndIndex = EndIndex;
            this.TokenPrep = TokenPrep;
        }
    }
}
