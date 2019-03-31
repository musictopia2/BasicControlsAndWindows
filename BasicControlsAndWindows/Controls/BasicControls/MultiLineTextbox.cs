using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using BasicControlsAndWindows.Controls.Interfaces;
namespace BasicControlsAndWindows.Controls.BasicControls
{

    

    public class MultiLineTextbox : TextBox, IBasicEntry //trying to do it this way.  i can do a list of basicentry and even if one is data entry will still work.
    {
        public EnumDataEntryCategory Category => EnumDataEntryCategory.MultiLineTextbox;

        public string BindingPath { get; set; }

        //public event EventHandler HasEntered;

        public bool HasValidationError()
        {
            BindingExpression ThisBind = GetBindingExpression(TextProperty);
            return ThisBind.HasValidationError; //i hope this is the only problem
        }

        void IBasicEntry.Focus()
        {
            Focus();
        }

        

        public MultiLineTextbox()
        {
            AcceptsReturn = true;
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            TextWrapping = TextWrapping.Wrap;
            Validation.SetErrorTemplate(this, null);
        }
    }
}
