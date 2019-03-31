using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CommonBasicStandardLibraries.CollectionClasses;
namespace BasicControlsAndWindows.Controls.BasicControls
{
    public class SimpleReader : UserControl
    {
        private TextBlock PreviousSelected;
        private StackPanel ThisStack = new StackPanel();
        private ScrollViewer ThisScroll = new ScrollViewer();
        public bool HasText()
        {
            if (ThisStack.Children.Count == 0)
                return false;
            return true;
        }

        public CustomBasicList<string> ItemSource
        {
            set {
                ThisStack.Children.Clear();
                value.ForEach(ThisItem =>
                {
                    TextBlock ThisLabel = new TextBlock
                    {
                        Text = ThisItem,
                        TextWrapping = TextWrapping.Wrap
                    };
                    ThisStack.Children.Add(ThisLabel);
                });
                ThisScroll.ScrollToTop();
                PreviousSelected = (TextBlock) ThisStack.Children[0];
                PreviousSelected.Background = Brushes.Aqua;
            }
        }

        public void ArrowDown()
        {
            if (PreviousSelected == null)
                return;
            var TempCon = ThisStack.Children[ThisStack.Children.Count - 1];
            if (PreviousSelected.Equals(TempCon) == true)
                return; //because its the same one
            TextBlock NextLabel;
            int Index;
            Index = ThisStack.Children.IndexOf(PreviousSelected);
            Index++; //i guess this implies by 1
            NextLabel = (TextBlock) ThisStack.Children[Index];
            PreviousSelected.Background = Brushes.Transparent;
            ScrollToItem(NextLabel, Index);
        }

        private void ScrollToItem(TextBlock NextLabel, int Index)
        {
            var ThisTop = NextLabel.ActualHeight * Index;
            PreviousSelected = NextLabel;
            PreviousSelected.Background = Brushes.Aqua;
            ThisScroll.ScrollToVerticalOffset(ThisTop);
        }

        public SimpleReader()
        {
            ThisScroll.Content = ThisStack;
            ThisScroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            ThisScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            Content = ThisScroll;
        }

    }
}
