using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UWPHelper.Models;

namespace UWPHelper.Services.Interface
{
    public interface ISourceCodeManager
    {
        //Check For Same Name or Same EnglishName
        bool VaildationCheck(SourceCodeForDisPlay item);

        //GetSourceCode
        List<SourceCodeForDisPlay> GetSourceCodeForDisPlays();

        //AddSourceCode
        Task<bool> AddSourceCode(SourceCodeForDisPlay temp);

        //FindSourceCode
        SourceCodeForDisPlay FindSourceCode(int id);
        SourceCodeForDisPlay FindSourceCode(string FileName);
        SourceCodeForDisPlay FindSourceCodeForName(string name);
        SourceCodeForDisPlay FindSourceCodeForEnglishName(string EnglishName);

        //UpdateSourceCode with id and new data
        Task<bool> UpdateSourceCode(SourceCodeForDisPlay temp);

        //RemoveSourceCode
        Task<bool> RemoveSourceCode(SourceCodeForDisPlay temp);
        Task<bool> RemoveSourceCode(int id);

        //Sort
        List<SourceCodeForDisPlay> GetSortedSourceCode();

    }
}
