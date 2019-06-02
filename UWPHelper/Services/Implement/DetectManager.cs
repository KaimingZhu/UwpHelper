using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UWPHelper.Services.Interface;
using UWPHelper.Model;
using UWPHelper.Models;

namespace UWPHelper.Services.Implement
{
    public class DetectManager : IDetectManager
    {
        private List<DetectResult> detectResults;
        private List<CloneInformation> cloneInformation;
        private List<TokenOfFile> tokenList;
        private Dictionary<int,string> FileMap;
        private static string SourceFileURL = "C:\\Users\\Administrator\\Source\\Repos\\UwpHelper\\UWPHelper\\SourceCodeData\\";
        private static string DetectDataURL = "C:\\Users\\Administrator\\Source\\Repos\\UwpHelper\\UWPHelper\\";

        private string cmdString;

        private Process process;

        public DetectManager()
        {
            process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.UseShellExecute = false;//是否使用操作系统shell启动
            process.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
            process.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
            process.StartInfo.RedirectStandardError = true;//重定向标准错误输出
            process.StartInfo.CreateNoWindow = true;//不显示程序窗口

            cmdString = "";
            tokenList = new List<TokenOfFile>();
            FileMap = new Dictionary<int, string>();
            cloneInformation = new List<CloneInformation>();
            detectResults = new List<DetectResult>();
        }

        //读取某一子字符串中小端模式存储的int数据 ： 从index = position开始，返回此时的index值（指向下一元素）
        private int ReadLittleEndianInVector_ForInt(string tempString, int position)
        {
            int i;
            int result;
            byte[] bytes = new byte[4];

            for (i = 0; i < 4; i++)
            {
                bytes[i] = (byte)(tempString[position++]);
            }
            result = BitConverter.ToInt32(bytes);

            return result;
        }

        //一直读取直到读到'\n'为止，返回此时的index值（指向下一元素）
        private int OmitFortheWrap(string FileString,int position)
        {
            char temp;
            while ((temp = FileString[position++]) != '\n')
            {
                ;
            }
            return position;
        }

        //一直读取直到读到两个'\n'为止，返回此时的index值（指向下一元素）
        private int OmitFortheSubsequentTwoWrap(string FileString, int position)
        {
            int length = 0;
            char temp;
            bool flag = false;

            while (true)
            {
                length++;
                temp = FileString[position++];
                if (flag == true)
                {
                    if (temp == '\n')
                    {
                        break;
                    }
                    else
                    {
                        flag = false;
                    }
                }
                if (temp == '\n')
                {
                    flag = true;
                }
            }

            return position;
        }

        //读取文件中的小端模式存储的int数据，从index = position开始，，返回此时的index值（指向下一元素）
        private int ReadLittleEndianInFile_ForInt(string FileString,ref int position)
        {
            int i;
            int result;
            byte[] bytes = new byte[4];
            for (i = 0; i <= 3; i++)
            {
                bytes[i] = (byte) (FileString[position++]);
            }
            result = BitConverter.ToInt32(bytes);

            return result;
        }

        //读取指定长度的字符串,长度为length，返回读取的字符串
        private string ReadAssginedLengthData(string FileString,ref int position, int length)
        {
            string ResultString = "";
            for(int i = 0;i < length; i++)
            {
                ResultString += FileString[position++];
            }

            return ResultString;
        }

        //读取指定长度的字符串，直到遇到EndChar，返回读取的字符串
        private string ReadStringUntil(string FileString, ref int position, char EndChar)
        {
            char temp;
            string result = "";
            while ((temp = FileString[position++]) != EndChar)
            {
                result += temp;
                if(position == FileString.Length)
                {
                    break;
                }
            }

            return result;
        }

        /** 计算字符对应的Hex值 **/
        private int valueOfHexChar(char b)
        {
            if ((b >= '0') && (b <= '9'))
            {
                return b - '0';
            }
            else if ((b >= 'a') && (b <= 'f'))
            {
                return b - 'a' + 0xa;
            }
            else if ((b >= 'A') && (b <= 'F'))
            {
                return b - 'A' + 0xa;
            }
            else
            {
                throw new Exception("prepFile ReadError");
            }
        }

