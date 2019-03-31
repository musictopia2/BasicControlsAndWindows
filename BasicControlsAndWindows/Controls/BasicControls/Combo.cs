using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using BasicControlsAndWindows.Controls.Interfaces;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using static BasicControlsAndWindows.Controls.BasicControls.Objects;
using CommonBasicStandardLibraries.Exceptions;
using System.Collections;
using System.Windows.Media;
namespace BasicControlsAndWindows.Controls.BasicControls
{
    public class Combo : UserControl, IDataEntry //decided to risk not implementing the notifypropertychange  since there was a warning that was never used.  if needed, then rethink.
    {
        public EnumDataEntryCategory Category => EnumDataEntryCategory.ComboBox;

        public string BindingPath { get; set; }

        //public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler HasEntered; //still needed

        private readonly Grid ThisGrid;
        private readonly TextBox ThisText;
        private readonly ListBox ThisList;
        private int PreviousSelected = -1;
        private bool IsProcessing;
        public event TextEventData EnteredSofar; //i think

        public bool AllowArrowKeys { get; set; } = true;

        internal enum EnumTextCategory
        {
            SimpleText = 1, EnumBinding, NormalBinding
        }

        EnumTextCategory TextC = EnumTextCategory.SimpleText; //until proven otherwise.
        //CustomBasicList<string> PrivateList = new CustomBasicList<string>();
        //i thought i needed but realized i did not after all.
        private string PropertyName;

        public void SendSelectedItemBinding(BindingBase ThisBind)
        {
            ThisList.SetBinding(ListBox.SelectedItemProperty, ThisBind);
        }

		public Combo()
		{
			ThisGrid = new Grid();
			MinHeight = 40;
			RowDefinition ThisRow = new RowDefinition
			{
				Height = new GridLength(40)
			};

			ThisGrid.RowDefinitions.Add(ThisRow);
			ThisRow = new RowDefinition
			{
				Height = new GridLength(1, GridUnitType.Star)
			};
			ThisGrid.RowDefinitions.Add(ThisRow);
            ThisText = new TextBox();
            Validation.SetErrorTemplate(ThisText, null);
            ThisText.FontSize = 20;
            ThisGrid.RowDefinitions.First().Height = new GridLength(29);
            ThisGrid.Children.Add(ThisText);
            ThisList = new ListBox()
            {
                //try to not have name
                FontSize = 16,
            };
            ThisGrid.Children.Add(ThisList);
            Validation.SetErrorTemplate(ThisList, null);
            Grid.SetRow(ThisList, 1);
            Content = ThisGrid;
            ThisText.TextChanged += ThisText_TextChanged;
            ThisList.SelectionChanged += ThisList_SelectionChanged;
            ThisText.KeyDown += ThisText_KeyDown;
            ThisText.KeyUp += ThisText_KeyUp;
        }

