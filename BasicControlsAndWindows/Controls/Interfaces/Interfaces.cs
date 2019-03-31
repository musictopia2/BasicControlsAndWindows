using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace BasicControlsAndWindows.Controls.Interfaces
{
    public enum EnumDataEntryCategory
    {
        SingleLineTextbox = 1, MultiLineTextbox, ComboBox
    }

    public interface IBasicEntry
    {
        EnumDataEntryCategory Category { get; } //i am guessing that is how i make it readonly.

        string BindingPath { get; set; }
        void Focus();
        bool HasValidationError();
    }

    public interface IDataEntry : IBasicEntry
    {
        
        //event HasEntered(object sender, EventArgs e)

        event EventHandler HasEntered; //i guess this is how its done via c#.

    }
    public interface IButtonData
    {
        Brush ButtonBackColor { get; set; }
        Brush ButtonForeColor { get; set; }
        void AddControl(UIElement ThisCon);
    }
}
