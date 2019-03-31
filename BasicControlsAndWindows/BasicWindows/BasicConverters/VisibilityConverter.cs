using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
namespace BasicControlsAndWindows.BasicWindows.BasicConverters
{
    public class VisibilityConverter : CommonBasicStandardLibraries.CommonConverters.VisibilityConverter, IValueConverter
    {

        public bool UseCollapsed { get; set; } = false;

        private object GetVisibleObjectResults(bool Results)
        {
            if (Results == true)
                return Visibility.Visible;
            if (UseCollapsed == true)
                return Visibility.Collapsed;
            return Visibility.Hidden;
        }

        public VisibilityConverter()
        {
            VisibleDelegate = GetVisibleObjectResults;
        }
    }
}
