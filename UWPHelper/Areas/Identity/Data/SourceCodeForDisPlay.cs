using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UWPHelper.Models;

namespace UWPHelper.Models
{
    public class SourceCodeForDisPlay
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string FileUrl { get; set; }
        public string SourceCode { get; set; }
        public bool ifExist { get; }

        [DataType(DataType.Date)]
        public DateTime LastEditTime { get; set; }
        public int SearchTime { get; set; }
        public string DocURL { get; set; }

        public SourceCodeForDisPlay()
        {
            this.FileUrl = "";
            this.Name = "";
            this.SourceCode = "";
            this.ifExist = true;
            this.ID = int.MinValue;
        }

        public SourceCodeForDisPlay(SourceCode sourceCode)
        {
            ID = sourceCode.ID;
            LastEditTime = sourceCode.LastEditDate;
            Name = sourceCode.Name;
            SearchTime = sourceCode.SearchTime;
            FileUrl = sourceCode.FileUrl;
            DocURL = sourceCode.DocURL;

            FileInfo fileInfo = new FileInfo(FileUrl);
            if (fileInfo.Exists)
            {
                ifExist = true;
                StreamReader sr = new StreamReader(FileUrl);
                SourceCode = sr.ReadToEnd();
                sr.Close();
            }
            else
            {
                ifExist = false;
                SourceCode = "";
            }
        }
    }
}