        private void ThisText_KeyUp(object sender, KeyEventArgs e)
        {
            if (AllowArrowKeys == false)
                return; //because used somewhere else
            if (e.Key == Key.Down)
            {
                if (ThisList.Items.Count > 0 && PreviousSelected < ThisList.Items.Count - 1)
                    ScrollToView(PreviousSelected + 1);
                ThisText.SelectionStart = ThisText.Text.ToString().Count();
                EnteredSofar?.Invoke(ThisText.Text);
                return;
            }
            if (e.Key == Key.Up)
            {
                if (PreviousSelected > 0)
                {
                    ScrollToView(PreviousSelected - 1);
                    ThisText.SelectionStart = ThisText.Text.ToString().Count(); //not sure why its called twice before. that is probably wrong.  well see.
                }
                return;
            }
            if (e.Key == Key.Return || e.Key == Key.F1 || e.Key == Key.F2 ||
                e.Key == Key.F3 || e.Key == Key.F4 || e.Key == Key.F5 ||
                e.Key == Key.F6 || e.Key == Key.F7 || e.Key == Key.F8 ||
                e.Key == Key.F9 || e.Key == Key.F10 || e.Key == Key.F11 || e.Key == Key.F12)
                return;

            if (ThisText.Text == "")
            {
                ThisList.SelectedIndex = -1;
                CurrentItem = null;
                if (TextC == EnumTextCategory.EnumBinding)
                    ThisText.Text = null;
                EnteredSofar?.Invoke(ThisText.Text);
                return;
            }
            if (ThisList.Items.Count == 0)
            {
                EnteredSofar("");
                return;
            }
            if (e.Key == Key.Back)
            {
                CurrentItem = null;
                ThisList.SelectedIndex = -1;
                if (TextC == EnumTextCategory.EnumBinding)
                    ThisText.Text = null;
                return;
            }
            string NewText;
            int WhichSelect;
            if (ThisText.SelectionStart == 0)
            {
                WhichSelect = ThisText.Text.ToString().Count();
                NewText = ThisText.Text.ToLower();
            }
            else
            {
                NewText = ThisText.Text.Substring(0, ThisText.SelectionStart).ToLower();
                WhichSelect = NewText.Count();
            }
            for (int x = 0; x <= ThisList.Items.Count - 1; x++)
            {
                string TempText = GetSpecificItem(x);
                if (string.IsNullOrWhiteSpace(TempText))
                    return;
                string TempT = TempText.ToLower();
                int TempI;
                TempI = TempText.ToLower().IndexOf(NewText);
                if (TempI == 0) //maybe 0 based
                {
                    ScrollToView(x);
                    if (ThisText.Text == "")
                        return;
                    try
                    {
                        ThisText.SelectionLength = ThisText.Text.ToString().Count() - WhichSelect;
                        ThisText.SelectionStart = WhichSelect;
                    }
                    catch
                    {

                    }
                    EnteredSofar?.Invoke(ThisText.Text);
                    return;
                }

            }
            CurrentItem = null;
            ThisList.SelectedIndex = -1;
            if (TextC == EnumTextCategory.EnumBinding)
                ThisText.Text = null;

        }

        private void ThisText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
                HasEntered?.Invoke(this, new EventArgs());
        }

