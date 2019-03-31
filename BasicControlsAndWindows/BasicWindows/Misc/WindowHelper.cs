using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using BasicControlsAndWindows.BasicWindows.BasicConverters;
namespace BasicControlsAndWindows.BasicWindows.Misc
{
    public static  class WindowHelper
    {
        public static Window CurrentWindow { get; set; }

        public static void SetTitleBindings(object Context)
        {
            CurrentWindow.DataContext = Context;
            CurrentWindow.SetBinding(Window.TitleProperty, new Binding("Title"));
        }

        public static void SetDefaultLocation()
        {
            CurrentWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        public static void SetSize(double Width, double Height)
        {
            CurrentWindow.Height = Height;
            CurrentWindow.Width = Width;
        }

        public static void SetKeyBindings(Key ThisKey, string Path, object Parameter = null)
        {
            KeyBinding ThisInput = new KeyBinding
            {
                Key = ThisKey
            };
            if (Parameter == null)
                ThisInput.CommandParameter = Parameter;
            Binding ThisBind = new Binding(Path);
            BindingOperations.SetBinding(ThisInput, InputBinding.CommandProperty, ThisBind);
            CurrentWindow.InputBindings.Add(ThisInput);
        }

        public static Binding GetVisibleBinding(string VisiblePath)
        {
            Binding ThisBind = new Binding(VisiblePath);
            VisibilityConverter ThisC = new VisibilityConverter
            {
                UseCollapsed = true //has to do an implementation because i need the extra.
            };
            ThisBind.Converter = ThisC;
            return ThisBind;
        }
        public static object GetPublicVisible(bool Results)
        {
            if (Results == true)
                return Visibility.Visible;
            return Visibility.Collapsed;
        }
    }
}
