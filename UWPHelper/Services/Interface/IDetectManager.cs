using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UWPHelper.Services.Interface
{
    public interface IDetectManager
    {
        void Analyse(string FileUrl);
        void ReadData(string FileUrl);
        void DetectBetweenFileList(string FileList1, string FileList2);
        void DetectWithFileList(string FileList1, string FileList2);
        void CreateFileList(string url);
        Task Run();
    }
}
