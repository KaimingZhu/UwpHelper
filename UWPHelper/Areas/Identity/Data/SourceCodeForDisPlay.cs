using System;
using System.Collections.Generic;
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

        public SourceCodeForDisPlay(int ID,string Name,string FileUrl)
        {

            this.ID = ID;
            this.Name = Name;
            this.FileUrl = FileUrl;

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
                this.SourceCode = "";
            }
        }

        public SourceCodeForDisPlay()
        {
            this.FileUrl = "";
            this.Name = "";
            this.SourceCode = "";
            this.ifExist = true;
            this.ID = int.MinValue;
        }
    }
}
