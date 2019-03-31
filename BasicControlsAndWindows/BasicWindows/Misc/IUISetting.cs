using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace BasicControlsAndWindows.BasicWindows.Misc
{
    public interface IUISetting
    {
        Button GetDefaultButton(string Text, string CommandPath, object CommandParameter = null);
        Binding GetVisibleBinding(string VisiblePath);
        TextBlock GetDefaultLabel();
    }
}
