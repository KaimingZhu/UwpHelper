using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UWPHelper.Services.Interface;

namespace UWPHelper.Services.Implement
{
    public class ValuePasserManager : IValuePasser
    {
        private string stringValue;
        private int intValue;

        public ValuePasserManager()
        {
            intValue = int.MinValue;
            stringValue = "";
        }

        public int getIntValue()
        {
            return intValue;
        }

        public string getStringValue()
        {
            return stringValue;
        }

        public void setIntValue(int value)
        {
            intValue = value;
        }

        public void setStringValue(string value)
        {
            stringValue = value;
        }
    }
}
