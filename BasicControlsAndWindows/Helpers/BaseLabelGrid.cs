using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using BasicControlsAndWindows.BasicWindows.Windows;
using CommonBasicStandardLibraries.MVVMHelpers;
using static BasicControlsAndWindows.Helpers.GridHelper;
using BasicControlsAndWindows.BasicWindows.BasicConverters;
namespace BasicControlsAndWindows.Helpers
{
    public class BaseLabelGrid
    {
        protected Color FontColor = Colors.Black;
        private Grid ThisGrid;
        public int FontSize { get; set; } = 0; //if fontsize is given, use it.

        public BaseLabelGrid()
        {
            ThisGrid = new Grid()
            {
                Margin = new Thickness(3, 3, 0, 0)
            };
            ColumnDefinition ThisCol = new ColumnDefinition
            {
                Width=GridLength.Auto
            };
            ThisGrid.ColumnDefinitions.Add(ThisCol);
        }

        public void AddRow(string Header, string BindingPath, IValueConverter ConverterChoice = null, string VisiblePath="", Brush ColorUsed = null)
        {
            SetUpReturns(Header);
            Binding ThisBind = new Binding(BindingPath)
            {
                Converter = ConverterChoice
            };
            TextBlock ThisText = new TextBlock
            {
                Margin=new Thickness(0, 0, 5, 3),
                TextWrapping= TextWrapping.Wrap,
                VerticalAlignment=VerticalAlignment.Bottom
            };
            ThisText.SetBinding(TextBlock.TextProperty, ThisBind);
            if (VisiblePath != "")
            {
                VisibilityConverter ThisV = new VisibilityConverter();
                ThisBind = new Binding(VisiblePath);

                ThisText.SetBinding(TextBlock.VisibilityProperty, ThisBind);

            }
            if (ColorUsed == null)
                ThisText.Foreground = new SolidColorBrush(FontColor);
            else
                ThisText.Foreground = ColorUsed;
            if (FontSize > 0)
                ThisText.FontSize = FontSize;
            AddControl(ThisText, false);
        }

        private void SetUpReturns(string Header)
        {
            RowDefinition ThisCol = new RowDefinition
            {
                Height = GridLength.Auto
            };
            ThisGrid.RowDefinitions.Add(ThisCol);
            AddLabel(Header);
        }

        private void AddLabel(string Header)
        {
            TextBlock ThisLabel = new TextBlock()
            {
                Text = $"{Header}:",
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(3, 3, 3, 3),
                Foreground = new SolidColorBrush(FontColor),
                VerticalAlignment=VerticalAlignment.Top
            };
            if (FontSize > 0)
                FontSize = FontSize;
            AddControl(ThisLabel, true);
        }

        private void AddControl(TextBlock ThisCon, bool IsHeader)
        {
            ThisGrid.Children.Add(ThisCon);
            if (IsHeader == false)
                Grid.SetColumn(ThisCon, 1);
            Grid.SetRow(ThisCon, ThisGrid.ColumnDefinitions.Count - 1); //because its 0 based.
        }

        public Grid GetContent() => ThisGrid;
    }
}
