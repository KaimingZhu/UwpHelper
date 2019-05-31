using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UWPHelper.Services.Interface;

namespace UWPHelper.Services.Implement
{
    public class DetectManager : IDetectManager
    {

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
        }

        void IDetectManager.Analyse(string FileUrl)
        {
            throw new NotImplementedException();
        }

        void IDetectManager.CreateFileList(string url)
        {
            throw new NotImplementedException();
        }

        void IDetectManager.DetectBetweenFileList(string FileList1, string FileList2)
        {
            throw new NotImplementedException();
        }

        void IDetectManager.DetectWithFileList(string FileList1, string FileList2)
        {
            throw new NotImplementedException();
        }

        void IDetectManager.ReadData(string FileUrl)
        {
            throw new NotImplementedException();
        }

        Task IDetectManager.Run()
        {
            throw new NotImplementedException();
        }

        ~DetectManager()
        {
            //如果正在运行
            process.Close();
        }
    }
}
