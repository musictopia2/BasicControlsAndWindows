using BasicControlsAndWindows.BasicWindows.Windows;
using CommonBasicStandardLibraries.MVVMHelpers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using static BasicControlsAndWindows.Helpers.GridHelper;
namespace BasicControlsAndWindows.Helpers
{
    public class SimpleLabelList<T> where T : BaseViewModel, new()
    {
        readonly PlainWindow<T> ThisWindow; //no need for viewmodel.  because it should already been set.
        readonly StackPanel ThisStack;
        public int DefaultLabelSize { get; set; }
        private int PrivateTop;

        public void AddRow(string Text, bool IsBold = false)
        {
            TextBlock ThisLabel = ThisWindow.GetDefaultLabel();
            if (DefaultLabelSize > 0)
            {
                ThisLabel.FontSize = DefaultLabelSize;
            }

            if (IsBold == true)
            {
                ThisLabel.FontWeight = FontWeights.Bold;
                ThisLabel.Margin = new Thickness(0, 0, 0, 10);
            }
            ThisLabel.Text = Text;
            ThisStack.Children.Add(ThisLabel);
        }

        public void AddRow(string Text, Key ThisKey, ICommand ThisCommand, object Parameter = null)
        {
            TextBlock ThisLabel = ThisWindow.GetDefaultLabel();
            if (DefaultLabelSize > 0)
            {
                ThisLabel.FontSize = DefaultLabelSize;
            }

            if (PrivateTop > 0)
            {
                ThisLabel.Margin = new Thickness(0, PrivateTop, 0, 0);
                PrivateTop = 0;
            }
            ThisLabel.Text = $"{Text} ({ThisKey.ToString()})";
            SetKeyBindings(ThisWindow, ThisKey, ThisCommand, Parameter);
            ThisStack.Children.Add(ThisLabel);
        }

        public void AddBindingRow(string MainText, string ValuePath, Key ThisKey, ICommand ThisCommand, string Parameter = "")
        {
            StackPanel TempStack = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };
            TextBlock ThisLabel = ThisWindow.GetDefaultLabel();
            if (DefaultLabelSize > 0)
            {
                ThisLabel.FontSize = DefaultLabelSize;
            }

            ThisLabel.Text = $"{MainText} ";
            TempStack.Children.Add(ThisLabel);
            ThisLabel = ThisWindow.GetDefaultLabel();
            if (DefaultLabelSize > 0)
            {
                ThisLabel.FontSize = DefaultLabelSize;
            }

            ThisLabel.SetBinding(TextBlock.TextProperty, ValuePath);
            TempStack.Children.Add(ThisLabel);
            ThisLabel = ThisWindow.GetDefaultLabel();
            if (DefaultLabelSize > 0)
            {
                ThisLabel.FontSize = DefaultLabelSize;
            }

            ThisLabel.Text = $" ({ThisKey.ToString()})";
            if (Parameter == "")
            {
                SetVariableKeyBindings(ThisWindow, ThisKey, ThisCommand, ValuePath);
            }
            else
            {
                SetVariableKeyBindings(ThisWindow, ThisKey, ThisCommand, Parameter);
            }

            TempStack.Children.Add(ThisLabel);
            ThisStack.Children.Add(TempStack);
        }

        public void AddBreak(int HowMuch)
        {
            PrivateTop = HowMuch;
        }

        public void AddBindingRow(string MainText, string ValuePath)
        {
            StackPanel TempStack = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };
            TextBlock ThisLabel = ThisWindow.GetDefaultLabel();
            if (DefaultLabelSize > 0)
            {
                ThisLabel.FontSize = DefaultLabelSize;
            }

            ThisLabel.FontWeight = FontWeights.Bold;
            ThisLabel.Text = $"{MainText}:  ";
            TempStack.Children.Add(ThisLabel);
            ThisLabel = ThisWindow.GetDefaultLabel();
            if (DefaultLabelSize > 0)
            {
                ThisLabel.FontSize = DefaultLabelSize;
            }

            ThisLabel.SetBinding(TextBlock.TextProperty, ValuePath);
            TempStack.Children.Add(ThisLabel);
            ThisStack.Children.Add(TempStack);
        }

        public StackPanel GetContent()
        {
            return ThisStack;
        }

        public SimpleLabelList(PlainWindow<T> _Window, string BindingPath, IValueConverter _Convert, object ThisParameter)
        {
            ThisWindow = _Window;
            ThisStack = new StackPanel();
            Binding ThisBind = new Binding(BindingPath)
            {
                Converter = _Convert,
                ConverterParameter = ThisParameter
            };
            ThisStack.SetBinding(UIElement.VisibilityProperty, ThisBind);
        }

        public SimpleLabelList(PlainWindow<T> _Window)
        {
            ThisWindow = _Window;
            ThisStack = new StackPanel();
        }
    }
}