        /** 计算字符串对应的Hex值 **/
        private int valueOfHexString(string CalString)
        {
            int i;
            int value = 0;
            for (i = 0; i < CalString.Length; ++i)
            {
                value <<= 4;
                value += valueOfHexChar(CalString[i]);
            }
            return value;
        }

        /** 创建比对文件的FileList **/
        private string CreateFileList(string FolderUrl)
        {
            FileInfo fileInfo = new FileInfo(DetectDataURL + FolderUrl + "List1.txt");
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }
            FileStream fp = new FileStream(DetectDataURL + FolderUrl + "List1.txt", FileMode.CreateNew);
            fp.Close();
            string url = DetectDataURL + FolderUrl + "1.cs";
            StreamWriter fw = new StreamWriter(DetectDataURL + FolderUrl + "List1.txt");
            fw.Write(url);
            fw.Close();

            return DetectDataURL + FolderUrl;
        }

        /** 将文件中的FileList转换为List<string>形式 **/
        private static List<string> FileListpreHandler(string rawdata)
        {
            bool ifOmit = false;

            rawdata.ToArray();

            List<string> fileList = new List<string>();

            string temp = "";

            //数据转换为数组
            //此时:FileList中文件夹间隔符均由\形式表示
            foreach (char ch in rawdata)
            {
                if(ifOmit == true)
                {
                    ifOmit = false;
                    continue;
                }
                if (ch == '\r')
                {
                    if(temp.Length != 0)
                    {
                        fileList.Add(temp);
                        ifOmit = true;
                    }
                    temp = "";
                }
                else
                {
                    temp += ch;
                }
            }
            //最后一行不以'\n'保存
            if(temp.Length != 0)
            {
                fileList.Add(temp);
            }

            return fileList;
        }

        /** 仅样例代码使用 : 传入新的List<string>作为新的FileList数据，保存到对应位置 **/
        private static bool updateSourceCodeFileList(List<string> fileList)
        {
            //如果已经全部删除，则删除文件即可
            FileInfo fp_search = new FileInfo(SourceFileURL + "List2.txt");
            if (fp_search.Exists)
            {
                fp_search.Delete();
            }
            if (fileList.Count > 0)
            {
                //否则，将其全部重新写入
                string rewrite = "";
                for (int i = 0; i < fileList.Count; i++)
                {
                    rewrite += fileList[i] + "\r\n";
                }
                FileStream fp = new FileStream(SourceFileURL + "List2.txt", FileMode.CreateNew);
                fp.Close();
                //开始写入
                StreamWriter fw = new StreamWriter(SourceFileURL + "\\List2.txt");
                fw.Write(rewrite);
                fw.Close();
            }
            else
            {
                fp_search.Delete();
            }

            return true;
        }

        /** 将筛选出的数据封装为DetectResult类型 **/
        private void GetDetectResult()
        {
            detectResults.Clear();
            foreach(var item in cloneInformation)
            {
                if(item.BeginToken > item.EndToken)
                {
                    //处理可能出现的脏数据
                    continue;
                }
                else
                {
                    //寻找token对应的信息
                    var temp = new DetectResult(tokenList[item.BeginToken].BeginRow, tokenList[item.EndToken].EndRow, item.FileName, item.RNR);
                    detectResults.Add(temp);
                }
            }
        }

