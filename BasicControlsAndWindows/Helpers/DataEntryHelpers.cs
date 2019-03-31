using BasicControlsAndWindows.BasicWindows.BasicConverters;
using BasicControlsAndWindows.Controls.BasicControls;
using BasicControlsAndWindows.Controls.Interfaces;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers;
using CommonBasicStandardLibraries.MVVMHelpers.SpecializedViewModels;
using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using static BasicControlsAndWindows.Helpers.GridHelper;
using static BasicControlsAndWindows.Helpers.SimpleControlHelpers;
namespace BasicControlsAndWindows.Helpers
{
    public class DataEntryHelpers : IButtonData
    {
        private readonly CustomBasicList<IBasicEntry> ControlList = new CustomBasicList<IBasicEntry>(); //we can use the type to cast to something else.
        private DataEntryViewModel ThisMod;

        public bool WhenPropertyChanges { get; set; } = false;//if set to true, must be set to that in cases where i use a f key to submit.
        private readonly Grid ThisGrid;
        private bool NeedsHints;

        public Brush ButtonBackColor { get; set; }
        public Brush ButtonForeColor { get; set; }
        ItemsControl CList;

        public Brush TextColor { get; set; } = Brushes.Black;
       

        public void AddGenericButton(Key ThisKey, ICommand ThisCommand, string OtherText, object Parameter = null, Window ThisWindow = null)
        {
            NeedsHints = true;
            ButtonHelperClass.AddButton(this, OtherText, ThisKey, ThisGrid, ThisCommand, Parameter: Parameter, ThisWindow: ThisWindow);
        }

        public void AddGenericButton(Key ThisKey, string CommandPath, string OtherText, object Parameter = null, Window ThisWindow = null)
        {
            NeedsHints = true;
            ButtonHelperClass.AddButton(this, OtherText, ThisKey, ThisGrid, CommandPath: CommandPath, Parameter: Parameter, ThisWindow: ThisWindow);
        }

        public void AddButtonToAddNewItem(Key ThisKey, string CommandPath)
        {
            AddAutoRows(ThisGrid, 1);
            NeedsHints = true;
            Button ThisButton = new Button
            {
                Content = $"Add And Save ({ThisKey.ToString()}), F5 to get validation errors",
                FontSize = 25,
            };
            ThisButton.SetBinding(ButtonBase.CommandProperty, new Binding(CommandPath));
            if (ButtonBackColor != null)
            {
                ThisButton.Background = ButtonBackColor;
            }

            if (ButtonForeColor != null)
            {
                ThisButton.Foreground = ButtonForeColor;
            }

            SetMargins(ThisButton);
            SetKeyBindings(ThisGrid, ThisKey, CommandPath);
            AddControl(ThisButton, false);
        }

        public void AddSaveAndInsertButtons(Key AddKey, Key SaveKey)
        {
            StackPanel TempStack = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };

            Button ThisButton = new Button
            {
                Content = $"Save ({SaveKey.ToString()})",
                FontSize = 20 //decided a little less when its save and insert
            };
            ThisButton.SetBinding(ButtonBase.CommandProperty, new Binding(nameof(DataEntryViewModel.SaveCommand)));
            if (ButtonBackColor != null)
            {
                ThisButton.Background = ButtonBackColor;
            }

            if (ButtonForeColor != null)
            {
                ThisButton.Foreground = ButtonForeColor;
            }

            SetMargins(ThisButton);
            SetKeyBindings(ThisGrid, SaveKey, nameof(DataEntryViewModel.SaveCommand));
            TempStack.Children.Add(ThisButton);

            ThisButton = new Button
            {
                Content = $"Add/Hide ({SaveKey.ToString()})",
                FontSize = 20 //decided a little less when its save and insert
            };
            ThisButton.SetBinding(ButtonBase.CommandProperty, new Binding(nameof(AddEditViewModel.EnterCommand)));
            if (ButtonBackColor != null)
            {
                ThisButton.Background = ButtonBackColor;
            }

            if (ButtonForeColor != null)
            {
                ThisButton.Foreground = ButtonForeColor;
            }

            SetMargins(ThisButton);
            SetKeyBindings(ThisGrid, AddKey, nameof(AddEditViewModel.EnterCommand)); //if you are using wrong one, then casting error will result.
            TempStack.Children.Add(ThisButton);

            AddAutoRows(ThisGrid, 1);
            AddControlToGrid(ThisGrid, TempStack, ThisGrid.RowDefinitions.Count - 1, 0);
            AddLeftOverRow(ThisGrid, 10);
        }

        public void AddDataGrid(IEnumerable ThisList, Key AddKey, Key SaveKey)
        {
            DataGrid NewD = new DataGrid
            {
                ItemsSource = ThisList
            };
            AddSaveAndInsertButtons(AddKey, SaveKey);
            AddControlToGrid(ThisGrid, NewD, ThisGrid.RowDefinitions.Count - 1, 0);
        }



