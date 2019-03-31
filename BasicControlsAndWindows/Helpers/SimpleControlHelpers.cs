using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BasicControlsAndWindows.Helpers
{
    public static class SimpleControlHelpers
    {
        /// <summary>
        /// This sets up 5 around everything.
        /// </summary>
        /// <param name="ThisCon"></param>
        public static void SetMargins(FrameworkElement ThisCon)
        {
            ThisCon.Margin = new Thickness(5, 5, 5, 5);
        }
    }
}