        /** 读取cloneMetrics.txt，并对cloneInformation中的数据进行RNR赋值 **/
        private async Task ReadCloneInformation(string FolderUrl)
        {
            //step 1 : 调用cmd，执行cloneMetrics计算操作
            cmdString = "ccfx m " + DetectDataURL + FolderUrl + "a.ccfxd -c -o " + DetectDataURL + FolderUrl + "cloneMetrics.txt";

            process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.UseShellExecute = false;//是否使用操作系统shell启动
            process.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
            process.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
            process.StartInfo.RedirectStandardError = true;//重定向标准错误输出
            process.StartInfo.CreateNoWindow = true;//不显示程序窗口
            process.Start();

            await process.StandardInput.WriteLineAsync(cmdString);
            await process.StandardInput.WriteLineAsync("exit");
            process.WaitForExit();
            process.Close();

            //step 2 : 读取对应文件
            StreamReader sr = new StreamReader(DetectDataURL + FolderUrl + "cloneMetrics.txt");
            var fileString = sr.ReadToEnd();

            //step 3 :根据格式读取
            string lineData = "";
            string tempData = "";
            int j,k;
            int CloneID;
            float RNR;
            for(int i = 0;i < fileString.Length;)
            {
                if(i == 0)
                {
                    lineData = ReadStringUntil(fileString, ref i, '\n');
                    continue;
                }
                else
                {
                    lineData = ReadStringUntil(fileString, ref i, '\n');
                    //读取第一个数据:CloneID，在第一个\t之前
                    j = 0;
                    tempData = ReadStringUntil(lineData, ref j, '\t');
                    CloneID = int.Parse(tempData);
                    for(k = 0;k < 4; k++)
                    {
                        tempData = ReadStringUntil(lineData, ref j, '\t');
                    }
                    //读取第二个数据：RNR，在第六个\t之前
                    tempData = ReadStringUntil(lineData, ref j, '\t');
                    RNR = float.Parse(tempData);

                    //根据CloneID对cloneInformation中的数据赋值
                    for(int kr = 0;kr < cloneInformation.Count; kr++)
                    {
                        if(cloneInformation[kr].CloneID == CloneID)
                        {
                            cloneInformation[kr].RNR = RNR;
                            break;
                        }
                    }
                }
            }

            //关闭文件，完成赋值
            sr.Close();
        }

        /** 比对SourceCode数组，并对cloneInformation中的数据进行fileName赋值 **/
        private void AddFileName(List<SourceCodeForDisPlay> sourceCodeList)
        {
            int i;
            foreach(var item in cloneInformation)
            {
                var tempString = FileMap[item.CloneFileID];
                for(i = tempString.Length - 1;i >= 0; i--)
                {
                    if(tempString[i] == '\\')
                    {
                        break;
                    }
                }
                string filename = "";
                for(int j = i + 1;j < tempString.Length; j++)
                {
                    filename += tempString[j];
                }
                foreach(var codeitem in sourceCodeList)
                {
                    if (codeitem.GetFileName().Equals(filename))
                    {
                        item.FileName = codeitem.Name;
                        break;
                    }
                }
            }
        }