        private void ContinueNew() //done
        {
            AddAutoColumns(ThisGrid, 1);
            AddLeftOverColumn(ThisGrid, 1);
            CList = new ItemsControl()
            {
                ItemsSource = ThisMod.ErrorLists,
                Foreground = Brushes.Red
            };
            VisibilityConverter ThisC = new VisibilityConverter()
            {
                UseCollapsed = true
            };
            Binding ThisBind = new Binding(nameof(DataEntryViewModel.AttemptedToSubmitForm))
            {
                Converter = ThisC
            };
            CList.SetBinding(UIElement.VisibilityProperty, ThisBind);
            AddAutoRows(ThisGrid, 1);
            ThisGrid.Children.Add(CList);
            Grid.SetColumnSpan(CList, 2);
            ThisGrid.IsVisibleChanged += ThisGrid_IsVisibleChanged;
            ThisGrid.KeyUp += ThisGrid_KeyUp;
        }

        private void ThisGrid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5 && NeedsHints == true)
            {
                ThisMod.AttemptedToSubmitForm = true;
            }
        }

        private async void ThisGrid_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (ThisGrid.Visibility == Visibility.Visible)
            {
                do
                {
                    if (Command.CurrentlyExecuting() == false)
                    {
                        await Task.Delay(10);
                        FocusFirstControl();
                        return;
                    }
                    await Task.Delay(50);
                } while (true);
            }
        }

        public void FocusFirstControl()
        {
            if (ControlList.Count == 0)
            {
                return;
            }

            ControlList.First().Focus();
        }

        private void Firsts(DataEntryViewModel _Model)
        {
            ThisMod = _Model;
            ContinueNew();
            ThisGrid.DataContext = _Model;
        }

        public DataEntryHelpers(DataEntryViewModel _Model)
        {
            ThisGrid = new Grid();
            Firsts(_Model);
        }

        public DataEntryHelpers(DataEntryViewModel _Model, Grid _Grid)
        {
            ThisGrid = _Grid;
            Firsts(_Model);
        }

        private IBasicEntry GetSpecificControl(string PropertyName) //i like this new use of the find.
        {
            return ControlList.Find(x => x.BindingPath == PropertyName);
        }

        public Combo GetSpecificCombo(string PropertyName)
        {
            var ThisCon = GetSpecificControl(PropertyName);
            if (ThisCon == null)
            {
                throw new BasicBlankException($"No Control With The Property Name {PropertyName} Was Found");
            }

            if (ThisCon.Category != EnumDataEntryCategory.ComboBox)
            {
                throw new BasicBlankException($"This Is Not A Combo.  Hint:  This Was {ThisCon.Category.ToString()}");
            }

            return (Combo)ThisCon;
        }

        public CustomTextbox GetSpecificSingleLineTextbox(string PropertyName)
        {
            var ThisCon = GetSpecificControl(PropertyName);
            if (ThisCon == null)
            {
                throw new BasicBlankException($"No Control With The Property Name {PropertyName} Was Found");
            }

            if (ThisCon.Category != EnumDataEntryCategory.SingleLineTextbox)
            {
                throw new BasicBlankException($"This Is Not A Single Line Text Box.  Hint:  This Was {ThisCon.Category.ToString()}");
            }

            return (CustomTextbox)ThisCon;
        }

        public MultiLineTextbox GetSpecificMultilineTextbox(string PropertyName)
        {
            var ThisCon = GetSpecificControl(PropertyName);
            if (ThisCon == null)
            {
                throw new BasicBlankException($"No Control With The Property Name {PropertyName} Was Found");
            }

            if (ThisCon.Category != EnumDataEntryCategory.SingleLineTextbox)
            {
                throw new BasicBlankException($"This Is Not A Multiline Text Box.  Hint:  This Was {ThisCon.Category.ToString()}");
            }

            return (MultiLineTextbox)ThisCon;
        }

        public void FocusOnSpecificControl(string PropertyName)
        {
            var ThisCon = GetSpecificControl(PropertyName);
            ThisCon.Focus();
        }

        //private CustomTextbox GetNextControl(IBasicEntry OldText)
        //{
        //    int Index = ControlList.IndexOf(OldText);
        //    if (Index == -1)
        //        throw new BasicBlankException("TextBox Not Found");
        //    Index += 1;
        //    return (CustomTextbox) ControlList[Index]; //if casting because its not right next, then rethink.
        //}
        //somehow it was never used.  if i am wrong, can fix.


        //protected void SetMargins(FrameworkElement ThisCon)
        //{
        //    ThisCon.Margin = new Thickness(5, 5, 5, 5);
        //}

        protected void SetUpReturns(string Header, bool IsAuto = true)
        {
            AddAutoRows(ThisGrid, 1);
            if (IsAuto == false)
            {
                var ThisRow = ThisGrid.RowDefinitions[ThisGrid.RowDefinitions.Count - 2];
                ThisRow.Height = new GridLength(1, GridUnitType.Star);
            }
            Grid.SetRow(CList, ThisGrid.RowDefinitions.Count - 1);
            AddLabel(Header);
        }

        protected virtual void RepositionLabel(TextBlock ThisLabel) { }

        private void AddLabel(string Header)
        {
            TextBlock ThisLabel = new TextBlock
            {
                Text = Header,
                Foreground = TextColor
            };
            AddControl(ThisLabel, true);
            RepositionLabel(ThisLabel);
        }

        private void AddControl(IBasicEntry ThisB, UIElement ThisCon)
        {
            SetMargins((FrameworkElement)ThisCon); //worked in vb.net.  hopefully no problems (?)
            if (ThisB.Category != EnumDataEntryCategory.MultiLineTextbox)
            {
                IDataEntry ThisD = (IDataEntry)ThisB;
                ThisD.HasEntered += FocusNext;
            }
            AddControl(ThisCon, false);
            ControlList.Add(ThisB);
        }

        private void FocusNext(object sender, EventArgs e)
        {
            if (ControlList.Count == 0)
            {
                return;
            }

            IBasicEntry ThisText = (IBasicEntry)sender;
            if (ThisText.Equals(ControlList.Last()))
            {
                //this means its the last one.
                //Dim TempList = (From Items In ControlList Take ControlList.Count - 1).ToList
                bool rets = ControlList.ForSpecificItem(x => x.HasValidationError() == true, x => x.Focus(), ControlList.Count - 1);
                if (rets == true)
                {
                    return;
                }
                return;
            }
            int ThisIndex = ControlList.IndexOf(ThisText);
            ThisIndex++;
            ControlList[ThisIndex].Focus();
        }

        protected void AddControl(UIElement ThisCon, bool IsHeader)
        {
            ThisGrid.Children.Add(ThisCon);
            if (IsHeader == false)
            {
                Grid.SetColumn(ThisCon, 1);
            }

            Grid.SetRow(ThisCon, ThisGrid.RowDefinitions.Count - 2); //has to be -2 because of the errors
        }

        public Grid GetContent() => ThisGrid;


        public void AddNormalTextRow(string Header, string Binding, string Command = "", IValueConverter Converter = null)
        {
            SetUpReturns(Header);
            CustomTextbox ThisText = new CustomTextbox()
            {
                BindingPath = Binding
            };
            Binding ThisBind = new Binding(Binding)
            {
                ValidatesOnDataErrors = true,
                Source = ThisMod
            };
            if (Converter != null)
            {
                ThisBind.Converter = Converter;
            }

            if (WhenPropertyChanges == true)
            {
                ThisBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            }

            if (Command != "")
            {
                Binding NewBind = new Binding(nameof(DataEntryViewModel.AttemptedToSubmitForm))
                {
                    Mode = BindingMode.TwoWay
                };
                ThisText.SetBinding(CustomTextbox.AttemptedToSubmitFormProperty, NewBind);
                ThisText.SetBinding(CustomTextbox.EnterCommandProperty, new Binding(Command));
            }
            ThisText.SendTextBinding(ThisBind);
            AddControl(ThisText, ThisText);
        }

        public void AddLabel(string Header, string Binding, IValueConverter Converter = null)
        {
            SetUpReturns(Header);
            TextBlock ThisLabel = new TextBlock()
            {
                Foreground = TextColor
            };
            Binding ThisBind = new Binding(Binding);
            if (Converter != null)
            {
                ThisBind.Converter = Converter;
            }

            ThisBind.Source = ThisMod;
            ThisBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            ThisLabel.SetBinding(TextBlock.TextProperty, ThisBind);
            AddControl(ThisLabel, false);
        }

        public void AddMultiLineTextRow(string Header, string Binding, double Height, bool ValidateWhileTyping = false)
        {
            if (Height > 0)
            {
                SetUpReturns(Header, true);
            }
            else
            {
                SetUpReturns(Header, false);
            }

            MultiLineTextbox ThisText = new MultiLineTextbox()
            {
                BindingPath = Binding,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };
            if (Height > 0)
            {
                ThisText.Height = Height;
            }
            else
            {
                ThisText.VerticalAlignment = VerticalAlignment.Stretch;
            }

            Binding ThisBind = new Binding(Binding)
            {
                ValidatesOnDataErrors = ValidateWhileTyping, //trying this part.  because now, the function is not even used.
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            ThisText.SetBinding(TextBox.TextProperty, ThisBind);
            AddControl(ThisText, ThisText);
        }

        public void AddCombo(string Header, object ItemsSource, string TextBinding, string ListBinding, double Height, string Command = "", string IndexName = "", string SelectedObject = "")
        {
            SetUpReturns(Header);
            Combo ThisCombo = new Combo()
            {
                Height = Height,
                BindingPath = IndexName,
                ItemsSource = ItemsSource
            };
            if (Command != "")
            {
                ThisCombo.SendEnteredCommand(Command);
            }

            if (TextBinding != "" || ListBinding != "")
            {
                ThisCombo.SendPropertyBinding(ListBinding, TextBinding);
            }

            if (SelectedObject != "")
            {
                Binding ThisBind = new Binding(SelectedObject);
                ThisCombo.SendSelectedItemBinding(ThisBind);
            }
            AddControl(ThisCombo, ThisCombo);
        }

        void IButtonData.AddControl(UIElement ThisCon)
        {
            AddControl(ThisCon, false);
        }
    }
}
