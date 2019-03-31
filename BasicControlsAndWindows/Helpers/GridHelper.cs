using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
namespace BasicControlsAndWindows.Helpers
{
    public static class GridHelper
    {
        public static void PositionDataGrid(DataGrid ThisGrid)
        {
            ThisGrid.HorizontalAlignment = HorizontalAlignment.Left;
            ThisGrid.VerticalAlignment = VerticalAlignment.Top;
        }

        public static void AddAutoRows(Grid ThisGrid, int HowMany = 1)
        {
            RowDefinition ThisD;
            for (int x = 1; x <= HowMany; x++)
            {
                ThisD = new RowDefinition
                {
                    Height = GridLength.Auto
                };
                ThisGrid.RowDefinitions.Add(ThisD);
            }
        }

        public static void AddLeftOverRow(Grid ThisGrid, int Percs = 1)
        {
            RowDefinition ThisD = new RowDefinition()
            {
                Height = new GridLength(Percs, GridUnitType.Star)
            };
            ThisGrid.RowDefinitions.Add(ThisD);
        }

        public static void AddPixelRow(Grid ThisGrid, int HowMany)
        {
            RowDefinition ThisD = new RowDefinition()
            {
                Height = new GridLength(HowMany, GridUnitType.Pixel)
            };
            ThisGrid.RowDefinitions.Add(ThisD);
        }

        public static void AddAutoColumns(Grid ThisGrid, int HowMany = 1)
        {
            ColumnDefinition ThisD;
            for (int x = 1; x <= HowMany; x++)
            {
                ThisD = new ColumnDefinition
                {
                    Width = GridLength.Auto
                };
                ThisGrid.ColumnDefinitions.Add(ThisD);
            }
        }

        public static void AddLeftOverColumn(Grid ThisGrid, int Percs = 1)
        {
            ColumnDefinition ThisD = new ColumnDefinition()
            {
                Width = new GridLength(Percs, GridUnitType.Star)
            };
            ThisGrid.ColumnDefinitions.Add(ThisD);
        }

        public static void AddPixelColumn(Grid ThisGrid, int HowMany)
        {
            ColumnDefinition ThisD = new ColumnDefinition()
            {
                Width = new GridLength(HowMany, GridUnitType.Pixel)
            };
            ThisGrid.ColumnDefinitions.Add(ThisD);
        }

        public static void AddControlToGrid(Grid ThisGrid, UIElement ThisElement, int Row, int Column)
        {
            ThisGrid.Children.Add(ThisElement);
            Grid.SetColumn(ThisElement, Column);
            Grid.SetRow(ThisElement, Row);
        }

        public static void SetKeyBindings(Grid ThisGrid, Key ThisKey, string Path, object Parameter = null)
        {
            KeyBinding ThisInput = new KeyBinding
            {
                Key = ThisKey // 
            };
            if (Parameter == null == false)
                ThisInput.CommandParameter = Parameter;
            Binding ThisBind = new Binding(Path);
            BindingOperations.SetBinding(ThisInput, InputBinding.CommandProperty, ThisBind);
            ThisGrid.InputBindings.Add(ThisInput);
        }

        public static void SetKeyBindings(Grid ThisGrid, Key ThisKey, ICommand Command, object Parameter = null)
        {
            KeyBinding ThisInput = new KeyBinding
            {
                Key = ThisKey // 
            };
            if (Parameter == null == false)
                ThisInput.CommandParameter = Parameter;
            ThisInput.Command = Command;
            ThisGrid.InputBindings.Add(ThisInput);
        }

        public static void SetKeyBindings(Window ThisWindow, Key ThisKey, string Path, object Parameter = null)
        {
            KeyBinding ThisInput = new KeyBinding
            {
                Key = ThisKey // 
            };
            if (Parameter == null == false)
                ThisInput.CommandParameter = Parameter;
            Binding ThisBind = new Binding(Path);
            BindingOperations.SetBinding(ThisInput, InputBinding.CommandProperty, ThisBind);
            ThisWindow.InputBindings.Add(ThisInput);
        } //somehow did not use the previous list

        public static  void SetKeyBindings(Window ThisWindow, Key ThisKey, ICommand Command, object Parameter = null)
        {
            KeyBinding ThisInput = new KeyBinding
            {
                Key = ThisKey // 
            };
            if (Parameter == null == false)
                ThisInput.CommandParameter = Parameter;
            ThisInput.Command = Command; // i think
            ThisWindow.InputBindings.Add(ThisInput);
        }

        public static void SetVariableKeyBindings(Window ThisWindow, Key ThisKey, ICommand Command, string Path)
        {
            KeyBinding ThisInput = new KeyBinding
            {
                Key = ThisKey // 
            };
            Binding ThisBind = new Binding(Path);
            BindingOperations.SetBinding(ThisInput, InputBinding.CommandParameterProperty, ThisBind);
            ThisInput.Command = Command;
            ThisWindow.InputBindings.Add(ThisInput);
        } // hopefully it can work (?)


    }
}