        private void ThisList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsProcessing == true)
                return;
            if (ThisList.SelectedIndex > -1)
            {
                ThisText.Text = GetSelectedItem();
                CurrentItem = ThisList.SelectedItem;
                PreviousSelected = ThisList.SelectedIndex;
            }
            if (ThisText.Text == "")
                PreviousSelected = -1;
            else if (ThisList.Items.Count == 0)
                PreviousSelected = -1;
            EnteredSofar?.Invoke(ThisText.Text);
            ThisText.Focus();
        }

        private void ThisText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ThisText.Text == "")
            {
                PreviousSelected = -1;
                ThisList.SelectedIndex = -1;
            }
        }

        public void ClearItems()
        {
            CurrentItem = null;
            ListIndex = -1;
            ThisText.Text = "";
        }

        private static readonly DependencyProperty CurrentItemProperty = DependencyProperty.Register("CurrentItem", typeof(object), typeof(Combo), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnCurrentItemPropertyChanged)));
        private object CurrentItem
        {
            get
            {
                return GetValue(CurrentItemProperty);
            }
            set
            {
                SetValue(CurrentItemProperty, value);
            }
        }

        private static void OnCurrentItemPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }

        public static readonly DependencyProperty ListIndexProperty = DependencyProperty.Register("ListIndex", typeof(int), typeof(Combo), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnListIndexPropertyChanged)));
        public int ListIndex
        {
            get
            {
                return ThisList.SelectedIndex;
            }
            set
            {
                SetValue(ListIndexProperty, value);
            }
        }

        private static void OnListIndexPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var ThisItem = (Combo)sender;
            {
                var withBlock = ThisItem;
                if ((int)e.NewValue == -1)
                {
                    if (withBlock.ThisList.Items.Count > 0)
                        withBlock.ThisList.SelectedIndex = 0;
                    withBlock.ThisList.SelectedIndex = -1;
                    withBlock.ThisText.Text = "";
                }
                else
                    withBlock.ScrollToView((int)e.NewValue);
            }
        }

        private void ScrollToView(int Index)
        {
            IsProcessing = true;
            ThisList.SelectedIndex = ThisList.Items.Count - 1;
            ThisList.ScrollIntoView(ThisList.SelectedItem);
            ThisList.UpdateLayout();
            IsProcessing = false;
            ThisList.SelectedIndex = Index;
            if (ThisList.SelectedIndex >= 1)
                PreviousSelected = ThisList.SelectedIndex;
            ThisList.ScrollIntoView(ThisList.SelectedItem);
            ThisText.Text = GetSelectedItem();
            if (ThisText.Text != "")
                CurrentItem = ThisList.SelectedItem;
        }

        private string GetSelectedItem()
        {
            if (TextC == EnumTextCategory.SimpleText)
                return ThisList.SelectedItem.ToString();
            if (TextC == EnumTextCategory.EnumBinding)
            {
                if (ThisList.SelectedItem == null)
                    return default;
                return ThisList.SelectedItem.ToString();
            }

            object ThisObj = ThisList.SelectedItem;
            return GetStringFromObject(ThisObj, PropertyName);
        }

        private string GetSpecificItem(int Index)
        {
            if (TextC == EnumTextCategory.SimpleText || TextC == EnumTextCategory.EnumBinding)
                return ThisList.Items[Index].ToString();
            object ThisObj = ThisList.Items[Index];
            return GetStringFromObject(ThisObj, PropertyName);
        }


        public bool HasValidationError()
        {
            if (TextC == EnumTextCategory.EnumBinding && string.IsNullOrEmpty(ThisText.Text) == true)
                return true;
            try
            {
                BindingExpression ThisItem = ThisText.GetBindingExpression(TextBox.TextProperty);
                return ThisItem.HasValidationError;
            }
            catch
            {
                return false;
            }
        }

		

		//new bool Focus()
		//{
		//	CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.VBCompat.Stop();
		//	return true;
		//}

		//bool Focus()
		//{
		//	return true;
		//}


		//     new bool Focus()
		//     {
		////throw new Exception("Trying To Focus");
		//         return ThisText.Focus();
		//     }

		//looks like somehow if i want to focus, i am always forced to use either custom focus or the interface version.  not sure why that is in c#.

		public new void Focus()
        {
			//throw new Exception("Hidden Focus");
			//Focus(); //this seems to work
			CustomFocus();
        }

		public void CustomFocus()
		{
			ThisText.Focus();
		}

        public void SendEnteredCommand(string _Command)
        {
            KeyBinding ThisInput = new KeyBinding
            {
                Key = Key.Enter
            };
            Binding ThisBind = new Binding(_Command);
            BindingOperations.SetBinding(ThisInput, InputBinding.CommandProperty, ThisBind);
            Binding NewBind = new Binding(nameof(Combo.CurrentItem))
            {
                Source = this
            };
            BindingOperations.SetBinding(ThisInput, InputBinding.CommandParameterProperty, NewBind);
            InputBindings.Add(ThisInput);
        }

        public void SendPropertyBinding(string _ListItemPropertyName, string _TextboxPropertyName = "", bool ComboUseEnter = false)
        {
            if (_ListItemPropertyName == "" && _TextboxPropertyName == "")
                throw new BasicBlankException("Must have at least listpropertyname or  text property name.  Otherwise, don't use it");
            if (_ListItemPropertyName != "")
            {
                PropertyName = _ListItemPropertyName;
                TextC = EnumTextCategory.NormalBinding;
                FrameworkElementFactory txt = new FrameworkElementFactory(typeof(TextBlock));
                Binding ThisBind = new Binding(PropertyName)
                {
                    ValidatesOnDataErrors = false
                };
                txt.SetBinding(TextBlock.TextProperty, ThisBind);
                ThisList.ItemTemplate = new DataTemplate(typeof(string))
                {
                    VisualTree = txt
                };

            }
            else
                TextC = EnumTextCategory.EnumBinding;

            if (_TextboxPropertyName == "")
                return;
            Binding NewBind = new Binding(_TextboxPropertyName);
            if (TextC == EnumTextCategory.NormalBinding && ComboUseEnter == false)
                NewBind.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
            else
                NewBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            NewBind.ValidatesOnDataErrors = true;
            ThisText.SetBinding(TextBox.TextProperty, NewBind); //accidently used textblock when it should have been textbox.
        }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(object), typeof(Combo), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnItemsSourcePropertyChanged)));
        public object ItemsSource
        {
            get
            {
                return GetValue(ItemsSourceProperty);
            }
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        private static void OnItemsSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var ThisItem = (Combo)sender;
            ThisItem.ThisList.ItemsSource = (IEnumerable)e.NewValue;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            var withBlock = e.Property;
            if (withBlock.Name == nameof(FontSize))
            {
                ThisText.FontSize = (double)e.NewValue;
                ThisList.FontSize = (double)e.NewValue;
				// not sure if i still need this or not.
				//bug seemed to be here
				int TempSize =  int.Parse(e.NewValue.ToString()); //hopefully this is better.
				TempSize += 9;
                ThisGrid.RowDefinitions[0].Height = new GridLength(TempSize);
            }
            else if (withBlock.Name == nameof(this.Foreground))
            {
                ThisText.Foreground = (Brush)e.NewValue;
                ThisList.Foreground = (Brush)e.NewValue; // just in case the old one was somehow used.
            }
            base.OnPropertyChanged(e);
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(Combo), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnTextPropertyChanged)));
        public string Text
        {
            get
            {
                return ThisText.Text;
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }

        private static void OnTextPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var ThisItem = (Combo)sender;
            {
                var withBlock = ThisItem;
                withBlock.Text = (string)e.NewValue;
                withBlock.ModifyList();
                if (withBlock.ThisText.Text == "")
                {
                    withBlock.ScrollToView(0);
                    withBlock.PreviousSelected = -1;
                    withBlock.ThisList.SelectedIndex = -1;
                    withBlock.ThisText.Text = "";
                }
            }
        }

        public T GetSelectedObject<T>()
        {
            var FirstObj = SelectedItem;
            return (T)FirstObj;
        }

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(object), typeof(Combo), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnSelectedPropertyChanged)));
        public object SelectedItem
        {
            get
            {
                return ThisList.SelectedItem; // try this way
            }
            set
            {
                SetValue(SelectedItemProperty, value);
            }
        }

        private static void OnSelectedPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var ThisItem = (Combo)sender;
            ThisItem.ThisList.SelectedItem = e.NewValue;
        }

        public void SelectAll()
        {
            ThisText.SelectAll(); // i think
        }

        private void SelectAll(object sender, RoutedEventArgs e)
        {
            TextBox _TextBox = (TextBox)sender;
            if (!(_TextBox == null))
                ThisText.SelectAll();
        }

        public void CompletelyClearItems() //messed up here.
        {
            ItemsSource = null;
            ThisText.Text = "";
        }
        public void ClearTextOnly() //forgot this part
        {
            var TempList = ItemsSource;
            ItemsSource = null;
            ThisText.Text = "";
            ItemsSource = TempList;
        }

        private void ModifyList()
        {
            int x;
            string firsttext;
            string secondtext;
            secondtext = ThisText.Text.ToLower();
            var loopTo = ThisList.Items.Count - 1;
            for (x = 0; x <= loopTo; x++)
            {
                firsttext = ThisList.Items[x].ToString().ToLower();
                if (firsttext == secondtext)
                {
                    ScrollToView(x);
                    return;
                }
            }
        }
    }
}