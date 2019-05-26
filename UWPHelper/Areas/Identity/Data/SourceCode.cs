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
        public string FileUrl { get; set; }

        public DateTime LastEditDate { get; set; }
        public int SearchTime { get; set; }
        public string DocURL { get; set; }
    }
}