        //读取a.ccfxd，并保存至cloneInformation数组中
        private bool ReadAnalyseData(string FolderUrl)
        {
            int i = 0;
            int position = 0;
            int temp;
            string tempString = "";
            cloneInformation.Clear();
            StreamReader sr = new StreamReader(DetectDataURL + FolderUrl + "a.ccfxd");
            var fileString = sr.ReadToEnd();

            if (fileString.Length == 0)
            {
                return false;
            }
            else
            {

                /**以 '大端模式' 读入8个字节，应该是ccfxraw0**/
                /**结果 : Success!**/
                position = 8;

                /**以 '小端模式' 读入三个int，对应Version1,2,3 然后以大端模式读入四个字符，会读到pa:d，代表格式**/
                /**结果 : Success! **/
                i = 0;
                while (i < 3)
                {
                    var j = ReadLittleEndianInFile_ForInt(fileString, ref position);
                    i++;
                }
                position += 4;

                /** 接下来是操作命令，以两个\n(ASCII码为10)作为结束标志 **/
                /** 结果 : Success! **/
                position = OmitFortheSubsequentTwoWrap(fileString, position);

                /** 接下来是预处理脚本信息，以\n为结束标志**/
                /** 结果 : Success! **/
                position = OmitFortheWrap(fileString, position);

                /** 接下来是文件信息，以两个连续的\n(或在上一个流程后接一个\n)和两个小端模式0作为结束标记 **/
                /** 结果 : Success! **/

                while (true)
                {
                    tempString = "";
                    tempString = ReadStringUntil(fileString, ref position, '\n');
                    if (tempString.Length == 0)
                    {
                        ReadLittleEndianInFile_ForInt(fileString, ref position);
                        ReadLittleEndianInFile_ForInt(fileString, ref position);
                        break;
                    }
                    string FileURL = tempString;
                    var FileId = ReadLittleEndianInFile_ForInt(fileString, ref position);
                    //Read the Length
                    ReadLittleEndianInFile_ForInt(fileString, ref position);

                    FileMap[FileId] = FileURL;
                }

                /** 接下来是文件标记，同样的，以两个连续的\n(或在上一个流程后接一个\n)和一个小端模式0作为结束标记 **/
                /** 结果 : Success! **/
                while (true)
                {
                    tempString = "";
                    tempString = ReadStringUntil(fileString, ref position, '\n');
                    if (tempString.Length == 0)
                    {
                        ReadLittleEndianInFile_ForInt(fileString, ref position);
                        break;
                    }
                    ReadLittleEndianInFile_ForInt(fileString, ref position);
                }

                /** 接下来是克隆数据 **/
                /** 结构(32字节) : 左文件ID(0,小端,int) position(左文件的下一字节,左文件开始位置) 右文件ID(12,小端模式,int) 开始位置(4,小端,int) 结束位置(8,小端,int) CloneSetID(24,小端,int)**/
                while (true)
                {
                    tempString = ReadAssginedLengthData(fileString, ref position, 32);
                    temp = ReadLittleEndianInVector_ForInt(tempString, 0);

                    if (temp == 0)
                    {
                        break;
                    }
                    else if (FileMap[temp].Equals(DetectDataURL + FolderUrl + "1.cs"))
                    {
                        int endToken = ReadLittleEndianInVector_ForInt(tempString, 8);
                        if(endToken >= tokenList.Count)
                        {
                            endToken = tokenList.Count - 1;
                        }
                        CloneInformation information = new CloneInformation(ReadLittleEndianInVector_ForInt(tempString, 24), ReadLittleEndianInVector_ForInt(tempString, 12), ReadLittleEndianInVector_ForInt(tempString, 4), endToken);
                        cloneInformation.Add(information);
                    }
                }
            }
            sr.Close();
            return true;
        }

