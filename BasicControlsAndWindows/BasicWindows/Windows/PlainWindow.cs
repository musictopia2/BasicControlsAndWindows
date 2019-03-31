using CommonBasicStandardLibraries.MVVMHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace BasicControlsAndWindows.BasicWindows.Windows
{
    public abstract class PlainWindow<VM> : BasicWindow<VM> where VM : BaseViewModel
    {
        protected override int GetFontSize()
        {
            return 20;
        }

        protected override Brush ButtonBackColor()
        {
            return null;
        }

        protected override Brush ButtonForeColor()
        {
            return null;
        }

        protected override Brush LabelForeColor()
        {
            return Brushes.Black;
        }

        protected override Brush WindowBackgroundColor()
        {
            return null;
        }
    }
}
