using System;
using System.Collections;
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
namespace BasicControlsAndWindows.Helpers
{
    public class ListViewHelperWithGrid
    {
        ListView ThisList;
        GridView TView;

        public bool IsReadOnly { get; set; }

        public bool WhenPropertyChanges { get; set; }

        public ListViewHelperWithGrid(IEnumerable ItemSource, bool IsMultiple = false)
        {
            ThisList = new ListView
            {
                Background = Brushes.Transparent,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                IsSynchronizedWithCurrentItem = true,
                Margin = new Thickness(0, 20, 0, 0)
            };
            Style ThisStyle = new Style
            {
                TargetType = typeof(ListBoxItem)
            };
            ThisStyle.Setters.Add(new Setter(Control.HorizontalContentAlignmentProperty, HorizontalAlignment.Stretch));
            ThisStyle.Setters.Add(new Setter(Control.VerticalContentAlignmentProperty, VerticalAlignment.Stretch));
            if (IsMultiple == false)
                ThisStyle.Setters.Add(new EventSetter(Control.GotFocusEvent, new RoutedEventHandler(Item_GotFocus)));
            ThisList.ItemContainerStyle = ThisStyle;
            TView = new GridView();
            ThisList.View = TView;
            ThisList.ItemsSource = ItemSource;
        }

        public ListView GetList()
        {
            return ThisList;
        }

        public void AddLabelColumn(string Header, double Width, string DisplayMemberPath, IValueConverter Converter = null)
        {
            GridViewColumn ThisC = new GridViewColumn();
            Binding ThisBind = new Binding(DisplayMemberPath)
            {
                Converter = Converter
            };
            ThisC.Header = Header;
            ThisC.Width = Width;
            FrameworkElementFactory txt = new FrameworkElementFactory(typeof(TextBlock));
            txt.SetBinding(TextBlock.TextProperty, ThisBind);
            txt.SetValue(TextBlock.TextWrappingProperty, TextWrapping.Wrap);
            ThisC.CellTemplate = new DataTemplate(typeof(string))
            {
                VisualTree = txt
            };
            TView.Columns.Add(ThisC);
        }

        public void AddDefaultSongColumns()
        {
            AddLabelColumn("ID", 50, "ID");
            AddLabelColumn("Artist", 250, "Artist.Artist1");
            AddLabelColumn("Song", 400, "Song1");
            AddLabelColumn("Weight", 50, "Weight");
        }

        public void AddCheckBoxTempSong()
        {
            GridViewColumn ThisC = new GridViewColumn();
            Binding ThisBind = new Binding("TempSongChosen")
            {
                Mode = BindingMode.TwoWay
            };
            ThisC.Header = "Chosen?";
            FrameworkElementFactory txt = new FrameworkElementFactory(typeof(CheckBox)); //decided to not use the custom checkbox. otherwise, would require skiasharp
            txt.SetBinding(CheckBox.IsCheckedProperty, ThisBind);
            ThisC.CellTemplate = new DataTemplate(typeof(string))
            {
                VisualTree = txt
            };
            TView.Columns.Add(ThisC);
        }

        public void AddSingleLineTextColumn(string Header, double Width, string PropertyName, IValueConverter Converter = null, bool PrivateReadOnly=false)
        {
            GridViewColumn ThisC = new GridViewColumn();
            Binding ThisBind = new Binding(PropertyName)
            {
                Converter = Converter
            };
            ThisC.Header = Header;
            ThisC.Width = Width;
			
            if (WhenPropertyChanges == true)
                ThisBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            FrameworkElementFactory txt = new FrameworkElementFactory(typeof(TextBox));
            txt.SetBinding(TextBox.TextProperty, ThisBind);
            txt.SetValue(TextBox.TextWrappingProperty, TextWrapping.Wrap);
			
            txt.SetValue(FrameworkElement.MarginProperty, new Thickness(-6, 0, -6, 0));
			txt.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Top);
            if (PrivateReadOnly == true || IsReadOnly == true)
                txt.SetValue(TextBoxBase.IsReadOnlyProperty, true);
            ThisC.CellTemplate = new DataTemplate(typeof(string))
            {
                VisualTree = txt
            };
            TView.Columns.Add(ThisC);
        }

        public void AddMultiLineTextColumn(string Header, double Width, double Height, string PropertyName, IValueConverter Converter = null, bool PrivateReadOnly = false)
        {
            GridViewColumn ThisC = new GridViewColumn();
            Binding ThisBind = new Binding(PropertyName)
            {
                Converter = Converter
            };
            ThisC.Header = Header;
            ThisC.Width = Width;
            if (WhenPropertyChanges == true)
                ThisBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            FrameworkElementFactory txt = new FrameworkElementFactory(typeof(TextBox));
            txt.SetBinding(TextBox.TextProperty, ThisBind);
            txt.SetValue(TextBox.TextWrappingProperty, TextWrapping.Wrap);
            txt.SetValue(FrameworkElement.MarginProperty, new Thickness(-6, 0, -6, 0));
            txt.SetValue(FrameworkElement.HeightProperty, Height);
            txt.SetValue(TextBoxBase.AcceptsReturnProperty, true);
            txt.SetValue(TextBoxBase.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Auto);
            if (PrivateReadOnly == true || IsReadOnly == true)
                txt.SetValue(TextBoxBase.IsReadOnlyProperty, true);
            ThisC.CellTemplate = new DataTemplate(typeof(string))
            {
                VisualTree = txt
            };
            TView.Columns.Add(ThisC);
        }

        private void Item_GotFocus(object sender, RoutedEventArgs e)
        {
            var Item = (ListViewItem)sender;
            ThisList.SelectedItem = Item.DataContext;
        }

        //we have 2 more classes left (baselabelgrid and dataentryhelpers)
        //will try to see if we can do away with the data entry view model.
        //if its at least that one, maybe can do it.
        //if not, then move on.

    }
}
