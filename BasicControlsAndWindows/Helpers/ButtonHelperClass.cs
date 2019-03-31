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
namespace BasicControlsAndWindows.Helpers
{
    internal static class ButtonHelperClass
    {
        public static void AddButton(IButtonData ThisData, string OtherText, Key ThisKey, Grid ThisGrid, ICommand ThisCommand = null, string CommandPath = "", object Parameter = null, Window ThisWindow = null)
        {
            if (ThisCommand == null && CommandPath == "")
                throw new BasicBlankException("You have to fill in either the command interface or command path");
            AddAutoRows(ThisGrid, 1);

            Button ThisButton = new Button
            {
                Content = $"{OtherText} ({ThisKey.ToString()})",
                FontSize = 25,
            };
            ThisButton.SetBinding(ButtonBase.CommandProperty, new Binding(CommandPath));
            if (ThisData.ButtonBackColor != null)
            {
                ThisButton.Background = ThisData.ButtonBackColor;
            }

            if (ThisData.ButtonForeColor != null)
            {
                ThisButton.Foreground = ThisData.ButtonForeColor;
            }
            SetMargins(ThisButton);
            if (Parameter != null)
            {
                ThisButton.CommandParameter = Parameter;
            }

            if (CommandPath !="")
            {
                ThisButton.SetBinding(Button.CommandProperty, new Binding(nameof(CommandPath)));
                if (ThisWindow != null)
                {
                    SetKeyBindings(ThisWindow, ThisKey, CommandPath, Parameter);
                }
                else
                {
                    SetKeyBindings(ThisGrid, ThisKey, CommandPath, Parameter);
                }
            }
            else
            {
                ThisButton.Command = ThisCommand; //i think
                if (ThisWindow != null)
                {
                    SetKeyBindings(ThisWindow, ThisKey, ThisCommand, Parameter);
                }
                else
                {
                    SetKeyBindings(ThisGrid, ThisKey, ThisCommand, Parameter);
                }
            }

            ThisData.AddControl(ThisButton);
        }
    }
}
