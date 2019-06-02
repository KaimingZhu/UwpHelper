using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UWPHelper.Model;
using UWPHelper.Models;

namespace UWPHelper.Services.Interface
{
    public interface IDetectManager
    {

        /** 此处传入的url，为双斜杠形式 **/

        //传入对应的文件夹所在地址，以\\结尾
        Task<List<DetectResult>> AnalyseAsync(string FolderUrl,List<SourceCodeForDisPlay> sourceCodeList);
        Task<bool> DetectBetweenFileListAsync(string FolderUrl);

        bool DeleteItemFromInitList(string name);
        bool AddItemtoInitList(string name);
        bool DeleteInitList();
        bool UpateItemFromInitList(string oldName, string newName);

    }
}