        //读取1.cs.csharp.2_0_0_0.default.ccfxprep，并将结果保存到tokenList中
        private bool GetTokenOfFile(string FolderUrl)
        {

            //目标：读取预处理文件，分析token结构，保存进入tokenList;
            int offset = 0;
            bool ifSameRow;
            char ch;
            string tempString;
            List<string> tokenReader = new List<string>();

            //读取文件
            try
            {
                StreamReader sr = new StreamReader(DetectDataURL + FolderUrl + "1.cs.csharp.2_0_0_0.default.ccfxprep");
                var prepfile = sr.ReadToEnd().ToArray();
                sr.Close();
                tokenList.Clear();

                for (int i = 0; i < prepfile.Length;)
                {
                    tokenReader.Clear();
                    tempString = "";

                    //读取BeginRow : 第一次读到.
                    while (i < prepfile.Length)
                    {
                        ch = prepfile[i++];
                        if (ch == '.')
                        {
                            break;
                        }
                        tempString += ch;
                    }
                    tokenReader.Add(tempString);
                    tempString = "";

                    /** 读取BeginCol : 第二次读到. **/
                    while (i < prepfile.Length)
                    {
                        ch = prepfile[i++];
                        if (ch == '.')
                        {
                            break;
                        }
                        tempString += ch;
                    }
                    tokenReader.Add(tempString);
                    tempString = "";

                    /** 读取BeginIndex : 读到\t **/
                    while (i < prepfile.Length)
                    {
                        ch = prepfile[i++];
                        if (ch == '\t')
                        {
                            break;
                        }
                        tempString += ch;
                    }
                    tokenReader.Add(tempString);
                    tempString = "";

                    if (i < prepfile.Length)
                    {
                        ch = prepfile[i++];
                        /** 结束的部分在同一行 **/
                        if (ch == '+')
                        {

                            ifSameRow = true;

                            /** 计算偏移量 **/
                            tempString = "";
                            while (i < prepfile.Length)
                            {
                                ch = prepfile[i++];
                                if (ch == '\t')
                                {
                                    break;
                                }
                                tempString += ch;
                            }

                            offset = valueOfHexString(tempString);

                            /** 先把endRow,endCol和endIndex复制过去，用ifSameRow确定是否需要添加偏移量 **/
                            int jr = 0;
                            while (jr < 3)
                            {
                                tokenReader.Add(tokenReader[0]);
                                jr++;
                            }

                            tempString = "";
                        }
                        else
                        {

                            /** 不在同一行，用和上面一样的方法处理 **/

                            ifSameRow = false;

                            //此时读到的字符是我们需要写入的
                            tempString += ch;

                            /** 读取EndRow : 第一次读到.**/
                            while (i < prepfile.Length)
                            {
                                ch = prepfile[i++];
                                if (ch == '.')
                                {
                                    break;
                                }
                                tempString += ch;
                            }

                            tokenReader.Add(tempString);
                            tempString = "";

                            /** 读取EndCol : 第二次读到. **/
                            while (i < prepfile.Length)
                            {
                                ch = prepfile[i++];
                                if (ch == '.')
                                {
                                    break;
                                }
                                tempString += ch;
                            }
                            tokenReader.Add(tempString);
                            tempString = "";

                            /** 读取EndIndex : 读到\t **/
                            while (i < prepfile.Length)
                            {
                                ch = prepfile[i++];
                                if (ch == '\t')
                                {
                                    break;
                                }
                                tempString += ch;
                            }
                            tokenReader.Add(tempString);
                            tempString = "";
                        }

                        /** 最后一步 : 读取字符串 **/
                        while (i < prepfile.Length)
                        {
                            ch = prepfile[i++];
                            if (ch == '\n')
                            {
                                break;
                            }
                            tempString += ch;
                        }

                        //检测是否为'\r''\n'，若是，删除'\r'
                        var j = tempString.Length;
                        var tempCharArray = tempString.ToArray();
                        if (tempCharArray[--j] == '\r')
                        {
                            tempCharArray[j] = '\0';
                            tempString = tempCharArray.ToString();
                        }

                        tokenReader.Add(tempString);

                        /** 放入token数组 **/

                        //计算
                        var BeginRow = valueOfHexString(tokenReader[0]);
                        var BeginCol = valueOfHexString(tokenReader[1]);
                        var BeginIndex = valueOfHexString(tokenReader[2]);
                        var EndRow = valueOfHexString(tokenReader[3]);
                        var EndCol = valueOfHexString(tokenReader[4]);
                        var EndIndex = valueOfHexString(tokenReader[5]);

                        if (ifSameRow)
                        {
                            EndCol += offset;
                            EndIndex += offset;
                        }

                        //放入token数组
                        TokenOfFile temp = new TokenOfFile(BeginRow, BeginCol, BeginIndex, EndRow, EndCol, EndIndex, tokenReader[6]);
                        tokenList.Add(temp);
                    }
                    else
                    {
                        break;
                    }
                }
                tokenList.RemoveAt(tokenList.Count - 1);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /** 将cloneInformation中的数据进行最大RNR筛选 **/
        private void GetCorrectClone()
        {
            //使用三个栈进行筛选
            Stack<CloneInformation> resultStack = new Stack<CloneInformation>() ;//栈1:存放结果
            Stack<CloneInformation> calStack = new Stack<CloneInformation>();//栈2:存放未比较数据
            Stack<CloneInformation> tempStack = new Stack<CloneInformation>();//栈3:存放暂存数据

            //预处理
            resultStack.Clear();
            calStack.Clear();
            tempStack.Clear();

            //首先先对List中数据排序:按BeginToken从大到小
            cloneInformation.Sort((x,y) => -x.CompareTo(y));
            
            //全部压入栈2中
            foreach(var item in cloneInformation)
            {
                calStack.Push(item);
            }

            //开始计算
            while(calStack.Count > 0)
            {
                if(resultStack.Count == 0) {
                    //结果栈无元素或栈顶元素已比较完毕，从运算栈中取一个元素到结果栈
                    resultStack.Push(calStack.Pop());
                }
                else if (resultStack.Peek().ifCompare){
                    resultStack.Push(calStack.Pop());
                }
                else
                {
                    //正在比较：判断栈2和栈1之间的行关系
                    if(resultStack.Peek().EndToken <= calStack.Peek().BeginToken)
                    {
                        //无冲突 : 栈1栈顶元素完成匹配
                        resultStack.Peek().ifCompare = true;
                        //清空栈3
                        tempStack.Clear();
                    }
                    else
                    {
                        //存在冲突
                        //先判断是否是包含关系
                        if (resultStack.Peek().EndToken >= calStack.Peek().EndToken)
                        {
                            //如果是，计算二者RNR
                            if(resultStack.Peek().RNR >= calStack.Peek().RNR)
                            {
                                //如果结果栈中元素更接近，运算栈中元素备选
                                tempStack.Push(calStack.Pop());
                            }
                            else
                            {
                                //如果结果栈中元素相差大，说明有更优匹配，取消结果栈栈顶元素比较资格
                                resultStack.Pop();
                                //暂存栈元素全部回到运算栈中
                                while(tempStack.Count > 0)
                                {
                                    calStack.Push(tempStack.Pop());
                                }
                            }
                        }
                        else
                        {
                            //计算冲突率，当冲突率大于60%时，则考虑取消结果栈栈顶中元素
                            //冲突率计算：计算冲突token数，除以比较原冲突栈token数
                            float value = (float)(resultStack.Peek().EndToken - calStack.Peek().BeginToken) / (float)(resultStack.Peek().EndToken - resultStack.Peek().BeginToken);
                            //如果冲突率大于60% : 比较RNR : 败者退出或进入败者组
                            if (value >= 0.6)
                            {
                                if (resultStack.Peek().RNR >= calStack.Peek().RNR)
                                {
                                    //如果结果栈中元素更接近，运算栈中元素备选
                                    tempStack.Push(calStack.Pop());
                                }
                                else
                                {
                                    //如果结果栈中元素相差大，说明有更优匹配，取消结果栈栈顶元素比较资格
                                    resultStack.Pop();
                                    //暂存栈元素全部回到运算栈中
                                    while (tempStack.Count > 0)
                                    {
                                        calStack.Push(tempStack.Pop());
                                    }
                                }
                            }
                            else
                            {
                                //否则，根据RNR，选择谁范围更大，结束战斗
                                if (resultStack.Peek().RNR >= calStack.Peek().RNR)
                                {
                                    //结果栈胜出 : 修改运算栈数据
                                    calStack.Peek().BeginToken = resultStack.Peek().EndToken + 1;
                                }
                                else
                                {
                                    //运算栈胜出，修改结果栈数据
                                    resultStack.Peek().EndToken = resultStack.Peek().BeginToken - 1;
                                }
                                resultStack.Peek().ifCompare = true;
                                tempStack.Clear();
                            }
                        }
                    }
                }
            }
            //此时，栈1中元素为结果元素，退回到List中
            //为方便，先反向退栈，以此显示从小到大的顺序

            cloneInformation.Clear();
            while(resultStack.Count > 0)
            {
                tempStack.Push(resultStack.Pop());
            }
            while(tempStack.Count > 0)
            {
                cloneInformation.Add(tempStack.Pop());
            }
        }

        /**
         * 
         * 
         * ImpleMent
         * 
         * 
         **/

        /** 执行检测 : Success **/
        public async Task<bool> DetectBetweenFileListAsync(string FolderURL)
        {
            //传入对应的文件夹所在地址，以\\结尾

            cmdString = "";
            //step 1 : 检查是否已加入样例代码
            FileInfo fp_search = new FileInfo(SourceFileURL + "List2.txt");
            if (!fp_search.Exists)
            {
                return false;
            }
            //step 2 : 为其添加FileList : 命名格式为1.txt
            var list1Folder = CreateFileList(FolderURL);
            var list2 = SourceFileURL + "List2.txt";

            //开始检测
            cmdString = "ccfx d csharp -i " + list1Folder + "List1.txt -is -i " + list2 + " -w f-w-g+ -o " + list1Folder + "a.ccfxd";

            process.Start();
            await process.StandardInput.WriteLineAsync(cmdString);
            await process.StandardInput.WriteLineAsync("exit");

            process.WaitForExit();
            process.Close();

            return true;
        }

        /** 获得比对结果 **/
        public async Task<List<DetectResult>> AnalyseAsync(string FolderUrl, List<SourceCodeForDisPlay> sourceCodeList)
        {
            //目标：读取克隆数据以比对
            //step1 : ReadPrepFile
            bool result = GetTokenOfFile(FolderUrl);
            if(result == false)
            {
                return null;
            }
            //step 2 : ReadAnalyseData : a.ccfxd
            ReadAnalyseData(FolderUrl);
            //step 3 : AddFileName
            AddFileName(sourceCodeList);
            //step 4 : ReadCloneInformation to Get RNR
            await ReadCloneInformation(FolderUrl);
            //step 5 : Compare to Choose the right clone
            GetCorrectClone();
            //step 6 : Make the Result Data to DetectResult
            GetDetectResult();

            return detectResults;
        }

        /** 从样例代码FileList中更新对应数据**/
        public bool UpateItemFromInitList(string oldName,string newName)
        {
            //在SourceCode的FileList中更新某一项

            //先判断是否存在
            FileInfo fp_search = new FileInfo(SourceFileURL + "List2.txt");
            if (!fp_search.Exists)
            {
                return false;
            }

            //第一步：读取数据
            var sr = new StreamReader(SourceFileURL + "List2.txt");
            var rawdata = sr.ReadToEnd();
            sr.Close();

            //第二步：处理数据
            var fileList = FileListpreHandler(rawdata);

            //第三步：比较，删除对应项
            fileList.Remove(SourceFileURL + oldName);
            fileList.Add(SourceFileURL + newName);

            //第四步，重新写入
            updateSourceCodeFileList(fileList);

            return true;
        }

        /** 从样例代码FileList中删除对应数据 **/
        public bool DeleteItemFromInitList(string name)
        {
            //在SourceCode的FileList中删除某一项

            //先判断是否存在
            FileInfo fp_search = new FileInfo(SourceFileURL + "List2.txt");
            if (!fp_search.Exists)
            {
                return false;
            }

            //第一步：读取数据
            var sr = new StreamReader(SourceFileURL + "List2.txt");
            var rawdata = sr.ReadToEnd();
            sr.Close();

            //第二步：处理数据
            var fileList = FileListpreHandler(rawdata);

            //第三步：比较，删除对应项
            fileList.Remove(SourceFileURL + name);

            //第四步，重新写入
            updateSourceCodeFileList(fileList);

            return true;
        }

        /** 从样例代码FileList中添加对应数据 **/
        public bool AddItemtoInitList(string name)
        {
            //在SourceCode的FileList中添加某一项

            //先判断是否存在，若不存在，则创建
            FileInfo fp_search = new FileInfo(SourceFileURL + "List2.txt");
            if (!fp_search.Exists)
            {
                FileStream fp = new FileStream(SourceFileURL + "List2.txt", FileMode.CreateNew);
                fp.Close();
            }

            //第一步：读取数据
            var sr = new StreamReader(SourceFileURL + "List2.txt");
            var rawdata = sr.ReadToEnd();
            sr.Close();

            //第二步：处理数据
            var fileList = FileListpreHandler(rawdata);

            //第三步：比较，删除对应项
            //为了避免重复，此时可以先预删除重复数据
            fileList.Remove(SourceFileURL + name);
            fileList.Add(SourceFileURL + name);

            //第四步，重新写入
            updateSourceCodeFileList(fileList);

            return true;
        }

        /** 删除样例代码FileList **/
        public bool DeleteInitList()
        {
            FileInfo fp_search = new FileInfo(SourceFileURL + "List2.txt");
            if (fp_search.Exists)
            {
                fp_search.Delete();
            }

            return true;
        }

        ~DetectManager()
        {
            //如果正在运行
            process.Close();
        }
    }
}
