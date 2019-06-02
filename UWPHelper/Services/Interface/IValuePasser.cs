using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UWPHelper.Services.Interface
{
    public interface IValuePasser
    {
        string getStringValue();
        int getIntValue();

        void setStringValue(string value);
        void setIntValue(int value);
    }
}
