using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UWPHelper.Models
{
    public class SourceCode
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string EnglishName { get; set; }

        public DateTime LastEditDate { get; set; }
        public int SearchTime { get; set; }
        public string DocURL { get; set; }

        public SourceCode()
        {
            ID = int.MaxValue;
            Name = "";
            EnglishName = "";
        }

        public SourceCode(SourceCodeForDisPlay sourceCodeForDisPlay,bool ifExist)
        {
            if (ifExist)
            {
                ID = sourceCodeForDisPlay.ID;
            }
            Name = sourceCodeForDisPlay.Name;
            EnglishName = sourceCodeForDisPlay.EnglishName;
            LastEditDate = sourceCodeForDisPlay.LastEditTime;
            SearchTime = sourceCodeForDisPlay.SearchTime;
            DocURL = sourceCodeForDisPlay.DocURL;
        }
    }
}
