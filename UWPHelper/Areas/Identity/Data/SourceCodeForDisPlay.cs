using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UWPHelper.Models;

namespace UWPHelper.Models
{
    public class SourceCodeForDisPlay : IComparable
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string EnglishName { get; set; }
        public string SourceCode { get; set; }
        public bool ifExist { get; }

        [DataType(DataType.Date)]
        public DateTime LastEditTime { get; set; }
        public int SearchTime { get; set; }
        public string DocURL { get; set; }

        public SourceCodeForDisPlay()
        {
            this.EnglishName = "";
            this.Name = "";
            this.SourceCode = "";
            this.ifExist = true;
            this.ID = int.MinValue;
        }

        private string TimeToString(DateTime Time)
        {
            return Time.Year.ToString() + "_" + Time.Month.ToString() + "_" + Time.Day.ToString() + "-" +
                Time.Hour.ToString() + "_" + Time.Minute.ToString() + "_" + Time.Second.ToString() + "_" + Time.Millisecond.ToString();
        }

        //返回.cs文件名称(带.cs后缀)
        public string GetFileName()
        {
            return TimeToString(LastEditTime) + "-" + EnglishName + ".cs";
        }

        public SourceCodeForDisPlay(SourceCode sourceCode)
        {
            ID = sourceCode.ID;
            LastEditTime = sourceCode.LastEditDate;
            Name = sourceCode.Name;
            SearchTime = sourceCode.SearchTime;
            EnglishName = sourceCode.EnglishName;
            DocURL = sourceCode.DocURL;

            FileInfo fileInfo = new FileInfo("SourceCodeData\\"  + TimeToString(LastEditTime) + "-" + sourceCode.EnglishName + ".cs");
            if (fileInfo.Exists)
            {
                ifExist = true;
                StreamReader sr = new StreamReader("SourceCodeData\\" + TimeToString(LastEditTime) + "-" + sourceCode.EnglishName + ".cs");
                SourceCode = sr.ReadToEnd();
                sr.Close();
            }
            else
            {
                ifExist = false;
                SourceCode = "";
            }
        }

        public int CompareTo(object obj)
        {
            SourceCodeForDisPlay sobj = (SourceCodeForDisPlay)obj;
            return SearchTime.CompareTo(sobj.SearchTime);
        }
    }
}
