using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UWPHelper.Models;

namespace UWPHelper.Services.Interface
{
    public interface ISourceCodeManager
    {
        //GetSourceCode
        List<SourceCodeForDisPlay> GetSourceCodeForDisPlays();

        //AddSourceCode
        void AddSourceCode(SourceCodeForDisPlay temp);

        //FindSourceCode
        SourceCodeForDisPlay FindSourceCode(int id);
        SourceCodeForDisPlay FindSourceCode(string FileUrl);

        //UpdateSourceCode
        void UpdateSourceCode(SourceCodeForDisPlay temp);
        void UpdateSourceCode(int id);

        //RemoveSourceCode
        void RemoveSourceCode(SourceCodeForDisPlay temp);
        void RemoveSourceCode(int id);

        //Sort
        void SortSourceCode();

    }
}
