using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CommonBasicStandardLibraries.Exceptions;
using BasicControlsAndWindows.Controls.Interfaces;
using System.Windows.Controls;
using static BasicControlsAndWindows.Helpers.GridHelper;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using static BasicControlsAndWindows.Helpers.SimpleControlHelpers;
using CommonBasicStandardLibraries.MVVMHelpers;
using System.Windows.Media;

namespace BasicControlsAndWindows.Helpers
{
    public class ButtonListHelper : IButtonData
    {
        //don't even care about the view model for this one.
        private readonly Grid ThisGrid;
        private readonly Window CurrentWindow;
        public ButtonListHelper(Window _Window)
        {
            ThisGrid = new Grid();
            CurrentWindow = _Window;
            //ThisGrid.DataContext = ThisMod;
        }
        public Brush ButtonBackColor { get; set; }
        public Brush ButtonForeColor { get; set; }
        void IButtonData.AddControl(UIElement ThisCon)
        {
            AddControlToGrid(ThisGrid, ThisCon, ThisGrid.RowDefinitions.Count - 1, 0);
            //this is a case where i should not achieve code reuse via inheritanc.
        }
        public void AddGenericButton(Key ThisKey, ICommand ThisCommand, string OtherText, object Parameter = null)
        {
            ButtonHelperClass.AddButton(this, OtherText, ThisKey, ThisGrid, ThisCommand, Parameter: Parameter, ThisWindow: CurrentWindow);
        }
        public void AddGenericButton(Key ThisKey, string CommandPath, string OtherText, string Parameter = null)
        {
            ButtonHelperClass.AddButton(this, OtherText, ThisKey, ThisGrid, CommandPath: CommandPath, Parameter: Parameter, ThisWindow: CurrentWindow);
        }
        public Grid GetContent() => ThisGrid;
    }
}