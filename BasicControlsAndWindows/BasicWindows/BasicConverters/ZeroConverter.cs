using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
namespace BasicControlsAndWindows.BasicWindows.BasicConverters
{
    public class ZeroConverter : CommonBasicStandardLibraries.CommonConverters.ZeroConverter, IValueConverter
    {
        public ZeroConverter()
        {
            DoPutBack = true; //desktop can put back.  xamarin forms will not.
        }
    }
}
